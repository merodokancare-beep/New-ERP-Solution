using Microsoft.EntityFrameworkCore;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Accounting;

namespace SDK.ERP.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        // 1. Seed Company
        var company = await context.Companies.FirstOrDefaultAsync();
        if (company == null)
        {
            company = new Company
            {
                CompanyCode = "CORP-01",
                CompanyName = "SDK Solutions Private Limited",
                LegalName = "SDK Solutions Private Limited",
                BrandShortName = "SDK",
                Tagline = "SOLUTIONS ERP",
                Industry = "IT Services & Consulting",
                Email = "contact@sdksolutions.com",
                Phone = "+91 98765 43210",
                Website = "https://sdksolutions.com",
                AddressLine1 = "Connaught Place, Central Business District",
                City = "New Delhi",
                State = "Delhi",
                StateCode = "07",
                Pincode = "110001",
                Gstin = "07AAAAA0000A1Z5",
                Pan = "AAAAA0000A",
                BankName = "HDFC Bank Ltd",
                BankAccountNumber = "50200084920194",
                BankIfsc = "HDFC0000123",
                BankBranch = "Connaught Place Branch, New Delhi",
                UpiId = "sdk@hdfcbank",
                AuthorizedSignatoryName = "Managing Director",
                AuthorizedSignatoryDesignation = "Authorized Signatory",
                TermsAndConditions = "1. Goods/Services once billed are subject to standard delivery terms.\n2. Payment is due within 30 days from invoice date.\n3. Applicable statutory GST has been assessed as per Indian GST Act.",
                BaseCurrency = "INR",
                FinancialYearStart = new DateTime(2026, 4, 1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }
        else
        {
            bool updated = false;
            if (string.IsNullOrEmpty(company.BrandShortName)) { company.BrandShortName = "SDK"; updated = true; }
            if (string.IsNullOrEmpty(company.Tagline)) { company.Tagline = "SOLUTIONS ERP"; updated = true; }
            if (string.IsNullOrEmpty(company.City)) { company.City = "New Delhi"; updated = true; }
            if (string.IsNullOrEmpty(company.StateCode)) { company.StateCode = "07"; updated = true; }
            if (string.IsNullOrEmpty(company.BankName)) { company.BankName = "HDFC Bank Ltd"; updated = true; }
            if (string.IsNullOrEmpty(company.BankAccountNumber)) { company.BankAccountNumber = "50200084920194"; updated = true; }
            if (string.IsNullOrEmpty(company.BankIfsc)) { company.BankIfsc = "HDFC0000123"; updated = true; }
            if (string.IsNullOrEmpty(company.AuthorizedSignatoryName)) { company.AuthorizedSignatoryName = "Managing Director"; updated = true; }
            if (string.IsNullOrEmpty(company.AuthorizedSignatoryDesignation)) { company.AuthorizedSignatoryDesignation = "Authorized Signatory"; updated = true; }
            if (updated) await context.SaveChangesAsync();
        }

        // 2. Seed Head Office Branch
        var branch = await context.Branches.FirstOrDefaultAsync();
        if (branch == null)
        {
            branch = new Branch
            {
                CompanyId = company.Id,
                BranchCode = "BR-HQ-DEL",
                BranchName = "Head Office - New Delhi",
                StateCode = "07",
                Gstin = "07AAAAA0000A1Z5",
                AddressLine1 = "Connaught Place, New Delhi",
                IsHeadOffice = true,
                IsActive = true
            };
            context.Branches.Add(branch);
            await context.SaveChangesAsync();
        }

        // 3. Seed System Roles
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "SUPER_ADMIN");
        if (superAdminRole == null)
        {
            superAdminRole = new Role { RoleName = "SUPER_ADMIN", Description = "Full Enterprise Administration", IsSystemRole = true };
            var pmRole = new Role { RoleName = "PROJECT_MANAGER", Description = "Project Management & Delivery", IsSystemRole = true };
            var accountantRole = new Role { RoleName = "ACCOUNTANT", Description = "Financial Accounting & GST Compliance", IsSystemRole = true };
            var procurementRole = new Role { RoleName = "PROCUREMENT_OFFICER", Description = "Purchasing & Vendor Management", IsSystemRole = true };
            var storeKeeperRole = new Role { RoleName = "STORE_KEEPER", Description = "Internal Office Stock Consumables", IsSystemRole = true };

            context.Roles.AddRange(superAdminRole, pmRole, accountantRole, procurementRole, storeKeeperRole);
            await context.SaveChangesAsync();
        }

        // 3.1 Seed Default Admin User
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                CompanyId = company.Id,
                BranchId = branch.Id,
                RoleId = superAdminRole.Id,
                Username = "admin",
                FullName = "Admin User",
                Designation = "Administrator",
                PhoneNumber = "+91 98765 43210",
                Email = "admin@sdksolutions.com",
                PasswordHash = "AQAAAAEAACcQAAAAEHASHEDADMINPASSWORDEXAMPLE==",
                IsActive = true
            };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
        else if (string.IsNullOrEmpty(adminUser.FullName))
        {
            adminUser.FullName = "Admin User";
            adminUser.Designation = "Administrator";
            await context.SaveChangesAsync();
        }

        // 4. Seed Standard GST Tax Rates
        if (!await context.TaxRates.AnyAsync())
        {
            var taxRates = new List<TaxRate>
            {
                new TaxRate { TaxName = "GST 0% (Exempt)", RatePercentage = 0.00m, CgstPercentage = 0.00m, SgstPercentage = 0.00m, IgstPercentage = 0.00m },
                new TaxRate { TaxName = "GST 5%", RatePercentage = 5.00m, CgstPercentage = 2.50m, SgstPercentage = 2.50m, IgstPercentage = 5.00m },
                new TaxRate { TaxName = "GST 12%", RatePercentage = 12.00m, CgstPercentage = 6.00m, SgstPercentage = 6.00m, IgstPercentage = 12.00m },
                new TaxRate { TaxName = "GST 18%", RatePercentage = 18.00m, CgstPercentage = 9.00m, SgstPercentage = 9.00m, IgstPercentage = 18.00m },
                new TaxRate { TaxName = "GST 28%", RatePercentage = 28.00m, CgstPercentage = 14.00m, SgstPercentage = 14.00m, IgstPercentage = 28.00m }
            };
            context.TaxRates.AddRange(taxRates);
            await context.SaveChangesAsync();
        }

        // 5. Seed Standard Units
        if (!await context.ItemUnits.AnyAsync())
        {
            var units = new List<ItemUnit>
            {
                new ItemUnit { UnitCode = "NOS", UnitName = "Numbers", IsDecimalAllowed = false },
                new ItemUnit { UnitCode = "BOX", UnitName = "Boxes", IsDecimalAllowed = false },
                new ItemUnit { UnitCode = "PKT", UnitName = "Packets", IsDecimalAllowed = false },
                new ItemUnit { UnitCode = "HRS", UnitName = "Hours", IsDecimalAllowed = true },
                new ItemUnit { UnitCode = "KGS", UnitName = "Kilograms", IsDecimalAllowed = true }
            };
            context.ItemUnits.AddRange(units);
            await context.SaveChangesAsync();
        }

        // 6. Seed Chart of Account Groups
        if (!await context.AccountGroups.AnyAsync())
        {
            var assetGroup = new AccountGroup { GroupCode = "1000", GroupName = "Current Assets", AccountCategory = "ASSET" };
            var liabGroup = new AccountGroup { GroupCode = "2000", GroupName = "Current Liabilities", AccountCategory = "LIABILITY" };
            var equityGroup = new AccountGroup { GroupCode = "3000", GroupName = "Equity & Reserves", AccountCategory = "EQUITY" };
            var revGroup = new AccountGroup { GroupCode = "4000", GroupName = "Project Revenue", AccountCategory = "REVENUE" };
            var expGroup = new AccountGroup { GroupCode = "5000", GroupName = "Direct Expenses", AccountCategory = "EXPENSE" };

            context.AccountGroups.AddRange(assetGroup, liabGroup, equityGroup, revGroup, expGroup);
            await context.SaveChangesAsync();
        }

        // 6.1 Seed Financial Year
        if (!await context.FinancialYears.AnyAsync())
        {
            var fy = new FinancialYear
            {
                CompanyId = company.Id,
                FyCode = "FY-2026-27",
                StartDate = new DateTime(2026, 4, 1),
                EndDate = new DateTime(2027, 3, 31),
                IsClosed = false
            };
            context.FinancialYears.Add(fy);
            await context.SaveChangesAsync();
        }

        // 7. Seed Genesis System Audit Logs
        if (!await context.AuditLogs.AnyAsync())
        {
            var genesisLogs = new List<AuditLog>
            {
                new AuditLog
                {
                    TableName = "SYSTEM_INITIALIZATION",
                    ActionType = "GENESIS_BOOTSTRAP",
                    CurrentHash = "a1b2c3d4e5f60718293a4b5c6d7e8f90a1b2c3d4e5f60718293a4b5c6d7e8f90",
                    PreviousHash = "0000000000000000000000000000000000000000000000000000000000000000",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                    IpAddress = "127.0.0.1",
                    UserAgent = "SDK_ERP_BOOTSTRAP"
                },
                new AuditLog
                {
                    TableName = "SECURITY_RBAC",
                    ActionType = "ROLE_PROVISIONING",
                    CurrentHash = "b2c3d4e5f6a10718293a4b5c6d7e8f90a1b2c3d4e5f60718293a4b5c6d7e8f91",
                    PreviousHash = "a1b2c3d4e5f60718293a4b5c6d7e8f90a1b2c3d4e5f60718293a4b5c6d7e8f90",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-20),
                    IpAddress = "127.0.0.1",
                    UserAgent = "SDK_ERP_BOOTSTRAP"
                },
                new AuditLog
                {
                    TableName = "MASTER_CATALOGS",
                    ActionType = "GST_SLABS_INITIALIZED",
                    CurrentHash = "c3d4e5f6a1b20718293a4b5c6d7e8f90a1b2c3d4e5f60718293a4b5c6d7e8f92",
                    PreviousHash = "b2c3d4e5f6a10718293a4b5c6d7e8f90a1b2c3d4e5f60718293a4b5c6d7e8f91",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    IpAddress = "127.0.0.1",
                    UserAgent = "SDK_ERP_BOOTSTRAP"
                }
            };
            context.AuditLogs.AddRange(genesisLogs);
            await context.SaveChangesAsync();
        }

        // 8. Auto-Post Ledger Journal Entries for any existing unposted Invoices
        var unpostedInvoices = await context.SalesInvoices
            .Where(i => !context.JournalEntries.Any(j => j.SourceEntityType == "SalesInvoice" && j.SourceEntityId == i.Id))
            .ToListAsync();

        if (unpostedInvoices.Any())
        {
            var fy = await context.FinancialYears.FirstOrDefaultAsync();
            var postingUser = await context.Users.FirstOrDefaultAsync();
            foreach (var inv in unpostedInvoices)
            {
                var jv = new JournalEntry
                {
                    CompanyId = inv.CompanyId,
                    BranchId = inv.BranchId,
                    FyId = fy?.Id ?? 1,
                    VoucherNo = $"JV-SALES-{inv.InvoiceNumber}",
                    VoucherDate = inv.InvoiceDate,
                    VoucherType = "SALES",
                    SourceEntityType = "SalesInvoice",
                    SourceEntityId = inv.Id,
                    ProjectId = inv.ProjectId,
                    Narration = $"Tax Invoice {inv.InvoiceNumber} auto-posted to Ledger",
                    TotalDebit = inv.TotalInvoiceValue,
                    TotalCredit = inv.TotalInvoiceValue,
                    IsBalanced = true,
                    CreatedBy = postingUser?.Id ?? 1
                };
                context.JournalEntries.Add(jv);
            }
            await context.SaveChangesAsync();
        }
    }
}
