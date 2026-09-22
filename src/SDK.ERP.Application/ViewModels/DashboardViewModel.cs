namespace SDK.ERP.Application.ViewModels;

public class DashboardViewModel
{
    // Date Range Filters
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string DateRangeDisplay { get; set; } = "All Time";
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    // Quick Summary Chips
    public decimal TotalSalesAmount { get; set; }
    public decimal NetMarginPercentage { get; set; }
    public int PendingGstReturnsCount { get; set; }

    // Primary Metric Cards
    public decimal TotalInvoicesAmount { get; set; }
    public int TotalInvoicesCount { get; set; }
    
    public decimal TotalPurchaseOrdersAmount { get; set; }
    public int TotalPurchaseOrdersCount { get; set; }

    public decimal TotalPayableToVendors { get; set; }
    public int UnpaidBillsCount { get; set; }

    public decimal TotalReceivableFromClients { get; set; }
    public int UnpaidInvoicesCount { get; set; }

    // GST Annual Breakdown
    public decimal TotalGstAmount => CgstAmount + SgstAmount + IgstAmount;
    public decimal CgstTaxable { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstTaxable { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstTaxable { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal TotalTaxableValue => CgstTaxable + SgstTaxable + IgstTaxable;

    // GST Monthly Summary List
    public List<MonthlyGstSummaryItem> MonthlyGstSummaries { get; set; } = new();

    // Future Provision 1: Financial Performance KPIs
    public decimal GrossMarginPercentage { get; set; }
    public decimal GrossMarginAmount { get; set; }
    public decimal NetMarginAmount { get; set; }
    public decimal CurrentRatio { get; set; }

    // Future Provision 2: Project KPIs
    public int ActiveProjectsCount { get; set; }
    public decimal TotalContractValue { get; set; }
    public int ProjectsOnTrack { get; set; }
    public int ProjectsReviewDue { get; set; }
    public int ProjectsOverdue { get; set; }

    // Future Provision 3: Receivables Aging Distribution
    public decimal ArUnder30Days { get; set; }
    public int ArUnder30Count { get; set; }
    public decimal Ar31To60Days { get; set; }
    public int Ar31To60Count { get; set; }
    public decimal Ar61To90Days { get; set; }
    public int Ar61To90Count { get; set; }
    public decimal ArAbove90Days { get; set; }
    public int ArAbove90Count { get; set; }

    // Future Provision 4: Payables & MSME Compliance
    public decimal MsmePayableAmount { get; set; }
    public decimal GeneralPayableAmount { get; set; }

    // Future Provision 5: Cash Flow Trajectory
    public decimal MonthInflows { get; set; }
    public decimal MonthOutflows { get; set; }
    public decimal NetLiquidity => MonthInflows - MonthOutflows;

    // Future Provision 6: Stock & Consumables
    public int TotalConsumablesCount { get; set; }
    public int LowStockItemsCount { get; set; }
    public List<StockAlertItem> LowStockAlerts { get; set; } = new();

    // Future Provision 7: Payroll & HR
    public decimal MonthlyPayrollOutflow { get; set; }
    public decimal EpfEsicLiability { get; set; }
    public int ActiveEmployeesCount { get; set; }

    // Future Provision 8: Alerts & Notifications
    public int PendingApprovalsCount { get; set; }
    public List<string> PendingAlerts { get; set; } = new();
}

public class MonthlyGstSummaryItem
{
    public string MonthName { get; set; } = string.Empty;
    public decimal OutputTax { get; set; }
    public decimal InputTax { get; set; }
    public decimal NetGstPayable => OutputTax - InputTax;
    public decimal GstPaid { get; set; }
}

public class StockAlertItem
{
    public string ItemName { get; set; } = string.Empty;
    public decimal CurrentStock { get; set; }
    public decimal MinThreshold { get; set; }
    public string Unit { get; set; } = string.Empty;
}
