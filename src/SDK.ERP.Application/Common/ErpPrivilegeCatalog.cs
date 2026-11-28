namespace SDK.ERP.Application.Common;

public class PrivilegeItem
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
}

public class PrivilegeModuleGroup
{
    public string ModuleCode { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public string Icon { get; set; } = "fa-solid fa-layer-group";
    public List<PrivilegeItem> Privileges { get; set; } = new();
}

public static class ErpPrivilegeCatalog
{
    public static readonly List<PrivilegeModuleGroup> AllGroups = new()
    {
        new PrivilegeModuleGroup
        {
            ModuleCode = "PROCUREMENT",
            ModuleName = "Procurement & Purchase Orders",
            Icon = "fa-solid fa-cart-flatbed",
            Privileges = new List<PrivilegeItem>
            {
                new() { Code = "PROCUREMENT_VIEW", Name = "View Purchase Orders", Description = "View purchase order listings and summary cards", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_CREATE", Name = "Create Purchase Order", Description = "Create new POs with vendor and items", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_EDIT", Name = "Edit Purchase Order", Description = "Modify line items, quantities, or terms on PO", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_DOWNLOAD", Name = "Download & Print PO", Description = "Download clean official PDF/Print ready PO copy", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_VIEW_LOGS", Name = "PO Transaction History", Description = "View complete lifecycle audit log and payment ledger for a PO", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_MARK_PAYMENT", Name = "Mark For Settlement", Description = "Request advance or payment release for an approved PO", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_APPROVE_PAYMENT", Name = "Approve Payment Request", Description = "Management privilege: Authorize finance/accounts to release funds", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_EXECUTE_PAYMENT", Name = "Make Payment (Disburse)", Description = "Accounts privilege: Disburse payment via Bank/Cash and issue voucher", ModuleCode = "PROCUREMENT" },
                new() { Code = "PROCUREMENT_UNDO", Name = "Undo Operation", Description = "Reverse pending payment requests or payment approvals", ModuleCode = "PROCUREMENT" },
                new() { Code = "INVENTORY_MANAGE", Name = "Office Inventory & Stock", Description = "Manage inventory inward, adjustment, and dispatch", ModuleCode = "PROCUREMENT" }
            }
        },
        new PrivilegeModuleGroup
        {
            ModuleCode = "EXPENSES",
            ModuleName = "Direct Payments & Expenses",
            Icon = "fa-solid fa-money-bill-transfer",
            Privileges = new List<PrivilegeItem>
            {
                new() { Code = "EXPENSES_VIEW", Name = "View Direct Expenses", Description = "Access direct payments ledger and vouchers", ModuleCode = "EXPENSES" },
                new() { Code = "EXPENSES_CREATE", Name = "Add Direct Payment", Description = "Create new direct expense vouchers", ModuleCode = "EXPENSES" },
                new() { Code = "EXPENSES_EDIT", Name = "Edit Expense Voucher", Description = "Modify pending direct payment details", ModuleCode = "EXPENSES" },
                new() { Code = "EXPENSES_APPROVE", Name = "Approve Direct Payment", Description = "Approve pending direct payments", ModuleCode = "EXPENSES" },
                new() { Code = "EXPENSES_DECLINE", Name = "Decline Direct Payment", Description = "Reject/Decline submitted expenses with reasons", ModuleCode = "EXPENSES" },
                new() { Code = "EXPENSES_SETTLE", Name = "Settle / Disburse Expense", Description = "Disburse approved direct expenses via bank/cash", ModuleCode = "EXPENSES" },
                new() { Code = "EXPENSES_PRINT", Name = "Print Payment Voucher", Description = "Print authorized expense vouchers", ModuleCode = "EXPENSES" }
            }
        },
        new PrivilegeModuleGroup
        {
            ModuleCode = "PROJECTS",
            ModuleName = "Operations & Projects",
            Icon = "fa-solid fa-diagram-project",
            Privileges = new List<PrivilegeItem>
            {
                new() { Code = "PROJECTS_VIEW", Name = "View Projects", Description = "View pipeline projects, progress, and financial summaries", ModuleCode = "PROJECTS" },
                new() { Code = "PROJECTS_CREATE", Name = "Create Project", Description = "Initiate and register new projects", ModuleCode = "PROJECTS" },
                new() { Code = "PROJECTS_EDIT", Name = "Edit Project", Description = "Modify milestones, budget, scope, and team assignments", ModuleCode = "PROJECTS" },
                new() { Code = "PROJECTS_CLOSE", Name = "Close / Complete Project", Description = "Sign-off and mark projects as completed", ModuleCode = "PROJECTS" }
            }
        },
        new PrivilegeModuleGroup
        {
            ModuleCode = "SALES",
            ModuleName = "Sales & Receivables",
            Icon = "fa-solid fa-file-invoice-dollar",
            Privileges = new List<PrivilegeItem>
            {
                new() { Code = "SALES_VIEW", Name = "View Sales Invoices", Description = "View invoices, quotations, and customer ledgers", ModuleCode = "SALES" },
                new() { Code = "SALES_CREATE_INVOICE", Name = "Generate Tax Invoice", Description = "Create and dispatch GST tax invoices", ModuleCode = "SALES" },
                new() { Code = "SALES_ALLOCATE_RECEIPT", Name = "Allocate Payment Receipts", Description = "Record customer payment receipts and map to invoices", ModuleCode = "SALES" },
                new() { Code = "SALES_CANCEL", Name = "Cancel / Issue Credit Note", Description = "Issue credit/debit notes or cancel invoices", ModuleCode = "SALES" }
            }
        },
        new PrivilegeModuleGroup
        {
            ModuleCode = "MASTERS",
            ModuleName = "Master Catalogs",
            Icon = "fa-solid fa-sliders",
            Privileges = new List<PrivilegeItem>
            {
                new() { Code = "MASTERS_VIEW", Name = "View Master Catalogs", Description = "Browse master lists of vendors, clients, items, and tax rates", ModuleCode = "MASTERS" },
                new() { Code = "MASTERS_MANAGE_VENDORS", Name = "Manage Vendors", Description = "Register, verify GST, and modify vendor details", ModuleCode = "MASTERS" },
                new() { Code = "MASTERS_MANAGE_CLIENTS", Name = "Manage Clients", Description = "Register and edit client organizations", ModuleCode = "MASTERS" },
                new() { Code = "MASTERS_MANAGE_ITEMS", Name = "Manage Materials & Items", Description = "Add and configure items, HSN/SAC, and tax rates", ModuleCode = "MASTERS" }
            }
        },
        new PrivilegeModuleGroup
        {
            ModuleCode = "FINANCE",
            ModuleName = "General Ledger & Statutory",
            Icon = "fa-solid fa-book-journal-whills",
            Privileges = new List<PrivilegeItem>
            {
                new() { Code = "ACCOUNTS_VIEW", Name = "View Chart of Accounts", Description = "Inspect COA and ledger balances", ModuleCode = "FINANCE" },
                new() { Code = "JOURNAL_POST", Name = "Post Journal Entries", Description = "Create manual debit/credit ledger adjustments", ModuleCode = "FINANCE" },
                new() { Code = "BANKING_MANAGE", Name = "Banking & Reconciliation", Description = "Manage company bank accounts and statements", ModuleCode = "FINANCE" },
                new() { Code = "PETTYCASH_MANAGE", Name = "Petty Cash Management", Description = "Issue, top-up, and settle petty cash expenses", ModuleCode = "FINANCE" },
                new() { Code = "GST_COMPLIANCE", Name = "GST Filing & Tax Summaries", Description = "Access GSTR-1, GSTR-3B, and tax reports", ModuleCode = "FINANCE" },
                new() { Code = "PAYROLL_MANAGE", Name = "Payroll & Salary Structures", Description = "Process monthly salary registers and payslips", ModuleCode = "FINANCE" }
            }
        },
        new PrivilegeModuleGroup
        {
            ModuleCode = "ADMIN",
            ModuleName = "System Administration",
            Icon = "fa-solid fa-user-shield",
            Privileges = new List<PrivilegeItem>
            {
                new() { Code = "ADMIN_ASSIGN_PRIVILEGES", Name = "Assign User Privileges", Description = "Centralized granting and revoking of user privileges", ModuleCode = "ADMIN" },
                new() { Code = "ADMIN_VIEW_AUDIT", Name = "View Cryptographic Audit Trail", Description = "Inspect SHA-256 tamper-evident logs", ModuleCode = "ADMIN" },
                new() { Code = "ADMIN_RESET_TEST_DATA", Name = "Execute Database Reset", Description = "SuperAdmin exclusive: wipe demo transactions", ModuleCode = "ADMIN" }
            }
        }
    };
}
