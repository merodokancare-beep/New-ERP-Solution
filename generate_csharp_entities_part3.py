import os

base_path = "src/SDK.ERP.Domain/Entities"

def create_directory(path):
    if not os.path.exists(path):
        os.makedirs(path)

# Domain 5: Procurement & Payables
create_directory(f"{base_path}/Procurement")

with open(f"{base_path}/Procurement/PurchaseRequisition.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseRequisition
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string PrNumber { get; set; } = string.Empty;
    public long RequestedByUserId { get; set; }
    public int? DepartmentId { get; set; }
    public long? ProjectId { get; set; }
    public string PurposeType { get; set; } = "OFFICE_USE";
    public DateTime RequiredByDate { get; set; }
    public string Status { get; set; } = "DRAFT";

    public Company Company { get; set; } = null!;
    public User RequestedByUser { get; set; } = null!;
    public Project? Project { get; set; }

    public ICollection<PurchaseRequisitionItem> Items { get; set; } = new List<PurchaseRequisitionItem>();
}
''')

with open(f"{base_path}/Procurement/PurchaseRequisitionItem.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseRequisitionItem
{
    public long Id { get; set; }
    public long PrId { get; set; }
    public long? ItemId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public decimal RequestedQty { get; set; }
    public int UnitId { get; set; }
    public decimal? EstimatedCost { get; set; }

    public PurchaseRequisition Requisition { get; set; } = null!;
    public Item? Item { get; set; }
    public ItemUnit Unit { get; set; } = null!;
}
''')

with open(f"{base_path}/Procurement/PurchaseOrder.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseOrder
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long VendorId { get; set; }
    public long? ProjectId { get; set; }
    public long? PrId { get; set; }
    public string PoNumber { get; set; } = string.Empty;
    public DateTime PoDate { get; set; }
    public DateTime? DeliveryDueDate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal GstAmount { get; set; }
    public decimal TotalPoValue { get; set; }
    public string ApprovalStatus { get; set; } = "DRAFT";
    public long? ApprovedBy { get; set; }

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;
    public Project? Project { get; set; }
    public PurchaseRequisition? Requisition { get; set; }
    public User? Approver { get; set; }

    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    public ICollection<GoodsReceiptNote> GoodsReceiptNotes { get; set; } = new List<GoodsReceiptNote>();
}
''')

with open(f"{base_path}/Procurement/PurchaseOrderItem.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseOrderItem
{
    public long Id { get; set; }
    public long PoId { get; set; }
    public long? ItemId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal OrderedQty { get; set; }
    public decimal ReceivedQty { get; set; }
    public int UnitId { get; set; }
    public decimal UnitRate { get; set; }
    public int TaxRateId { get; set; }
    public decimal LineTotal { get; set; }

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public Item? Item { get; set; }
    public ItemUnit Unit { get; set; } = null!;
    public TaxRate TaxRate { get; set; } = null!;
}
''')

with open(f"{base_path}/Procurement/GoodsReceiptNote.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Procurement;

public class GoodsReceiptNote
{
    public long Id { get; set; }
    public long PoId { get; set; }
    public string GrnNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string? VendorDcNumber { get; set; }
    public long ReceivedByUserId { get; set; }
    public string InspectionStatus { get; set; } = "ACCEPTED";

    public PurchaseOrder PurchaseOrder { get; set; } = null!;
    public User ReceivedByUser { get; set; } = null!;

    public ICollection<GrnItem> Items { get; set; } = new List<GrnItem>();
}
''')

with open(f"{base_path}/Procurement/GrnItem.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class GrnItem
{
    public long Id { get; set; }
    public long GrnId { get; set; }
    public long PoItemId { get; set; }
    public long? ItemId { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AcceptedQty { get; set; }
    public decimal RejectedQty { get; set; }
    public string? RejectionReason { get; set; }

    public GoodsReceiptNote GoodsReceiptNote { get; set; } = null!;
    public PurchaseOrderItem PoItem { get; set; } = null!;
    public Item? Item { get; set; }
}
''')

