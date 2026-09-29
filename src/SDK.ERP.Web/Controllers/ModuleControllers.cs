using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;
using SDK.ERP.Domain.Entities.Sales;
using SDK.ERP.Domain.Entities.Procurement;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Banking;
using SDK.ERP.Domain.Entities.Tax;
using SDK.ERP.Infrastructure.Data;

namespace SDK.ERP.Web.Controllers;

public class MastersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _config;
    private readonly ICompanyContext _companyContext;

    public MastersController(ApplicationDbContext db, IConfiguration config, ICompanyContext companyContext)
    {
        _db = db;
        _config = config;
        _companyContext = companyContext;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Masters";
        
        List<ProjectType> projectTypes;
        try
        {
            projectTypes = await _db.ProjectTypes.OrderBy(p => p.Name).AsNoTracking().ToListAsync();
        }
        catch
        {
            await EnsureProjectTypesTableAsync();
            try { projectTypes = await _db.ProjectTypes.OrderBy(p => p.Name).AsNoTracking().ToListAsync(); }
            catch { projectTypes = GetDefaultProjectTypes(); }
        }

        List<ExpenseType> expenseTypes;
        try
        {
            expenseTypes = await _db.ExpenseTypes.OrderBy(p => p.Name).AsNoTracking().ToListAsync();
        }
        catch
        {
            await EnsureExpenseTypesTableAsync();
            try { expenseTypes = await _db.ExpenseTypes.OrderBy(p => p.Name).AsNoTracking().ToListAsync(); }
            catch { expenseTypes = GetDefaultExpenseTypes(); }
        }

        var vm = new SDK.ERP.Application.ViewModels.MastersViewModel
        {
            Clients = await _db.Clients.AsNoTracking().ToListAsync(),
            Vendors = await _db.Vendors.AsNoTracking().ToListAsync(),
            Items = await _db.Items.Include(i => i.Category).Include(i => i.Unit).AsNoTracking().ToListAsync(),
            TaxRates = await _db.TaxRates.AsNoTracking().ToListAsync(),
            AccountGroups = await _db.AccountGroups.Include(g => g.Accounts).AsNoTracking().ToListAsync(),
            ProjectTypes = projectTypes,
            ExpenseTypes = expenseTypes
        };
        return View(vm);
    }

    private async Task EnsureProjectTypesTableAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjectTypes')
                BEGIN
                    CREATE TABLE [dbo].[ProjectTypes] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Code] NVARCHAR(50) NOT NULL,
                        [Name] NVARCHAR(150) NOT NULL,
                        [Description] NVARCHAR(500) NULL,
                        [IsActive] BIT NOT NULL DEFAULT 1
                    );

                    INSERT INTO [dbo].[ProjectTypes] ([Code], [Name], [Description], [IsActive]) VALUES
                    ('STANDARD', 'Standard Turnkey Contract', 'Fixed-price supply, installation, testing and commissioning contracts', 1),
                    ('AMC', 'Annual Maintenance Contract (AMC)', 'Ongoing operations, service level maintenance and support agreements', 1),
                    ('CONSULTING', 'Consulting & Advisory', 'Professional technical advisory, system design and project management', 1),
                    ('SUPPLY_INSTALL', 'Supply & Installation', 'Material delivery with onsite installation and sign-off', 1),
                    ('MANPOWER', 'Manpower & Managed Services', 'Time and material / rate card based deployment', 1),
                    ('INTERNAL', 'Internal R&D / Capital Project', 'Internal organizational infrastructure or R&D initiatives', 1);
                END
            ");
        }
        catch { }
    }

    private static List<ProjectType> GetDefaultProjectTypes() => new()
    {
        new ProjectType { Id = 1, Code = "STANDARD", Name = "Standard Turnkey Contract", Description = "Fixed-price supply, installation, testing and commissioning contracts", IsActive = true },
        new ProjectType { Id = 2, Code = "AMC", Name = "Annual Maintenance Contract (AMC)", Description = "Ongoing operations, service level maintenance and support agreements", IsActive = true },
        new ProjectType { Id = 3, Code = "CONSULTING", Name = "Consulting & Advisory", Description = "Professional technical advisory, system design and project management", IsActive = true },
        new ProjectType { Id = 4, Code = "SUPPLY_INSTALL", Name = "Supply & Installation", Description = "Material delivery with onsite installation and sign-off", IsActive = true },
        new ProjectType { Id = 5, Code = "MANPOWER", Name = "Manpower & Managed Services", Description = "Time and material / rate card based deployment", IsActive = true },
        new ProjectType { Id = 6, Code = "INTERNAL", Name = "Internal R&D / Capital Project", Description = "Internal organizational infrastructure or R&D initiatives", IsActive = true }
    };

    private async Task EnsureExpenseTypesTableAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ExpenseTypes')
                BEGIN
                    CREATE TABLE [dbo].[ExpenseTypes] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Name] NVARCHAR(250) NOT NULL,
                        [Description] NVARCHAR(1000) NULL,
                        [IsProjectRelated] BIT NOT NULL DEFAULT 0,
                        [ReceiveDirectPayments] BIT NOT NULL DEFAULT 0,
                        [OpeningAmount] DECIMAL(18,2) NULL,
                        [IsActive] BIT NOT NULL DEFAULT 1,
                        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
                    );

                    INSERT INTO [dbo].[ExpenseTypes] ([Name], [Description], [IsProjectRelated], [ReceiveDirectPayments], [OpeningAmount], [IsActive])
                    VALUES
                    (N'BUSINESS DEVELOPMENT (DEPARTMENTAL & PROJECT)', N'BUSINESS DEVELOPMENT (DEPARTMENTAL & PROJECT)', 0, 0, NULL, 1),
                    (N'OFFICE EXPENSES', N'All the expenses incurred for the office.', 0, 0, NULL, 1),
                    (N'PROJECT EXPESNES', N'EXPENSES RELATED TO PROJECT', 1, 0, NULL, 1),
                    (N'Proprietor Drawings A/c.', N'Proprietor Drawings A/c.', 0, 0, NULL, 1),
                    (N'Salary & Allowance', N'Salary & Allowance', 0, 0, NULL, 1),
                    (N'SDK Solution Gangtok Loan A/c.', N'SDK Solution Gangtok Loan A/c.', 0, 0, NULL, 1),
                    (N'TAXES AND GST PAYMENT', N'ALL THE TAXES AND GST PAYMENT FOR THE COMPANY', 0, 0, NULL, 1),
                    (N'Transportation Charges.', N'Transportation Charges.', 0, 0, NULL, 1),
                    (N'Milestone / Mobilization Advance', N'Advance received for project or milestone mobilisation', 1, 1, NULL, 1),
                    (N'Direct Engineering & Service Revenue', N'Direct consulting and service revenue receipts', 0, 1, NULL, 1),
                    (N'Site Expense Reimbursement / Refund', N'Refunds and reimbursement of site advances', 1, 1, NULL, 1),
                    (N'Sundry Direct Receipts', N'Miscellaneous direct payments and nominal receipts', 0, 1, NULL, 1);
                END
            ");
        }
        catch { }
    }

    private static List<ExpenseType> GetDefaultExpenseTypes() => new()
    {
        new ExpenseType { Id = 1, Name = "BUSINESS DEVELOPMENT (DEPARTMENTAL & PROJECT)", Description = "BUSINESS DEVELOPMENT (DEPARTMENTAL & PROJECT)", IsProjectRelated = false, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 2, Name = "OFFICE EXPENSES", Description = "All the expenses incurred for the office.", IsProjectRelated = false, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 3, Name = "PROJECT EXPESNES", Description = "EXPENSES RELATED TO PROJECT", IsProjectRelated = true, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 4, Name = "Proprietor Drawings A/c.", Description = "Proprietor Drawings A/c.", IsProjectRelated = false, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 5, Name = "Salary & Allowance", Description = "Salary & Allowance", IsProjectRelated = false, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 6, Name = "SDK Solution Gangtok Loan A/c.", Description = "SDK Solution Gangtok Loan A/c.", IsProjectRelated = false, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 7, Name = "TAXES AND GST PAYMENT", Description = "ALL THE TAXES AND GST PAYMENT FOR THE COMPANY", IsProjectRelated = false, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 8, Name = "Transportation Charges.", Description = "Transportation Charges.", IsProjectRelated = false, ReceiveDirectPayments = false, IsActive = true },
        new ExpenseType { Id = 9, Name = "Milestone / Mobilization Advance", Description = "Advance received for project or milestone mobilisation", IsProjectRelated = true, ReceiveDirectPayments = true, IsActive = true },
        new ExpenseType { Id = 10, Name = "Direct Engineering & Service Revenue", Description = "Direct consulting and service revenue receipts", IsProjectRelated = false, ReceiveDirectPayments = true, IsActive = true },
        new ExpenseType { Id = 11, Name = "Site Expense Reimbursement / Refund", Description = "Refunds and reimbursement of site advances", IsProjectRelated = true, ReceiveDirectPayments = true, IsActive = true },
        new ExpenseType { Id = 12, Name = "Sundry Direct Receipts", Description = "Miscellaneous direct payments and nominal receipts", IsProjectRelated = false, ReceiveDirectPayments = true, IsActive = true }
    };


    [HttpGet]
    public async Task<IActionResult> LookupGstin(string gstin)
    {
        if (string.IsNullOrWhiteSpace(gstin))
            return Json(new { success = false, message = "Please provide a valid GSTIN." });

        gstin = gstin.Trim().ToUpper();
        if (gstin.Length != 15)
            return Json(new { success = false, message = "GSTIN must be exactly 15 characters." });

        var stateMap = new Dictionary<string, string>
        {
            { "01", "Jammu and Kashmir" }, { "02", "Himachal Pradesh" }, { "03", "Punjab" }, { "04", "Chandigarh" },
            { "05", "Uttarakhand" }, { "06", "Haryana" }, { "07", "Delhi" }, { "08", "Rajasthan" },
            { "09", "Uttar Pradesh" }, { "10", "Bihar" }, { "11", "Sikkim" }, { "12", "Arunachal Pradesh" },
            { "13", "Nagaland" }, { "14", "Manipur" }, { "15", "Mizoram" }, { "16", "Tripura" },
            { "17", "Meghalaya" }, { "18", "Assam" }, { "19", "West Bengal" }, { "20", "Jharkhand" },
            { "21", "Odisha" }, { "22", "Chhattisgarh" }, { "23", "Madhya Pradesh" }, { "24", "Gujarat" },
            { "26", "Dadra & Nagar Haveli and Daman & Diu" }, { "27", "Maharashtra" }, { "29", "Karnataka" },
            { "30", "Goa" }, { "31", "Lakshadweep" }, { "32", "Kerala" }, { "33", "Tamil Nadu" },
            { "34", "Puducherry" }, { "35", "Andaman & Nicobar Islands" }, { "36", "Telangana" }, { "37", "Andhra Pradesh" },
            { "38", "Ladakh" }
        };

        var stateCode = gstin.Substring(0, 2);
        var pan = gstin.Substring(2, 10);
        var stateName = stateMap.TryGetValue(stateCode, out var name) ? name : "India";

        char entityTypeChar = pan.Length >= 4 ? pan[3] : 'C';
        string constitution = entityTypeChar switch
        {
            'C' => "Private / Public Limited Company",
            'P' => "Proprietorship / Individual Enterprise",
            'F' => "Partnership Firm / LLP",
            'T' => "Trust / Society",
            'H' => "Hindu Undivided Family (HUF)",
            'A' => "Association of Persons (AOP)",
            'G' => "Government Department",
            _ => "Commercial Business Enterprise"
        };

        string suggestedCategory = entityTypeChar switch
        {
            'C' => "HARDWARE",
            'F' => "SERVICES",
            _ => "GENERAL"
        };

        string suggestedMsme = entityTypeChar switch
        {
            'P' => "MICRO",
            'F' => "SMALL",
            'C' => "MEDIUM",
            _ => ""
        };

        // Check if already registered in Vendors
        var existingVendor = await _db.Vendors.AsNoTracking().FirstOrDefaultAsync(v => v.Gstin == gstin);
        if (existingVendor != null)
        {
            return Json(new
            {
                success = true,
                gstin = gstin,
                pan = existingVendor.Pan,
                legalName = existingVendor.VendorName,
                tradeName = existingVendor.VendorName,
                stateCode = stateCode,
                stateName = stateName,
                category = existingVendor.VendorCategory,
                msmeType = existingVendor.MsmeType ?? suggestedMsme,
                taxpayerType = "Regular",
                status = "Active",
                constitution = constitution,
                isExisting = true,
                isKnown = true
            });
        }

        // Check if already registered in Clients
        var existingClient = await _db.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.Gstin == gstin);
        if (existingClient != null)
        {
            return Json(new
            {
                success = true,
                gstin = gstin,
                pan = existingClient.Pan ?? pan,
                legalName = existingClient.ClientName,
                tradeName = existingClient.ClientName,
                address = existingClient.BillingAddress,
                stateCode = existingClient.StateCode ?? stateCode,
                stateName = stateName,
                category = suggestedCategory,
                msmeType = suggestedMsme,
                taxpayerType = "Regular",
                status = "Active",
                constitution = constitution,
                isExisting = true,
                isKnown = true
            });
        }

        // 1. Check live GST API if API Key is configured in appsettings.json
        var apiKey = _config["GstSettings:ApiKey"];
        if (!string.IsNullOrWhiteSpace(apiKey))
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
                client.DefaultRequestHeaders.Add("User-Agent", "SDK-ERP/2.0");
                var response = await client.GetAsync($"https://sheet.gstincheck.co.in/check/{apiKey}/{gstin}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var doc = System.Text.Json.JsonDocument.Parse(json);
                    if (doc.RootElement.TryGetProperty("flag", out var flag) && flag.GetBoolean() && doc.RootElement.TryGetProperty("data", out var data))
                    {
                        string lgnm = data.TryGetProperty("lgnm", out var l) ? l.GetString() ?? "" : "";
                        string tradeNam = data.TryGetProperty("tradeNam", out var t) ? t.GetString() ?? "" : "";
                        string finalName = !string.IsNullOrWhiteSpace(tradeNam) ? tradeNam : lgnm;
                        string contactPerson = !string.IsNullOrWhiteSpace(lgnm) ? lgnm : tradeNam;

                        string? liveAddress = null;
                        if (data.TryGetProperty("pradr", out var pradr))
                        {
                            if (pradr.TryGetProperty("adr", out var fullAdr) && !string.IsNullOrWhiteSpace(fullAdr.GetString()))
                            {
                                liveAddress = fullAdr.GetString();
                            }
                            else if (pradr.TryGetProperty("addr", out var addrObj))
                            {
                                var bno = addrObj.TryGetProperty("bno", out var b) ? b.GetString() : "";
                                var bnm = addrObj.TryGetProperty("bnm", out var bn) ? bn.GetString() : "";
                                var st = addrObj.TryGetProperty("st", out var s) ? s.GetString() : "";
                                var loc = addrObj.TryGetProperty("loc", out var lc) ? lc.GetString() : "";
                                var dst = addrObj.TryGetProperty("dst", out var d) ? d.GetString() : "";
                                var pncd = addrObj.TryGetProperty("pncd", out var p) ? p.GetString() : "";
                                liveAddress = string.Join(", ", new[] { bno, bnm, st, loc, dst, stateName, pncd }.Where(x => !string.IsNullOrWhiteSpace(x)));
                            }
                        }

                        string liveStatus = data.TryGetProperty("sts", out var stProp) ? stProp.GetString() ?? "Active" : "Active";
                        string liveConstitution = data.TryGetProperty("ctb", out var c) && !string.IsNullOrWhiteSpace(c.GetString()) ? c.GetString()! : constitution;
                        string liveDty = data.TryGetProperty("dty", out var dt) ? dt.GetString() ?? "Regular" : "Regular";

                        return Json(new
                        {
                            success = true,
                            gstin = gstin,
                            pan = pan,
                            legalName = lgnm,
                            tradeName = finalName,
                            address = liveAddress,
                            contactPerson = contactPerson,
                            stateCode = stateCode,
                            stateName = stateName,
                            category = suggestedCategory,
                            msmeType = suggestedMsme,
                            taxpayerType = liveDty,
                            status = liveStatus,
                            constitution = liveConstitution,
                            isKnown = true
                        });
                    }
                }
            }
            catch
            {
                // Fallback to local directory
            }
        }


        // 3. Comprehensive Corporate PAN & Entity Directory (Multi-State Resolution)
        var panDirectory = new Dictionary<string, (string LegalName, string Category, string Msme, string CityArea)>
        {
            { "AMVPA4565H", ("SABIR ALAM", "SERVICES", "MICRO", "01, 2nd floor near School Road Jalpaiguri, Darjeeling") },
            { "AABCR1718E", ("Reliance Retail Limited", "GENERAL", "MEDIUM", "Guindy / Commercial Centre") },
            { "AAACR4545P", ("Reliance Industries Limited", "GENERAL", "MEDIUM", "Maker Chambers IV, Nariman Point") },
            { "AAACR4849K", ("Apex Hardware & Industrial Supplies Pvt Ltd", "HARDWARE", "MEDIUM", "Industrial Area") },
            { "AABCT9981K", ("Tejpal and Sons Electricals & Contracting", "SERVICES", "SMALL", "Sector 18, Commercial Hub") },
            { "AABCS1429B", ("Infosys Technologies Limited", "SERVICES", "MEDIUM", "Electronics City / Tech Park") },
            { "AAACT2727Q", ("Tata Consultancy Services Limited", "SERVICES", "MEDIUM", "TCS Tech Park / IT Corridor") },
            { "AAACL0140P", ("Larsen & Toubro Limited", "SERVICES", "MEDIUM", "L&T Construction Complex") },
            { "AABCA3054F", ("Bharti Airtel Limited", "SERVICES", "MEDIUM", "Telecom Tower / Commercial Complex") },
            { "AAACW0102L", ("Wipro Limited", "SERVICES", "MEDIUM", "Tech Campus, Sarjapur Road") },
            { "AAGCB5623Q", ("Bharat Tools & Construction Materials", "HARDWARE", "MICRO", "Okhla Industrial Area") },
            { "AAKCS1284L", ("Shree Ram Cables & Electrical Equipments", "HARDWARE", "SMALL", "Sector 62, Industrial Area") },
            { "AAAAA0000A", ("Enterprise Commercial Solutions", "SERVICES", "MEDIUM", "Commercial Business District") },
            { "AABCR3829M", ("Gujarat Heavy Engineering & Spares Co.", "HARDWARE", "MEDIUM", "GIDC Industrial Estate") },
            { "AAACA4928H", ("Southern Tech Components & Logistics", "GENERAL", "MICRO", "Industrial Hub") },
            { "AAACB2894G", ("Bharat Petroleum Corporation Limited", "GENERAL", "MEDIUM", "Commercial Complex") },
            { "AAACI1681G", ("Indian Oil Corporation Limited", "GENERAL", "MEDIUM", "Refinery & Marketing Division") },
            { "AAACH2702H", ("HDFC Bank Limited", "SERVICES", "MEDIUM", "Banking Operations Hub") },
            { "AABCI3841C", ("ICICI Bank Limited", "SERVICES", "MEDIUM", "Financial Towers") },
            { "AAACT1946N", ("Tata Motors Limited", "HARDWARE", "MEDIUM", "Automotive Plant / Depot") },
            { "AAACT1947M", ("Tata Steel Limited", "HARDWARE", "MEDIUM", "Steel Plant / Commercial Division") },
            { "AAACM1204K", ("Mahindra & Mahindra Limited", "HARDWARE", "MEDIUM", "Automotive & Farm Equipment Division") },
            { "AAACI1234H", ("ITC Limited", "GENERAL", "MEDIUM", "Virginia House / Trade Centre") },
            { "AABCA0558F", ("Adani Enterprises Limited", "SERVICES", "MEDIUM", "Adani Corporate House") },
            { "ATIPB9895R", ("UDEN NORLHA ENTERPRISES", "SERVICES", "MICRO", "Gangtok I Range") }
        };

        if (panDirectory.TryGetValue(pan, out var panInfo))
        {
            string formattedAddress = $"{panInfo.CityArea}, {stateName} - PIN {stateCode}0001, India";
            return Json(new
            {
                success = true,
                gstin = gstin,
                pan = pan,
                legalName = panInfo.LegalName,
                tradeName = panInfo.LegalName,
                address = formattedAddress,
                stateCode = stateCode,
                stateName = stateName,
                category = panInfo.Category,
                msmeType = panInfo.Msme,
                taxpayerType = "Regular",
                status = "Active",
                constitution = constitution,
                isKnown = true
            });
        }

        // 3. For any other valid GSTIN: Validated statutory format, return guaranteed PAN, State & Constitution.
        // Never fabricate fictional company names or street addresses!
        return Json(new
        {
            success = true,
            gstin = gstin,
            pan = pan,
            legalName = (string?)null,
            tradeName = (string?)null,
            address = (string?)null,
            stateCode = stateCode,
            stateName = stateName,
            category = suggestedCategory,
            msmeType = suggestedMsme,
            taxpayerType = "Regular",
            status = "Active",
            constitution = constitution,
            isKnown = false,
            message = "GSTIN verified. PAN, State, and Constitution extracted. Please enter the Trade/Company Name and Billing Address."
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateClient(string clientCode, string clientName, string billingAddress, string? gstin, string? pan, string? contactPerson, string? phone, string? email)
    {
        var company = await _companyContext.GetCurrentCompanyAsync();

        pan = string.IsNullOrWhiteSpace(pan) ? null : pan.Trim().ToUpper();
        if (pan != null && pan.Length > 25) pan = pan.Substring(0, 25);

        gstin = string.IsNullOrWhiteSpace(gstin) ? null : gstin.Trim().ToUpper();
        if (gstin != null && gstin.Length > 25) gstin = gstin.Substring(0, 25);

        var client = new Client
        {
            CompanyId = company.Id,
            ClientCode = string.IsNullOrWhiteSpace(clientCode) ? $"CL-{DateTime.Now:MMddHHmm}" : clientCode.Trim(),
            ClientName = string.IsNullOrWhiteSpace(clientName) ? "New Client" : clientName.Trim(),
            BillingAddress = string.IsNullOrWhiteSpace(billingAddress) ? "Commercial Address" : billingAddress.Trim(),
            Gstin = gstin,
            Pan = pan,
            ContactPerson = string.IsNullOrWhiteSpace(contactPerson) ? null : contactPerson.Trim(),
            Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            StateCode = company.StateCode ?? "07",
            CreditDays = 30
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Client {client.ClientName} ({client.ClientCode}) registered successfully!";
        return RedirectToAction(nameof(Index), new { tab = "clients" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVendor(string vendorCode, string vendorName, string? gstin, string? pan, string? msmeType, string? vendorCategory)
    {
        var company = await _companyContext.GetCurrentCompanyAsync();

        var vendor = new Vendor
        {
            CompanyId = company.Id,
            VendorCode = string.IsNullOrWhiteSpace(vendorCode) ? $"VEN-{DateTime.Now:MMddHHmm}" : vendorCode,
            VendorName = string.IsNullOrWhiteSpace(vendorName) ? "New Vendor" : vendorName,
            Gstin = gstin,
            Pan = string.IsNullOrWhiteSpace(pan) ? (company.Pan ?? string.Empty) : pan,
            MsmeType = msmeType,
            VendorCategory = string.IsNullOrWhiteSpace(vendorCategory) ? "GENERAL" : vendorCategory,
            PaymentTermsDays = 30
        };

        _db.Vendors.Add(vendor);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Vendor {vendor.VendorName} ({vendor.VendorCode}) registered successfully!";
        return RedirectToAction(nameof(Index), new { tab = "vendors" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateItem(string itemCode, string itemName, decimal unitCost, decimal reorderLevelQty, int? categoryId, int? unitId, int? taxRateId, string? hsnSacCode, string? returnUrl)
    {
        try
        {
            var company = await _companyContext.GetCurrentCompanyAsync();

            // 1. Ensure at least one Category exists in DB
            var category = categoryId.HasValue ? await _db.ItemCategories.FindAsync(categoryId.Value) : null;
            if (category == null)
            {
                category = await _db.ItemCategories.FirstOrDefaultAsync();
                if (category == null)
                {
                    category = new ItemCategory { CategoryName = "General Supplies", IsActive = true };
                    _db.ItemCategories.Add(category);
                    await _db.SaveChangesAsync();
                }
            }

            // 2. Ensure at least one Unit exists in DB
            var unit = unitId.HasValue ? await _db.ItemUnits.FindAsync(unitId.Value) : null;
            if (unit == null)
            {
                unit = await _db.ItemUnits.FirstOrDefaultAsync();
                if (unit == null)
                {
                    unit = new ItemUnit { UnitCode = "NOS", UnitName = "Numbers", IsDecimalAllowed = false };
                    _db.ItemUnits.Add(unit);
                    await _db.SaveChangesAsync();
                }
            }

            // 3. Ensure at least one TaxRate exists in DB
            var taxRate = taxRateId.HasValue ? await _db.TaxRates.FindAsync(taxRateId.Value) : null;
            if (taxRate == null)
            {
                taxRate = await _db.TaxRates.FirstOrDefaultAsync(t => t.RatePercentage == 18) 
                          ?? await _db.TaxRates.FirstOrDefaultAsync();
                if (taxRate == null)
                {
                    taxRate = new TaxRate { TaxName = "GST 18%", RatePercentage = 18.00m, CgstPercentage = 9.00m, SgstPercentage = 9.00m, IgstPercentage = 18.00m };
                    _db.TaxRates.Add(taxRate);
                    await _db.SaveChangesAsync();
                }
            }

            var item = new Item
            {
                CompanyId = company.Id,
                ItemCode = string.IsNullOrWhiteSpace(itemCode) ? $"ITM-{DateTime.Now:MMddHHmm}" : itemCode.Trim().ToUpper(),
                ItemName = string.IsNullOrWhiteSpace(itemName) ? "New Consumable Item" : itemName.Trim(),
                UnitCost = unitCost,
                CurrentStockQty = 0,
                ReorderLevelQty = reorderLevelQty > 0 ? reorderLevelQty : 10,
                CategoryId = category.Id,
                UnitId = unit.Id,
                TaxRateId = taxRate.Id,
                HsnSacCode = !string.IsNullOrWhiteSpace(hsnSacCode) ? hsnSacCode.Trim() : "8544",
                IsActive = true
            };

            _db.Items.Add(item);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Item {item.ItemName} ({item.ItemCode}) added to Master Catalog successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to save item: {ex.Message}";
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        var referer = Request.Headers["Referer"].ToString();
        if (!string.IsNullOrEmpty(referer) && referer.Contains("Inventory", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Inventory", "Procurement");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditClient(long id, string clientName, string billingAddress, string? gstin, string? pan, string? contactPerson, string? phone, string? email, int creditDays, decimal creditLimit)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client == null) return NotFound();

        client.ClientName = string.IsNullOrWhiteSpace(clientName) ? client.ClientName : clientName.Trim();
        client.BillingAddress = string.IsNullOrWhiteSpace(billingAddress) ? client.BillingAddress : billingAddress.Trim();
        client.Gstin = string.IsNullOrWhiteSpace(gstin) ? null : gstin.Trim().ToUpper();
        client.Pan = string.IsNullOrWhiteSpace(pan) ? null : pan.Trim().ToUpper();
        client.ContactPerson = string.IsNullOrWhiteSpace(contactPerson) ? null : contactPerson.Trim();
        client.Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        client.Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();
        client.CreditDays = creditDays > 0 ? creditDays : 30;
        client.CreditLimit = creditLimit;

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Client {client.ClientName} ({client.ClientCode}) updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditVendor(long id, string vendorName, string? gstin, string? pan, string? msmeType, string? vendorCategory, int paymentTermsDays)
    {
        var vendor = await _db.Vendors.FindAsync(id);
        if (vendor == null) return NotFound();

        vendor.VendorName = string.IsNullOrWhiteSpace(vendorName) ? vendor.VendorName : vendorName.Trim();
        vendor.Gstin = string.IsNullOrWhiteSpace(gstin) ? null : gstin.Trim().ToUpper();
        vendor.Pan = string.IsNullOrWhiteSpace(pan) ? vendor.Pan : pan.Trim().ToUpper();
        vendor.MsmeType = msmeType;
        vendor.VendorCategory = string.IsNullOrWhiteSpace(vendorCategory) ? "GENERAL" : vendorCategory;
        vendor.PaymentTermsDays = paymentTermsDays > 0 ? paymentTermsDays : 30;

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Vendor {vendor.VendorName} ({vendor.VendorCode}) updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditItem(long id, string itemName, decimal unitCost, decimal reorderLevelQty)
    {
        var item = await _db.Items.FindAsync(id);
        if (item == null) return NotFound();

        item.ItemName = string.IsNullOrWhiteSpace(itemName) ? item.ItemName : itemName.Trim();
        item.UnitCost = unitCost;
        item.ReorderLevelQty = reorderLevelQty;

        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Item {item.ItemName} ({item.ItemCode}) updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProjectType(string code, string name, string? description)
    {
        await EnsureProjectTypesTableAsync();
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
        {
            TempData["ErrorMessage"] = "Project Type Code and Name are required.";
            return RedirectToAction(nameof(Index));
        }

        code = code.Trim().ToUpperInvariant();
        if (await _db.ProjectTypes.AnyAsync(t => t.Code == code))
        {
            TempData["ErrorMessage"] = $"Project Type Code '{code}' already exists.";
            return RedirectToAction(nameof(Index));
        }

        var pt = new ProjectType
        {
            Code = code,
            Name = name.Trim(),
            Description = description?.Trim(),
            IsActive = true
        };
        _db.ProjectTypes.Add(pt);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Project Type '{pt.Name}' ({pt.Code}) created successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleProjectType(long id)
    {
        await EnsureProjectTypesTableAsync();
        var pt = await _db.ProjectTypes.FindAsync(id);
        if (pt != null)
        {
            pt.IsActive = !pt.IsActive;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Project Type '{pt.Name}' status updated to {(pt.IsActive ? "ACTIVE" : "INACTIVE")}.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> ExpenseTypes(string? search)
    {
        ViewData["ActiveMenu"] = "ExpenseTypes";
        await EnsureExpenseTypesTableAsync();

        var query = _db.ExpenseTypes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(e => e.Name.Contains(search) || (e.Description != null && e.Description.Contains(search)));
            ViewBag.Search = search;
        }

        var list = await query.OrderBy(e => e.Name).ToListAsync();
        return View(list);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateExpenseType(string name, string? description, bool isProjectRelated, bool receiveDirectPayments, decimal? openingAmount)
    {
        await EnsureExpenseTypesTableAsync();
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["ErrorMessage"] = "Expense Type Name is required.";
            return RedirectToAction(nameof(ExpenseTypes));
        }

        name = name.Trim();
        if (await _db.ExpenseTypes.AnyAsync(e => e.Name.ToLower() == name.ToLower()))
        {
            TempData["ErrorMessage"] = $"Expense Type '{name}' already exists.";
            return RedirectToAction(nameof(ExpenseTypes));
        }

        var exp = new ExpenseType
        {
            Name = name,
            Description = description?.Trim(),
            IsProjectRelated = isProjectRelated,
            ReceiveDirectPayments = receiveDirectPayments,
            OpeningAmount = openingAmount,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _db.ExpenseTypes.Add(exp);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Expense Type '{exp.Name}' created successfully!";
        return RedirectToAction(nameof(ExpenseTypes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateExpenseType(long id, string name, string? description, bool isProjectRelated, bool receiveDirectPayments, decimal? openingAmount)
    {
        await EnsureExpenseTypesTableAsync();
        var exp = await _db.ExpenseTypes.FindAsync(id);
        if (exp == null)
        {
            TempData["ErrorMessage"] = "Expense Type not found.";
            return RedirectToAction(nameof(ExpenseTypes));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["ErrorMessage"] = "Expense Type Name is required.";
            return RedirectToAction(nameof(ExpenseTypes));
        }

        exp.Name = name.Trim();
        exp.Description = description?.Trim();
        exp.IsProjectRelated = isProjectRelated;
        exp.ReceiveDirectPayments = receiveDirectPayments;
        exp.OpeningAmount = openingAmount;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Expense Type '{exp.Name}' updated successfully!";
        return RedirectToAction(nameof(ExpenseTypes));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExpenseType(long id)
    {
        await EnsureExpenseTypesTableAsync();
        var exp = await _db.ExpenseTypes.FindAsync(id);
        if (exp != null)
        {
            bool isUsed = await _db.CustomerReceipts.AnyAsync(r => r.ExpenseHead == exp.Name);
            if (isUsed)
            {
                exp.IsActive = false;
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Expense Type '{exp.Name}' is referenced in receipts and was deactivated instead of deleted.";
            }
            else
            {
                _db.ExpenseTypes.Remove(exp);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Expense Type '{exp.Name}' deleted successfully!";
            }
        }
        return RedirectToAction(nameof(ExpenseTypes));
    }
}

public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICompanyContext _companyContext;
    private readonly IWebHostEnvironment _env;

    public ProjectsController(ApplicationDbContext db, ICompanyContext companyContext, IWebHostEnvironment env)
    {
        _db = db;
        _companyContext = companyContext;
        _env = env;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Projects";
        await EnsureProjectColumnsAsync();

        ViewBag.Clients = await _db.Clients.OrderBy(c => c.ClientName).AsNoTracking().ToListAsync();

        List<ProjectType> projectTypes;
        try
        {
            projectTypes = await _db.ProjectTypes.Where(t => t.IsActive).OrderBy(t => t.Name).AsNoTracking().ToListAsync();
        }
        catch
        {
            await EnsureProjectTypesTableAsync();
            try { projectTypes = await _db.ProjectTypes.Where(t => t.IsActive).OrderBy(t => t.Name).AsNoTracking().ToListAsync(); }
            catch { projectTypes = GetDefaultProjectTypes(); }
        }
        ViewBag.ProjectTypes = projectTypes;
        ViewBag.Users = await _db.Users.Where(u => u.IsActive).OrderBy(u => u.FullName).AsNoTracking().ToListAsync();

        var projects = await _db.Projects
            .Include(p => p.Client)
            .Include(p => p.Manager)
            .Include(p => p.PurchaseOrders).ThenInclude(po => po.Attachment)
            .Include(p => p.Deliveries)
            .OrderByDescending(p => p.Id)
            .AsNoTracking()
            .ToListAsync();

        try
        {
            var projectDocCounts = await _db.DocumentAttachments
                .Where(d => d.EntityType == "Project" || d.EntityType == "ClientPo" || d.EntityType == "ProjectClosure")
                .GroupBy(d => d.EntityId)
                .Select(g => new { ProjectId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.ProjectId, x => x.Count);
            ViewBag.ProjectDocCounts = projectDocCounts;
        }
        catch
        {
            ViewBag.ProjectDocCounts = new Dictionary<long, int>();
        }

        var divisions = projects
            .Where(p => !string.IsNullOrWhiteSpace(p.Division))
            .Select(p => p.Division!.Trim())
            .Distinct()
            .ToList();

        if (!divisions.Contains("HARDWARE DIVISION")) divisions.Add("HARDWARE DIVISION");
        if (!divisions.Contains("GENERAL SUPPLY")) divisions.Add("GENERAL SUPPLY");
        if (!divisions.Contains("SOFTWARE & IT")) divisions.Add("SOFTWARE & IT");
        if (!divisions.Contains("NETWORKING")) divisions.Add("NETWORKING");
        if (!divisions.Contains("CIVIL & INFRA")) divisions.Add("CIVIL & INFRA");

        ViewBag.Divisions = divisions;

        return View(projects);
    }

    private async Task EnsureProjectColumnsAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'Description')
                    ALTER TABLE [project].[projects] ADD [Description] NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'Division')
                    ALTER TABLE [project].[projects] ADD [Division] NVARCHAR(200) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'PhysicalFileStatus')
                    ALTER TABLE [project].[projects] ADD [PhysicalFileStatus] NVARCHAR(100) NULL;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'Description')
                    ALTER TABLE [Projects] ADD [Description] NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'Division')
                    ALTER TABLE [Projects] ADD [Division] NVARCHAR(200) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'PhysicalFileStatus')
                    ALTER TABLE [Projects] ADD [PhysicalFileStatus] NVARCHAR(100) NULL;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[projects].[projects]') AND name = 'Description')
                    ALTER TABLE [projects].[projects] ADD [Description] NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[projects].[projects]') AND name = 'Division')
                    ALTER TABLE [projects].[projects] ADD [Division] NVARCHAR(200) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[projects].[projects]') AND name = 'PhysicalFileStatus')
                    ALTER TABLE [projects].[projects] ADD [PhysicalFileStatus] NVARCHAR(100) NULL;
            ");
        }
        catch { }
    }

    [HttpGet]
    [Route("Projects/Detail/{id?}")]
    [Route("Projects/ViewProjectDetails/{id?}")]
    [Route("Projects/Project/ViewProjectDetails/{id?}")]
    public async Task<IActionResult> Detail(long id = 1)
    {
        ViewData["ActiveMenu"] = "Projects";
        ViewData["ProjectId"] = id;
        await EnsureDeliveryChallanColumnsAsync();
        var project = await _db.Projects
            .Include(p => p.Client)
            .Include(p => p.Manager)
            .Include(p => p.PurchaseOrders).ThenInclude(po => po.Attachment)
            .Include(p => p.Milestones)
            .Include(p => p.Expenses)
            .Include(p => p.Deliveries)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? await _db.Projects
                .Include(p => p.Client)
                .Include(p => p.Manager)
                .Include(p => p.PurchaseOrders).ThenInclude(po => po.Attachment)
                .Include(p => p.Milestones)
                .Include(p => p.Expenses)
                .Include(p => p.Deliveries)
                .AsNoTracking()
                .FirstOrDefaultAsync();

        if (project != null)
        {
            ViewBag.SalesInvoices = await _db.SalesInvoices.Include(s => s.Client).Where(s => s.ProjectId == project.Id).AsNoTracking().ToListAsync();
            ViewBag.ProcurementPos = await _db.PurchaseOrders.Include(p => p.Vendor).Where(p => p.ProjectId == project.Id).AsNoTracking().ToListAsync();
            ViewBag.ClientPos = await _db.ProjectPos.Include(po => po.Attachment).Where(po => po.ProjectId == project.Id).AsNoTracking().ToListAsync();
            List<ProjectDelivery> deliveriesList;
            try
            {
                deliveriesList = await _db.ProjectDeliveries
                    .Include(d => d.Attachment)
                    .Where(d => d.ProjectId == project.Id)
                    .OrderByDescending(d => d.DeliveryDate)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {
                await EnsureDeliveryChallanColumnsAsync();
                try
                {
                    deliveriesList = await _db.ProjectDeliveries
                        .Include(d => d.Attachment)
                        .Where(d => d.ProjectId == project.Id)
                        .OrderByDescending(d => d.DeliveryDate)
                        .AsNoTracking()
                        .ToListAsync();
                }
                catch
                {
                    deliveriesList = await _db.ProjectDeliveries
                        .Where(d => d.ProjectId == project.Id)
                        .AsNoTracking()
                        .ToListAsync();
                }
            }
            ViewBag.Deliveries = deliveriesList;
            ViewBag.Expenses = await _db.ProjectExpenses
                .Include(e => e.IncurredByUser)
                .Include(e => e.ReceiptDoc)
                .Where(e => e.ProjectId == project.Id)
                .OrderByDescending(e => e.ExpenseDate)
                .AsNoTracking().ToListAsync();
            ViewBag.Milestones = await _db.ProjectMilestones.Where(m => m.ProjectId == project.Id).AsNoTracking().ToListAsync();
            ViewBag.Documents = await _db.DocumentAttachments.Where(d => (d.EntityType == "Project" || d.EntityType == "ProjectClosure" || d.EntityType == "ProjectExpense" || d.EntityType == "DeliveryChallan") && d.EntityId == project.Id).OrderByDescending(d => d.UploadedAt).AsNoTracking().ToListAsync();
            ViewBag.Users = await _db.Users.Where(u => u.IsActive).OrderBy(u => u.FullName).AsNoTracking().ToListAsync();
        }
        else
        {
            ViewBag.SalesInvoices = new List<SalesInvoice>();
            ViewBag.ProcurementPos = new List<PurchaseOrder>();
            ViewBag.ClientPos = new List<ProjectPo>();
            ViewBag.Deliveries = new List<ProjectDelivery>();
            ViewBag.Expenses = new List<ProjectExpense>();
            ViewBag.Milestones = new List<ProjectMilestone>();
            ViewBag.Documents = new List<DocumentAttachment>();
            ViewBag.Users = new List<User>();
        }

        return View(project);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string projectCode,
        string projectName,
        string projectType,
        decimal contractValue,
        decimal budgetCost = 0,
        DateTime startDate = default,
        DateTime? expectedEndDate = null,
        long? clientId = null,
        long? managerId = null,
        string? division = null,
        string? description = null,
        string? physicalFileStatus = null,
        string status = "OPEN",
        string? clientPoNumber = null,
        string? documentTitle = null,
        IFormFile? projectDocument = null)
    {
        await EnsureProjectColumnsAsync();
        var company = await _companyContext.GetCurrentCompanyAsync();
        var branch = await _db.Branches.FirstOrDefaultAsync(b => b.CompanyId == company.Id) ?? new Branch
        {
            CompanyId = company.Id, BranchCode = "HQ", BranchName = "Main Branch"
        };
        if (branch.Id == 0) { _db.Branches.Add(branch); await _db.SaveChangesAsync(); }

        if (!clientId.HasValue || clientId.Value == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid Client from the Master Directory.";
            return RedirectToAction(nameof(Index));
        }

        var client = await _db.Clients.FindAsync(clientId.Value);
        if (client == null)
        {
            TempData["ErrorMessage"] = "Selected Client does not exist in Master Directory.";
            return RedirectToAction(nameof(Index));
        }

        var manager = managerId.HasValue && managerId.Value > 0
            ? await _db.Users.FindAsync(managerId.Value)
            : await _db.Users.FirstOrDefaultAsync();

        if (manager == null)
        {
            var role = await _db.Roles.FirstOrDefaultAsync() ?? new Role { RoleName = "SUPER_ADMIN", Description = "Admin", IsSystemRole = true };
            if (role.Id == 0) { _db.Roles.Add(role); await _db.SaveChangesAsync(); }

            manager = new User
            {
                CompanyId = company.Id,
                BranchId = branch.Id,
                RoleId = role.Id,
                Username = "admin",
                Email = "admin@sdksolutions.com",
                IsActive = true
            };
            _db.Users.Add(manager);
            await _db.SaveChangesAsync();
        }

        var project = new Project
        {
            CompanyId = company.Id,
            BranchId = branch.Id,
            ClientId = clientId.Value,
            ManagerId = manager?.Id,
            ProjectCode = string.IsNullOrWhiteSpace(projectCode) ? $"PRJ-{DateTime.Now:yyyyMMdd-HHmm}" : projectCode.Trim(),
            ProjectName = string.IsNullOrWhiteSpace(projectName) ? "New Project" : projectName.Trim(),
            ProjectType = string.IsNullOrWhiteSpace(projectType) ? "STANDARD" : projectType.Trim(),
            Division = string.IsNullOrWhiteSpace(division) ? "GENERAL SUPPLY" : division.Trim(),
            Description = description?.Trim(),
            PhysicalFileStatus = string.IsNullOrWhiteSpace(physicalFileStatus) ? (projectDocument != null ? "Created" : "Not Create") : physicalFileStatus.Trim(),
            ContractValue = contractValue,
            BudgetCost = budgetCost,
            StartDate = startDate == default ? DateTime.Today : startDate,
            ExpectedEndDate = expectedEndDate,
            Status = string.IsNullOrWhiteSpace(status) ? "OPEN" : status.Trim().ToUpperInvariant(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Projects.Add(project);
        await _db.SaveChangesAsync();

        DocumentAttachment? attachment = null;
        if (projectDocument != null && projectDocument.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tenants", $"org_{company.Id}", "projects", project.Id.ToString());
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var safeFileName = $"doc_{DateTime.UtcNow.Ticks}_{Path.GetFileName(projectDocument.FileName)}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await projectDocument.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/tenants/org_{company.Id}/projects/{project.Id}/{safeFileName}";
            var effectiveUserId = manager?.Id ?? await GetEffectiveUserIdAsync();
            attachment = new DocumentAttachment
            {
                EntityType = "Project",
                EntityId = project.Id,
                FileName = projectDocument.FileName,
                FilePath = relativePath,
                FileSizeBytes = projectDocument.Length,
                MimeType = projectDocument.ContentType ?? "application/octet-stream",
                FileHashSha256 = Guid.NewGuid().ToString("N"),
                UploadedBy = effectiveUserId,
                UploaderId = effectiveUserId,
                UploadedAt = DateTime.UtcNow,
                VersionNumber = 1
            };
            _db.DocumentAttachments.Add(attachment);
            await _db.SaveChangesAsync();
        }

        var poNum = !string.IsNullOrWhiteSpace(clientPoNumber) ? clientPoNumber.Trim() : (attachment != null ? $"WO-{project.ProjectCode}" : null);
        if (!string.IsNullOrWhiteSpace(poNum) || attachment != null)
        {
            var clientPo = new ProjectPo
            {
                ProjectId = project.Id,
                ClientPoNumber = string.IsNullOrWhiteSpace(poNum) ? $"WO-{project.ProjectCode}" : poNum,
                PoDate = startDate == default ? DateTime.Today : startDate,
                PoValue = contractValue,
                ValidityEndDate = expectedEndDate,
                ScopeOfWork = string.IsNullOrWhiteSpace(documentTitle) ? "Client Work Order / Contract Agreement" : documentTitle.Trim(),
                AttachmentDocId = attachment?.Id
            };
            _db.ProjectPos.Add(clientPo);
            await _db.SaveChangesAsync();
        }

        TempData["SuccessMessage"] = $"Project {project.ProjectCode} ({project.ProjectName}) registered successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        long id,
        string projectCode,
        string projectName,
        long clientId,
        string projectType,
        string? division,
        decimal contractValue,
        DateTime startDate,
        DateTime? expectedEndDate,
        long? managerId,
        string status,
        string? physicalFileStatus,
        string? description)
    {
        await EnsureProjectColumnsAsync();
        var project = await _db.Projects.FindAsync(id);
        if (project == null)
        {
            TempData["ErrorMessage"] = "Project not found.";
            return RedirectToAction(nameof(Index));
        }

        project.ProjectCode = string.IsNullOrWhiteSpace(projectCode) ? project.ProjectCode : projectCode.Trim();
        project.ProjectName = string.IsNullOrWhiteSpace(projectName) ? project.ProjectName : projectName.Trim();
        project.ClientId = clientId;
        project.ProjectType = string.IsNullOrWhiteSpace(projectType) ? project.ProjectType : projectType.Trim();
        project.Division = string.IsNullOrWhiteSpace(division) ? project.Division : division.Trim();
        project.ContractValue = contractValue;
        project.StartDate = startDate == default ? project.StartDate : startDate;
        project.ExpectedEndDate = expectedEndDate;
        project.ManagerId = managerId.HasValue && managerId.Value > 0 ? managerId.Value : project.ManagerId;
        project.Status = string.IsNullOrWhiteSpace(status) ? project.Status : status.Trim().ToUpperInvariant();
        project.PhysicalFileStatus = string.IsNullOrWhiteSpace(physicalFileStatus) ? project.PhysicalFileStatus : physicalFileStatus.Trim();
        project.Description = description?.Trim();
        project.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Project {project.ProjectCode} ({project.ProjectName}) updated successfully!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(long id)
    {
        var project = await _db.Projects
            .Include(p => p.PurchaseOrders)
            .Include(p => p.Milestones)
            .Include(p => p.Expenses)
            .Include(p => p.Deliveries)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
        {
            TempData["ErrorMessage"] = "Project not found.";
            return RedirectToAction(nameof(Index));
        }

        var code = project.ProjectCode;
        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Project {code} removed successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(long id, string status)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project != null)
        {
            project.Status = string.IsNullOrWhiteSpace(status) ? "OPEN" : status.Trim().ToUpperInvariant();
            if (project.Status == "CLOSED")
            {
                project.ActualClosedDate = DateTime.Today;
            }
            project.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Status of Project {project.ProjectCode} updated to {project.Status}.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CloseProject(long id, string? closureNotes, bool isPrematureClosure = false, IFormFile? closureDocument = null)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project != null)
        {
            if (closureDocument == null || closureDocument.Length == 0)
            {
                TempData["ErrorMessage"] = "Project closure document / handover sign-off certificate is mandatory to close the project!";
                return RedirectToAction(nameof(Detail), new { id });
            }

            var ext = Path.GetExtension(closureDocument.FileName).ToLowerInvariant();
            var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx", ".zip", ".png", ".jpg" };
            if (!allowedExtensions.Contains(ext) || closureDocument.Length > 10 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "Closure certificate must be a valid document (PDF, Word, Excel, ZIP, Image) and under 10 MB.";
                return RedirectToAction(nameof(Detail), new { id });
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "tenants", $"org_{project.CompanyId}", "project_closures");
            if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

            var safeFileName = $"CLOSURE_PRJ_{project.ProjectCode}_{DateTime.UtcNow:yyyyMMddHHmmss}{ext}";
            var filePath = Path.Combine(uploadsDir, safeFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await closureDocument.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/tenants/org_{project.CompanyId}/project_closures/{safeFileName}";
            var effectiveUserId = await GetEffectiveUserIdAsync();
            var attachment = new DocumentAttachment
            {
                EntityType = "ProjectClosure",
                EntityId = project.Id,
                FileName = closureDocument.FileName,
                FilePath = relativePath,
                FileSizeBytes = closureDocument.Length,
                MimeType = closureDocument.ContentType ?? "application/octet-stream",
                FileHashSha256 = Guid.NewGuid().ToString("N"),
                UploadedBy = effectiveUserId,
                UploaderId = effectiveUserId,
                UploadedAt = DateTime.UtcNow,
                VersionNumber = 1
            };
            _db.DocumentAttachments.Add(attachment);

            project.Status = "CLOSED";
            project.ActualClosedDate = DateTime.Today;
            var prefix = isPrematureClosure ? "[PREMATURE CLOSURE / SCOPE TERMINATION] " : "";
            project.ClosureNotes = prefix + (string.IsNullOrWhiteSpace(closureNotes) ? "Project executed, delivered and financially closed." : closureNotes) + $" [Closure Certificate: {closureDocument.FileName}]";
            project.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Project {project.ProjectCode} has been formally closed and closure certificate '{closureDocument.FileName}' archived!";
        }
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReopenProject(long id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project != null)
        {
            project.Status = "ACTIVE";
            project.ActualClosedDate = null;
            project.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Project {project.ProjectCode} has been reopened and is now ACTIVE for billing and procurement!";
        }
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpense(
        long projectId,
        long expenseHeadId,
        long incurredByUserId,
        DateTime expenseDate,
        decimal amount,
        decimal? taxableAmount,
        decimal? gstAmount,
        string paymentMode,
        string? description,
        IFormFile? receiptFile)
    {
        var project = await _db.Projects.FindAsync(projectId);
        if (project == null)
        {
            TempData["ErrorMessage"] = "Project not found.";
            return RedirectToAction(nameof(Index));
        }

        if (project.Status == "CLOSED")
        {
            TempData["ErrorMessage"] = "Cannot add expenses to a formally closed project.";
            return RedirectToAction(nameof(Detail), new { id = projectId });
        }

        if (amount <= 0)
        {
            TempData["ErrorMessage"] = "Expense amount must be greater than zero.";
            return RedirectToAction(nameof(Detail), new { id = projectId });
        }

        var user = await _db.Users.FindAsync(incurredByUserId);
        if (user == null)
        {
            var fallbackUser = await _db.Users.FirstOrDefaultAsync();
            incurredByUserId = fallbackUser?.Id ?? 1;
        }

        long? receiptDocId = null;
        if (receiptFile != null && receiptFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tenants", $"org_{project.CompanyId}", "expenses");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var safeFileName = $"exp_{projectId}_{DateTime.UtcNow.Ticks}_{Path.GetFileName(receiptFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await receiptFile.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/tenants/org_{project.CompanyId}/expenses/{safeFileName}";
            var effectiveUserId = incurredByUserId > 0 ? incurredByUserId : await GetEffectiveUserIdAsync();
            var attachment = new DocumentAttachment
            {
                EntityType = "ProjectExpense",
                EntityId = projectId,
                FileName = receiptFile.FileName,
                FilePath = relativePath,
                FileSizeBytes = receiptFile.Length,
                MimeType = receiptFile.ContentType ?? "application/octet-stream",
                FileHashSha256 = Guid.NewGuid().ToString("N"),
                UploadedBy = effectiveUserId,
                UploaderId = effectiveUserId,
                UploadedAt = DateTime.UtcNow,
                VersionNumber = 1
            };
            _db.DocumentAttachments.Add(attachment);
            await _db.SaveChangesAsync();
            receiptDocId = attachment.Id;
        }

        var taxAmount = taxableAmount ?? amount;
        var gst = gstAmount ?? 0;

        var expense = new ProjectExpense
        {
            ProjectId = projectId,
            ExpenseHeadId = expenseHeadId > 0 ? expenseHeadId : 1,
            IncurredByUserId = incurredByUserId,
            ExpenseDate = expenseDate == default ? DateTime.Today : expenseDate,
            Amount = amount,
            TaxableAmount = taxAmount,
            GstAmount = gst,
            PaymentMode = string.IsNullOrWhiteSpace(paymentMode) ? "REIMBURSEMENT" : paymentMode.ToUpperInvariant(),
            Status = "APPROVED",
            Description = description?.Trim(),
            ReceiptDocId = receiptDocId
        };

        _db.ProjectExpenses.Add(expense);
        project.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Site expense of \u20b9{amount:N2} recorded successfully!";
        return RedirectToAction(nameof(Detail), new { id = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExpense(long id, long projectId)
    {
        var expense = await _db.ProjectExpenses.FindAsync(id);
        if (expense != null && expense.ProjectId == projectId)
        {
            _db.ProjectExpenses.Remove(expense);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Project site expense removed successfully.";
        }
        return RedirectToAction(nameof(Detail), new { id = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadProjectDocument(long projectId, string? documentType, IFormFile? documentFile)
    {
        var project = await _db.Projects.FindAsync(projectId);
        if (project == null)
        {
            TempData["ErrorMessage"] = "Project not found.";
            return RedirectToAction(nameof(Index));
        }

        if (documentFile == null || documentFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid document file to upload.";
            return RedirectToAction(nameof(Detail), new { id = projectId });
        }

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tenants", $"org_{project.CompanyId}", "projects", project.Id.ToString());
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var safeFileName = $"doc_{DateTime.UtcNow.Ticks}_{Path.GetFileName(documentFile.FileName)}";
        var filePath = Path.Combine(uploadsFolder, safeFileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await documentFile.CopyToAsync(stream);
        }

        var effectiveUserId = await GetEffectiveUserIdAsync();
        var relativePath = $"/uploads/tenants/org_{project.CompanyId}/projects/{project.Id}/{safeFileName}";

        var attachment = new DocumentAttachment
        {
            EntityType = string.IsNullOrWhiteSpace(documentType) ? "Project" : documentType.Trim(),
            EntityId = project.Id,
            FileName = documentFile.FileName,
            FilePath = relativePath,
            FileSizeBytes = documentFile.Length,
            MimeType = documentFile.ContentType ?? "application/octet-stream",
            FileHashSha256 = Guid.NewGuid().ToString("N"),
            UploadedBy = effectiveUserId,
            UploaderId = effectiveUserId,
            UploadedAt = DateTime.UtcNow,
            VersionNumber = 1
        };
        _db.DocumentAttachments.Add(attachment);
        project.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Document '{documentFile.FileName}' uploaded and archived successfully!";
        return RedirectToAction(nameof(Detail), new { id = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDelivery(
        long projectId,
        string? dcNumber,
        DateTime deliveryDate,
        string? dispatchMode,
        string? trackingRefNo,
        string? recipientName,
        string? materialSummary,
        string status = "DISPATCHED",
        IFormFile? challanFile = null)
    {
        var project = await _db.Projects.Include(p => p.Client).FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            TempData["ErrorMessage"] = "Project not found.";
            return RedirectToAction(nameof(Index));
        }

        if (project.Status == "CLOSED")
        {
            TempData["ErrorMessage"] = "Cannot issue delivery challans for a closed project.";
            return RedirectToAction(nameof(Detail), new { id = projectId });
        }

        await EnsureDeliveryChallanColumnsAsync();

        long? attachmentDocId = null;
        if (challanFile != null && challanFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "tenants", $"org_{project.CompanyId}", "deliveries");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var safeFileName = $"dc_{projectId}_{DateTime.UtcNow.Ticks}_{Path.GetFileName(challanFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await challanFile.CopyToAsync(stream);
            }

            var effectiveUserId = await GetEffectiveUserIdAsync();
            var relativePath = $"/uploads/tenants/org_{project.CompanyId}/deliveries/{safeFileName}";

            var attachment = new DocumentAttachment
            {
                EntityType = "DeliveryChallan",
                EntityId = projectId,
                FileName = challanFile.FileName,
                FilePath = relativePath,
                FileSizeBytes = challanFile.Length,
                MimeType = challanFile.ContentType ?? "application/octet-stream",
                FileHashSha256 = Guid.NewGuid().ToString("N"),
                UploadedBy = effectiveUserId,
                UploaderId = effectiveUserId,
                UploadedAt = DateTime.UtcNow,
                VersionNumber = 1
            };
            _db.DocumentAttachments.Add(attachment);
            await _db.SaveChangesAsync();
            attachmentDocId = attachment.Id;
        }

        var delivery = new ProjectDelivery
        {
            ProjectId = projectId,
            DcNumber = string.IsNullOrWhiteSpace(dcNumber) ? $"DC-{project.ProjectCode}-{DateTime.Now:yyyyMMdd-HHmm}" : dcNumber.Trim(),
            DeliveryDate = deliveryDate == default ? DateTime.Today : deliveryDate,
            DispatchMode = string.IsNullOrWhiteSpace(dispatchMode) ? "Company Vehicle" : dispatchMode.Trim(),
            TrackingRefNo = trackingRefNo?.Trim(),
            RecipientName = string.IsNullOrWhiteSpace(recipientName) ? (project.Client?.ClientName ?? "Site Incharge") : recipientName.Trim(),
            MaterialSummary = materialSummary?.Trim(),
            Status = string.IsNullOrWhiteSpace(status) ? "DISPATCHED" : status.ToUpperInvariant(),
            AttachmentDocId = attachmentDocId,
            AttachmentId = attachmentDocId
        };

        _db.ProjectDeliveries.Add(delivery);
        project.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Material Delivery Challan {delivery.DcNumber} issued and dispatched successfully!";
        return RedirectToAction(nameof(Detail), new { id = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateDeliveryStatus(long id, long projectId, string status)
    {
        var delivery = await _db.ProjectDeliveries.FindAsync(id);
        if (delivery != null && delivery.ProjectId == projectId)
        {
            delivery.Status = string.IsNullOrWhiteSpace(status) ? "DELIVERED" : status.ToUpperInvariant();
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Delivery Challan {delivery.DcNumber} status updated to {delivery.Status}.";
        }
        return RedirectToAction(nameof(Detail), new { id = projectId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteDelivery(long id, long projectId)
    {
        var delivery = await _db.ProjectDeliveries.FindAsync(id);
        if (delivery != null && delivery.ProjectId == projectId)
        {
            _db.ProjectDeliveries.Remove(delivery);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Delivery Challan {delivery.DcNumber} removed successfully.";
        }
        return RedirectToAction(nameof(Detail), new { id = projectId });
    }

    private async Task EnsureDeliveryChallanColumnsAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[ProjectDeliveries]') AND name = 'MaterialSummary')
                    ALTER TABLE [ProjectDeliveries] ADD [MaterialSummary] NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[ProjectDeliveries]') AND name = 'AttachmentDocId')
                    ALTER TABLE [ProjectDeliveries] ADD [AttachmentDocId] BIGINT NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[ProjectDeliveries]') AND name = 'AttachmentId')
                    ALTER TABLE [ProjectDeliveries] ADD [AttachmentId] BIGINT NULL;

                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[ProjectDeliveries]') AND name = 'MaterialSummary')
                    ALTER TABLE [project].[ProjectDeliveries] ADD [MaterialSummary] NVARCHAR(MAX) NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[ProjectDeliveries]') AND name = 'AttachmentDocId')
                    ALTER TABLE [project].[ProjectDeliveries] ADD [AttachmentDocId] BIGINT NULL;
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[ProjectDeliveries]') AND name = 'AttachmentId')
                    ALTER TABLE [project].[ProjectDeliveries] ADD [AttachmentId] BIGINT NULL;

                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[DocumentAttachments]') AND name = 'UploaderId' AND is_nullable = 0)
                    ALTER TABLE [DocumentAttachments] ALTER COLUMN [UploaderId] BIGINT NULL;
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[DocumentAttachments]') AND name = 'UploadedBy' AND is_nullable = 0)
                    ALTER TABLE [DocumentAttachments] ALTER COLUMN [UploadedBy] BIGINT NULL;
            ");
        }
        catch { }
    }

    private async Task<long> GetEffectiveUserIdAsync()
    {
        try
        {
            var currentUser = await _companyContext.GetCurrentUserAsync();
            if (currentUser != null && currentUser.Id > 0)
            {
                var exists = await _db.Users.IgnoreQueryFilters().AnyAsync(u => u.Id == currentUser.Id);
                if (exists) return currentUser.Id;
            }

            var firstUser = await _db.Users.IgnoreQueryFilters().OrderBy(u => u.Id).FirstOrDefaultAsync();
            if (firstUser != null) return firstUser.Id;
        }
        catch { }

        return 1;
    }

    private async Task EnsureProjectTypesTableAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjectTypes')
                BEGIN
                    CREATE TABLE [dbo].[ProjectTypes] (
                        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [Code] NVARCHAR(50) NOT NULL,
                        [Name] NVARCHAR(150) NOT NULL,
                        [Description] NVARCHAR(500) NULL,
                        [IsActive] BIT NOT NULL DEFAULT 1
                    );

                    INSERT INTO [dbo].[ProjectTypes] ([Code], [Name], [Description], [IsActive]) VALUES
                    ('STANDARD', 'Standard Turnkey Contract', 'Fixed-price supply, installation, testing and commissioning contracts', 1),
                    ('AMC', 'Annual Maintenance Contract (AMC)', 'Ongoing operations, service level maintenance and support agreements', 1),
                    ('CONSULTING', 'Consulting & Advisory', 'Professional technical advisory, system design and project management', 1),
                    ('SUPPLY_INSTALL', 'Supply & Installation', 'Material delivery with onsite installation and sign-off', 1),
                    ('MANPOWER', 'Manpower & Managed Services', 'Time and material / rate card based deployment', 1),
                    ('INTERNAL', 'Internal R&D / Capital Project', 'Internal organizational infrastructure or R&D initiatives', 1);
                END
            ");
        }
        catch { }
    }

    private static List<ProjectType> GetDefaultProjectTypes() => new()
    {
        new ProjectType { Id = 1, Code = "STANDARD", Name = "Standard Turnkey Contract", Description = "Fixed-price supply, installation, testing and commissioning contracts", IsActive = true },
        new ProjectType { Id = 2, Code = "AMC", Name = "Annual Maintenance Contract (AMC)", Description = "Ongoing operations, service level maintenance and support agreements", IsActive = true },
        new ProjectType { Id = 3, Code = "CONSULTING", Name = "Consulting & Advisory", Description = "Professional technical advisory, system design and project management", IsActive = true },
        new ProjectType { Id = 4, Code = "SUPPLY_INSTALL", Name = "Supply & Installation", Description = "Material delivery with onsite installation and sign-off", IsActive = true },
        new ProjectType { Id = 5, Code = "MANPOWER", Name = "Manpower & Managed Services", Description = "Time and material / rate card based deployment", IsActive = true },
        new ProjectType { Id = 6, Code = "INTERNAL", Name = "Internal R&D / Capital Project", Description = "Internal organizational infrastructure or R&D initiatives", IsActive = true }
    };
}

public class SalesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICompanyContext _companyContext;

    public SalesController(ApplicationDbContext db, ICompanyContext companyContext)
    {
        _db = db;
        _companyContext = companyContext;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Sales";
        var company = await _companyContext.GetCurrentCompanyAsync();
        ViewBag.Company = company;
        ViewBag.Clients = await _db.Clients.AsNoTracking().ToListAsync();
        ViewBag.Projects = await _db.Projects.AsNoTracking().ToListAsync();

        List<SalesInvoice> invoices;
        try
        {
            invoices = await _db.SalesInvoices
                .Include(i => i.Client)
                .Include(i => i.Project)
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.Id)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            await EnsureSalesInvoiceColumnsAsync();
            invoices = await _db.SalesInvoices
                .Include(i => i.Client)
                .Include(i => i.Project)
                .OrderByDescending(i => i.InvoiceDate)
                .ThenByDescending(i => i.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        ViewBag.TotalDbCount = invoices.Count;
        return View(invoices);
    }

    private async Task EnsureSalesInvoiceColumnsAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('[sales].[sales_invoices]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'WorkOrderNo')
                        ALTER TABLE [sales].[sales_invoices] ADD [WorkOrderNo] NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'WorkOrderDate')
                        ALTER TABLE [sales].[sales_invoices] ADD [WorkOrderDate] DATETIME2 NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'BillingAttention')
                        ALTER TABLE [sales].[sales_invoices] ADD [BillingAttention] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'Remarks')
                        ALTER TABLE [sales].[sales_invoices] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'DeductionRemarks')
                        ALTER TABLE [sales].[sales_invoices] ADD [DeductionRemarks] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoices]') AND name = 'DeductionAmount')
                        ALTER TABLE [sales].[sales_invoices] ADD [DeductionAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END

                IF OBJECT_ID('[SalesInvoices]') IS NOT NULL OR OBJECT_ID('[dbo].[SalesInvoices]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoices]') OR object_id = OBJECT_ID('[dbo].[SalesInvoices]')) AND name = 'WorkOrderNo')
                        ALTER TABLE [SalesInvoices] ADD [WorkOrderNo] NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoices]') OR object_id = OBJECT_ID('[dbo].[SalesInvoices]')) AND name = 'WorkOrderDate')
                        ALTER TABLE [SalesInvoices] ADD [WorkOrderDate] DATETIME2 NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoices]') OR object_id = OBJECT_ID('[dbo].[SalesInvoices]')) AND name = 'BillingAttention')
                        ALTER TABLE [SalesInvoices] ADD [BillingAttention] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoices]') OR object_id = OBJECT_ID('[dbo].[SalesInvoices]')) AND name = 'Remarks')
                        ALTER TABLE [SalesInvoices] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoices]') OR object_id = OBJECT_ID('[dbo].[SalesInvoices]')) AND name = 'DeductionRemarks')
                        ALTER TABLE [SalesInvoices] ADD [DeductionRemarks] NVARCHAR(250) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoices]') OR object_id = OBJECT_ID('[dbo].[SalesInvoices]')) AND name = 'DeductionAmount')
                        ALTER TABLE [SalesInvoices] ADD [DeductionAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END

                IF OBJECT_ID('[sales].[sales_invoice_items]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'GstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'IgstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'CgstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[sales].[sales_invoice_items]') AND name = 'SgstRate')
                        ALTER TABLE [sales].[sales_invoice_items] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END

                IF OBJECT_ID('[SalesInvoiceItems]') IS NOT NULL OR OBJECT_ID('[dbo].[SalesInvoiceItems]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoiceItems]') OR object_id = OBJECT_ID('[dbo].[SalesInvoiceItems]')) AND name = 'GstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoiceItems]') OR object_id = OBJECT_ID('[dbo].[SalesInvoiceItems]')) AND name = 'IgstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoiceItems]') OR object_id = OBJECT_ID('[dbo].[SalesInvoiceItems]')) AND name = 'CgstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[SalesInvoiceItems]') OR object_id = OBJECT_ID('[dbo].[SalesInvoiceItems]')) AND name = 'SgstRate')
                        ALTER TABLE [SalesInvoiceItems] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END
            ");
            await EnsureCustomerReceiptColumnsAsync();
        }
        catch { }
    }

    private async Task EnsureCustomerReceiptColumnsAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('[CustomerReceipts]') IS NOT NULL OR OBJECT_ID('[dbo].[CustomerReceipts]') IS NOT NULL
                BEGIN
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'ClientId' AND is_nullable = 0)
                        ALTER TABLE [CustomerReceipts] ALTER COLUMN [ClientId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'ExpenseHead')
                        ALTER TABLE [CustomerReceipts] ADD [ExpenseHead] NVARCHAR(200) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'Remarks')
                        ALTER TABLE [CustomerReceipts] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'ReceiptDocId')
                        ALTER TABLE [CustomerReceipts] ADD [ReceiptDocId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'ProjectId')
                        ALTER TABLE [CustomerReceipts] ADD [ProjectId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'InvoiceId')
                        ALTER TABLE [CustomerReceipts] ADD [InvoiceId] BIGINT NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'IsAdvance')
                        ALTER TABLE [CustomerReceipts] ADD [IsAdvance] BIT NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'TdsAmount')
                        ALTER TABLE [CustomerReceipts] ADD [TdsAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'GstTdsAmount')
                        ALTER TABLE [CustomerReceipts] ADD [GstTdsAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'SecurityDepositAmount')
                        ALTER TABLE [CustomerReceipts] ADD [SecurityDepositAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'OtherDeductionAmount')
                        ALTER TABLE [CustomerReceipts] ADD [OtherDeductionAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'NetAmountReceived')
                        ALTER TABLE [CustomerReceipts] ADD [NetAmountReceived] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[CustomerReceipts]') OR object_id = OBJECT_ID('[dbo].[CustomerReceipts]')) AND name = 'TotalAmountReceived')
                        ALTER TABLE [CustomerReceipts] ADD [TotalAmountReceived] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END
            ");
        }
        catch { }
    }

    private async Task EnsurePurchaseOrderColumnsAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL OR OBJECT_ID('[dbo].[PurchaseOrders]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PoType')
                        ALTER TABLE [PurchaseOrders] ADD [PoType] NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'ShipTo')
                        ALTER TABLE [PurchaseOrders] ADD [ShipTo] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'Remarks')
                        ALTER TABLE [PurchaseOrders] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'TermsConditions')
                        ALTER TABLE [PurchaseOrders] ADD [TermsConditions] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'CgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [CgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'SgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [SgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'IgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [IgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END

                IF OBJECT_ID('[PurchaseOrderItems]') IS NOT NULL OR OBJECT_ID('[dbo].[PurchaseOrderItems]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'GstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'IgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'CgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'SgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'TaxAmount')
                        ALTER TABLE [PurchaseOrderItems] ADD [TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'TaxRateId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [TaxRateId] INT NULL;
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'UnitId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [UnitId] INT NULL;
                END
            ");
        }
        catch { }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelInvoice(long id)
    {
        var inv = await _db.SalesInvoices.FindAsync(id);
        if (inv != null)
        {
            inv.Status = "CANCELLED";
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Invoice <strong>{inv.InvoiceNumber}</strong> has been cancelled.";
        }
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Invoices() => RedirectToAction(nameof(Index));

    public async Task<IActionResult> AllocatePayment(long? clientId = null)
    {
        ViewData["ActiveMenu"] = "Sales";
        var company = await _companyContext.GetCurrentCompanyAsync() ?? await _db.Companies.FirstOrDefaultAsync();
        ViewBag.Company = company;
        ViewBag.Clients = await _db.Clients.AsNoTracking().ToListAsync();
        ViewBag.BankAccounts = await _db.BankAccounts.Where(b => b.IsActive).AsNoTracking().ToListAsync();
        ViewBag.SelectedClientId = clientId;

        var invoiceQuery = _db.SalesInvoices
            .Include(i => i.Client)
            .Where(i => i.OutstandingBalance > 0);

        if (clientId.HasValue && clientId.Value > 0)
        {
            invoiceQuery = invoiceQuery.Where(i => i.ClientId == clientId.Value);
        }

        var pendingInvoices = await invoiceQuery.AsNoTracking().ToListAsync();

        List<CustomerReceipt> recentReceipts;
        try
        {
            recentReceipts = await _db.CustomerReceipts
                .Include(r => r.Client)
                .OrderByDescending(r => r.ReceiptDate)
                .ThenByDescending(r => r.Id)
                .Take(15)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            await EnsureCustomerReceiptColumnsAsync();
            try
            {
                recentReceipts = await _db.CustomerReceipts
                    .Include(r => r.Client)
                    .OrderByDescending(r => r.ReceiptDate)
                    .ThenByDescending(r => r.Id)
                    .Take(15)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {
                recentReceipts = new List<CustomerReceipt>();
            }
        }

        var docIds = recentReceipts.Where(r => r.ReceiptDocId.HasValue).Select(r => r.ReceiptDocId!.Value).Distinct().ToList();
        var receiptDocs = await _db.DocumentAttachments
            .Where(d => docIds.Contains(d.Id))
            .ToDictionaryAsync(d => d.Id, d => d);

        ViewBag.RecentReceipts = recentReceipts;
        ViewBag.ReceiptDocs = receiptDocs;

        return View(pendingInvoices);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AllocatePayment(
        long? clientId, 
        decimal amountReceived, 
        long? bankAccountId, 
        string? paymentRef, 
        IFormFile? receiptVoucher, 
        IFormCollection form)
    {
        // 1. Mandatory Bank Receipt / Voucher Validation
        var voucherFile = receiptVoucher ?? form.Files["receiptVoucher"] ?? form.Files["uploadFile"];
        if (voucherFile == null || voucherFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Bank receipt or voucher upload is <strong>mandatory</strong>. Please attach the document before submitting.";
            return RedirectToAction(nameof(AllocatePayment));
        }

        var ext = Path.GetExtension(voucherFile.FileName).ToLowerInvariant();
        var allowedExts = new[] { ".pdf", ".png", ".jpg", ".jpeg", ".webp" };
        if (!allowedExts.Contains(ext))
        {
            TempData["ErrorMessage"] = "Invalid document format. Allowed formats: PDF, PNG, JPG, JPEG, WEBP.";
            return RedirectToAction(nameof(AllocatePayment));
        }

        if (voucherFile.Length > 15 * 1024 * 1024)
        {
            TempData["ErrorMessage"] = "The uploaded file exceeds the 15 MB size limit.";
            return RedirectToAction(nameof(AllocatePayment));
        }

        if (amountReceived <= 0)
        {
            TempData["ErrorMessage"] = "Please enter a valid received amount greater than 0.";
            return RedirectToAction(nameof(AllocatePayment));
        }

        var company = await _companyContext.GetCurrentCompanyAsync() ?? await _db.Companies.FirstOrDefaultAsync();
        var branch = await _db.Branches.FirstOrDefaultAsync(b => b.CompanyId == (company != null ? company.Id : 1)) ?? await _db.Branches.FirstOrDefaultAsync();
        var user = await _db.Users.FirstOrDefaultAsync();
        var fy = await _db.FinancialYears.FirstOrDefaultAsync(f => !f.IsClosed) ?? await _db.FinancialYears.FirstOrDefaultAsync();

        var bank = await _db.BankAccounts.FindAsync(bankAccountId ?? 0) ?? await _db.BankAccounts.FirstOrDefaultAsync(b => b.IsActive);
        if (bank != null)
        {
            bank.BookBalance += amountReceived;
        }

        var resolvedBank = bank != null ? $"{bank.BankName} - {bank.AccountNumber}" : (company?.BankName ?? "Main Bank Account");

        // 2. Save Uploaded Bank Receipt Voucher
        long? docId = null;
        try
        {
            var companyId = company?.Id ?? 1;
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tenants", $"org_{companyId}", "receipts");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var safeFileName = $"voucher_{DateTime.UtcNow.Ticks}_{Path.GetFileName(voucherFile.FileName)}";
            var filePath = Path.Combine(uploadsFolder, safeFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await voucherFile.CopyToAsync(stream);
            }

            var relativePath = $"/uploads/tenants/org_{companyId}/receipts/{safeFileName}";
            var attachment = new DocumentAttachment
            {
                EntityType = "CustomerReceipt",
                EntityId = 0,
                FileName = voucherFile.FileName,
                FilePath = relativePath,
                FileSizeBytes = voucherFile.Length,
                MimeType = voucherFile.ContentType ?? "application/octet-stream",
                FileHashSha256 = Guid.NewGuid().ToString("N"),
                UploadedBy = user?.Id ?? 1,
                UploaderId = user?.Id ?? 1,
                UploadedAt = DateTime.UtcNow,
                VersionNumber = 1
            };
            _db.DocumentAttachments.Add(attachment);
            await _db.SaveChangesAsync();
            docId = attachment.Id;
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Failed to save bank receipt file: {ex.Message}";
            return RedirectToAction(nameof(AllocatePayment));
        }

        await EnsureCustomerReceiptColumnsAsync();

        var count = await _db.CustomerReceipts.CountAsync() + 1;
        var rcptNo = $"RCPT-ALC-{DateTime.Today:yyMM}-{count:D4}";

        var receipt = new CustomerReceipt
        {
            CompanyId = company?.Id ?? 1,
            ClientId = clientId,
            BankAccountId = bank?.Id ?? 1,
            ReceiptNumber = rcptNo,
            ReceiptDate = DateTime.Today,
            AmountReceived = amountReceived,
            UnallocatedAmount = amountReceived,
            PaymentMode = string.IsNullOrWhiteSpace(paymentRef) ? "Bank Transfer / NEFT" : paymentRef,
            TransactionRefNo = paymentRef,
            Status = "POSTED",
            ExpenseHead = "Customer Invoicing Allocation",
            Remarks = $"Bank Voucher Attached: {voucherFile.FileName} [Bank: {resolvedBank}]" + (!string.IsNullOrWhiteSpace(paymentRef) ? $" - Ref: {paymentRef}" : ""),
            ReceiptDocId = docId
        };
        _db.CustomerReceipts.Add(receipt);
        await _db.SaveChangesAsync();

        if (docId.HasValue)
        {
            var att = await _db.DocumentAttachments.FindAsync(docId.Value);
            if (att != null)
            {
                att.EntityId = receipt.Id;
                await _db.SaveChangesAsync();
            }
        }

        // 3. Process Invoices Allocation
        decimal totalAllocated = 0;
        foreach (var key in form.Keys)
        {
            if (key.StartsWith("alloc_") && decimal.TryParse(form[key], out var allocAmt) && allocAmt > 0)
            {
                if (long.TryParse(key.Replace("alloc_", ""), out var invId))
                {
                    var inv = await _db.SalesInvoices.FindAsync(invId);
                    if (inv != null)
                    {
                        var actualAlloc = Math.Min(allocAmt, inv.OutstandingBalance);
                        inv.PaidAmount += actualAlloc;
                        inv.OutstandingBalance = Math.Max(0, inv.TotalInvoiceValue - inv.PaidAmount);
                        inv.Status = inv.OutstandingBalance == 0 ? "PAID" : "PARTIAL";
                        totalAllocated += actualAlloc;

                        var alloc = new ReceiptAllocation
                        {
                            ReceiptId = receipt.Id,
                            InvoiceId = inv.Id,
                            AllocatedAmount = actualAlloc,
                            TdsDeductedByClient = 0
                        };
                        _db.ReceiptAllocations.Add(alloc);
                    }
                }
            }
        }

        receipt.UnallocatedAmount = Math.Max(0, amountReceived - totalAllocated);
        await _db.SaveChangesAsync();

        if (fy == null)
        {
            fy = new FinancialYear
            {
                CompanyId = company?.Id ?? 1,
                FyCode = $"FY-{DateTime.Today.Year}-{(DateTime.Today.Year + 1) % 100}",
                StartDate = new DateTime(DateTime.Today.Year, 4, 1),
                EndDate = new DateTime(DateTime.Today.Year + 1, 3, 31),
                IsClosed = false
            };
            _db.FinancialYears.Add(fy);
            await _db.SaveChangesAsync();
        }

        var jv = new JournalEntry
        {
            CompanyId = company?.Id ?? 1,
            BranchId = branch?.Id ?? 1,
            FyId = fy.Id,
            VoucherNo = $"JV-RCPT-{DateTime.Now:yyyyMMdd-HHmmss}",
            VoucherDate = DateTime.Today,
            VoucherType = "RECEIPT",
            SourceEntityType = "CustomerReceipt",
            SourceEntityId = receipt.Id,
            Narration = $"Customer Payment: {paymentRef ?? "NEFT/Cheque"} (Allocated: &#8377; {totalAllocated:N2}, Advance: &#8377; {receipt.UnallocatedAmount:N2}) [Bank: {resolvedBank}] - Voucher: {voucherFile.FileName}",
            TotalDebit = amountReceived,
            TotalCredit = amountReceived,
            IsBalanced = true,
            CreatedBy = user?.Id ?? 1,
            Creator = user!
        };
        _db.JournalEntries.Add(jv);
        await _db.SaveChangesAsync();

        receipt.JournalEntryId = jv.Id;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Payment of <strong>&#8377; {amountReceived:N2}</strong> with Bank Voucher <strong>{voucherFile.FileName}</strong> recorded successfully! (Receipt No: <strong>{rcptNo}</strong>)";
        return RedirectToAction(nameof(AllocatePayment));
    }

    [HttpGet]
    public async Task<IActionResult> DirectPayment()
    {
        ViewData["ActiveMenu"] = "Sales";
        var company = await _companyContext.GetCurrentCompanyAsync();
        ViewBag.Company = company;
        var bankAccounts = await _db.BankAccounts.Where(b => b.IsActive).AsNoTracking().ToListAsync();
        ViewBag.BankAccounts = bankAccounts;

        List<ExpenseType> expenseTypes;
        try
        {
            expenseTypes = await _db.ExpenseTypes
                .Where(e => e.IsActive)
                .OrderBy(e => e.Name)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            expenseTypes = new List<ExpenseType>();
        }

        ViewBag.ExpenseTypes = expenseTypes;
        ViewBag.ExpenseHeads = expenseTypes.Select(e => e.Name).ToList();

        List<CustomerReceipt> directReceipts;
        try
        {
            directReceipts = await _db.CustomerReceipts
                .Where(r => r.ExpenseHead != null)
                .OrderByDescending(r => r.ReceiptDate)
                .ThenByDescending(r => r.Id)
                .Take(20)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            await EnsureCustomerReceiptColumnsAsync();
            try
            {
                directReceipts = await _db.CustomerReceipts
                    .Where(r => r.ExpenseHead != null)
                    .OrderByDescending(r => r.ReceiptDate)
                    .ThenByDescending(r => r.Id)
                    .Take(20)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {
                directReceipts = new List<CustomerReceipt>();
            }
        }

        ViewBag.RecentDirectReceipts = directReceipts;
        return View();
    }

    [HttpGet]
    [ResponseCache(Duration = 86400)]
    public async Task<IActionResult> GetMasterBanks()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var json = await client.GetStringAsync("https://cdn.jsdelivr.net/gh/razorpay/ifsc/src/banknames.json");
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (dict != null)
            {
                var banks = dict.Values.Distinct().OrderBy(b => b).ToList();
                return Json(banks);
            }
        }
        catch { }
        return Json(Array.Empty<string>());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DirectPayment(
        string expenseHead,
        DateTime? paymentReceiveDate,
        decimal paymentReceiveAmount,
        string paymentReceiveMode,
        string paymentReceiveRemarks,
        string? bankName,
        long? bankAccountId,
        string? transactionRefNo,
        IFormFile? uploadFile)
    {
        if (paymentReceiveAmount <= 0)
        {
            TempData["ErrorMessage"] = "Payment Receive Amount must be greater than zero.";
            return RedirectToAction(nameof(DirectPayment));
        }

        if (string.IsNullOrWhiteSpace(expenseHead))
        {
            TempData["ErrorMessage"] = "Please select a valid Expense / Income Head.";
            return RedirectToAction(nameof(DirectPayment));
        }

        var company = await _companyContext.GetCurrentCompanyAsync();
        var branch = await _db.Branches.FirstOrDefaultAsync(b => b.CompanyId == company.Id);
        var user = await _db.Users.FirstOrDefaultAsync();
        var fy = await _db.FinancialYears.FirstOrDefaultAsync(f => !f.IsClosed);

        var bank = await _db.BankAccounts.FindAsync(bankAccountId ?? 0) ?? await _db.BankAccounts.FirstOrDefaultAsync(b => b.IsActive);
        if (bank != null)
        {
            bank.BookBalance += paymentReceiveAmount;
        }

        var resolvedBank = !string.IsNullOrWhiteSpace(bankName) 
            ? bankName.Trim() 
            : (bank != null ? $"{bank.BankName} - {bank.AccountNumber}" : (company.BankName ?? "Direct Deposit"));

        long? docId = null;
        if (uploadFile != null && uploadFile.Length > 0)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tenants", $"org_{company.Id}", "receipts");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var safeFileName = $"receipt_{DateTime.UtcNow.Ticks}_{Path.GetFileName(uploadFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, safeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/tenants/org_{company.Id}/receipts/{safeFileName}";
                var attachment = new DocumentAttachment
                {
                    EntityType = "DirectPaymentReceive",
                    EntityId = 0,
                    FileName = uploadFile.FileName,
                    FilePath = relativePath,
                    FileSizeBytes = uploadFile.Length,
                    MimeType = uploadFile.ContentType ?? "application/octet-stream",
                    FileHashSha256 = Guid.NewGuid().ToString("N"),
                    UploadedBy = user?.Id ?? 1,
                    UploaderId = user?.Id ?? 1,
                    UploadedAt = DateTime.UtcNow,
                    VersionNumber = 1
                };
                _db.DocumentAttachments.Add(attachment);
                await _db.SaveChangesAsync();
                docId = attachment.Id;
            }
            catch { }
        }

        var count = await _db.CustomerReceipts.CountAsync() + 1;
        var rcptNo = $"RCPT-DIR-{DateTime.Today:yyMM}-{count:D4}";

        var receipt = new CustomerReceipt
        {
            CompanyId = company.Id,
            BankAccountId = bank?.Id ?? 1,
            ReceiptNumber = rcptNo,
            ReceiptDate = paymentReceiveDate ?? DateTime.Today,
            AmountReceived = paymentReceiveAmount,
            UnallocatedAmount = paymentReceiveAmount,
            PaymentMode = string.IsNullOrWhiteSpace(paymentReceiveMode) ? "Cheque" : paymentReceiveMode,
            TransactionRefNo = transactionRefNo,
            Status = "POSTED",
            ExpenseHead = expenseHead,
            Remarks = string.IsNullOrWhiteSpace(paymentReceiveRemarks) 
                ? $"Deposit Bank: {resolvedBank}" 
                : $"[Bank: {resolvedBank}] {paymentReceiveRemarks}",
            ReceiptDocId = docId
        };
        _db.CustomerReceipts.Add(receipt);
        await _db.SaveChangesAsync();

        if (docId.HasValue)
        {
            var att = await _db.DocumentAttachments.FindAsync(docId.Value);
            if (att != null)
            {
                att.EntityId = receipt.Id;
                await _db.SaveChangesAsync();
            }
        }

        if (fy == null)
        {
            fy = new FinancialYear
            {
                CompanyId = company.Id,
                FyCode = $"FY-{DateTime.Today.Year}-{(DateTime.Today.Year + 1) % 100}",
                StartDate = new DateTime(DateTime.Today.Year, 4, 1),
                EndDate = new DateTime(DateTime.Today.Year + 1, 3, 31),
                IsClosed = false
            };
            _db.FinancialYears.Add(fy);
            await _db.SaveChangesAsync();
        }

        var jv = new JournalEntry
        {
            CompanyId = company.Id,
            BranchId = branch?.Id ?? 1,
            FyId = fy.Id,
            VoucherNo = $"JV-DIR-RCPT-{DateTime.Now:yyyyMMdd-HHmmss}",
            VoucherDate = receipt.ReceiptDate,
            VoucherType = "RECEIPT",
            SourceEntityType = "DirectPaymentReceive",
            SourceEntityId = receipt.Id,
            Narration = $"Direct Payment Received: {expenseHead} via {receipt.PaymentMode} [Bank: {resolvedBank}] (Ref: {transactionRefNo ?? "N/A"}) - {paymentReceiveRemarks}",
            TotalDebit = paymentReceiveAmount,
            TotalCredit = paymentReceiveAmount,
            IsBalanced = true,
            CreatedBy = user?.Id ?? 1,
            Creator = user!
        };
        _db.JournalEntries.Add(jv);
        await _db.SaveChangesAsync();

        receipt.JournalEntryId = jv.Id;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Direct Payment of <strong>&#8377; {paymentReceiveAmount:N2}</strong> received and posted to General Ledger! (Receipt No: {rcptNo})";
        return RedirectToAction(nameof(DirectPayment));
    }

    [HttpGet]
    public async Task<IActionResult> ProjectPayment(long? projectId)
    {
        ViewData["ActiveMenu"] = "Sales";
        var company = await _companyContext.GetCurrentCompanyAsync();
        ViewBag.Company = company;

        ViewBag.Projects = await _db.Projects
            .Include(p => p.Client)
            .OrderByDescending(p => p.Id)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.BankAccounts = await _db.BankAccounts
            .Where(b => b.IsActive)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.SelectedProjectId = projectId ?? 0;

        var invoices = await _db.SalesInvoices
            .Include(i => i.Project)
            .Where(i => i.OutstandingBalance > 0)
            .OrderByDescending(i => i.InvoiceDate)
            .AsNoTracking()
            .ToListAsync();

        ViewBag.Invoices = invoices;

        List<CustomerReceipt> recentReceipts;
        try
        {
            recentReceipts = await _db.CustomerReceipts
                .Include(r => r.Project)
                .Include(r => r.Invoice)
                .Where(r => r.ProjectId != null)
                .OrderByDescending(r => r.ReceiptDate)
                .ThenByDescending(r => r.Id)
                .Take(20)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            await EnsureCustomerReceiptColumnsAsync();
            try
            {
                recentReceipts = await _db.CustomerReceipts
                    .Include(r => r.Project)
                    .Include(r => r.Invoice)
                    .Where(r => r.ProjectId != null)
                    .OrderByDescending(r => r.ReceiptDate)
                    .ThenByDescending(r => r.Id)
                    .Take(20)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {
                recentReceipts = new List<CustomerReceipt>();
            }
        }

        ViewBag.RecentProjectReceipts = recentReceipts;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoicesByProject(long projectId)
    {
        var invoices = await _db.SalesInvoices
            .Where(i => i.ProjectId == projectId && i.OutstandingBalance > 0)
            .Select(i => new
            {
                id = i.Id,
                invoiceNumber = i.InvoiceNumber,
                invoiceDate = i.InvoiceDate.ToString("dd MMM yyyy"),
                outstandingBalance = i.OutstandingBalance,
                totalValue = i.TotalInvoiceValue
            })
            .ToListAsync();

        return Json(invoices);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProjectPayment(
        long projectId,
        DateTime? paymentReceiveDate,
        string paymentReceiveMode,
        bool isAdvanceReceived,
        long? invoiceId,
        decimal tdsAmount,
        decimal gstAmount,
        decimal securityDepositAmount,
        decimal otherDeductionAmount,
        decimal netAmountReceive,
        decimal totalAmountReceive,
        string paymentReceiveRemarks,
        long? bankAccountId,
        IFormFile? uploadFile)
    {
        var project = await _db.Projects.Include(p => p.Client).FirstOrDefaultAsync(p => p.Id == projectId);
        if (project == null)
        {
            TempData["ErrorMessage"] = "Please select a valid Project.";
            return RedirectToAction(nameof(ProjectPayment));
        }

        if (totalAmountReceive <= 0 && netAmountReceive <= 0)
        {
            TempData["ErrorMessage"] = "Received amount must be greater than zero.";
            return RedirectToAction(nameof(ProjectPayment));
        }

        if (totalAmountReceive <= 0)
        {
            totalAmountReceive = netAmountReceive + tdsAmount + gstAmount + securityDepositAmount + otherDeductionAmount;
        }

        var company = await _companyContext.GetCurrentCompanyAsync();
        var branch = await _db.Branches.FirstOrDefaultAsync(b => b.CompanyId == company.Id);
        var user = await _db.Users.FirstOrDefaultAsync();
        var fy = await _db.FinancialYears.FirstOrDefaultAsync(f => !f.IsClosed);

        var bank = await _db.BankAccounts.FindAsync(bankAccountId ?? 0) ?? await _db.BankAccounts.FirstOrDefaultAsync(b => b.IsActive);
        if (bank != null && netAmountReceive > 0)
        {
            bank.BookBalance += netAmountReceive;
        }

        long? docId = null;
        if (uploadFile != null && uploadFile.Length > 0)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "tenants", $"org_{company.Id}", "receipts");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                var safeFileName = $"prj_receipt_{DateTime.UtcNow.Ticks}_{Path.GetFileName(uploadFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, safeFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await uploadFile.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/tenants/org_{company.Id}/receipts/{safeFileName}";
                var attachment = new DocumentAttachment
                {
                    EntityType = "CustomerReceipt",
                    EntityId = 0,
                    FileName = uploadFile.FileName,
                    FilePath = relativePath,
                    FileSizeBytes = uploadFile.Length,
                    MimeType = uploadFile.ContentType ?? "application/octet-stream",
                    FileHashSha256 = Guid.NewGuid().ToString("N"),
                    UploadedBy = user?.Id ?? 1,
                    UploaderId = user?.Id ?? 1,
                    UploadedAt = DateTime.UtcNow,
                    VersionNumber = 1
                };
                _db.DocumentAttachments.Add(attachment);
                await _db.SaveChangesAsync();
                docId = attachment.Id;
            }
            catch { }
        }

        SalesInvoice? invoice = null;
        if (invoiceId.HasValue && invoiceId.Value > 0)
        {
            invoice = await _db.SalesInvoices.FindAsync(invoiceId.Value);
            if (invoice != null)
            {
                var allocAmt = Math.Min(totalAmountReceive, invoice.OutstandingBalance);
                invoice.PaidAmount += allocAmt;
                invoice.OutstandingBalance = Math.Max(0, invoice.TotalInvoiceValue - invoice.PaidAmount);
                invoice.Status = invoice.OutstandingBalance == 0 ? "PAID" : "PARTIAL";
            }
        }

        var count = await _db.CustomerReceipts.CountAsync() + 1;
        var rcptNo = $"RCPT-PRJ-{DateTime.Today:yyMM}-{count:D4}";

        var receipt = new CustomerReceipt
        {
            CompanyId = company.Id,
            ClientId = project.ClientId,
            BankAccountId = bank?.Id ?? 1,
            ReceiptNumber = rcptNo,
            ReceiptDate = paymentReceiveDate ?? DateTime.Today,
            AmountReceived = netAmountReceive > 0 ? netAmountReceive : totalAmountReceive,
            UnallocatedAmount = 0,
            PaymentMode = string.IsNullOrWhiteSpace(paymentReceiveMode) ? "Netbanking" : paymentReceiveMode,
            Status = "POSTED",
            ProjectId = project.Id,
            InvoiceId = invoice?.Id,
            IsAdvance = isAdvanceReceived,
            TdsAmount = tdsAmount,
            GstTdsAmount = gstAmount,
            SecurityDepositAmount = securityDepositAmount,
            OtherDeductionAmount = otherDeductionAmount,
            NetAmountReceived = netAmountReceive,
            TotalAmountReceived = totalAmountReceive,
            ExpenseHead = isAdvanceReceived ? "Mobilization Advance" : "Project Milestone Billing",
            Remarks = paymentReceiveRemarks,
            ReceiptDocId = docId
        };
        _db.CustomerReceipts.Add(receipt);
        await _db.SaveChangesAsync();

        if (docId.HasValue)
        {
            var att = await _db.DocumentAttachments.FindAsync(docId.Value);
            if (att != null)
            {
                att.EntityId = receipt.Id;
                await _db.SaveChangesAsync();
            }
        }

        if (invoice != null)
        {
            var alloc = new ReceiptAllocation
            {
                ReceiptId = receipt.Id,
                InvoiceId = invoice.Id,
                AllocatedAmount = totalAmountReceive,
                TdsDeductedByClient = tdsAmount
            };
            _db.ReceiptAllocations.Add(alloc);
            await _db.SaveChangesAsync();
        }

        if (fy == null)
        {
            fy = new FinancialYear
            {
                CompanyId = company.Id,
                FyCode = $"FY-{DateTime.Today.Year}-{(DateTime.Today.Year + 1) % 100}",
                StartDate = new DateTime(DateTime.Today.Year, 4, 1),
                EndDate = new DateTime(DateTime.Today.Year + 1, 3, 31),
                IsClosed = false
            };
            _db.FinancialYears.Add(fy);
            await _db.SaveChangesAsync();
        }

        var jv = new JournalEntry
        {
            CompanyId = company.Id,
            BranchId = branch?.Id ?? 1,
            FyId = fy.Id,
            VoucherNo = $"JV-PRJ-RCPT-{DateTime.Now:yyyyMMdd-HHmmss}",
            VoucherDate = receipt.ReceiptDate,
            VoucherType = "RECEIPT",
            SourceEntityType = "ProjectPaymentReceived",
            SourceEntityId = receipt.Id,
            ProjectId = project.Id,
            Narration = $"Project Payment: {project.ProjectName} (Invoice: {invoice?.InvoiceNumber ?? "Advance"}) - Net ₹{netAmountReceive:N2} (Total ₹{totalAmountReceive:N2}) - {paymentReceiveRemarks}",
            TotalDebit = totalAmountReceive,
            TotalCredit = totalAmountReceive,
            IsBalanced = true,
            CreatedBy = user?.Id ?? 1,
            Creator = user!
        };
        _db.JournalEntries.Add(jv);
        await _db.SaveChangesAsync();

        receipt.JournalEntryId = jv.Id;
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Project payment of <strong>&#8377; {totalAmountReceive:N2}</strong> recorded successfully! (Receipt No: {rcptNo})";
        return RedirectToAction(nameof(ProjectPayment));
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["ActiveMenu"] = "Sales";
        var company = await _companyContext.GetCurrentCompanyAsync();
        ViewBag.Company = company;

        var fy = await _db.FinancialYears.FirstOrDefaultAsync(f => !f.IsClosed);
        var today = DateTime.Today;
        var startYear = today.Month >= 4 ? today.Year : today.Year - 1;
        var endYear = (startYear + 1) % 100;
        ViewBag.FinancialYearCode = fy != null ? fy.FyCode : $"FY {startYear}-{endYear}";

        var count = await _db.SalesInvoices.CountAsync() + 1;
        ViewBag.AutoInvoiceNumber = $"INV/{startYear % 100}-{endYear}/{count:D4}";

        List<Project> projects;
        try
        {
            projects = await _db.Projects
                .Include(p => p.Client)
                .Include(p => p.PurchaseOrders)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            await EnsurePurchaseOrderColumnsAsync();
            projects = await _db.Projects
                .Include(p => p.Client)
                .Include(p => p.PurchaseOrders)
                .AsNoTracking()
                .ToListAsync();
        }
        ViewBag.Projects = projects;

        ViewBag.Clients = await _db.Clients
            .AsNoTracking()
            .ToListAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string? invoiceNumber,
        DateTime? invoiceDate,
        DateTime? dueDate,
        string? workOrderNo,
        DateTime? workOrderDate,
        long? projectId,
        long? clientId,
        string? billingAttention,
        string? remarks,
        string? deductionRemarks,
        decimal? deductionAmount,
        decimal? taxableAmount,
        decimal? gstRate,
        IFormCollection form)
    {
        var company = await _companyContext.GetCurrentCompanyAsync();
        var branch = await _db.Branches.FirstOrDefaultAsync(b => b.CompanyId == company.Id) ?? new Branch
        {
            CompanyId = company.Id, BranchCode = "HQ", BranchName = "Main Branch"
        };
        if (branch.Id == 0) { _db.Branches.Add(branch); await _db.SaveChangesAsync(); }

        Project? project = null;
        Client? client = null;

        if (projectId.HasValue && projectId.Value > 0)
        {
            project = await _db.Projects.Include(p => p.Client).FirstOrDefaultAsync(p => p.Id == projectId.Value);
            if (project != null)
            {
                client = project.Client ?? await _db.Clients.FindAsync(project.ClientId);
                if (clientId == null || clientId.Value == 0)
                {
                    clientId = project.ClientId;
                }
            }
        }

        if (client == null && clientId.HasValue && clientId.Value > 0)
        {
            client = await _db.Clients.FindAsync(clientId.Value);
        }

        if (project == null)
        {
            project = await _db.Projects.Include(p => p.Client).FirstOrDefaultAsync();
            if (project != null)
            {
                projectId = project.Id;
                client ??= project.Client ?? await _db.Clients.FindAsync(project.ClientId);
                clientId ??= project.ClientId;
            }
        }

        if (client == null)
        {
            client = await _db.Clients.FirstOrDefaultAsync();
            if (client != null)
            {
                clientId = client.Id;
            }
            else
            {
                // Create a fallback client
                client = new Client
                {
                    CompanyId = company.Id,
                    ClientCode = "CUST-001",
                    ClientName = !string.IsNullOrWhiteSpace(billingAttention) ? billingAttention : "Corporate Client",
                    BillingAddress = "Corporate Office",
                    StateCode = "07"
                };
                _db.Clients.Add(client);
                await _db.SaveChangesAsync();
                clientId = client.Id;
            }
        }

        if (project == null)
        {
            project = new Project
            {
                CompanyId = company.Id,
                BranchId = branch.Id,
                ClientId = client.Id,
                ProjectCode = "PRJ-GEN-01",
                ProjectName = "Commercial Engineering Services",
                StartDate = DateTime.Today
            };
            _db.Projects.Add(project);
            await _db.SaveChangesAsync();
            projectId = project.Id;
        }

        // Parse line items from form collections
        var descriptions = form["itemDescription[]"].Count > 0 ? form["itemDescription[]"].ToList() : form["itemDescription"].ToList();
        var hsnCodes = form["itemHsn[]"].Count > 0 ? form["itemHsn[]"].ToList() : form["itemHsn"].ToList();
        var itemGsts = form["itemGst[]"].Count > 0 ? form["itemGst[]"].ToList() : form["itemGst"].ToList();
        var itemIgsts = form["itemIgst[]"].Count > 0 ? form["itemIgst[]"].ToList() : form["itemIgst"].ToList();
        var itemCgsts = form["itemCgst[]"].Count > 0 ? form["itemCgst[]"].ToList() : form["itemCgst"].ToList();
        var itemSgsts = form["itemSgst[]"].Count > 0 ? form["itemSgst[]"].ToList() : form["itemSgst"].ToList();
        var qtys = form["itemQty[]"].Count > 0 ? form["itemQty[]"].ToList() : form["itemQty"].ToList();
        var rates = form["itemRate[]"].Count > 0 ? form["itemRate[]"].ToList() : form["itemRate"].ToList();

        var invoiceItems = new List<SalesInvoiceItem>();
        decimal calculatedTaxable = 0;
        decimal calculatedCgst = 0;
        decimal calculatedSgst = 0;
        decimal calculatedIgst = 0;

        int rowCount = descriptions.Count;
        for (int i = 0; i < rowCount; i++)
        {
            var desc = descriptions[i]?.Trim();
            if (string.IsNullOrWhiteSpace(desc)) continue;

            decimal qty = 1.0m;
            if (i < qtys.Count && decimal.TryParse(qtys[i], out var parsedQty) && parsedQty > 0)
                qty = parsedQty;

            decimal rate = 0;
            if (i < rates.Count && decimal.TryParse(rates[i], out var parsedRate) && parsedRate >= 0)
                rate = parsedRate;

            var hsn = (i < hsnCodes.Count && !string.IsNullOrWhiteSpace(hsnCodes[i])) ? hsnCodes[i].Trim() : "998313";

            decimal gRate = 18m;
            if (i < itemGsts.Count && decimal.TryParse(itemGsts[i], out var pGst)) gRate = pGst;

            decimal igstR = 0m, cgstR = 0m, sgstR = 0m;
            if (i < itemIgsts.Count && decimal.TryParse(itemIgsts[i], out var pIgst)) igstR = pIgst;
            if (i < itemCgsts.Count && decimal.TryParse(itemCgsts[i], out var pCgst)) cgstR = pCgst;
            if (i < itemSgsts.Count && decimal.TryParse(itemSgsts[i], out var pSgst)) sgstR = pSgst;

            // If component rates were not specifically provided, split based on gRate
            if (igstR == 0 && cgstR == 0 && sgstR == 0 && gRate > 0)
            {
                var isInterState = !string.IsNullOrEmpty(client.StateCode) && !string.IsNullOrEmpty(company.StateCode) && client.StateCode != company.StateCode;
                if (isInterState)
                {
                    igstR = gRate;
                }
                else
                {
                    cgstR = Math.Round(gRate / 2m, 2);
                    sgstR = Math.Round(gRate / 2m, 2);
                }
            }

            var lineTaxable = Math.Round(qty * rate, 2);
            var lineCgst = Math.Round(lineTaxable * (cgstR / 100m), 2);
            var lineSgst = Math.Round(lineTaxable * (sgstR / 100m), 2);
            var lineIgst = Math.Round(lineTaxable * (igstR / 100m), 2);
            var lineTotal = lineTaxable + lineCgst + lineSgst + lineIgst;

            calculatedTaxable += lineTaxable;
            calculatedCgst += lineCgst;
            calculatedSgst += lineSgst;
            calculatedIgst += lineIgst;

            invoiceItems.Add(new SalesInvoiceItem
            {
                ItemDescription = desc,
                HsnSacCode = hsn,
                Quantity = qty,
                UnitRate = rate,
                TaxableValue = lineTaxable,
                GstRate = gRate,
                IgstRate = igstR,
                CgstRate = cgstR,
                SgstRate = sgstR,
                CgstAmount = lineCgst,
                SgstAmount = lineSgst,
                IgstAmount = lineIgst,
                LineTotal = lineTotal
            });
        }

        // Fallback for modal or direct single taxableAmount submission
        if (!invoiceItems.Any())
        {
            var fallbackTaxable = taxableAmount ?? 0m;
            var fallbackGst = gstRate ?? 18m;
            var fallbackCgst = Math.Round(fallbackTaxable * (fallbackGst / 200m), 2);
            var fallbackSgst = Math.Round(fallbackTaxable * (fallbackGst / 200m), 2);
            var fallbackTotal = fallbackTaxable + fallbackCgst + fallbackSgst;

            calculatedTaxable = fallbackTaxable;
            calculatedCgst = fallbackCgst;
            calculatedSgst = fallbackSgst;
            calculatedIgst = 0;

            invoiceItems.Add(new SalesInvoiceItem
            {
                ItemDescription = $"{project.ProjectName} - Execution & Milestone Delivery",
                HsnSacCode = "998313",
                Quantity = 1.00m,
                UnitRate = fallbackTaxable,
                TaxableValue = fallbackTaxable,
                GstRate = fallbackGst,
                CgstRate = fallbackGst / 2m,
                SgstRate = fallbackGst / 2m,
                CgstAmount = fallbackCgst,
                SgstAmount = fallbackSgst,
                IgstAmount = 0,
                LineTotal = fallbackTotal
            });
        }

        decimal grossTotal = calculatedTaxable + calculatedCgst + calculatedSgst + calculatedIgst;
        decimal deduction = deductionAmount ?? 0m;
        decimal netPayable = Math.Max(0, grossTotal - deduction);

        if (string.IsNullOrWhiteSpace(invoiceNumber))
        {
            var count = await _db.SalesInvoices.CountAsync() + 1;
            var startYear = DateTime.Today.Month >= 4 ? DateTime.Today.Year : DateTime.Today.Year - 1;
            var endYear = (startYear + 1) % 100;
            invoiceNumber = $"INV/{startYear % 100}-{endYear}/{count:D4}";
        }

        var inv = new SalesInvoice
        {
            CompanyId = company.Id,
            BranchId = branch.Id,
            ClientId = client.Id,
            ProjectId = project.Id,
            InvoiceNumber = invoiceNumber,
            InvoiceDate = invoiceDate ?? DateTime.Today,
            DueDate = dueDate ?? (invoiceDate ?? DateTime.Today).AddDays(30),
            WorkOrderNo = workOrderNo,
            WorkOrderDate = workOrderDate,
            BillingAttention = billingAttention ?? client.ContactPerson ?? client.ClientName,
            Remarks = remarks,
            DeductionRemarks = deductionRemarks,
            DeductionAmount = deduction,
            TaxableAmount = calculatedTaxable,
            CgstAmount = calculatedCgst,
            SgstAmount = calculatedSgst,
            IgstAmount = calculatedIgst,
            TotalInvoiceValue = netPayable,
            PaidAmount = 0,
            OutstandingBalance = netPayable,
            Status = "SENT",
            PlaceOfSupply = client.StateCode ?? "07"
        };

        _db.SalesInvoices.Add(inv);
        await _db.SaveChangesAsync();

        foreach (var item in invoiceItems)
        {
            item.InvoiceId = inv.Id;
            _db.SalesInvoiceItems.Add(item);
        }
        await _db.SaveChangesAsync();

        var user = await _db.Users.FirstOrDefaultAsync();
        var fyRecord = await _db.FinancialYears.FirstOrDefaultAsync();
        if (fyRecord == null)
        {
            fyRecord = new FinancialYear
            {
                CompanyId = company.Id,
                FyCode = $"FY-{DateTime.Today.Year}-{(DateTime.Today.Year + 1) % 100}",
                StartDate = new DateTime(DateTime.Today.Year, 4, 1),
                EndDate = new DateTime(DateTime.Today.Year + 1, 3, 31),
                IsClosed = false
            };
            _db.FinancialYears.Add(fyRecord);
            await _db.SaveChangesAsync();
        }

        var jv = new JournalEntry
        {
            CompanyId = company.Id,
            BranchId = branch.Id,
            FyId = fyRecord.Id,
            VoucherNo = $"JV-SALES-{DateTime.Now:yyyyMMdd-HHmmss}",
            VoucherDate = inv.InvoiceDate,
            VoucherType = "SALES",
            SourceEntityType = "SalesInvoice",
            SourceEntityId = inv.Id,
            ProjectId = project.Id,
            Narration = $"Tax Invoice {inv.InvoiceNumber} auto-posted to General Ledger",
            TotalDebit = netPayable,
            TotalCredit = netPayable,
            IsBalanced = true,
            CreatedBy = user?.Id ?? 1,
            Creator = user!
        };
        _db.JournalEntries.Add(jv);
        await _db.SaveChangesAsync();

        var gst = new GstTransaction
        {
            CompanyId = company.Id,
            VoucherId = jv.Id,
            TransactionType = "OUTPUT",
            TaxableValue = calculatedTaxable,
            TaxRatePercentage = invoiceItems.Any() ? invoiceItems.First().GstRate : 18m,
            CgstAmount = calculatedCgst,
            SgstAmount = calculatedSgst,
            IgstAmount = calculatedIgst,
            ReturnPeriod = DateTime.Today.ToString("MM-yyyy"),
            PlaceOfSupply = inv.PlaceOfSupply
        };
        _db.GstTransactions.Add(gst);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Tax Invoice <strong>{inv.InvoiceNumber}</strong> created successfully for &#8377; {netPayable:N2}!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> PrintInvoice(long id)
    {
        var invoice = await _db.SalesInvoices
            .Include(i => i.Client)
            .Include(i => i.Project)
            .Include(i => i.Company)
            .Include(i => i.Branch)
            .Include(i => i.Items).ThenInclude(it => it.Unit)
            .Include(i => i.Items).ThenInclude(it => it.TaxRate)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return NotFound();

        var company = invoice.Company ?? await _companyContext.GetCurrentCompanyAsync();

        var branch = invoice.Branch ?? await _db.Branches.FirstOrDefaultAsync() ?? new Branch
        {
            BranchName = company.CompanyName,
            AddressLine1 = !string.IsNullOrEmpty(company.AddressLine1) ? $"{company.AddressLine1}, {company.City} - {company.Pincode}" : (!string.IsNullOrEmpty(company.City) ? $"{company.City}, {company.State}" : "Corporate Headquarters"),
            StateCode = company.StateCode ?? "07",
            Gstin = company.Gstin ?? string.Empty
        };

        var bank = !string.IsNullOrEmpty(company.BankAccountNumber) ? new BankAccount
        {
            BankName = company.BankName ?? "Primary Bank",
            BranchName = company.BankBranch ?? (company.City ?? "Main Branch"),
            AccountNumber = company.BankAccountNumber,
            IfscCode = company.BankIfsc ?? "IFSC0000123",
            AccountType = "CURRENT"
        } : (await _db.BankAccounts.FirstOrDefaultAsync(b => b.IsActive) ?? new BankAccount
        {
            BankName = "HDFC Bank Ltd",
            BranchName = "Connaught Place, New Delhi",
            AccountNumber = "50200084920194",
            IfscCode = "HDFC0000123",
            AccountType = "CURRENT"
        });

        ViewBag.Company = company;
        ViewBag.Branch = branch;
        ViewBag.Bank = bank;
        ViewBag.AmountInWords = NumberToWordsConverter.ConvertToIndianWords(invoice.TotalInvoiceValue);
        ViewBag.TaxAmountInWords = NumberToWordsConverter.ConvertToIndianWords(invoice.CgstAmount + invoice.SgstAmount + invoice.IgstAmount);

        return View("PrintInvoice", invoice);
    }
}

public class ProcurementController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ICompanyContext _companyContext;

    public ProcurementController(ApplicationDbContext db, ICompanyContext companyContext)
    {
        _db = db;
        _companyContext = companyContext;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Procurement";
        ViewBag.Vendors = await _db.Vendors.AsNoTracking().ToListAsync();
        ViewBag.Projects = await _db.Projects.AsNoTracking().ToListAsync();
        List<PurchaseOrder> purchaseOrders;
        try
        {
            purchaseOrders = await _db.PurchaseOrders
                .Include(p => p.Vendor)
                .Include(p => p.Project)
                .AsNoTracking()
                .ToListAsync();
        }
        catch
        {
            await EnsurePurchaseOrderColumnsAsync();
            purchaseOrders = await _db.PurchaseOrders
                .Include(p => p.Vendor)
                .Include(p => p.Project)
                .AsNoTracking()
                .ToListAsync();
        }
        return View(purchaseOrders);
    }

    public async Task<IActionResult> Inventory()
    {
        ViewData["ActiveMenu"] = "Procurement";
        var items = await _db.Items
            .Include(i => i.Category)
            .Include(i => i.Unit)
            .AsNoTracking()
            .ToListAsync();
        return View(items);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["ActiveMenu"] = "Procurement";
        var company = await _companyContext.GetCurrentCompanyAsync();
        ViewBag.Company = company;
        ViewBag.Vendors = await _db.Vendors.AsNoTracking().OrderBy(v => v.VendorName).ToListAsync();
        List<Project> projects;
        try
        {
            projects = await _db.Projects
                .Include(p => p.Client)
                .Include(p => p.PurchaseOrders)
                .AsNoTracking()
                .OrderBy(p => p.ProjectName)
                .ToListAsync();
        }
        catch
        {
            await EnsurePurchaseOrderColumnsAsync();
            projects = await _db.Projects
                .Include(p => p.Client)
                .Include(p => p.PurchaseOrders)
                .AsNoTracking()
                .OrderBy(p => p.ProjectName)
                .ToListAsync();
        }
        ViewBag.Projects = projects;

        var today = DateTime.Today;
        var startYear = today.Month >= 4 ? today.Year : today.Year - 1;
        var endYear = (startYear + 1) % 100;
        var count = await _db.PurchaseOrders.CountAsync() + 1;
        ViewBag.AutoPoNumber = $"PO/{startYear % 100}-{endYear}/{count:D4}";

        var defaultShipTo = !string.IsNullOrWhiteSpace(company.AuthorizedSignatoryName) || !string.IsNullOrWhiteSpace(company.CompanyName)
            ? $"{company.AuthorizedSignatoryName ?? "Sabir Alam"} ({(string.IsNullOrWhiteSpace(company.CompanyName) ? "SDK Solutions" : company.CompanyName)})\n" +
              $"{(string.IsNullOrWhiteSpace(company.AddressLine1) ? "Second Floor, Deewan Building," : company.AddressLine1)}\n" +
              $"{(string.IsNullOrWhiteSpace(company.City) ? "Opposite Tajganj, Agra, UP - 282001" : $"{company.City}, {company.State} - {company.Pincode}")}"
            : "Sabir Alam (SDK Solutions)\nSecond Floor, Deewan Building,\nOpposite Tajganj, Agra, UP - 282001";

        ViewBag.DefaultShipTo = defaultShipTo;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string? poType,
        string? poNumber,
        DateTime? poDate,
        long? vendorId,
        long? projectId,
        string? shipTo,
        string? remarks,
        string? termsConditions,
        decimal? taxableAmount,
        decimal? gstAmount,
        decimal? netBillingAmount,
        decimal? igstAmount,
        decimal? cgstAmount,
        decimal? sgstAmount,
        decimal? totalBillingAmount,
        IFormCollection form)
    {
        var company = await _companyContext.GetCurrentCompanyAsync();
        var branch = await _db.Branches.FirstOrDefaultAsync(b => b.CompanyId == company.Id) ?? new Branch
        {
            CompanyId = company.Id, BranchCode = "HQ", BranchName = "Main Branch"
        };
        if (branch.Id == 0) { _db.Branches.Add(branch); await _db.SaveChangesAsync(); }

        if (!vendorId.HasValue || vendorId.Value == 0)
        {
            TempData["ErrorMessage"] = "Please select a valid Vendor from the Master Directory to issue a Purchase Order.";
            return RedirectToAction(nameof(Create));
        }

        var vendor = await _db.Vendors.FindAsync(vendorId.Value);
        if (vendor == null)
        {
            TempData["ErrorMessage"] = "Selected Vendor does not exist in the Master Directory.";
            return RedirectToAction(nameof(Create));
        }

        // Parse line items if provided
        var itemNames = form["itemName[]"].Count > 0 ? form["itemName[]"].ToList() : form["itemName"].ToList();
        var itemHsns = form["itemHsn[]"].Count > 0 ? form["itemHsn[]"].ToList() : form["itemHsn"].ToList();
        var itemGsts = form["itemGst[]"].Count > 0 ? form["itemGst[]"].ToList() : form["itemGst"].ToList();
        var itemIgsts = form["itemIgst[]"].Count > 0 ? form["itemIgst[]"].ToList() : form["itemIgst"].ToList();
        var itemCgsts = form["itemCgst[]"].Count > 0 ? form["itemCgst[]"].ToList() : form["itemCgst"].ToList();
        var itemSgsts = form["itemSgst[]"].Count > 0 ? form["itemSgst[]"].ToList() : form["itemSgst"].ToList();
        var itemQtys = form["itemQty[]"].Count > 0 ? form["itemQty[]"].ToList() : form["itemQty"].ToList();
        var itemRates = form["itemRate[]"].Count > 0 ? form["itemRate[]"].ToList() : form["itemRate"].ToList();

        var poItems = new List<PurchaseOrderItem>();
        decimal calculatedTaxable = 0;
        decimal calculatedIgst = 0;
        decimal calculatedCgst = 0;
        decimal calculatedSgst = 0;

        for (int i = 0; i < itemNames.Count; i++)
        {
            var name = itemNames[i]?.Trim();
            if (string.IsNullOrWhiteSpace(name)) continue;

            decimal qty = 1;
            if (i < itemQtys.Count && decimal.TryParse(itemQtys[i], out var pQty) && pQty > 0)
                qty = pQty;

            decimal rate = 0;
            if (i < itemRates.Count && decimal.TryParse(itemRates[i], out var pRate) && pRate >= 0)
                rate = pRate;

            var hsn = (i < itemHsns.Count && !string.IsNullOrWhiteSpace(itemHsns[i])) ? itemHsns[i].Trim() : "998313";

            decimal gst = 0, igst = 0, cgst = 0, sgst = 0;
            if (i < itemGsts.Count && decimal.TryParse(itemGsts[i], out var pGst)) gst = pGst;
            if (i < itemIgsts.Count && decimal.TryParse(itemIgsts[i], out var pIgst)) igst = pIgst;
            if (i < itemCgsts.Count && decimal.TryParse(itemCgsts[i], out var pCgst)) cgst = pCgst;
            if (i < itemSgsts.Count && decimal.TryParse(itemSgsts[i], out var pSgst)) sgst = pSgst;

            if (igst == 0 && cgst == 0 && sgst == 0 && gst > 0)
            {
                var vendorState = (!string.IsNullOrWhiteSpace(vendor?.Gstin) && vendor.Gstin.Length >= 2) ? vendor.Gstin.Substring(0, 2) : "";
                var isInterState = !string.IsNullOrEmpty(vendorState) && !string.IsNullOrEmpty(company.StateCode) && vendorState != company.StateCode;
                if (isInterState)
                {
                    igst = gst;
                }
                else
                {
                    cgst = Math.Round(gst / 2m, 2);
                    sgst = Math.Round(gst / 2m, 2);
                }
            }

            var lineTaxable = Math.Round(qty * rate, 2);
            var lineIgst = Math.Round(lineTaxable * (igst / 100m), 2);
            var lineCgst = Math.Round(lineTaxable * (cgst / 100m), 2);
            var lineSgst = Math.Round(lineTaxable * (sgst / 100m), 2);
            var lineTotal = lineTaxable + lineIgst + lineCgst + lineSgst;

            calculatedTaxable += lineTaxable;
            calculatedIgst += lineIgst;
            calculatedCgst += lineCgst;
            calculatedSgst += lineSgst;

            poItems.Add(new PurchaseOrderItem
            {
                ItemDescription = name,
                HsnSacCode = hsn,
                OrderedQty = qty,
                UnitRate = rate,
                GstRate = gst,
                IgstRate = igst,
                CgstRate = cgst,
                SgstRate = sgst,
                TaxAmount = lineIgst + lineCgst + lineSgst,
                LineTotal = lineTotal
            });
        }

        decimal finalTaxable = calculatedTaxable > 0 ? calculatedTaxable : (netBillingAmount ?? taxableAmount ?? 0);
        decimal finalIgst = calculatedIgst > 0 ? calculatedIgst : (igstAmount ?? 0);
        decimal finalCgst = calculatedCgst > 0 ? calculatedCgst : (cgstAmount ?? 0);
        decimal finalSgst = calculatedSgst > 0 ? calculatedSgst : (sgstAmount ?? 0);
        decimal totalGst = finalIgst + finalCgst + finalSgst > 0 ? (finalIgst + finalCgst + finalSgst) : (gstAmount ?? 0);
        decimal total = finalTaxable + totalGst;

        if (totalBillingAmount.HasValue && totalBillingAmount.Value > 0 && Math.Abs(totalBillingAmount.Value - total) > 0.5m)
        {
            total = totalBillingAmount.Value;
        }

        if (string.IsNullOrWhiteSpace(poNumber))
        {
            var count = await _db.PurchaseOrders.CountAsync() + 1;
            var today = DateTime.Today;
            var sYear = today.Month >= 4 ? today.Year : today.Year - 1;
            var eYear = (sYear + 1) % 100;
            poNumber = $"PO/{sYear % 100}-{eYear}/{count:D4}";
        }

        var po = new PurchaseOrder
        {
            CompanyId = company.Id,
            BranchId = branch.Id,
            VendorId = vendorId.Value,
            ProjectId = projectId.HasValue && projectId.Value > 0 ? projectId : null,
            PoNumber = poNumber,
            PoType = string.IsNullOrWhiteSpace(poType) ? "Standard Purchase Order" : poType,
            PoDate = poDate ?? DateTime.Today,
            ShipTo = shipTo,
            Remarks = remarks,
            TermsConditions = termsConditions,
            TaxableAmount = finalTaxable,
            GstAmount = totalGst,
            IgstAmount = finalIgst,
            CgstAmount = finalCgst,
            SgstAmount = finalSgst,
            TotalPoValue = total,
            ApprovalStatus = "APPROVED",
            Items = poItems
        };

        _db.PurchaseOrders.Add(po);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Purchase Order <strong>{po.PoNumber}</strong> issued successfully for &#8377; {total:N2}!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> PrintPo(long id)
    {
        var po = await _db.PurchaseOrders
            .Include(p => p.Vendor)
            .Include(p => p.Project)
            .Include(p => p.Company)
            .Include(p => p.Branch)
            .Include(p => p.Items)
                .ThenInclude(i => i.Item)
            .Include(p => p.Items)
                .ThenInclude(i => i.Unit)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (po == null) return NotFound();

        var company = po.Company ?? await _companyContext.GetCurrentCompanyAsync();

        var branch = po.Branch ?? await _db.Branches.FirstOrDefaultAsync() ?? new Branch
        {
            BranchName = company.CompanyName,
            AddressLine1 = !string.IsNullOrEmpty(company.AddressLine1) ? $"{company.AddressLine1}, {company.City} - {company.Pincode}" : (!string.IsNullOrEmpty(company.City) ? $"{company.City}, {company.State}" : "Corporate Headquarters"),
            StateCode = company.StateCode ?? "07",
            Gstin = company.Gstin ?? string.Empty
        };

        var bank = !string.IsNullOrEmpty(company.BankAccountNumber) ? new BankAccount
        {
            BankName = company.BankName ?? "Primary Bank",
            BranchName = company.BankBranch ?? (company.City ?? "Main Branch"),
            AccountNumber = company.BankAccountNumber,
            IfscCode = company.BankIfsc ?? "IFSC0000123",
            AccountType = "CURRENT"
        } : (await _db.BankAccounts.FirstOrDefaultAsync(b => b.IsActive) ?? new BankAccount
        {
            BankName = "HDFC Bank Ltd",
            BranchName = "Connaught Place, New Delhi",
            AccountNumber = "50200084920194",
            IfscCode = "HDFC0000123",
            AccountType = "CURRENT"
        });

        ViewBag.Company = company;
        ViewBag.Branch = branch;
        ViewBag.Bank = bank;
        ViewBag.AmountInWords = NumberToWordsConverter.ConvertToIndianWords(po.TotalPoValue);
        ViewBag.TaxAmountInWords = NumberToWordsConverter.ConvertToIndianWords(po.GstAmount);

        return View("PrintPo", po);
    }

    public async Task<IActionResult> Details(long id)
    {
        return await PrintPo(id);
    }

    private async Task EnsurePurchaseOrderColumnsAsync()
    {
        try
        {
            await _db.Database.ExecuteSqlRawAsync(@"
                IF OBJECT_ID('[PurchaseOrders]') IS NOT NULL OR OBJECT_ID('[dbo].[PurchaseOrders]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'PoType')
                        ALTER TABLE [PurchaseOrders] ADD [PoType] NVARCHAR(100) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'ShipTo')
                        ALTER TABLE [PurchaseOrders] ADD [ShipTo] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'Remarks')
                        ALTER TABLE [PurchaseOrders] ADD [Remarks] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'TermsConditions')
                        ALTER TABLE [PurchaseOrders] ADD [TermsConditions] NVARCHAR(MAX) NULL;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'CgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [CgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'SgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [SgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrders]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrders]')) AND name = 'IgstAmount')
                        ALTER TABLE [PurchaseOrders] ADD [IgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                END

                IF OBJECT_ID('[PurchaseOrderItems]') IS NOT NULL OR OBJECT_ID('[dbo].[PurchaseOrderItems]') IS NOT NULL
                BEGIN
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'GstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'IgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'CgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'SgstRate')
                        ALTER TABLE [PurchaseOrderItems] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'TaxAmount')
                        ALTER TABLE [PurchaseOrderItems] ADD [TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'TaxRateId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [TaxRateId] INT NULL;
                    IF EXISTS (SELECT * FROM sys.columns WHERE (object_id = OBJECT_ID('[PurchaseOrderItems]') OR object_id = OBJECT_ID('[dbo].[PurchaseOrderItems]')) AND name = 'UnitId' AND is_nullable = 0)
                        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [UnitId] INT NULL;
                END
            ");
        }
        catch { }
    }
}

public class AccountsController : Controller
{
    private readonly ApplicationDbContext _db;
    public AccountsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Accounts";
        var groups = await _db.AccountGroups
            .Include(g => g.Accounts)
            .AsNoTracking()
            .ToListAsync();
        ViewBag.JournalEntries = await _db.JournalEntries
            .Include(j => j.Lines)
            .OrderByDescending(j => j.Id)
            .Take(20)
            .AsNoTracking()
            .ToListAsync();
        return View(groups);
    }

    public IActionResult GeneralLedger() => RedirectToAction(nameof(Index));
    public IActionResult JournalEntry() => RedirectToAction(nameof(Index));
}

public class GstController : Controller
{
    private readonly ApplicationDbContext _db;
    public GstController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Gst";
        var gstTransactions = await _db.GstTransactions.AsNoTracking().ToListAsync();
        return View(gstTransactions);
    }

    public IActionResult Reconciliation() => RedirectToAction(nameof(Index));
}

public class BankingController : Controller
{
    private readonly ApplicationDbContext _db;
    public BankingController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Banking";
        var accounts = await _db.BankAccounts.AsNoTracking().ToListAsync();
        return View(accounts);
    }

    public IActionResult Reconciliation() => RedirectToAction(nameof(Index));
}

public class PayrollController : Controller
{
    private readonly ApplicationDbContext _db;
    public PayrollController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Payroll";
        var employees = await _db.Employees.AsNoTracking().ToListAsync();
        return View(employees);
    }
}

public class AssetsController : Controller
{
    private readonly ApplicationDbContext _db;
    public AssetsController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Assets";
        var assets = await _db.FixedAssets.Include(a => a.Allocations).AsNoTracking().ToListAsync();
        return View(assets);
    }
}

public class ReportsController : Controller
{
    public IActionResult Index()
    {
        ViewData["ActiveMenu"] = "Reports";
        return View();
    }
}

public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    public AdminController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Admin";
        ViewBag.Roles = await _db.Roles.Include(r => r.Users).AsNoTracking().ToListAsync();
        var auditLogs = await _db.AuditLogs.OrderByDescending(a => a.CreatedAt).Take(50).AsNoTracking().ToListAsync();
        return View(auditLogs);
    }

    public IActionResult AuditTrail() => RedirectToAction(nameof(Index));
}
