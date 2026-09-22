using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SDK.ERP.Domain.Entities.Admin;
using SDK.ERP.Domain.Entities.MasterData;
using SDK.ERP.Domain.Entities.Projects;
using SDK.ERP.Domain.Entities.Sales;
using SDK.ERP.Domain.Entities.Procurement;
using SDK.ERP.Domain.Entities.Inventory;
using SDK.ERP.Domain.Entities.Accounting;
using SDK.ERP.Domain.Entities.Tax;
using SDK.ERP.Domain.Entities.Banking;
using SDK.ERP.Domain.Entities.Payroll;
using SDK.ERP.Domain.Entities.AssetsAndSupport;

namespace SDK.ERP.Infrastructure.Configurations;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.CompanyCode).IsUnique();
        builder.Property(e => e.CompanyCode).HasMaxLength(20).IsRequired();
        builder.Property(e => e.CompanyName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.LegalName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(15);
        builder.Property(e => e.Pan).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Tan).HasMaxLength(10);
        builder.Property(e => e.BaseCurrency).HasMaxLength(3).HasDefaultValue("INR");
    }
}

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.BranchCode).IsUnique();
        builder.Property(e => e.BranchCode).HasMaxLength(30).IsRequired();
        builder.Property(e => e.BranchName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.StateCode).HasMaxLength(5).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(15);
        builder.Property(e => e.AddressLine1).HasMaxLength(255).IsRequired();
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.RoleName).IsUnique();
        builder.Property(e => e.RoleName).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Description).HasMaxLength(255);
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users", "admin");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.Username).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Username).HasMaxLength(50).IsRequired();
        builder.Property(e => e.Email).HasMaxLength(100).IsRequired();
        builder.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
    }
}

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients", "master");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ClientCode).IsUnique();
        builder.Property(e => e.ClientCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.ClientName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.StateCode).HasMaxLength(10).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(30);
        builder.Property(e => e.Pan).HasMaxLength(30);
        builder.Property(e => e.CreditLimit).HasColumnType("decimal(15,2)");
    }
}

public class VendorConfiguration : IEntityTypeConfiguration<Vendor>
{
    public void Configure(EntityTypeBuilder<Vendor> builder)
    {
        builder.ToTable("vendors", "master");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.VendorCode).IsUnique();
        builder.Property(e => e.VendorCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.VendorName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Gstin).HasMaxLength(30);
        builder.Property(e => e.Pan).HasMaxLength(30).IsRequired();
        builder.Property(e => e.MsmeType).HasMaxLength(50);
        builder.Property(e => e.IfscCode).HasMaxLength(30);
    }
}

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("items", "master");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ItemCode).IsUnique();
        builder.Property(e => e.ItemCode).HasMaxLength(30).IsRequired();
        builder.Property(e => e.ItemName).HasMaxLength(150).IsRequired();
        builder.Property(e => e.HsnSacCode).HasMaxLength(10);
        builder.Property(e => e.UnitCost).HasColumnType("decimal(12,2)");
        builder.Property(e => e.CurrentStockQty).HasColumnType("decimal(10,2)");
        builder.Property(e => e.ReorderLevelQty).HasColumnType("decimal(10,2)");
    }
}

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects", "project");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.ProjectCode).IsUnique();
        builder.Property(e => e.ProjectCode).HasMaxLength(50).IsRequired();
        builder.Property(e => e.ProjectName).HasMaxLength(200).IsRequired();
        builder.Property(e => e.ContractValue).HasColumnType("decimal(15,2)");
        builder.Property(e => e.BudgetCost).HasColumnType("decimal(15,2)");
        builder.Property(e => e.Status).HasMaxLength(30);
    }
}

public class SalesInvoiceConfiguration : IEntityTypeConfiguration<SalesInvoice>
{
    public void Configure(EntityTypeBuilder<SalesInvoice> builder)
    {
        builder.ToTable("sales_invoices", "sales");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.InvoiceNumber).IsUnique();
        builder.HasIndex(e => new { e.ClientId, e.Status, e.InvoiceDate });
        builder.Property(e => e.InvoiceNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PlaceOfSupply).HasMaxLength(5).IsRequired();
        builder.Property(e => e.TaxableAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.CgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.SgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.IgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.TotalInvoiceValue).HasColumnType("decimal(15,2)");
        builder.Property(e => e.PaidAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.OutstandingBalance).HasColumnType("decimal(15,2)");
        builder.Property(e => e.Status).HasMaxLength(20);
    }
}

public class PurchaseBillConfiguration : IEntityTypeConfiguration<PurchaseBill>
{
    public void Configure(EntityTypeBuilder<PurchaseBill> builder)
    {
        builder.ToTable("purchase_bills", "procurement");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.VendorId, e.Status, e.DueDate });
        builder.Property(e => e.VendorBillNumber).HasMaxLength(50).IsRequired();
        builder.Property(e => e.PlaceOfSupply).HasMaxLength(5).IsRequired();
        builder.Property(e => e.TaxableAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.CgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.SgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.IgstAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.TotalBillAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.PaidAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.BalanceDue).HasColumnType("decimal(15,2)");
        builder.Property(e => e.Status).HasMaxLength(20);
    }
}

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("journal_entries", "accounting");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => e.VoucherNo).IsUnique();
        builder.Property(e => e.VoucherNo).HasMaxLength(50).IsRequired();
        builder.Property(e => e.VoucherType).HasMaxLength(30).IsRequired();
        builder.Property(e => e.TotalDebit).HasColumnType("decimal(15,2)");
        builder.Property(e => e.TotalCredit).HasColumnType("decimal(15,2)");
        
        builder.HasOne(e => e.FinancialYear)
               .WithMany()
               .HasForeignKey(e => e.FyId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Creator)
               .WithMany()
               .HasForeignKey(e => e.CreatedBy)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

public class JournalLineConfiguration : IEntityTypeConfiguration<JournalLine>
{
    public void Configure(EntityTypeBuilder<JournalLine> builder)
    {
        builder.ToTable("journal_lines", "accounting");
        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.AccountId, e.JournalEntryId });
        builder.Property(e => e.DebitAmount).HasColumnType("decimal(15,2)");
        builder.Property(e => e.CreditAmount).HasColumnType("decimal(15,2)");
    }
}