with open(f"{base_path}/Procurement/PurchaseBill.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseBill
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long VendorId { get; set; }
    public long? PoId { get; set; }
    public long? ProjectId { get; set; }
    public string VendorBillNumber { get; set; } = string.Empty;
    public DateTime BillDate { get; set; }
    public DateTime DueDate { get; set; }
    public string PlaceOfSupply { get; set; } = string.Empty;
    public decimal TaxableAmount { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal TotalBillAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public string Status { get; set; } = "UNPAID";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;
    public PurchaseOrder? PurchaseOrder { get; set; }
    public Project? Project { get; set; }

    public ICollection<PurchaseBillItem> Items { get; set; } = new List<PurchaseBillItem>();
    public ICollection<VendorPaymentAllocation> Allocations { get; set; } = new List<VendorPaymentAllocation>();
}
''')

with open(f"{base_path}/Procurement/PurchaseBillItem.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class PurchaseBillItem
{
    public long Id { get; set; }
    public long PurchaseBillId { get; set; }
    public long ExpenseAccountId { get; set; }
    public long? ItemId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal TaxableAmount { get; set; }
    public int TaxRateId { get; set; }
    public decimal GstAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public PurchaseBill PurchaseBill { get; set; } = null!;
    public Item? Item { get; set; }
    public TaxRate TaxRate { get; set; } = null!;
}
''')

with open(f"{base_path}/Procurement/VendorPayment.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Procurement;

public class VendorPayment
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long VendorId { get; set; }
    public long BankAccountId { get; set; }
    public string PaymentVoucherNo { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public decimal TotalAmountPaid { get; set; }
    public string PaymentMode { get; set; } = "NEFT";
    public string? ChequeUtrNo { get; set; }
    public string Status { get; set; } = "PROCESSED";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;

    public ICollection<VendorPaymentAllocation> Allocations { get; set; } = new List<VendorPaymentAllocation>();
}
''')

with open(f"{base_path}/Procurement/VendorPaymentAllocation.cs", "w", encoding="utf-8") as f:
    f.write('''namespace SDK.ERP.Domain.Entities.Procurement;

public class VendorPaymentAllocation
{
    public long Id { get; set; }
    public long VendorPaymentId { get; set; }
    public long PurchaseBillId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal TdsDeducted { get; set; }

    public VendorPayment Payment { get; set; } = null!;
    public PurchaseBill PurchaseBill { get; set; } = null!;
}
''')

# Domain 6: Office Inventory
create_directory(f"{base_path}/Inventory")

with open(f"{base_path}/Inventory/StockTransaction.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Inventory;

public class StockTransaction
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ItemId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public long ReferenceId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal BalanceQtyAfter { get; set; }

    public Company Company { get; set; } = null!;
    public Item Item { get; set; } = null!;
}
''')

