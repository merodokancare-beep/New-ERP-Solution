using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Domain.Entities.Procurement;
using SDK.ERP.Infrastructure.Data;

namespace SDK.ERP.Web.Controllers;

public class NotificationsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICompanyContext _companyContext;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        ApplicationDbContext db,
        ICompanyContext companyContext,
        ILogger<NotificationsController> logger)
    {
        _db = db;
        _companyContext = companyContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications(CancellationToken ct)
    {
        try
        {
            var company = await _companyContext.GetCurrentCompanyAsync();
            var currentUser = await _companyContext.GetCurrentUserAsync();
            var notifications = new List<NotificationItemDto>();
            var today = DateTime.Today;

            // 1. PENDING PURCHASE ORDERS AWAITING APPROVAL
            try
            {
                var pendingPos = await _db.PurchaseOrders
                    .AsNoTracking()
                    .Include(p => p.Vendor)
                    .Where(p => p.ApprovalStatus == "PENDING" || p.ApprovalStatus == "PENDING_APPROVAL")
                    .OrderByDescending(p => p.PoDate)
                    .Take(15)
                    .ToListAsync(ct);

                foreach (var po in pendingPos)
                {
                    notifications.Add(new NotificationItemDto
                    {
                        Id = $"po-{po.Id}",
                        Type = "PO_APPROVAL",
                        Category = "APPROVAL",
                        Title = $"PO #{po.PoNumber} Awaiting Approval",
                        Description = $"Amount: ₹ {po.TotalPoValue:N2} • Vendor: {(po.Vendor?.VendorName ?? "Vendor")} • Needs managerial sign-off",
                        ActionUrl = $"/Procurement?filter=PENDING",
                        ActionLabel = "Review & Approve",
                        BadgeText = "Pending Approval",
                        BadgeColor = "purple",
                        Icon = "fa-solid fa-stamp",
                        Timestamp = po.PoDate,
                        TimeAgo = FormatTimeAgo(po.PoDate),
                        CanQuickApprove = true,
                        EntityId = po.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error querying pending POs for notifications: {Message}", ex.Message);
            }

            // 2. GENERAL APPROVAL REQUESTS (from Admin ApprovalRequest table)
            try
            {
                var pendingReqs = await _db.ApprovalRequests
                    .AsNoTracking()
                    .Include(r => r.Requester)
                    .Where(r => r.Status == "PENDING" || r.Status == "Pending")
                    .OrderByDescending(r => r.Id)
                    .Take(10)
                    .ToListAsync(ct);

                foreach (var req in pendingReqs)
                {
                    notifications.Add(new NotificationItemDto
                    {
                        Id = $"req-{req.Id}",
                        Type = "APPROVAL_REQ",
                        Category = "APPROVAL",
                        Title = $"{req.EntityType} Sign-off Required",
                        Description = $"Requested by {req.Requester?.FullName ?? "Staff"} • Action needed",
                        ActionUrl = "/Admin",
                        ActionLabel = "Review Request",
                        BadgeText = "Sign-off",
                        BadgeColor = "amber",
                        Icon = "fa-solid fa-user-check",
                        Timestamp = DateTime.UtcNow,
                        TimeAgo = "Action Required",
                        CanQuickApprove = false,
                        EntityId = req.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error querying general approval requests: {Message}", ex.Message);
            }

            // 3. OVERDUE CLIENT INVOICES (Receivables action required)
            try
            {
                var overdueInvoices = await _db.SalesInvoices
                    .AsNoTracking()
                    .Include(i => i.Client)
                    .Where(i => i.OutstandingBalance > 0 && i.DueDate < today)
                    .OrderBy(i => i.DueDate)
                    .Take(10)
                    .ToListAsync(ct);

                foreach (var inv in overdueInvoices)
                {
                    var daysOverdue = (int)(today - inv.DueDate).TotalDays;
                    notifications.Add(new NotificationItemDto
                    {
                        Id = $"inv-{inv.Id}",
                        Type = "OVERDUE_INVOICE",
                        Category = "ALERT",
                        Title = $"Invoice #{inv.InvoiceNumber} Overdue",
                        Description = $"Outstanding: ₹ {inv.OutstandingBalance:N2} • Client: {(inv.Client?.ClientName ?? "Client")} • Due: {inv.DueDate:dd MMM yyyy}",
                        ActionUrl = $"/Sales/AllocatePayment?clientId={inv.ClientId}",
                        ActionLabel = "Allocate Payment",
                        BadgeText = $"Overdue {daysOverdue}d",
                        BadgeColor = "danger",
                        Icon = "fa-solid fa-file-invoice-dollar",
                        Timestamp = inv.DueDate,
                        TimeAgo = $"{daysOverdue} days overdue",
                        CanQuickApprove = false,
                        EntityId = inv.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error querying overdue invoices: {Message}", ex.Message);
            }

            // 4. LOW STOCK / INVENTORY ALERTS
            try
            {
                var lowStockItems = await _db.Items
                    .AsNoTracking()
                    .Include(i => i.Unit)
                    .Where(i => i.IsActive && i.ReorderLevelQty > 0 && i.CurrentStockQty <= i.ReorderLevelQty)
                    .OrderBy(i => i.CurrentStockQty)
                    .Take(10)
                    .ToListAsync(ct);

                foreach (var item in lowStockItems)
                {
                    notifications.Add(new NotificationItemDto
                    {
                        Id = $"stock-{item.Id}",
                        Type = "LOW_STOCK",
                        Category = "ALERT",
                        Title = $"Low Stock: {item.ItemName}",
                        Description = $"Stock: {item.CurrentStockQty:G} {(item.Unit?.UnitName ?? "units")} (Reorder Level: {item.ReorderLevelQty:G}) • Restock recommended",
                        ActionUrl = "/Procurement/Create",
                        ActionLabel = "Raise PO",
                        BadgeText = "Low Stock",
                        BadgeColor = "warning",
                        Icon = "fa-solid fa-triangle-exclamation",
                        Timestamp = DateTime.UtcNow,
                        TimeAgo = "Reorder Level Hit",
                        CanQuickApprove = false,
                        EntityId = item.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error querying low stock items: {Message}", ex.Message);
            }

            // 5. UNALLOCATED CUSTOMER RECEIPTS (Requires Invoicing Allocation)
            try
            {
                var unallocatedReceipts = await _db.CustomerReceipts
                    .AsNoTracking()
                    .Include(r => r.Client)
                    .Where(r => r.UnallocatedAmount > 0 && !r.IsAdvance)
                    .OrderByDescending(r => r.ReceiptDate)
                    .Take(10)
                    .ToListAsync(ct);

                foreach (var rcpt in unallocatedReceipts)
                {
                    notifications.Add(new NotificationItemDto
                    {
                        Id = $"rcpt-{rcpt.Id}",
                        Type = "UNALLOCATED_RECEIPT",
                        Category = "ALERT",
                        Title = $"Receipt #{rcpt.ReceiptNumber} Unallocated",
                        Description = $"Unallocated Amount: ₹ {rcpt.UnallocatedAmount:N2} • Client: {(rcpt.Client?.ClientName ?? "Client")} • Link to open invoices",
                        ActionUrl = $"/Sales/AllocatePayment?clientId={rcpt.ClientId}",
                        ActionLabel = "Allocate Now",
                        BadgeText = "Unallocated",
                        BadgeColor = "info",
                        Icon = "fa-solid fa-coins",
                        Timestamp = rcpt.ReceiptDate,
                        TimeAgo = FormatTimeAgo(rcpt.ReceiptDate),
                        CanQuickApprove = false,
                        EntityId = rcpt.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error querying unallocated receipts: {Message}", ex.Message);
            }

            // 6. OVERDUE VENDOR BILLS (Payables Attention)
            try
            {
                var overdueBills = await _db.PurchaseBills
                    .AsNoTracking()
                    .Include(b => b.Vendor)
                    .Where(b => b.BalanceDue > 0 && b.DueDate < today)
                    .OrderBy(b => b.DueDate)
                    .Take(10)
                    .ToListAsync(ct);

                foreach (var bill in overdueBills)
                {
                    var daysOverdue = (int)(today - bill.DueDate).TotalDays;
                    notifications.Add(new NotificationItemDto
                    {
                        Id = $"bill-{bill.Id}",
                        Type = "OVERDUE_BILL",
                        Category = "ALERT",
                        Title = $"Vendor Bill #{bill.VendorBillNumber} Due",
                        Description = $"Payable Due: ₹ {bill.BalanceDue:N2} • Vendor: {(bill.Vendor?.VendorName ?? "Vendor")} • Due: {bill.DueDate:dd MMM yyyy}",
                        ActionUrl = "/Procurement",
                        ActionLabel = "View Bill",
                        BadgeText = $"Overdue {daysOverdue}d",
                        BadgeColor = "secondary",
                        Icon = "fa-solid fa-money-bill-wave",
                        Timestamp = bill.DueDate,
                        TimeAgo = $"{daysOverdue} days overdue",
                        CanQuickApprove = false,
                        EntityId = bill.Id
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error querying overdue bills: {Message}", ex.Message);
            }

            var approvalsCount = notifications.Count(n => n.Category == "APPROVAL");
            var alertsCount = notifications.Count(n => n.Category == "ALERT");

            return Json(new
            {
                success = true,
                totalCount = notifications.Count,
                approvalsCount = approvalsCount,
                alertsCount = alertsCount,
                items = notifications
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load notifications");
            return Json(new { success = false, message = ex.Message, totalCount = 0, items = new List<NotificationItemDto>() });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuickApprovePo([FromForm] long id)
    {
        try
        {
            var po = await _db.PurchaseOrders.FindAsync(id);
            if (po == null)
            {
                return Json(new { success = false, message = "Purchase Order not found." });
            }

            var currentUser = await _companyContext.GetCurrentUserAsync();
            po.ApprovalStatus = "APPROVED";
            po.ApprovedBy = currentUser.Id;
            po.ApprovedAt = DateTime.UtcNow;
            po.RejectionReason = null;

            await _db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Purchase Order <strong>{po.PoNumber}</strong> approved successfully!"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during QuickApprovePo");
            return Json(new { success = false, message = $"Failed to approve PO: {ex.Message}" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> QuickRejectPo([FromForm] long id, [FromForm] string? reason)
    {
        try
        {
            var po = await _db.PurchaseOrders.FindAsync(id);
            if (po == null)
            {
                return Json(new { success = false, message = "Purchase Order not found." });
            }

            po.ApprovalStatus = "REJECTED";
            po.RejectionReason = string.IsNullOrWhiteSpace(reason) ? "Rejected from Notification Center." : reason.Trim();

            await _db.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Purchase Order <strong>{po.PoNumber}</strong> was marked as rejected."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during QuickRejectPo");
            return Json(new { success = false, message = $"Failed to reject PO: {ex.Message}" });
        }
    }

    private static string FormatTimeAgo(DateTime dt)
    {
        var diff = DateTime.UtcNow - (dt.Kind == DateTimeKind.Utc ? dt : dt.ToUniversalTime());
        if (diff.TotalDays >= 7) return $"{dt:dd MMM yyyy}";
        if (diff.TotalDays >= 2) return $"{(int)diff.TotalDays} days ago";
        if (diff.TotalDays >= 1) return "Yesterday";
        if (diff.TotalHours >= 2) return $"{(int)diff.TotalHours} hours ago";
        if (diff.TotalMinutes >= 1) return $"{(int)diff.TotalMinutes} mins ago";
        return "Just now";
    }
}

public class NotificationItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ActionUrl { get; set; } = string.Empty;
    public string ActionLabel { get; set; } = string.Empty;
    public string BadgeText { get; set; } = string.Empty;
    public string BadgeColor { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public bool CanQuickApprove { get; set; }
    public long? EntityId { get; set; }
}
