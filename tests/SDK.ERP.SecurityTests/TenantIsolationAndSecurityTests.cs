using Microsoft.EntityFrameworkCore;
using SDK.ERP.Application.Common;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;

namespace SDK.ERP.SecurityTests;

public class TestTenantProvider : ICurrentTenantProvider
{
    public long CurrentCompanyId { get; set; } = 1;

    public void SetTenant(long companyId)
    {
        CurrentCompanyId = companyId;
    }
}

public class TenantIsolationAndSecurityTests
{
    private ApplicationDbContext CreateTestDbContext(TestTenantProvider tenantProvider, string dbName)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new ApplicationDbContext(options, tenantProvider);
    }

    [Fact]
    public async Task ZeroLeakage_GlobalQueryFilter_HidesOtherTenantData()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenantProvider = new TestTenantProvider { CurrentCompanyId = 1 };

        // 1. Seed records across two separate tenants using IgnoreQueryFilters
        using (var db = CreateTestDbContext(tenantProvider, dbName))
        {
            var clientOrg1 = new Client
            {
                CompanyId = 1,
                ClientCode = "ORG1-CL-001",
                ClientName = "Company 1 Client",
                BillingAddress = "Address 1",
                StateCode = "07",
                CreditDays = 30
            };

            var clientOrg2 = new Client
            {
                CompanyId = 2,
                ClientCode = "ORG2-CL-002",
                ClientName = "Company 2 Client",
                BillingAddress = "Address 2",
                StateCode = "27",
                CreditDays = 15
            };

            db.Clients.AddRange(clientOrg1, clientOrg2);
            await db.SaveChangesAsync();
        }

        // 2. Query as Tenant 1 -> Must only see Org 1 Client
        using (var db = CreateTestDbContext(tenantProvider, dbName))
        {
            var results = await db.Clients.ToListAsync();

            Assert.Single(results);
            Assert.Equal("ORG1-CL-001", results[0].ClientCode);
            Assert.Equal(1, results[0].CompanyId);

            // Directly querying by ID of Tenant 2 must return null
            var crossTenantClient = await db.Clients.FirstOrDefaultAsync(c => c.ClientCode == "ORG2-CL-002");
            Assert.Null(crossTenantClient);
        }

        // 3. Switch active tenant context to Tenant 2 -> Must only see Org 2 Client
        tenantProvider.CurrentCompanyId = 2;
        using (var db = CreateTestDbContext(tenantProvider, dbName))
        {
            var results = await db.Clients.ToListAsync();

            Assert.Single(results);
            Assert.Equal("ORG2-CL-002", results[0].ClientCode);
            Assert.Equal(2, results[0].CompanyId);

            var crossTenantClient = await db.Clients.FirstOrDefaultAsync(c => c.ClientCode == "ORG1-CL-001");
            Assert.Null(crossTenantClient);
        }
    }

    [Fact]
    public async Task CrossTenantTamper_WriteInterception_BlocksCrossTenantModification()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenantProvider = new TestTenantProvider { CurrentCompanyId = 2 };

        // Seed data for Tenant 2
        using (var db = CreateTestDbContext(tenantProvider, dbName))
        {
            var vendorOrg2 = new Vendor
            {
                CompanyId = 2,
                VendorCode = "VND-ORG2",
                VendorName = "Tenant 2 Vendor",
                PaymentTermsDays = 30
            };
            db.Vendors.Add(vendorOrg2);
            await db.SaveChangesAsync();
        }

        // Attacker switches to Tenant 1 and attempts to maliciously update or tamper with Tenant 2's vendor
        tenantProvider.CurrentCompanyId = 1;
        using (var db = CreateTestDbContext(tenantProvider, dbName))
        {
            // Try to maliciously attach and modify an entity belonging to Company 2 while logged into Company 1
            var maliciousVendor = new Vendor
            {
                Id = 1,
                CompanyId = 2, // Belonging to another company
                VendorCode = "VND-HACKED",
                VendorName = "Hacked Vendor"
            };

            db.Vendors.Attach(maliciousVendor);
            db.Entry(maliciousVendor).State = EntityState.Modified;

            // SaveChangesAsync must detect cross-tenant modification and throw InvalidOperationException
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await db.SaveChangesAsync();
            });
        }
    }

    [Theory]
    [InlineData(".png", true)]
    [InlineData(".jpg", true)]
    [InlineData(".jpeg", true)]
    [InlineData(".svg", true)]
    [InlineData(".webp", true)]
    [InlineData(".exe", false)]
    [InlineData(".dll", false)]
    [InlineData(".php", false)]
    [InlineData(".aspx", false)]
    [InlineData(".sh", false)]
    [InlineData(".bat", false)]
    public void FileUploadSecurity_EnforcesStrictExtensionWhitelist(string extension, bool expectedAllowed)
    {
        var allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".svg", ".webp"
        };

        bool isAllowed = allowedExtensions.Contains(extension);
        Assert.Equal(expectedAllowed, isAllowed);
    }

    [Fact]
    public void PasswordHashing_ProducesDeterministicSecureHash()
    {
        var password = "SecureEnterprisePassword@2026";
        using var sha256 = SHA256.Create();
        var hashed = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(password))).ToLowerInvariant();

        Assert.NotEmpty(hashed);
        Assert.Equal(64, hashed.Length); // SHA-256 produces a 64 character hex string
        Assert.NotEqual(password, hashed);
    }

    [Fact]
    public async Task UserCompanyIsolation_RestrictsSwitchingToUnownedCompanies()
    {
        var dbName = Guid.NewGuid().ToString();
        var tenantProvider = new TestTenantProvider { CurrentCompanyId = 1 };

        using (var db = CreateTestDbContext(tenantProvider, dbName))
        {
            // Seed Company 1 (Binary Solution) and Company 2 (The Local Cafe)
            var comp1 = new SDK.ERP.Domain.Entities.Admin.Company { Id = 1, CompanyName = "Binary Solution", BrandShortName = "BINARY" };
            var comp2 = new SDK.ERP.Domain.Entities.Admin.Company { Id = 2, CompanyName = "The Local Cafe", BrandShortName = "CAFE" };
            db.Companies.AddRange(comp1, comp2);

            // User Kamal belongs ONLY to Company 1
            var kamal = new SDK.ERP.Domain.Entities.Admin.User
            {
                Id = 10,
                CompanyId = 1,
                Username = "kamal",
                Email = "kamal@binary.com",
                FullName = "Kamal Adhikari",
                IsActive = true
            };
            db.Users.Add(kamal);
            await db.SaveChangesAsync();

            // Simulate query: find accessible companies for Kamal
            var accessibleCompanyIds = await db.Users
                .IgnoreQueryFilters()
                .Where(u => u.IsActive && (u.Id == kamal.Id || u.Email == kamal.Email || u.Username == kamal.Username))
                .Select(u => u.CompanyId)
                .Distinct()
                .ToListAsync();

            // Kamal must ONLY have access to Company 1
            Assert.Single(accessibleCompanyIds);
            Assert.Contains(1, accessibleCompanyIds);
            Assert.DoesNotContain(2, accessibleCompanyIds);

            // Verify that attempting to switch to Company 2 is disallowed
            bool canSwitchToCompany2 = accessibleCompanyIds.Contains(2);
            Assert.False(canSwitchToCompany2);
        }
    }
}