with open(f"{base_path}/Inventory/StockIssue.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Inventory;

public class StockIssue
{
    public long Id { get; set; }
    public string IssueVoucherNo { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public long ItemId { get; set; }
    public decimal QuantityIssued { get; set; }
    public long? IssuedToEmployeeId { get; set; }
    public int? DepartmentId { get; set; }
    public string PurposeRemarks { get; set; } = "OFFICE_USE";
    public long ApprovedBy { get; set; }

    public Item Item { get; set; } = null!;
    public User Approver { get; set; } = null!;
}
''')

# Domain 7: Accounting
create_directory(f"{base_path}/Accounting")

with open(f"{base_path}/Accounting/FinancialYear.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Accounting;

public class FinancialYear
{
    public int Id { get; set; }
    public long CompanyId { get; set; }
    public string FyCode { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsClosed { get; set; }
    public DateTime? ClosedAt { get; set; }
    public long? ClosedBy { get; set; }

    public Company Company { get; set; } = null!;
}
''')

with open(f"{base_path}/Accounting/AccountGroup.cs", "w", encoding="utf-8") as f:
    f.write('''namespace SDK.ERP.Domain.Entities.Accounting;

public class AccountGroup
{
    public int Id { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string AccountCategory { get; set; } = string.Empty;
    public int? ParentGroupId { get; set; }

    public AccountGroup? ParentGroup { get; set; }
    public ICollection<AccountGroup> SubGroups { get; set; } = new List<AccountGroup>();
    public ICollection<ChartOfAccount> Accounts { get; set; } = new List<ChartOfAccount>();
}
''')

with open(f"{base_path}/Accounting/ChartOfAccount.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Accounting;

public class ChartOfAccount
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public decimal OpeningBalance { get; set; }
    public string OpeningBalanceType { get; set; } = "DR";
    public decimal CurrentBalance { get; set; }
    public bool IsSystemAccount { get; set; }
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public AccountGroup Group { get; set; } = null!;
}
''')

with open(f"{base_path}/Accounting/JournalEntry.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Accounting;

public class JournalEntry
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public int FyId { get; set; }
    public string VoucherNo { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string? SourceEntityType { get; set; }
    public long? SourceEntityId { get; set; }
    public long? ProjectId { get; set; }
    public string Narration { get; set; } = string.Empty;
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public bool IsBalanced { get; set; } = true;
    public bool IsReversal { get; set; }
    public long CreatedBy { get; set; }

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public FinancialYear FinancialYear { get; set; } = null!;
    public Project? Project { get; set; }
    public User Creator { get; set; } = null!;

    public ICollection<JournalLine> Lines { get; set; } = new List<JournalLine>();
}
''')

