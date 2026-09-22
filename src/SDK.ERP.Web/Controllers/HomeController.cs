using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.ViewModels;
using SDK.ERP.Infrastructure.Data;
using SDK.ERP.Web.Models;

namespace SDK.ERP.Web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ApplicationDbContext db, ILogger<HomeController> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<IActionResult> Index(DateTime? fromDate = null, DateTime? toDate = null)
    {
        ViewData["Title"] = "Dashboard";
        ViewData["ActiveMenu"] = "Dashboard";

        var vm = new DashboardViewModel
        {
            FromDate = fromDate,
            ToDate = toDate,
            LastUpdated = DateTime.Now
        };

        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var ct = cts.Token;

            // Check if DB is reachable
            if (await _db.Database.CanConnectAsync(ct))
            {
                var invoiceQuery = _db.SalesInvoices.AsNoTracking();
                var poQuery = _db.PurchaseOrders.AsNoTracking();
                var billQuery = _db.PurchaseBills.AsNoTracking();
                var gstQuery = _db.GstTransactions.AsNoTracking();
                var projectQuery = _db.Projects.AsNoTracking();

                if (fromDate.HasValue)
                {
                    invoiceQuery = invoiceQuery.Where(x => x.InvoiceDate >= fromDate.Value);
                    poQuery = poQuery.Where(x => x.PoDate >= fromDate.Value);
                    billQuery = billQuery.Where(x => x.BillDate >= fromDate.Value);
                }

                if (toDate.HasValue)
                {
                    invoiceQuery = invoiceQuery.Where(x => x.InvoiceDate <= toDate.Value);
                    poQuery = poQuery.Where(x => x.PoDate <= toDate.Value);
                    billQuery = billQuery.Where(x => x.BillDate <= toDate.Value);
                }

                vm.TotalInvoicesCount = await invoiceQuery.CountAsync(ct);
                vm.TotalInvoicesAmount = await invoiceQuery.SumAsync(x => (decimal?)x.TotalInvoiceValue, ct) ?? 0m;
                vm.TotalSalesAmount = vm.TotalInvoicesAmount;

                vm.TotalPurchaseOrdersCount = await poQuery.CountAsync(ct);
                vm.TotalPurchaseOrdersAmount = await poQuery.SumAsync(x => (decimal?)x.TotalPoValue, ct) ?? 0m;

                vm.TotalPayableToVendors = await billQuery.Where(x => x.BalanceDue > 0).SumAsync(x => (decimal?)x.BalanceDue, ct) ?? 0m;
                vm.UnpaidBillsCount = await billQuery.CountAsync(x => x.BalanceDue > 0, ct);

                vm.TotalReceivableFromClients = await invoiceQuery.Where(x => x.OutstandingBalance > 0).SumAsync(x => (decimal?)x.OutstandingBalance, ct) ?? 0m;
                vm.UnpaidInvoicesCount = await invoiceQuery.CountAsync(x => x.OutstandingBalance > 0, ct);

                var gstList = await gstQuery.ToListAsync(ct);
                vm.CgstAmount = gstList.Sum(x => x.CgstAmount);
                vm.SgstAmount = gstList.Sum(x => x.SgstAmount);
                vm.IgstAmount = gstList.Sum(x => x.IgstAmount);
                vm.CgstTaxable = gstList.Where(x => x.CgstAmount > 0).Sum(x => x.TaxableValue);
                vm.SgstTaxable = gstList.Where(x => x.SgstAmount > 0).Sum(x => x.TaxableValue);
                vm.IgstTaxable = gstList.Where(x => x.IgstAmount > 0).Sum(x => x.TaxableValue);

                vm.ActiveProjectsCount = await projectQuery.CountAsync(ct);
                vm.TotalContractValue = await projectQuery.SumAsync(x => (decimal?)x.ContractValue, ct) ?? 0m;

                vm.LowStockItemsCount = await _db.Items.CountAsync(x => x.CurrentStockQty <= x.ReorderLevelQty && x.ReorderLevelQty > 0, ct);
                vm.PendingApprovalsCount = await _db.ApprovalRequests.CountAsync(x => x.Status == "Pending", ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Dashboard query safely defaulted to zero metrics: {Message}", ex.Message);
        }

        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

