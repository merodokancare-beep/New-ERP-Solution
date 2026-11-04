using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Application.ViewModels;
using SDK.ERP.Domain.Entities.Banking;
using SDK.ERP.Infrastructure.Data;

namespace SDK.ERP.Web.Controllers;

public class PettyCashController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICurrentTenantProvider _tenantProvider;
    private readonly IWebHostEnvironment _env;

    public PettyCashController(ApplicationDbContext db, ICurrentTenantProvider tenantProvider, IWebHostEnvironment env)
    {
        _db = db;
        _tenantProvider = tenantProvider;
        _env = env;
    }

    private long GetTenantCompanyId() => _tenantProvider?.CurrentCompanyId > 0 ? _tenantProvider.CurrentCompanyId : 1;

    // GET: /PettyCash
    public async Task<IActionResult> Index(string tab = "overview")
    {
        ViewData["ActiveMenu"] = "PettyCash";
        var companyId = GetTenantCompanyId();

        var withdrawals = new List<CashWithdrawal>();
        var utilisations = new List<CashUtilisation>();
        var allocations = new List<CashAllocation>();
        var bankAccounts = new List<BankAccount>();
        var projects = new List<Project>();
        var expenseTypes = new List<ExpenseType>();
        var bankTransactions = new List<BankTransaction>();
        var users = new List<User>();

        try
        {
            withdrawals = await _db.CashWithdrawals
                .Include(w => w.BankAccount)
                .Include(w => w.Allocations)
                    .ThenInclude(a => a.Utilisation)
                .OrderByDescending(w => w.WithdrawalDate)
                .ThenByDescending(w => w.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        try
        {
            utilisations = await _db.CashUtilisations
                .Include(u => u.Project)
                .Include(u => u.Allocations)
                    .ThenInclude(a => a.Withdrawal)
                .OrderByDescending(u => u.ExpenseDate)
                .ThenByDescending(u => u.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        try
        {
            allocations = await _db.CashAllocations
                .Include(a => a.Withdrawal)
                .Include(a => a.Utilisation)
                .OrderByDescending(a => a.AllocationDate)
                .ThenByDescending(a => a.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        try
        {
            bankAccounts = await _db.BankAccounts
                .Where(b => b.IsActive)
                .OrderBy(b => b.BankName)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        try
        {
            projects = await _db.Projects
                .OrderBy(p => p.ProjectName)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        try
        {
            expenseTypes = await _db.ExpenseTypes
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        try
        {
            bankTransactions = await _db.BankTransactions
                .Where(t => !t.IsReconciled)
                .OrderByDescending(t => t.TransactionDate)
                .Take(50)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        try
        {
            users = await _db.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.FullName)
                .AsNoTracking()
                .ToListAsync();
        }
        catch { }

        var totalInflow = withdrawals.Sum(w => w.Amount);
        var totalUtilised = utilisations.Where(u => u.Status != "DRAFT").Sum(u => u.Amount);
        var totalAllocated = allocations.Sum(a => a.Amount);

        var model = new PettyCashViewModel
        {
            ActiveTab = tab,
            Withdrawals = withdrawals,
            Utilisations = utilisations,
            Allocations = allocations,
            BankAccounts = bankAccounts,
            Projects = projects,
            ExpenseTypes = expenseTypes,
            BankTransactions = bankTransactions,
            Users = users,
            TotalCashInflow = totalInflow,
            TotalCashUtilised = totalUtilised,
            TotalAllocated = totalAllocated,
            OpenWithdrawalsCount = withdrawals.Count(w => w.Status == "OPEN" || w.Status == "PARTLY_UTILISED"),
            PendingUtilisationsCount = utilisations.Count(u => u.Status == "DRAFT" || u.AllocatedAmount < u.Amount)
        };

        return View(model);
    }

    // POST: /PettyCash/CreateWithdrawal
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateWithdrawal(CashWithdrawalInputModel model)
    {
        var companyId = GetTenantCompanyId();

        if (model.Amount <= 0)
        {
            TempData["ErrorMessage"] = "Withdrawal amount must be greater than zero.";
            return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
        }

        // Generate serial reference number
        var prefix = model.SourceType == "BANK_WITHDRAWAL" ? "CW" : "CR";
        var year = DateTime.Today.Year;
        var count = await _db.CashWithdrawals.CountAsync(w => w.SourceType == model.SourceType && w.WithdrawalDate.Year == year);
        var refNumber = $"{prefix}-{year}-{(count + 1):D4}";

        var withdrawal = new CashWithdrawal
        {
            CompanyId = companyId,
            ReferenceNumber = refNumber,
            WithdrawalDate = model.WithdrawalDate,
            SourceType = model.SourceType,
            BankAccountId = model.BankAccountId,
            ChequeOrRefNo = model.ChequeOrRefNo,
            Amount = model.Amount,
            UtilisedAmount = 0,
            WithdrawnBy = string.IsNullOrWhiteSpace(model.WithdrawnBy) ? "Custodian" : model.WithdrawnBy.Trim(),
            Purpose = model.Purpose.Trim(),
            ApprovedBy = string.IsNullOrWhiteSpace(model.ApprovedBy) ? "Finance Manager" : model.ApprovedBy.Trim(),
            BankStatementLineId = model.BankStatementLineId,
            BankStatementRef = model.BankStatementRef,
            Status = "OPEN",
            Notes = model.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // If linked to bank account and is withdrawal, update bank account book balance
        if (model.SourceType == "BANK_WITHDRAWAL" && model.BankAccountId.HasValue)
        {
            var bank = await _db.BankAccounts.FindAsync(model.BankAccountId.Value);
            if (bank != null)
            {
                if (bank.BookBalance < model.Amount)
                {
                    TempData["ErrorMessage"] = $"Cannot withdraw ₹{model.Amount:N2} from {bank.BankName}. The account book balance is only ₹{bank.BookBalance:N2}.";
                    return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
                }
                bank.BookBalance -= model.Amount;
            }
        }

        // If linked to statement line, reconcile statement line
        if (model.BankStatementLineId.HasValue && model.BankStatementLineId.Value > 0)
        {
            var statementLine = await _db.BankTransactions.FindAsync(model.BankStatementLineId.Value);
            if (statementLine != null)
            {
                statementLine.IsReconciled = true;
            }
        }

        _db.CashWithdrawals.Add(withdrawal);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"{(model.SourceType == "BANK_WITHDRAWAL" ? "Bank Cash Withdrawal" : "Cash Inflow")} {refNumber} of ₹{model.Amount:N2} recorded successfully.";
        return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
    }

    // POST: /PettyCash/CreateUtilisation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUtilisation(CashUtilisationInputModel model, IFormFile? receiptFile)
    {
        var companyId = GetTenantCompanyId();

        if (model.Amount <= 0)
        {
            TempData["ErrorMessage"] = "Expense amount must be greater than zero.";
            return RedirectToAction(nameof(Index), new { tab = "utilisations" });
        }

        // Validate available cash-in-hand for Approved or Posted vouchers
        var totalInflow = await _db.CashWithdrawals.SumAsync(w => w.Amount);
        var totalUtilised = await _db.CashUtilisations
            .Where(u => u.Status != "DRAFT")
            .SumAsync(u => u.Amount);
        var availableCashInHand = totalInflow - totalUtilised;

        if ((model.Status == "APPROVED" || model.Status == "POSTED") && model.Amount > availableCashInHand)
        {
            TempData["ErrorMessage"] = $"Cannot create {model.Status} expense voucher of ₹{model.Amount:N2}. Current available cash-in-hand is only ₹{Math.Max(0, availableCashInHand):N2}. You cannot disburse more cash than the physical float. Please record a bank cash withdrawal first, or save as 'DRAFT'.";
            return RedirectToAction(nameof(Index), new { tab = "utilisations" });
        }

        // Generate serial voucher number
        var year = DateTime.Today.Year;
        var count = await _db.CashUtilisations.CountAsync(u => u.ExpenseDate.Year == year);
        var voucherNumber = $"PCV-{year}-{(count + 1):D4}";

        string? attachmentFileName = null;
        string? attachmentFilePath = null;

        if (receiptFile != null && receiptFile.Length > 0)
        {
            try
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "tenants", $"org_{companyId}", "pettycash");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var safeFileName = $"pc_{DateTime.UtcNow.Ticks}_{Path.GetFileName(receiptFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, safeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await receiptFile.CopyToAsync(stream);
                }

                attachmentFileName = receiptFile.FileName;
                attachmentFilePath = $"/uploads/tenants/org_{companyId}/pettycash/{safeFileName}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"File upload notice: {ex.Message}");
            }
        }

        var utilisation = new CashUtilisation
        {
            CompanyId = companyId,
            VoucherNumber = voucherNumber,
            ExpenseDate = model.ExpenseDate,
            Amount = model.Amount,
            AllocatedAmount = 0,
            Payee = model.Payee.Trim(),
            ExpenseCategory = model.ExpenseCategory.Trim(),
            ExpenseAccountId = model.ExpenseAccountId,
            ProjectId = model.ProjectId,
            CostCenter = model.CostCenter,
            Purpose = model.Purpose.Trim(),
            AttachmentFileName = attachmentFileName,
            AttachmentFilePath = attachmentFilePath,
            PaidBy = string.IsNullOrWhiteSpace(model.PaidBy) ? "Custodian" : model.PaidBy.Trim(),
            ApprovedBy = string.IsNullOrWhiteSpace(model.ApprovedBy) ? "Authorized Approver" : model.ApprovedBy.Trim(),
            Status = model.Status,
            CreatedAt = DateTime.UtcNow
        };

        _db.CashUtilisations.Add(utilisation);
        await _db.SaveChangesAsync();

        // If status is APPROVED or POSTED and AutoAllocateFifo is requested, trigger FIFO
        if ((model.Status == "APPROVED" || model.Status == "POSTED") && model.AutoAllocateFifo)
        {
            await InternalFifoAllocateAsync(utilisation, "System (FIFO Auto-Allocation)");
            await _db.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = $"Petty Cash Voucher {voucherNumber} for ₹{model.Amount:N2} created successfully with status '{model.Status}'.";
        return RedirectToAction(nameof(Index), new { tab = "utilisations" });
    }

    // POST: /PettyCash/UpdateUtilisationStatus
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateUtilisationStatus(long id, string status, bool autoAllocate = true)
    {
        var utilisation = await _db.CashUtilisations
            .Include(u => u.Allocations)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (utilisation == null)
        {
            TempData["ErrorMessage"] = "Petty cash voucher not found.";
            return RedirectToAction(nameof(Index), new { tab = "utilisations" });
        }

        // Validate available cash-in-hand before advancing to Approved or Posted
        if (status == "APPROVED" || status == "POSTED")
        {
            var totalInflow = await _db.CashWithdrawals.SumAsync(w => w.Amount);
            var otherUtilised = await _db.CashUtilisations
                .Where(u => u.Id != id && u.Status != "DRAFT")
                .SumAsync(u => u.Amount);
            var availableCashInHand = totalInflow - otherUtilised;

            if (utilisation.Amount > availableCashInHand)
            {
                TempData["ErrorMessage"] = $"Cannot approve/post voucher {utilisation.VoucherNumber} of ₹{utilisation.Amount:N2}. Current available cash-in-hand is only ₹{Math.Max(0, availableCashInHand):N2}. Please record a bank cash withdrawal to replenish the float first.";
                return RedirectToAction(nameof(Index), new { tab = "utilisations" });
            }
        }

        utilisation.Status = status;

        if ((status == "APPROVED" || status == "POSTED") && autoAllocate && utilisation.AllocatedAmount < utilisation.Amount)
        {
            await InternalFifoAllocateAsync(utilisation, "System (FIFO on Approval)");
        }

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Voucher {utilisation.VoucherNumber} status updated to {status}.";
        return RedirectToAction(nameof(Index), new { tab = "utilisations" });
    }

    // POST: /PettyCash/DeleteUtilisation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUtilisation(long id)
    {
        var utilisation = await _db.CashUtilisations
            .Include(u => u.Allocations)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (utilisation == null)
        {
            TempData["ErrorMessage"] = "Petty cash voucher not found.";
            return RedirectToAction(nameof(Index), new { tab = "utilisations" });
        }

        // Unlink allocations and restore withdrawal amounts
        var allocations = await _db.CashAllocations
            .Where(a => a.UtilisationId == id)
            .ToListAsync();

        foreach (var alloc in allocations)
        {
            var w = await _db.CashWithdrawals.FindAsync(alloc.WithdrawalId);
            if (w != null)
            {
                w.UtilisedAmount = Math.Max(0, w.UtilisedAmount - alloc.Amount);
                w.Status = w.UtilisedAmount == 0 ? "OPEN" : "PARTLY_UTILISED";
            }
            _db.CashAllocations.Remove(alloc);
        }

        _db.CashUtilisations.Remove(utilisation);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Petty cash voucher {utilisation.VoucherNumber} deleted successfully and float balances restored.";
        return RedirectToAction(nameof(Index), new { tab = "utilisations" });
    }

    // POST: /PettyCash/DeleteWithdrawal
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteWithdrawal(long id)
    {
        var withdrawal = await _db.CashWithdrawals
            .Include(w => w.Allocations)
            .FirstOrDefaultAsync(w => w.Id == id);

        if (withdrawal == null)
        {
            TempData["ErrorMessage"] = "Withdrawal record not found.";
            return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
        }

        if (withdrawal.Allocations.Any())
        {
            TempData["ErrorMessage"] = $"Cannot delete withdrawal {withdrawal.ReferenceNumber} because it is currently funding {withdrawal.Allocations.Count} expense voucher(s). Please unlink allocations first.";
            return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
        }

        // Restore bank balance if it was a bank withdrawal
        if (withdrawal.SourceType == "BANK_WITHDRAWAL" && withdrawal.BankAccountId.HasValue)
        {
            var bank = await _db.BankAccounts.FindAsync(withdrawal.BankAccountId.Value);
            if (bank != null)
            {
                bank.BookBalance += withdrawal.Amount;
            }
        }

        _db.CashWithdrawals.Remove(withdrawal);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Withdrawal {withdrawal.ReferenceNumber} removed successfully.";
        return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
    }

    // POST: /PettyCash/ReconcileWithdrawal
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReconcileWithdrawal(long id, string bankStatementRef, string? notes)
    {
        var withdrawal = await _db.CashWithdrawals.FindAsync(id);
        if (withdrawal == null)
        {
            TempData["ErrorMessage"] = "Withdrawal record not found.";
            return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
        }

        if (string.IsNullOrWhiteSpace(bankStatementRef))
        {
            TempData["ErrorMessage"] = "Please provide the bank statement line reference or transaction ID.";
            return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
        }

        withdrawal.BankStatementRef = bankStatementRef.Trim();
        if (!string.IsNullOrWhiteSpace(notes))
        {
            withdrawal.Notes = string.IsNullOrEmpty(withdrawal.Notes) ? notes : $"{withdrawal.Notes} | BRS: {notes}";
        }

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Withdrawal {withdrawal.ReferenceNumber} reconciled successfully against bank statement ref '{bankStatementRef}'.";
        return RedirectToAction(nameof(Index), new { tab = "withdrawals" });
    }

    // POST: /PettyCash/AutoAllocateFifo
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AutoAllocateFifo(long? utilisationId = null)
    {
        var companyId = GetTenantCompanyId();

        if (utilisationId.HasValue && utilisationId.Value > 0)
        {
            var utilisation = await _db.CashUtilisations
                .Include(u => u.Allocations)
                .FirstOrDefaultAsync(u => u.Id == utilisationId.Value);

            if (utilisation == null)
            {
                TempData["ErrorMessage"] = "Voucher not found.";
                return RedirectToAction(nameof(Index), new { tab = "utilisations" });
            }

            var allocatedNow = await InternalFifoAllocateAsync(utilisation, "Manual Trigger (FIFO)");
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = allocatedNow > 0
                ? $"Successfully allocated ₹{allocatedNow:N2} across open cash withdrawals (FIFO) for {utilisation.VoucherNumber}."
                : $"No open cash withdrawal funds were available to allocate for {utilisation.VoucherNumber}.";

            return RedirectToAction(nameof(Index), new { tab = "utilisations" });
        }
        else
        {
            // Global FIFO allocation for all approved/posted utilisations that have unallocated amounts
            var pendingUtilisations = await _db.CashUtilisations
                .Include(u => u.Allocations)
                .Where(u => u.Status != "DRAFT" && u.AllocatedAmount < u.Amount)
                .OrderBy(u => u.ExpenseDate)
                .ThenBy(u => u.Id)
                .ToListAsync();

            decimal totalAllocated = 0;
            int countAllocated = 0;

            foreach (var u in pendingUtilisations)
            {
                var allocated = await InternalFifoAllocateAsync(u, "Global Auto-Allocation (FIFO)");
                if (allocated > 0)
                {
                    totalAllocated += allocated;
                    countAllocated++;
                }
            }

            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = totalAllocated > 0
                ? $"Global FIFO auto-allocation complete: Allocated ₹{totalAllocated:N2} across {countAllocated} petty cash voucher(s)."
                : "No pending unallocated vouchers or open cash withdrawal funds found.";

            return RedirectToAction(nameof(Index), new { tab = "allocations" });
        }
    }

    // POST: /PettyCash/SaveManualAllocation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveManualAllocation(long utilisationId, string allocationsJson, string? notes)
    {
        var companyId = GetTenantCompanyId();

        var utilisation = await _db.CashUtilisations
            .Include(u => u.Allocations)
            .FirstOrDefaultAsync(u => u.Id == utilisationId);

        if (utilisation == null)
        {
            TempData["ErrorMessage"] = "Petty cash voucher not found.";
            return RedirectToAction(nameof(Index), new { tab = "allocations" });
        }

        List<ManualAllocationItem>? items = null;
        try
        {
            items = System.Text.Json.JsonSerializer.Deserialize<List<ManualAllocationItem>>(allocationsJson, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Invalid allocation input: {ex.Message}";
            return RedirectToAction(nameof(Index), new { tab = "allocations" });
        }

        if (items == null || !items.Any())
        {
            TempData["ErrorMessage"] = "No allocation rows provided.";
            return RedirectToAction(nameof(Index), new { tab = "allocations" });
        }

        // 1. Remove existing allocations for this utilisation and restore withdrawal utilised amounts
        var existingAllocations = await _db.CashAllocations
            .Where(a => a.UtilisationId == utilisationId)
            .ToListAsync();

        foreach (var oldAlloc in existingAllocations)
        {
            var w = await _db.CashWithdrawals.FindAsync(oldAlloc.WithdrawalId);
            if (w != null)
            {
                w.UtilisedAmount = Math.Max(0, w.UtilisedAmount - oldAlloc.Amount);
                w.Status = w.UtilisedAmount == 0 ? "OPEN" : "PARTLY_UTILISED";
            }
            _db.CashAllocations.Remove(oldAlloc);
        }

        utilisation.AllocatedAmount = 0;

        // 2. Add new manual allocations
        decimal newTotalAllocated = 0;
        foreach (var item in items.Where(i => i.Amount > 0))
        {
            var withdrawal = await _db.CashWithdrawals.FindAsync(item.WithdrawalId);
            if (withdrawal == null) continue;

            var available = withdrawal.Amount - withdrawal.UtilisedAmount;
            if (item.Amount > available)
            {
                TempData["ErrorMessage"] = $"Allocation of ₹{item.Amount:N2} exceeds available balance ₹{available:N2} in {withdrawal.ReferenceNumber}.";
                return RedirectToAction(nameof(Index), new { tab = "allocations" });
            }

            var alloc = new CashAllocation
            {
                CompanyId = companyId,
                WithdrawalId = withdrawal.Id,
                UtilisationId = utilisation.Id,
                Amount = item.Amount,
                AllocationDate = DateTime.Today,
                AllocatedBy = "User Manual Override",
                IsManualOverride = true,
                Notes = notes
            };

            _db.CashAllocations.Add(alloc);

            withdrawal.UtilisedAmount += item.Amount;
            withdrawal.Status = withdrawal.UtilisedAmount >= withdrawal.Amount ? "FULLY_UTILISED" : "PARTLY_UTILISED";

            newTotalAllocated += item.Amount;
        }

        utilisation.AllocatedAmount = newTotalAllocated;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Manual allocation saved for {utilisation.VoucherNumber}: Total allocated ₹{newTotalAllocated:N2}.";
        return RedirectToAction(nameof(Index), new { tab = "allocations" });
    }

    // POST: /PettyCash/DeleteAllocation
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAllocation(long allocationId)
    {
        var alloc = await _db.CashAllocations
            .Include(a => a.Withdrawal)
            .Include(a => a.Utilisation)
            .FirstOrDefaultAsync(a => a.Id == allocationId);

        if (alloc == null)
        {
            TempData["ErrorMessage"] = "Allocation record not found.";
            return RedirectToAction(nameof(Index), new { tab = "allocations" });
        }

        var withdrawal = alloc.Withdrawal;
        var utilisation = alloc.Utilisation;

        if (withdrawal != null)
        {
            withdrawal.UtilisedAmount = Math.Max(0, withdrawal.UtilisedAmount - alloc.Amount);
            withdrawal.Status = withdrawal.UtilisedAmount == 0 ? "OPEN" : "PARTLY_UTILISED";
        }

        if (utilisation != null)
        {
            utilisation.AllocatedAmount = Math.Max(0, utilisation.AllocatedAmount - alloc.Amount);
        }

        _db.CashAllocations.Remove(alloc);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = "Allocation removed. Utilised cash balance restored to withdrawal.";
        return RedirectToAction(nameof(Index), new { tab = "allocations" });
    }

    // GET: /PettyCash/GetUtilisationAllocationsJson
    [HttpGet]
    public async Task<IActionResult> GetUtilisationAllocationsJson(long id)
    {
        var utilisation = await _db.CashUtilisations
            .Include(u => u.Allocations)
                .ThenInclude(a => a.Withdrawal)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

        if (utilisation == null) return NotFound(new { error = "Utilisation not found" });

        var openWithdrawals = await _db.CashWithdrawals
            .Include(w => w.BankAccount)
            .Where(w => w.Status != "CLOSED" && (w.Amount - w.UtilisedAmount) > 0)
            .OrderBy(w => w.WithdrawalDate)
            .AsNoTracking()
            .Select(w => new
            {
                w.Id,
                w.ReferenceNumber,
                w.WithdrawalDate,
                w.SourceType,
                BankName = w.BankAccount != null ? w.BankAccount.BankName : "Cash / Non-Bank",
                w.Amount,
                w.UtilisedAmount,
                AvailableAmount = w.Amount - w.UtilisedAmount,
                w.Purpose
            })
            .ToListAsync();

        var existing = utilisation.Allocations.Select(a => new
        {
            a.Id,
            a.WithdrawalId,
            WithdrawalRef = a.Withdrawal.ReferenceNumber,
            WithdrawalDate = a.Withdrawal.WithdrawalDate.ToString("yyyy-MM-dd"),
            SourceType = a.Withdrawal.SourceType,
            a.Amount,
            a.IsManualOverride,
            a.AllocatedBy
        });

        return Json(new
        {
            utilisation = new
            {
                utilisation.Id,
                utilisation.VoucherNumber,
                ExpenseDate = utilisation.ExpenseDate.ToString("yyyy-MM-dd"),
                utilisation.Amount,
                utilisation.AllocatedAmount,
                RemainingUnallocated = utilisation.Amount - utilisation.AllocatedAmount,
                utilisation.Payee,
                utilisation.ExpenseCategory,
                utilisation.Purpose
            },
            existingAllocations = existing,
            availableWithdrawals = openWithdrawals
        });
    }

    // Helper: Internal FIFO Allocation
    private async Task<decimal> InternalFifoAllocateAsync(CashUtilisation utilisation, string allocatedBy)
    {
        var needed = utilisation.Amount - utilisation.AllocatedAmount;
        if (needed <= 0) return 0;

        var availableWithdrawals = await _db.CashWithdrawals
            .Where(w => w.Status != "CLOSED" && (w.Amount - w.UtilisedAmount) > 0)
            .OrderBy(w => w.WithdrawalDate)
            .ThenBy(w => w.Id)
            .ToListAsync();

        decimal allocatedTotal = 0;

        foreach (var w in availableWithdrawals)
        {
            var capacity = w.Amount - w.UtilisedAmount;
            var allocAmount = Math.Min(needed, capacity);

            if (allocAmount > 0)
            {
                var alloc = new CashAllocation
                {
                    CompanyId = utilisation.CompanyId,
                    WithdrawalId = w.Id,
                    UtilisationId = utilisation.Id,
                    Amount = allocAmount,
                    AllocationDate = DateTime.Today,
                    AllocatedBy = allocatedBy,
                    IsManualOverride = false
                };

                _db.CashAllocations.Add(alloc);

                w.UtilisedAmount += allocAmount;
                w.Status = w.UtilisedAmount >= w.Amount ? "FULLY_UTILISED" : "PARTLY_UTILISED";

                utilisation.AllocatedAmount += allocAmount;
                needed -= allocAmount;
                allocatedTotal += allocAmount;
            }

            if (needed <= 0) break;
        }

        return allocatedTotal;
    }
}