with open(f"{base_path}/Accounting/JournalLine.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Accounting;

public class JournalLine
{
    public long Id { get; set; }
    public long JournalEntryId { get; set; }
    public long AccountId { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public long? ClientId { get; set; }
    public long? VendorId { get; set; }
    public string? LineNarration { get; set; }

    public JournalEntry JournalEntry { get; set; } = null!;
    public ChartOfAccount Account { get; set; } = null!;
    public Client? Client { get; set; }
    public Vendor? Vendor { get; set; }
}
''')

# Domain 8: Tax
create_directory(f"{base_path}/Tax")

with open(f"{base_path}/Tax/GstTransaction.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Domain.Entities.Tax;

public class GstTransaction
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long VoucherId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string? PartyGstin { get; set; }
    public string PlaceOfSupply { get; set; } = string.Empty;
    public string? HsnSacCode { get; set; }
    public decimal TaxableValue { get; set; }
    public decimal TaxRatePercentage { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public string ReturnPeriod { get; set; } = string.Empty;
    public bool IsFiled { get; set; }

    public Company Company { get; set; } = null!;
    public JournalEntry Voucher { get; set; } = null!;
}
''')

with open(f"{base_path}/Tax/GstReconciliation.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Procurement;

namespace SDK.ERP.Domain.Entities.Tax;

public class GstReconciliation
{
    public long Id { get; set; }
    public string FinancialPeriod { get; set; } = string.Empty;
    public string PortalGstr2bInvoiceNo { get; set; } = string.Empty;
    public string PortalVendorGstin { get; set; } = string.Empty;
    public decimal PortalTaxableValue { get; set; }
    public decimal PortalTaxAmount { get; set; }
    public long? InternalPurchaseBillId { get; set; }
    public decimal? InternalTaxableValue { get; set; }
    public string MatchStatus { get; set; } = string.Empty;
    public string ItcEligibility { get; set; } = "ELIGIBLE";
    public long? ReconciledBy { get; set; }

    public PurchaseBill? InternalPurchaseBill { get; set; }
    public User? Reconciler { get; set; }
}
''')

# Domain 9: Banking
create_directory(f"{base_path}/Banking")

with open(f"{base_path}/Banking/BankAccount.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Domain.Entities.Banking;

public class BankAccount
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long GlAccountId { get; set; }
    public string BankName { get; set; } = string.Empty;
    public string? BranchName { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string IfscCode { get; set; } = string.Empty;
    public string AccountType { get; set; } = "CURRENT";
    public decimal BookBalance { get; set; }
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ChartOfAccount GlAccount { get; set; } = null!;
}
''')

with open(f"{base_path}/Banking/BankTransaction.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Domain.Entities.Banking;

public class BankTransaction
{
    public long Id { get; set; }
    public long BankAccountId { get; set; }
    public long? VoucherId { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime? ValueDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ReferenceNumber { get; set; }
    public bool IsReconciled { get; set; }

    public BankAccount BankAccount { get; set; } = null!;
    public JournalEntry? Voucher { get; set; }
}
''')

with open(f"{base_path}/Banking/BankReconciliation.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Banking;

public class BankReconciliation
{
    public long Id { get; set; }
    public long BankAccountId { get; set; }
    public DateTime StatementDate { get; set; }
    public decimal ClosingBookBalance { get; set; }
    public decimal StatementBalance { get; set; }
    public decimal UnreconciledDifference { get; set; }
    public bool IsReconciled { get; set; }
    public long ReconciledBy { get; set; }

    public BankAccount BankAccount { get; set; } = null!;
    public User Reconciler { get; set; } = null!;
}
''')

with open(f"{base_path}/Banking/PettyCashTransaction.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Banking;

public class PettyCashTransaction
{
    public long Id { get; set; }
    public long BankAccountId { get; set; }
    public long CustodianUserId { get; set; }
    public DateTime EntryDate { get; set; }
    public long ExpenseAccountId { get; set; }
    public long? ProjectId { get; set; }
    public decimal Amount { get; set; }
    public string PaidTo { get; set; } = string.Empty;
    public long? ReceiptDocId { get; set; }
    public string ApprovalStatus { get; set; } = "APPROVED";

    public BankAccount BankAccount { get; set; } = null!;
    public User CustodianUser { get; set; } = null!;
    public ChartOfAccount ExpenseAccount { get; set; } = null!;
    public Project? Project { get; set; }
    public DocumentAttachment? ReceiptDoc { get; set; }
}
''')

# Domain 10: Payroll
create_directory(f"{base_path}/Payroll")

with open(f"{base_path}/Payroll/Employee.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Payroll;

public class Employee
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public string EmployeeCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string DesignationTitle { get; set; } = string.Empty;
    public long? ReportingManagerId { get; set; }
    public DateTime DateOfJoining { get; set; }
    public string Pan { get; set; } = string.Empty;
    public string? Uan { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankIfsc { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Employee? ReportingManager { get; set; }

    public ICollection<SalaryStructure> SalaryStructures { get; set; } = new List<SalaryStructure>();
    public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
}
''')

with open(f"{base_path}/Payroll/SalaryStructure.cs", "w", encoding="utf-8") as f:
    f.write('''namespace SDK.ERP.Domain.Entities.Payroll;

public class SalaryStructure
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public decimal CtcAnnual { get; set; }
    public decimal GrossMonthly { get; set; }
    public decimal BasicPay { get; set; }
    public decimal Hra { get; set; }
    public decimal SpecialAllowance { get; set; }
    public decimal PfEmployee { get; set; }
    public decimal ProfessionalTax { get; set; }
    public bool IsActive { get; set; } = true;

    public Employee Employee { get; set; } = null!;
}
''')

with open(f"{base_path}/Payroll/PayrollRun.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Payroll;

public class PayrollRun
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string MonthYear { get; set; } = string.Empty;
    public int TotalEmployeesProcessed { get; set; }
    public decimal TotalGrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetPayable { get; set; }
    public string Status { get; set; } = "DRAFT";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public ICollection<PayrollItem> Items { get; set; } = new List<PayrollItem>();
}
''')

with open(f"{base_path}/Payroll/PayrollItem.cs", "w", encoding="utf-8") as f:
    f.write('''namespace SDK.ERP.Domain.Entities.Payroll;

public class PayrollItem
{
    public long Id { get; set; }
    public long PayrollRunId { get; set; }
    public long EmployeeId { get; set; }
    public int WorkingDaysInMonth { get; set; } = 30;
    public decimal DaysPayable { get; set; } = 30.0m;
    public decimal GrossEarned { get; set; }
    public decimal PfDeduction { get; set; }
    public decimal PtDeduction { get; set; }
    public decimal TdsDeduction { get; set; }
    public decimal AdvanceDeduction { get; set; }
    public decimal NetSalary { get; set; }
    public string PaymentStatus { get; set; } = "UNPAID";

    public PayrollRun PayrollRun { get; set; } = null!;
    public Employee Employee { get; set; } = null!;
}
''')

with open(f"{base_path}/Payroll/EmployeeAdvance.cs", "w", encoding="utf-8") as f:
    f.write('''namespace SDK.ERP.Domain.Entities.Payroll;

public class EmployeeAdvance
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public decimal AdvanceAmount { get; set; }
    public DateTime DisbursementDate { get; set; }
    public decimal MonthlyRecoveryAmount { get; set; }
    public decimal RecoveredAmount { get; set; }
    public decimal BalanceDue { get; set; }
    public string Status { get; set; } = "ACTIVE";

    public Employee Employee { get; set; } = null!;
}
''')

with open(f"{base_path}/Payroll/LeaveApplication.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Payroll;

public class LeaveApplication
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string LeaveType { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = "PENDING";
    public long? ApprovedBy { get; set; }

    public Employee Employee { get; set; } = null!;
    public User? Approver { get; set; }
}
''')

