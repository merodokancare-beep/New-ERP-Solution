using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Banking;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;
using SDK.ERP.Infrastructure.Data;

namespace SDK.ERP.Web.Controllers;

public class ExpensesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICompanyContext _companyContext;
    private readonly IConfiguration _config;

    public ExpensesController(ApplicationDbContext db, ICompanyContext companyContext, IConfiguration config)
    {
        _db = db;
        _companyContext = companyContext;
        _config = config;
    }

    /// <summary>
    /// List of direct payments and expenses with tabs: Unsettled, Settled, Pending, Declined (Matching Image 2).
    /// </summary>
    public async Task<IActionResult> Index(string tab = "unsettled", string search = "", long? projectId = null, int page = 1, int pageSize = 10)
    {
        ViewData["ActiveMenu"] = tab.Equals("pending", StringComparison.OrdinalIgnoreCase) ? "ExpensesApproval" : "Expenses";
        await EnsureDirectExpensesTableAsync();

        var normalizedTab = (tab ?? "unsettled").Trim().ToLowerInvariant();
        if (normalizedTab != "unsettled" && normalizedTab != "settled" && normalizedTab != "pending" && normalizedTab != "declined")
        {
            normalizedTab = "unsettled";
        }

        // Base query for counts (optionally filtered by projectId)
        var countQuery = _db.DirectExpenses.AsQueryable();
        if (projectId.HasValue) countQuery = countQuery.Where(e => e.ProjectId == projectId.Value);

        var unsettledCount = await countQuery.CountAsync(e => e.Status == "UNSETTLED");
        var settledCount = await countQuery.CountAsync(e => e.Status == "SETTLED");
        var pendingCount = await countQuery.CountAsync(e => e.Status == "PENDING");
        var declinedCount = await countQuery.CountAsync(e => e.Status == "DECLINED");

        ViewBag.ActiveTab = normalizedTab;
        ViewBag.Search = search ?? string.Empty;
        ViewBag.SelectedProjectId = projectId;
        ViewBag.UnsettledCount = unsettledCount;
        ViewBag.SettledCount = settledCount;
        ViewBag.PendingCount = pendingCount;
        ViewBag.DeclinedCount = declinedCount;

        var query = _db.DirectExpenses
            .Include(e => e.Vendor)
            .Include(e => e.Project)
            .Where(e => e.Status == normalizedTab.ToUpper());

        if (projectId.HasValue)
        {
            query = query.Where(e => e.ProjectId == projectId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            query = query.Where(e => e.Purpose.Contains(s) ||
                                     e.ExpenseHead.Contains(s) ||
                                     e.LedgerName.Contains(s) ||
                                     (e.Remarks != null && e.Remarks.Contains(s)) ||
                                     (e.Project != null && (e.Project.ProjectName.Contains(s) || e.Project.ProjectCode.Contains(s))));
        }

        var totalRecords = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling((double)totalRecords / pageSize));
        if (page < 1) page = 1;
        if (page > totalPages) page = totalPages;

        var items = await query
            .OrderByDescending(e => e.PaymentDate)
            .ThenByDescending(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalRecords = totalRecords;
        ViewBag.TotalPages = totalPages;

        // Load active projects for project filter dropdown
        ViewBag.Projects = await _db.Projects.OrderBy(p => p.ProjectName).AsNoTracking().ToListAsync();

        // Load bank accounts for settlement modal
        ViewBag.BankAccounts = await _db.BankAccounts.Where(b => b.IsActive).OrderBy(b => b.BankName).AsNoTracking().ToListAsync();

        return View(items);
    }

    /// <summary>
    /// Form to add a direct payment or expense release against a Project or Vendor (Matching Image 1).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["ActiveMenu"] = "Expenses";
        await EnsureDirectExpensesTableAsync();
        await PopulateDropdownsAsync();

        var model = new DirectExpense
        {
            PaymentDate = DateTime.Today,
            Status = "PENDING"
        };

        return View(model);
    }

    /// <summary>
    /// Submits a new direct payment / expense. It enters the workflow as PENDING for authorization.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DirectExpense model)
    {
        ViewData["ActiveMenu"] = "Expenses";
        await EnsureDirectExpensesTableAsync();

        if (string.IsNullOrWhiteSpace(model.ExpenseHead))
        {
            ModelState.AddModelError("ExpenseHead", "Expense Head is required.");
        }

        if (model.Amount <= 0)
        {
            ModelState.AddModelError("Amount", "Amount must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(model.Purpose))
        {
            ModelState.AddModelError("Purpose", "Purpose is required.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(model);
        }

        var company = await _companyContext.GetCurrentCompanyAsync();
        var user = await _companyContext.GetCurrentUserAsync();

        model.CompanyId = company.Id;
        model.Status = "PENDING";
        model.CreatedByUserId = user.Id;
        model.CreatedByUserName = string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName;
        model.CreatedAt = DateTime.UtcNow;

        // Resolve LedgerName if not provided
        if (string.IsNullOrWhiteSpace(model.LedgerName) && model.LedgerId.HasValue)
        {
            var coa = await _db.ChartOfAccounts.FindAsync(model.LedgerId.Value);
            if (coa != null) model.LedgerName = coa.AccountName;
        }

        // If vendor not checked, reset vendor ID
        if (!model.IsVendorApplicable)
        {
            model.VendorId = null;
        }

        // If pipeline project not checked, reset project ID
        if (!model.IsPipelineProject)
        {
            model.ProjectId = null;
        }

        _db.DirectExpenses.Add(model);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Direct Payment / Expense for ₹{model.Amount:N2} submitted successfully! It is now pending review in the approval cycle.";
        return RedirectToAction(nameof(Index), new { tab = "pending" });
    }

    /// <summary>
    /// Edit an existing direct payment / expense.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        ViewData["ActiveMenu"] = "Expenses";
        var expense = await _db.DirectExpenses.FindAsync(id);
        if (expense == null) return NotFound();

        await PopulateDropdownsAsync();
        return View(expense);
    }

    /// <summary>
    /// Updates an existing direct payment / expense.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(DirectExpense model)
    {
        ViewData["ActiveMenu"] = "Expenses";
        var expense = await _db.DirectExpenses.FindAsync(model.Id);
        if (expense == null) return NotFound();

        if (expense.Status == "SETTLED")
        {
            TempData["ErrorMessage"] = "Settled transactions cannot be modified. They are sealed for audit compliance.";
            return RedirectToAction(nameof(Index), new { tab = "settled" });
        }

        if (string.IsNullOrWhiteSpace(model.ExpenseHead))
        {
            ModelState.AddModelError("ExpenseHead", "Expense Head is required.");
        }
        if (model.Amount <= 0)
        {
            ModelState.AddModelError("Amount", "Amount must be greater than zero.");
        }
        if (string.IsNullOrWhiteSpace(model.Purpose))
        {
            ModelState.AddModelError("Purpose", "Purpose is required.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync();
            return View(model);
        }

        expense.ExpenseHead = model.ExpenseHead;
        expense.ExpenseHeadId = model.ExpenseHeadId;
        expense.IsVendorApplicable = model.IsVendorApplicable;
        expense.VendorId = model.IsVendorApplicable ? model.VendorId : null;
        expense.LedgerId = model.LedgerId;
        expense.LedgerName = model.LedgerName;
        expense.PaymentDate = model.PaymentDate;
        expense.Amount = model.Amount;
        expense.IsPipelineProject = model.IsPipelineProject;
        expense.ProjectId = model.IsPipelineProject ? model.ProjectId : null;
        expense.Purpose = model.Purpose;
        expense.Remarks = model.Remarks;

        // If it was declined and now edited, reset to PENDING for re-approval
        if (expense.Status == "DECLINED")
        {
            expense.Status = "PENDING";
            expense.DeclineReason = null;
            expense.DeclinedAt = null;
            expense.DeclinedByUserId = null;
            expense.DeclinedByUserName = null;
        }

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Expense record #{expense.Id} updated successfully.";
        return RedirectToAction(nameof(Index), new { tab = expense.Status.ToLower() });
    }

    /// <summary>
    /// Approver Action: Approves a PENDING expense and transitions it to UNSETTLED (ready for settlement).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(long id, string? approvalRemarks)
    {
        var expense = await _db.DirectExpenses.FindAsync(id);
        if (expense == null) return NotFound();

        if (expense.Status != "PENDING")
        {
            TempData["ErrorMessage"] = $"Cannot approve an item with status '{expense.Status}'. Only PENDING items can be approved.";
            return RedirectToAction(nameof(Index), new { tab = expense.Status.ToLower() });
        }

        var user = await _companyContext.GetCurrentUserAsync();
        expense.Status = "UNSETTLED";
        expense.ApprovedByUserId = user.Id;
        expense.ApprovedByUserName = string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName;
        expense.ApprovedAt = DateTime.UtcNow;
        expense.ApprovalRemarks = string.IsNullOrWhiteSpace(approvalRemarks) ? "Approved for payment release." : approvalRemarks.Trim();

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Expense #{expense.Id} (₹{expense.Amount:N2}) approved successfully! It has been moved to the Unsettled payments queue.";
        return RedirectToAction(nameof(Index), new { tab = "unsettled" });
    }

    /// <summary>
    /// Approver Action: Declines a PENDING expense with a reason.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Decline(long id, string reason)
    {
        var expense = await _db.DirectExpenses.FindAsync(id);
        if (expense == null) return NotFound();

        if (expense.Status != "PENDING")
        {
            TempData["ErrorMessage"] = $"Cannot decline an item with status '{expense.Status}'.";
            return RedirectToAction(nameof(Index), new { tab = expense.Status.ToLower() });
        }

        var user = await _companyContext.GetCurrentUserAsync();
        expense.Status = "DECLINED";
        expense.DeclinedByUserId = user.Id;
        expense.DeclinedByUserName = string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName;
        expense.DeclinedAt = DateTime.UtcNow;
        expense.DeclineReason = string.IsNullOrWhiteSpace(reason) ? "Declined by approver without comment." : reason.Trim();

        await _db.SaveChangesAsync();

        TempData["WarningMessage"] = $"Expense #{expense.Id} has been declined.";
        return RedirectToAction(nameof(Index), new { tab = "declined" });
    }

    /// <summary>
    /// Finance Action: Settles an UNSETTLED expense, recording payment disbursement mode & reference.
    /// Also posts general ledger journal entry to balance company books.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Settle(long id, long? bankAccountId, string settlementMode, string? referenceNo, DateTime? settlementDate, string? settlementNotes)
    {
        var expense = await _db.DirectExpenses.FindAsync(id);
        if (expense == null) return NotFound();

        if (expense.Status != "UNSETTLED")
        {
            TempData["ErrorMessage"] = $"Only UNSETTLED expenses can be settled.";
            return RedirectToAction(nameof(Index), new { tab = expense.Status.ToLower() });
        }

        var user = await _companyContext.GetCurrentUserAsync();
        var company = await _companyContext.GetCurrentCompanyAsync();

        expense.Status = "SETTLED";
        expense.SettledByUserId = user.Id;
        expense.SettledByUserName = string.IsNullOrEmpty(user.FullName) ? user.Username : user.FullName;
        expense.SettledAt = settlementDate ?? DateTime.UtcNow;
        expense.SettlementPaymentMode = string.IsNullOrWhiteSpace(settlementMode) ? "Bank Transfer" : settlementMode.Trim();
        expense.SettlementReference = referenceNo?.Trim();
        expense.SettlementNotes = settlementNotes?.Trim();

        if (bankAccountId.HasValue)
        {
            var bank = await _db.BankAccounts.FindAsync(bankAccountId.Value);
            if (bank != null)
            {
                expense.SettlementBankAccountId = bank.Id;
                expense.SettlementBankName = $"{bank.BankName} - {bank.AccountNumber}";
            }
        }

        // Post balanced Journal Entry to General Ledger
        try
        {
            var fy = await _db.FinancialYears.FirstOrDefaultAsync(f => !f.IsClosed) ?? await _db.FinancialYears.FirstOrDefaultAsync();
            var jv = new JournalEntry
            {
                CompanyId = company.Id,
                BranchId = 1,
                FyId = fy?.Id ?? 1,
                VoucherNo = $"DP-SETTLE-{expense.Id:D5}",
                VoucherDate = expense.SettledAt ?? DateTime.Today,
                VoucherType = "PAYMENT",
                SourceEntityType = "DirectExpense",
                SourceEntityId = expense.Id,
                ProjectId = expense.ProjectId,
                Narration = $"Direct Payment Settled: {expense.ExpenseHead} - {expense.Purpose} [Ref: {expense.SettlementReference ?? "N/A"}]",
                TotalDebit = expense.Amount,
                TotalCredit = expense.Amount,
                IsBalanced = true,
                CreatedBy = user.Id
            };
            _db.JournalEntries.Add(jv);
            await _db.SaveChangesAsync();

            expense.JournalEntryId = jv.Id;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Journal posting notification: {ex.Message}");
        }

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Payment of ₹{expense.Amount:N2} settled successfully via {expense.SettlementPaymentMode}! General Ledger updated.";
        return RedirectToAction(nameof(Index), new { tab = "settled" });
    }

    /// <summary>
    /// Printable payment release / expense voucher.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> PrintVoucher(long id)
    {
        var expense = await _db.DirectExpenses
            .Include(e => e.Vendor)
            .Include(e => e.Project)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (expense == null) return NotFound();

        var company = await _companyContext.GetCurrentCompanyAsync();
        ViewBag.Company = company;

        return View(expense);
    }

    /// <summary>
    /// Monthly Direct Payments & Expenses Report with filters and printable summary (Matching "+ Monthly Report" button).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> MonthlyReport(int? year, int? month)
    {
        ViewData["ActiveMenu"] = "Expenses";
        var curYear = year ?? DateTime.Today.Year;
        var curMonth = month ?? DateTime.Today.Month;

        ViewBag.SelectedYear = curYear;
        ViewBag.SelectedMonth = curMonth;

        var startOfMonth = new DateTime(curYear, curMonth, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var expenses = await _db.DirectExpenses
            .Include(e => e.Vendor)
            .Include(e => e.Project)
            .Where(e => e.PaymentDate >= startOfMonth && e.PaymentDate <= endOfMonth)
            .OrderByDescending(e => e.PaymentDate)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.Company = await _companyContext.GetCurrentCompanyAsync();
        return View(expenses);
    }

    private async Task PopulateDropdownsAsync()
    {
        // 1. Expense Heads
        var expenseHeads = new List<string>();
        try
        {
            var dbHeads = await _db.ExpenseTypes
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .Select(e => e.Name)
                .AsNoTracking()
                .ToListAsync();

            if (dbHeads.Any()) expenseHeads.AddRange(dbHeads);
        }
        catch { }

        if (!expenseHeads.Any())
        {
            expenseHeads.AddRange(new[]
            {
                "Project",
                "Tax Payment",
                "Office Expense",
                "Business Development",
                "Proprietor Drawings A/c.",
                "Salary & Allowance",
                "Transportation Charges.",
                "Site Expense Reimbursement / Refund",
                "Direct Engineering & Service Revenue",
                "Sundry Direct Receipts"
            });
        }
        ViewBag.ExpenseHeads = expenseHeads;

        // 2. Vendors
        try
        {
            ViewBag.Vendors = await _db.Vendors
                .Where(v => v.IsActive)
                .OrderBy(v => v.VendorName)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            ViewBag.Vendors = new List<Vendor>();
        }

        // 3. Ledgers (Chart of Accounts / Expense & Bank Ledgers)
        var ledgers = new List<string>();
        try
        {
            var dbLedgers = await _db.ChartOfAccounts
                .Where(c => c.IsActive)
                .OrderBy(c => c.AccountName)
                .Select(c => c.AccountName)
                .AsNoTracking()
                .ToListAsync();

            if (dbLedgers.Any()) ledgers.AddRange(dbLedgers);
        }
        catch { }

        if (!ledgers.Any())
        {
            ledgers.AddRange(new[]
            {
                "GST Payment",
                "Electricity Expenses",
                "ESIC",
                "Business Development (Project)",
                "Office Rent",
                "Internet & Communication",
                "Telephone Expenses",
                "Printing & Stationery",
                "Travelling & Conveyance",
                "Subcontractor / Site Service Ledger",
                "Bank Charges & Processing Fee",
                "Audit & Legal Fees",
                "Miscellaneous Expenses"
            });
        }
        ViewBag.Ledgers = ledgers;

        // 4. Projects
        try
        {
            ViewBag.Projects = await _db.Projects
                .OrderBy(p => p.ProjectName)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            ViewBag.Projects = new List<Project>();
        }
    }

    private async Task EnsureDirectExpensesTableAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('[DirectExpenses]') IS NULL AND OBJECT_ID('[dbo].[DirectExpenses]') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[DirectExpenses] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [CompanyId] BIGINT NOT NULL,
                        [ExpenseHead] NVARCHAR(250) NOT NULL,
                        [ExpenseHeadId] BIGINT NULL,
                        [IsVendorApplicable] BIT NOT NULL DEFAULT 0,
                        [VendorId] BIGINT NULL,
                        [LedgerId] BIGINT NULL,
                        [LedgerName] NVARCHAR(250) NOT NULL DEFAULT '',
                        [PaymentDate] DATETIME2 NOT NULL,
                        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
                        [IsPipelineProject] BIT NOT NULL DEFAULT 0,
                        [ProjectId] BIGINT NULL,
                        [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
                        [Remarks] NVARCHAR(MAX) NULL,
                        [Status] NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
                        [CreatedByUserId] BIGINT NULL,
                        [CreatedByUserName] NVARCHAR(150) NULL,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
                        [ApprovedByUserId] BIGINT NULL,
                        [ApprovedByUserName] NVARCHAR(150) NULL,
                        [ApprovedAt] DATETIME2 NULL,
                        [ApprovalRemarks] NVARCHAR(MAX) NULL,
                        [DeclinedByUserId] BIGINT NULL,
                        [DeclinedByUserName] NVARCHAR(150) NULL,
                        [DeclinedAt] DATETIME2 NULL,
                        [DeclineReason] NVARCHAR(MAX) NULL,
                        [SettledByUserId] BIGINT NULL,
                        [SettledByUserName] NVARCHAR(150) NULL,
                        [SettledAt] DATETIME2 NULL,
                        [SettlementPaymentMode] NVARCHAR(100) NULL,
                        [SettlementReference] NVARCHAR(150) NULL,
                        [SettlementBankAccountId] BIGINT NULL,
                        [SettlementBankName] NVARCHAR(150) NULL,
                        [SettlementNotes] NVARCHAR(MAX) NULL,
                        [JournalEntryId] BIGINT NULL
                    );
                END
            ");
        }
        catch { }
    }
}
