import os

def create_directory(path):
    if not os.path.exists(path):
        os.makedirs(path)

base_path = "src/SDK.ERP.Domain/Entities"

# Domain 1: Admin & Security
create_directory(f"{base_path}/Admin")

with open(f"{base_path}/Admin/Company.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class Company
{
    public long Id { get; set; }
    public string CompanyCode { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string? Gstin { get; set; }
    public string Pan { get; set; } = string.Empty;
    public string? Tan { get; set; }
    public string BaseCurrency { get; set; } = "INR";
    public DateTime FinancialYearStart { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
""")

with open(f"{base_path}/Admin/Branch.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class Branch
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string StateCode { get; set; } = string.Empty;
    public string? Gstin { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public bool IsHeadOffice { get; set; }
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ICollection<User> Users { get; set; } = new List<User>();
}
""")

with open(f"{base_path}/Admin/Role.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class Role
{
    public int Id { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}
""")

with open(f"{base_path}/Admin/User.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class User
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public int RoleId { get; set; }
    public long? EmployeeId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public long? ReportingManagerId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public User? ReportingManager { get; set; }
}
""")

with open(f"{base_path}/Admin/RolePermission.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class RolePermission
{
    public long Id { get; set; }
    public int RoleId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string SubmoduleCode { get; set; } = string.Empty;
    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
    public bool CanApprove { get; set; }
    public bool CanPost { get; set; }
    public bool CanExport { get; set; }

    public Role Role { get; set; } = null!;
}
""")

with open(f"{base_path}/Admin/AuditLog.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class AuditLog
{
    public long Id { get; set; }
    public long? UserId { get; set; }
    public string TableName { get; set; } = string.Empty;
    public long RecordId { get; set; }
    public string ActionType { get; set; } = string.Empty;
    public string? OldPayload { get; set; }
    public string? NewPayload { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? PreviousHash { get; set; }
    public string? CurrentHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}
""")

with open(f"{base_path}/Admin/ApprovalRule.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class ApprovalRule
{
    public long Id { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public int ApproverRoleId { get; set; }
    public int LevelOrder { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public Role ApproverRole { get; set; } = null!;
}
""")

with open(f"{base_path}/Admin/ApprovalRequest.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class ApprovalRequest
{
    public long Id { get; set; }
    public long RuleId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public long RequestedBy { get; set; }
    public int AssignedToRoleId { get; set; }
    public string Status { get; set; } = "PENDING";
    public long? ActionBy { get; set; }
    public DateTime? ActionAt { get; set; }
    public string? Comments { get; set; }

    public ApprovalRule Rule { get; set; } = null!;
    public User Requester { get; set; } = null!;
    public Role AssignedRole { get; set; } = null!;
    public User? ActionUser { get; set; }
}
""")

with open(f"{base_path}/Admin/DocumentAttachment.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Admin;

public class DocumentAttachment
{
    public long Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public long EntityId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string MimeType { get; set; } = string.Empty;
    public string FileHashSha256 { get; set; } = string.Empty;
    public long UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public int VersionNumber { get; set; } = 1;

    public User Uploader { get; set; } = null!;
}
""")

# Domain 2: Master Data
create_directory(f"{base_path}/MasterData")

with open(f"{base_path}/MasterData/Client.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.MasterData;

public class Client
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string ClientCode { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string BillingAddress { get; set; } = string.Empty;
    public string? ShippingAddress { get; set; }
    public string StateCode { get; set; } = string.Empty;
    public string? Gstin { get; set; }
    public string? Pan { get; set; }
    public decimal CreditLimit { get; set; }
    public int CreditDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
}
""")

with open(f"{base_path}/MasterData/Vendor.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.MasterData;

public class Vendor
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string VendorCode { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string VendorCategory { get; set; } = "GENERAL";
    public string? Gstin { get; set; }
    public string Pan { get; set; } = string.Empty;
    public string? MsmeType { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNo { get; set; }
    public string? IfscCode { get; set; }
    public int PaymentTermsDays { get; set; } = 30;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
}
""")

with open(f"{base_path}/MasterData/ItemCategory.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.MasterData;

public class ItemCategory
{
    public int Id { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    public ItemCategory? ParentCategory { get; set; }
    public ICollection<ItemCategory> SubCategories { get; set; } = new List<ItemCategory>();
}
""")

with open(f"{base_path}/MasterData/ItemUnit.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.MasterData;

public class ItemUnit
{
    public int Id { get; set; }
    public string UnitCode { get; set; } = string.Empty;
    public string UnitName { get; set; } = string.Empty;
    public bool IsDecimalAllowed { get; set; }
}
""")

with open(f"{base_path}/MasterData/TaxRate.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.MasterData;

public class TaxRate
{
    public int Id { get; set; }
    public string TaxName { get; set; } = string.Empty;
    public decimal RatePercentage { get; set; }
    public decimal CgstPercentage { get; set; }
    public decimal SgstPercentage { get; set; }
    public decimal IgstPercentage { get; set; }
    public bool IsActive { get; set; } = true;
}
""")

with open(f"{base_path}/MasterData/HsnSacCode.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.MasterData;

public class HsnSacCode
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "GOODS";
    public int? DefaultTaxRateId { get; set; }

    public TaxRate? DefaultTaxRate { get; set; }
}
""")

with open(f"{base_path}/MasterData/Item.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.MasterData;

public class Item
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int UnitId { get; set; }
    public string? HsnSacCode { get; set; }
    public int TaxRateId { get; set; }
    public decimal UnitCost { get; set; }
    public decimal CurrentStockQty { get; set; }
    public decimal ReorderLevelQty { get; set; }
    public bool IsDurableAsset { get; set; } = false;
    public bool IsActive { get; set; } = true;

    public Company Company { get; set; } = null!;
    public ItemCategory Category { get; set; } = null!;
    public ItemUnit Unit { get; set; } = null!;
    public TaxRate TaxRate { get; set; } = null!;
}
""")

# Domain 3: Projects
create_directory(f"{base_path}/Projects")

with open(f"{base_path}/Projects/Project.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Projects;

public class Project
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long ClientId { get; set; }
    public string ProjectCode { get; set; } = string.Empty;
    public string ProjectName { get; set; } = string.Empty;
    public string ProjectType { get; set; } = "STANDARD";
    public long ManagerId { get; set; }
    public decimal ContractValue { get; set; }
    public decimal BudgetCost { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? ExpectedEndDate { get; set; }
    public DateTime? ActualClosedDate { get; set; }
    public string Status { get; set; } = "PIPELINE";
    public long? ClosureApprovedBy { get; set; }
    public string? ClosureNotes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public User Manager { get; set; } = null!;
    public User? ClosureApprover { get; set; }

    public ICollection<ProjectPo> PurchaseOrders { get; set; } = new List<ProjectPo>();
    public ICollection<ProjectMilestone> Milestones { get; set; } = new List<ProjectMilestone>();
    public ICollection<ProjectExpense> Expenses { get; set; } = new List<ProjectExpense>();
    public ICollection<ProjectDelivery> Deliveries { get; set; } = new List<ProjectDelivery>();
}
""")

with open(f"{base_path}/Projects/ProjectPo.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectPo
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string ClientPoNumber { get; set; } = string.Empty;
    public DateTime PoDate { get; set; }
    public decimal PoValue { get; set; }
    public DateTime? ValidityEndDate { get; set; }
    public string? ScopeOfWork { get; set; }
    public long? AttachmentDocId { get; set; }

    public Project Project { get; set; } = null!;
    public DocumentAttachment? Attachment { get; set; }
}
""")

with open(f"{base_path}/Projects/ProjectMilestone.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectMilestone
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string MilestoneName { get; set; } = string.Empty;
    public DateTime ExpectedDate { get; set; }
    public decimal MilestoneAmount { get; set; }
    public decimal? PercentageOfContract { get; set; }
    public string Status { get; set; } = "PENDING";
    public DateTime? CompletionDate { get; set; }

    public Project Project { get; set; } = null!;
}
""")

with open(f"{base_path}/Projects/ProjectExpense.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;

namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectExpense
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public long ExpenseHeadId { get; set; }
    public long IncurredByUserId { get; set; }
    public DateTime ExpenseDate { get; set; }
    public decimal Amount { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal GstAmount { get; set; }
    public string PaymentMode { get; set; } = "REIMBURSEMENT";
    public string Status { get; set; } = "SUBMITTED";
    public long? ApprovedBy { get; set; }
    public long? ReceiptDocId { get; set; }

    public Project Project { get; set; } = null!;
    public User IncurredByUser { get; set; } = null!;
    public User? Approver { get; set; }
    public DocumentAttachment? ReceiptDoc { get; set; }
}
""")

with open(f"{base_path}/Projects/ProjectDelivery.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Projects;

public class ProjectDelivery
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string DcNumber { get; set; } = string.Empty;
    public DateTime DeliveryDate { get; set; }
    public string? DispatchMode { get; set; }
    public string? TrackingRefNo { get; set; }
    public string? RecipientName { get; set; }
    public string Status { get; set; } = "DISPATCHED";

    public Project Project { get; set; } = null!;
}
""")

# Domain 4: Sales & Billing
create_directory(f"{base_path}/Sales")

with open(f"{base_path}/Sales/Quotation.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Sales;

public class Quotation
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ClientId { get; set; }
    public long? ProjectId { get; set; }
    public string QuoteNumber { get; set; } = string.Empty;
    public DateTime QuoteDate { get; set; }
    public DateTime ValidityDate { get; set; }
    public decimal SubtotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public string Status { get; set; } = "DRAFT";

    public Company Company { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Project? Project { get; set; }
}
""")

with open(f"{base_path}/Sales/SalesOrder.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Sales;

public class SalesOrder
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ClientId { get; set; }
    public long ProjectId { get; set; }
    public long? QuotationId { get; set; }
    public string SoNumber { get; set; } = string.Empty;
    public DateTime SoDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "CONFIRMED";

    public Company Company { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public Quotation? Quotation { get; set; }
}
""")

with open(f"{base_path}/Sales/SalesInvoice.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;

namespace SDK.ERP.Domain.Entities.Sales;

public class SalesInvoice
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long BranchId { get; set; }
    public long ClientId { get; set; }
    public long ProjectId { get; set; }
    public long? SalesOrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public string PlaceOfSupply { get; set; } = string.Empty;
    public bool IsReverseCharge { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal TotalInvoiceValue { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public string Status { get; set; } = "DRAFT";
    public string? IrnNumber { get; set; }
    public string? QrCodePayload { get; set; }
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Branch Branch { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public SalesOrder? SalesOrder { get; set; }

    public ICollection<SalesInvoiceItem> Items { get; set; } = new List<SalesInvoiceItem>();
    public ICollection<ReceiptAllocation> Allocations { get; set; } = new List<ReceiptAllocation>();
}
""")

with open(f"{base_path}/Sales/SalesInvoiceItem.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Sales;

public class SalesInvoiceItem
{
    public long Id { get; set; }
    public long InvoiceId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public string HsnSacCode { get; set; } = string.Empty;
    public decimal Quantity { get; set; } = 1.00m;
    public int? UnitId { get; set; }
    public decimal UnitRate { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxableValue { get; set; }
    public int TaxRateId { get; set; }
    public decimal CgstAmount { get; set; }
    public decimal SgstAmount { get; set; }
    public decimal IgstAmount { get; set; }
    public decimal LineTotal { get; set; }

    public SalesInvoice Invoice { get; set; } = null!;
    public ItemUnit? Unit { get; set; }
    public TaxRate TaxRate { get; set; } = null!;
}
""")

with open(f"{base_path}/Sales/CustomerReceipt.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Sales;

public class CustomerReceipt
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public long ClientId { get; set; }
    public long BankAccountId { get; set; }
    public string ReceiptNumber { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public decimal AmountReceived { get; set; }
    public decimal UnallocatedAmount { get; set; }
    public string PaymentMode { get; set; } = "NEFT";
    public string? TransactionRefNo { get; set; }
    public string Status { get; set; } = "POSTED";
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Client Client { get; set; } = null!;

    public ICollection<ReceiptAllocation> Allocations { get; set; } = new List<ReceiptAllocation>();
}
""")

with open(f"{base_path}/Sales/ReceiptAllocation.cs", "w", encoding="utf-8") as f:
    f.write("""namespace SDK.ERP.Domain.Entities.Sales;

public class ReceiptAllocation
{
    public long Id { get; set; }
    public long ReceiptId { get; set; }
    public long InvoiceId { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal TdsDeductedByClient { get; set; }
    public decimal CashDiscountAllowed { get; set; }

    public CustomerReceipt Receipt { get; set; } = null!;
    public SalesInvoice Invoice { get; set; } = null!;
}
""")

with open(f"{base_path}/Sales/CreditDebitNote.cs", "w", encoding="utf-8") as f:
    f.write("""using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;

namespace SDK.ERP.Domain.Entities.Sales;

public class CreditDebitNote
{
    public long Id { get; set; }
    public long CompanyId { get; set; }
    public string NoteType { get; set; } = string.Empty;
    public string PartyType { get; set; } = string.Empty;
    public long? ClientId { get; set; }
    public long? VendorId { get; set; }
    public long? OriginalInvoiceId { get; set; }
    public long? OriginalPurchaseBillId { get; set; }
    public string NoteNumber { get; set; } = string.Empty;
    public DateTime NoteDate { get; set; }
    public decimal TaxableAmount { get; set; }
    public decimal GstAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public long? JournalEntryId { get; set; }

    public Company Company { get; set; } = null!;
    public Client? Client { get; set; }
    public Vendor? Vendor { get; set; }
    public SalesInvoice? OriginalInvoice { get; set; }
}
""")

print("Generated Admin, MasterData, Projects and Sales entities successfully.")