# Domain 11: Assets & Support
create_directory(f"{base_path}/AssetsAndSupport")

with open(f"{base_path}/AssetsAndSupport/FixedAsset.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Procurement;

namespace SDK.ERP.Domain.Entities.AssetsAndSupport;

public class FixedAsset
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public string AssetTagCode { get; set; } = string.Empty;
    public string AssetName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SerialNumber { get; set; }
    public long? PurchaseBillId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal PurchaseCost { get; set; }
    public decimal CurrentBookValue { get; set; }
    public decimal DepreciationRate { get; set; } = 40.00m;
    public string Status { get; set; } = "AVAILABLE";

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public PurchaseBill? PurchaseBill { get; set; }

    public ICollection<AssetAllocation> Allocations { get; set; } = new List<AssetAllocation>();
}
''')

with open(f"{base_path}/AssetsAndSupport/AssetAllocation.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.Payroll;

namespace SDK.ERP.Domain.Entities.AssetsAndSupport;

public class AssetAllocation
{
    public long Id { get; set; }
    public long AssetId { get; set; }
    public long AllocatedToEmployeeId { get; set; }
    public DateTime AllocatedDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string? HandoverCondition { get; set; }
    public long AllocatedBy { get; set; }

    public FixedAsset Asset { get; set; } = null!;
    public Employee AllocatedToEmployee { get; set; } = null!;
    public User AllocatedByUser { get; set; } = null!;
}
''')

with open(f"{base_path}/AssetsAndSupport/SupportTicket.cs", "w", encoding="utf-8") as f:
    f.write('''using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.AssetsAndSupport;

public class SupportTicket
{
    public long Id { get; set; }
    public string TicketNumber { get; set; } = string.Empty;
    public long RaisedByUserId { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Priority { get; set; } = "MEDIUM";
    public string Subject { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long? AssignedToUserId { get; set; }
    public string Status { get; set; } = "OPEN";
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public User RaisedByUser { get; set; } = null!;
    public User? AssignedToUser { get; set; }
}
''')

print("Generated Procurement, Inventory, Accounting, Tax, Banking, Payroll, and Assets entities.")
