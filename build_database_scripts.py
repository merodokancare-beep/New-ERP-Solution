# -*- coding: utf-8 -*-
"""
Database Script Generator for SDK Solutions ERP
Generates:
1. SDK_ERP_Database_Complete.sql (Full Schema + Current Data & Masters)
2. SDK_ERP_Database_Fresh_Install.sql (Full Schema + Clean Production Seeds)
"""

import os
import re

def escape_sql_string(val):
    if val is None:
        return "NULL"
    val_str = str(val).replace("'", "''")
    return f"N'{val_str}'"

def format_value(val, col_type="str"):
    if val is None:
        return "NULL"
    if col_type == "bool":
        return "1" if val is True or val == 1 or str(val).lower() == "true" else "0"
    if col_type in ("int", "bigint"):
        return str(val)
    if col_type in ("decimal", "float"):
        return f"{float(val):.2f}"
    if col_type == "datetime":
        dt_str = str(val).replace("T", " ")
        if "." in dt_str:
            dt_str = dt_str[:23]  # trim to milliseconds
        return f"'{dt_str}'"
    return escape_sql_string(val)

def generate_header(title, description):
    return f"""-- ============================================================================
-- PROJECT: SDK SOLUTIONS ENTERPRISE ERP
-- FILE: {title}
-- DESCRIPTION: {description}
-- TARGET ENGINE: Microsoft SQL Server 2016 / 2019 / 2022 / Azure SQL / LocalDB
-- GENERATED AT: 2026-09-24
-- ============================================================================

SET NOCOUNT ON;
GO

-- 1. DATABASE CREATION (IF NOT EXISTS)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SDK_ERP_DB')
BEGIN
    PRINT 'Creating database [SDK_ERP_DB]...';
    CREATE DATABASE [SDK_ERP_DB]
    COLLATE SQL_Latin1_General_CP1_CI_AS;
END
ELSE
BEGIN
    PRINT 'Database [SDK_ERP_DB] already exists. Proceeding with schema configuration.';
END
GO

USE [SDK_ERP_DB];
GO

-- 2. ENABLE RECOMMENDED DATABASE OPTIONS
ALTER DATABASE CURRENT SET ANSI_NULL_DEFAULT ON;
ALTER DATABASE CURRENT SET ANSI_NULLS ON;
ALTER DATABASE CURRENT SET ANSI_PADDING ON;
ALTER DATABASE CURRENT SET ANSI_WARNINGS ON;
ALTER DATABASE CURRENT SET ARITHABORT ON;
ALTER DATABASE CURRENT SET CONCAT_NULL_YIELDS_NULL ON;
ALTER DATABASE CURRENT SET QUOTED_IDENTIFIER ON;
GO

"""

def get_clean_schema_sql():
    with open("schema_raw.sql", "r", encoding="utf-8-sig") as f:
        content = f.read()

    # Wrap table creations with IF NOT EXISTS checks for idempotency and safety
    # Split by batches (GO)
    batches = content.split("\nGO\n")
    cleaned_batches = []
    
    # Ensure tax schema is also created
    schema_pre = """IF SCHEMA_ID(N'tax') IS NULL EXEC(N'CREATE SCHEMA [tax];');\nGO\n"""
    
    for batch in batches:
        b = batch.strip()
        if not b:
            continue
        # Table creation wrap
        m_table = re.match(r'CREATE TABLE\s+(\[?[a-zA-Z0-9_]+\]?(?:\.\[?[a-zA-Z0-9_]+\]?)?)\s*\(', b)
        if m_table:
            tbl_name = m_table.group(1)
            wrapped = f"""IF OBJECT_ID(N'{tbl_name}', N'U') IS NULL
BEGIN
{b}
END"""
            cleaned_batches.append(wrapped)
        # Index creation wrap
        elif b.startswith("CREATE INDEX ") or b.startswith("CREATE UNIQUE INDEX "):
            m_idx = re.match(r'CREATE\s+(?:UNIQUE\s+)?INDEX\s+(\[[a-zA-Z0-9_]+\])\s+ON\s+(\[?[a-zA-Z0-9_]+\]?(?:\.\[?[a-zA-Z0-9_]+\]?)?)', b)
            if m_idx:
                idx_name = m_idx.group(1).strip("[]")
                on_table = m_idx.group(2)
                wrapped = f"""IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'{idx_name}' AND object_id = OBJECT_ID(N'{on_table}'))
BEGIN
{b}
END"""
                cleaned_batches.append(wrapped)
            else:
                cleaned_batches.append(b)
        else:
            cleaned_batches.append(b)

    return "\nGO\n\n".join(cleaned_batches) + "\nGO\n"

def get_runtime_patches():
    return """
-- ============================================================================
-- 4. APPLICATION RUNTIME COMPATIBILITY & COLUMN EXPANSIONS
-- ============================================================================
PRINT 'Verifying runtime column definitions & constraints...';

-- Accommodate flexible GSTIN / PAN inputs
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'Pan')
    ALTER TABLE [master].[clients] ALTER COLUMN [Pan] NVARCHAR(50) NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[clients]') AND name = 'Gstin')
    ALTER TABLE [master].[clients] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'Pan')
    ALTER TABLE [master].[vendors] ALTER COLUMN [Pan] NVARCHAR(50) NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[master].[vendors]') AND name = 'Gstin')
    ALTER TABLE [master].[vendors] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'ManagerId')
    ALTER TABLE [project].[projects] ALTER COLUMN [ManagerId] BIGINT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[GstTransactions]') AND name = 'VoucherId')
    ALTER TABLE [GstTransactions] ALTER COLUMN [VoucherId] BIGINT NULL;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_journal_entries_FinancialYears_FinancialYearId')
    ALTER TABLE [accounting].[journal_entries] DROP CONSTRAINT [FK_journal_entries_FinancialYears_FinancialYearId];

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[accounting].[journal_entries]') AND name = 'FinancialYearId')
    ALTER TABLE [accounting].[journal_entries] ALTER COLUMN [FinancialYearId] INT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[DocumentAttachments]') AND name = 'UploaderId' AND is_nullable = 0)
    ALTER TABLE [DocumentAttachments] ALTER COLUMN [UploaderId] BIGINT NULL;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[DocumentAttachments]') AND name = 'UploadedBy' AND is_nullable = 0)
    ALTER TABLE [DocumentAttachments] ALTER COLUMN [UploadedBy] BIGINT NULL;
GO
"""

def generate_fresh_install_script():
    header = generate_header(
        "SDK_ERP_Database_Fresh_Install.sql",
        "Clean production installation script with full schema, tables, indexes, constraints, and standard foundational seed data."
    )
    schema = get_clean_schema_sql()
    patches = get_runtime_patches()

    seed = """
-- ============================================================================
-- 5. FOUNDATIONAL SEED DATA (FRESH INSTALLATION)
-- ============================================================================
PRINT 'Seeding foundational master records...';

-- 5.1 Default Organization / Company
IF NOT EXISTS (SELECT * FROM [admin].[companies] WHERE [Id] = 1)
BEGIN
    SET IDENTITY_INSERT [admin].[companies] ON;
    INSERT INTO [admin].[companies] (
        [Id], [CompanyCode], [CompanyName], [LegalName], [BrandShortName], [Tagline],
        [Industry], [Email], [Phone], [Website], [AddressLine1], [AddressLine2],
        [City], [State], [StateCode], [Pincode], [Gstin], [Pan],
        [BankName], [BankAccountNumber], [BankIfsc], [BankBranch], [UpiId],
        [AuthorizedSignatoryName], [AuthorizedSignatoryDesignation], [TermsAndConditions],
        [BaseCurrency], [FinancialYearStart], [CreatedAt], [UpdatedAt]
    ) VALUES (
        1, N'CORP-01', N'SDK Solutions Private Limited', N'SDK Solutions Private Limited', N'SDK', N'SOLUTIONS ERP',
        N'IT Services & Consulting', N'contact@sdksolutions.com', N'+91 98765 43210', N'https://sdksolutions.com',
        N'Connaught Place, Central Business District', NULL,
        N'New Delhi', N'Delhi', N'07', N'110001', N'07AAAAA0000A1Z5', N'AAAAA0000A',
        N'HDFC Bank Ltd', N'50200084920194', N'HDFC0000123', N'Connaught Place Branch, New Delhi', N'sdk@hdfcbank',
        N'Managing Director', N'Authorized Signatory',
        N'1. Goods/Services once billed are subject to standard delivery terms.
2. Payment is due within 30 days from invoice date.
3. Applicable statutory GST has been assessed as per Indian GST Act.',
        N'INR', '2026-04-01', GETUTCDATE(), GETUTCDATE()
    );
    SET IDENTITY_INSERT [admin].[companies] OFF;
    PRINT '-> Company seeded: SDK Solutions Private Limited';
END
GO

-- 5.2 Head Office Branch
IF NOT EXISTS (SELECT * FROM [admin].[branches] WHERE [Id] = 1)
BEGIN
    SET IDENTITY_INSERT [admin].[branches] ON;
    INSERT INTO [admin].[branches] (
        [Id], [CompanyId], [BranchCode], [BranchName], [StateCode], [Gstin], [AddressLine1], [IsHeadOffice], [IsActive]
    ) VALUES (
        1, 1, N'BR-HQ-DEL', N'Head Office - New Delhi', N'07', N'07AAAAA0000A1Z5', N'Connaught Place, New Delhi', 1, 1
    );
    SET IDENTITY_INSERT [admin].[branches] OFF;
    PRINT '-> Branch seeded: Head Office - New Delhi';
END
GO

-- 5.3 System Security Roles (RBAC)
SET IDENTITY_INSERT [admin].[roles] ON;
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 1)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (1, N'SUPER_ADMIN', N'Full Enterprise Administration', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 2)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (2, N'PROJECT_MANAGER', N'Project Management & Delivery', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 3)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (3, N'ACCOUNTANT', N'Financial Accounting & GST Compliance', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 4)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (4, N'PROCUREMENT_OFFICER', N'Purchasing & Vendor Management', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 5)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (5, N'STORE_KEEPER', N'Internal Office Stock Consumables', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 1002)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (1002, N'COMPANY_ADMIN', N'Organization Administrator & Workspace Owner', 1);
SET IDENTITY_INSERT [admin].[roles] OFF;
PRINT '-> System Roles seeded (SUPER_ADMIN, PROJECT_MANAGER, ACCOUNTANT, PROCUREMENT_OFFICER, STORE_KEEPER, COMPANY_ADMIN)';
GO

-- 5.4 Super Administrator User (Username: admin, Password: Admin@123 or admin123)
-- SHA-256 for Admin@123: c63b46eb0c2a21e40ebad34ff7f9aa5bb9f48ca4fc11409ae0012bbecf5d22d2
IF NOT EXISTS (SELECT * FROM [admin].[users] WHERE [Username] = N'admin')
BEGIN
    SET IDENTITY_INSERT [admin].[users] ON;
    INSERT INTO [admin].[users] (
        [Id], [CompanyId], [BranchId], [RoleId], [Username], [FullName], [Designation],
        [PhoneNumber], [Email], [PasswordHash], [IsActive]
    ) VALUES (
        1, 1, 1, 1, N'admin', N'System Administrator', N'Administrator',
        N'+91 98765 43210', N'admin@sdksolutions.com',
        N'c63b46eb0c2a21e40ebad34ff7f9aa5bb9f48ca4fc11409ae0012bbecf5d22d2', 1
    );
    SET IDENTITY_INSERT [admin].[users] OFF;
    PRINT '-> Default Admin User seeded: Username: admin | Password: Admin@123';
END
GO

-- 5.5 Standard GST Tax Rates
SET IDENTITY_INSERT [TaxRates] ON;
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 1)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (1, N'GST 0% (Exempt)', 0.00, 0.00, 0.00, 0.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 2)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (2, N'GST 5%', 5.00, 2.50, 2.50, 5.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 3)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (3, N'GST 12%', 12.00, 6.00, 6.00, 12.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 4)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (4, N'GST 18%', 18.00, 9.00, 9.00, 18.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 5)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (5, N'GST 28%', 28.00, 14.00, 14.00, 28.00, 1);
SET IDENTITY_INSERT [TaxRates] OFF;
PRINT '-> GST Tax Rates seeded (0%, 5%, 12%, 18%, 28%)';
GO

-- 5.6 Standard Item Units (UOM)
SET IDENTITY_INSERT [ItemUnits] ON;
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 1)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (1, N'NOS', N'Numbers', 0);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 2)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (2, N'BOX', N'Boxes', 0);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 3)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (3, N'PKT', N'Packets', 0);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 4)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (4, N'HRS', N'Hours', 1);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 5)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (5, N'KGS', N'Kilograms', 1);
SET IDENTITY_INSERT [ItemUnits] OFF;
PRINT '-> Standard Units of Measurement seeded (NOS, BOX, PKT, HRS, KGS)';
GO

-- 5.7 Standard Item Categories
SET IDENTITY_INSERT [ItemCategories] ON;
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 1)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (1, N'Hardware & Equipment', 1);
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 2)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (2, N'Cables & Networking', 1);
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 3)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (3, N'Office Consumables', 1);
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 4)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (4, N'Tools & Safety Gear', 1);
SET IDENTITY_INSERT [ItemCategories] OFF;
PRINT '-> Item Categories seeded';
GO

-- 5.8 Standard Project Types
SET IDENTITY_INSERT [ProjectTypes] ON;
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 1)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (1, N'STANDARD', N'Standard Turnkey Contract', N'Fixed-price supply, installation, testing and commissioning contracts', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 2)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (2, N'AMC', N'Annual Maintenance Contract (AMC)', N'Ongoing operations, service level maintenance and support agreements', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 3)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (3, N'CONSULTING', N'Consulting & Advisory', N'Professional technical advisory, system design and project management', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 4)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (4, N'SUPPLY_INSTALL', N'Supply & Installation', N'Material delivery with onsite installation and sign-off', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 5)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (5, N'MANPOWER', N'Manpower & Managed Services', N'Time and material / rate card based deployment', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 6)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (6, N'INTERNAL', N'Internal R&D / Capital Project', N'Internal organizational infrastructure or R&D initiatives', 1);
SET IDENTITY_INSERT [ProjectTypes] OFF;
PRINT '-> Project Types seeded';
GO

-- 5.9 Chart of Account Groups
SET IDENTITY_INSERT [AccountGroups] ON;
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 1)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (1, N'1000', N'Current Assets', N'ASSET', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 2)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (2, N'2000', N'Current Liabilities', N'LIABILITY', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 3)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (3, N'3000', N'Equity & Reserves', N'EQUITY', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 4)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (4, N'4000', N'Project Revenue', N'REVENUE', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 5)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (5, N'5000', N'Direct Expenses', N'EXPENSE', NULL);
SET IDENTITY_INSERT [AccountGroups] OFF;
PRINT '-> Chart of Account Groups seeded';
GO

-- 5.10 Financial Year
SET IDENTITY_INSERT [FinancialYears] ON;
IF NOT EXISTS (SELECT * FROM [FinancialYears] WHERE [Id] = 1)
    INSERT INTO [FinancialYears] ([Id], [CompanyId], [FyCode], [StartDate], [EndDate], [IsClosed]) VALUES (1, 1, N'FY-2026-27', '2026-04-01', '2027-03-31', 0);
SET IDENTITY_INSERT [FinancialYears] OFF;
PRINT '-> Financial Year seeded: FY-2026-27';
GO

-- 5.11 Genesis Audit Log
IF NOT EXISTS (SELECT * FROM [AuditLogs] WHERE [TableName] = N'SYSTEM_INITIALIZATION')
BEGIN
    INSERT INTO [AuditLogs] ([TableName], [RecordId], [ActionType], [CurrentHash], [PreviousHash], [CreatedAt], [IpAddress], [UserAgent])
    VALUES (
        N'SYSTEM_INITIALIZATION', 0, N'GENESIS_BOOTSTRAP',
        N'a1b2c3d4e5f60718293a4b5c6d7e8f90a1b2c3d4e5f60718293a4b5c6d7e8f90',
        N'0000000000000000000000000000000000000000000000000000000000000000',
        GETUTCDATE(), N'127.0.0.1', N'SDK_ERP_BOOTSTRAP'
    );
END
GO

PRINT '=============================================================================';
PRINT '  SDK SOLUTIONS ERP FRESH DATABASE SETUP COMPLETED SUCCESSFULLY!            ';
PRINT '  - Database Name: SDK_ERP_DB                                               ';
PRINT '  - Default Admin Username: admin                                           ';
PRINT '  - Default Admin Password: Admin@123 (or admin123)                         ';
PRINT '=============================================================================';
"""
    return header + schema + patches + seed

def generate_complete_with_data_script():
    header = generate_header(
        "SDK_ERP_Database_Complete.sql",
        "Full installation script with complete schema, indexes, constraints, multi-tenant organizations, demo projects, clients, vendors, and transactions."
    )
    schema = get_clean_schema_sql()
    patches = get_runtime_patches()

    seed = """
-- ============================================================================
-- 5. COMPREHENSIVE SEED & WORKING DATA INSERTION
-- ============================================================================
PRINT 'Seeding all organizations, security roles, users, masters & demo records...';

-- 5.1 Companies / Organizations
SET IDENTITY_INSERT [admin].[companies] ON;
IF NOT EXISTS (SELECT * FROM [admin].[companies] WHERE [Id] = 1)
    INSERT INTO [admin].[companies] (
        [Id], [CompanyCode], [CompanyName], [LegalName], [BrandShortName], [Tagline],
        [Industry], [Email], [Phone], [Website], [AddressLine1], [AddressLine2],
        [City], [State], [StateCode], [Pincode], [Gstin], [Pan],
        [BankName], [BankAccountNumber], [BankIfsc], [BankBranch], [UpiId],
        [AuthorizedSignatoryName], [AuthorizedSignatoryDesignation], [TermsAndConditions],
        [BaseCurrency], [FinancialYearStart], [CreatedAt], [UpdatedAt]
    ) VALUES (
        1, N'SDK-CORP', N'SDK Solution', N'SDK Solutions', N'SDK', N'SDK SOLUTIONS ERP',
        N'IT Services & Consulting', N'contact@sdksolutions.com', N'+91 98765 43210', N'https://sdksolutions.com',
        N'Connaught Place, Central Business District', NULL,
        N'New Delhi', N'Delhi', N'07', N'110001', N'07AAAAA0000A1Z5', N'AAAAA0000A',
        N'HDFC Bank Ltd', N'50200084920194', N'HDFC0000123', N'Connaught Place Branch, New Delhi', N'sdk@hdfcbank',
        N'Managing Director', N'Authorized Signatory',
        N'1. Goods/Services once billed are subject to standard delivery terms.
2. Payment is due within 30 days from invoice date.
3. Applicable statutory GST has been assessed as per Indian GST Act.',
        N'INR', '2026-04-01', '2026-09-10 08:23:13.167', '2026-09-22 17:05:51.015'
    );

IF NOT EXISTS (SELECT * FROM [admin].[companies] WHERE [Id] = 2)
    INSERT INTO [admin].[companies] (
        [Id], [CompanyCode], [CompanyName], [LegalName], [BrandShortName], [Tagline],
        [City], [Pan], [BaseCurrency], [FinancialYearStart], [CreatedAt], [UpdatedAt]
    ) VALUES (
        2, N'CORP-02', N'Vani ERP', N'Vani ERP Management System', N'VANI', N'ENTERPRISE ERP',
        N'Sikkim', N'', N'INR', '2026-04-01', '2026-09-21 18:06:28.569', '2026-09-21 18:06:28.569'
    );

IF NOT EXISTS (SELECT * FROM [admin].[companies] WHERE [Id] = 3)
    INSERT INTO [admin].[companies] (
        [Id], [CompanyCode], [CompanyName], [LegalName], [BrandShortName], [Tagline],
        [Industry], [City], [Pan], [BaseCurrency], [FinancialYearStart], [CreatedAt], [UpdatedAt]
    ) VALUES (
        3, N'BINARY', N'Binary Solution', N'Binary Solution', N'BINARY', N'ENTERPRISE SUITE',
        N'IT Services & Consulting', N'East Sikkim', N'', N'INR', '2026-04-01', '2026-09-21 18:33:25.555', '2026-09-21 18:34:19.057'
    );
SET IDENTITY_INSERT [admin].[companies] OFF;
GO

-- 5.2 Branches
SET IDENTITY_INSERT [admin].[branches] ON;
IF NOT EXISTS (SELECT * FROM [admin].[branches] WHERE [Id] = 1)
    INSERT INTO [admin].[branches] ([Id], [CompanyId], [BranchCode], [BranchName], [StateCode], [Gstin], [AddressLine1], [IsHeadOffice], [IsActive])
    VALUES (1, 1, N'BR-HQ-DEL', N'Head Office - New Delhi', N'07', N'07AAAAA0000A1Z5', N'Connaught Place, New Delhi', 1, 1);

IF NOT EXISTS (SELECT * FROM [admin].[branches] WHERE [Id] = 2)
    INSERT INTO [admin].[branches] ([Id], [CompanyId], [BranchCode], [BranchName], [StateCode], [Gstin], [AddressLine1], [IsHeadOffice], [IsActive])
    VALUES (2, 3, N'BINARY-HQ', N'Headquarters', N'07', NULL, N'East Sikkim', 1, 1);
SET IDENTITY_INSERT [admin].[branches] OFF;
GO

-- 5.3 System Security Roles
SET IDENTITY_INSERT [admin].[roles] ON;
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 1)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (1, N'SUPER_ADMIN', N'Full Enterprise Administration', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 2)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (2, N'PROJECT_MANAGER', N'Project Management & Delivery', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 3)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (3, N'ACCOUNTANT', N'Financial Accounting & GST Compliance', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 4)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (4, N'PROCUREMENT_OFFICER', N'Purchasing & Vendor Management', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 5)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (5, N'STORE_KEEPER', N'Internal Office Stock Consumables', 1);
IF NOT EXISTS (SELECT * FROM [admin].[roles] WHERE [Id] = 1002)
    INSERT INTO [admin].[roles] ([Id], [RoleName], [Description], [IsSystemRole]) VALUES (1002, N'COMPANY_ADMIN', N'Organization Administrator & Workspace Owner', 1);
SET IDENTITY_INSERT [admin].[roles] OFF;
GO

-- 5.4 Users
SET IDENTITY_INSERT [admin].[users] ON;
IF NOT EXISTS (SELECT * FROM [admin].[users] WHERE [Id] = 1)
    INSERT INTO [admin].[users] ([Id], [CompanyId], [BranchId], [RoleId], [Username], [Email], [PasswordHash], [IsActive], [LastLoginAt], [FullName], [Designation])
    VALUES (1, 1, 1, 1, N'sabir', N'admin@sdksolutions.com', N'240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 1, '2026-09-22 17:34:29.931', N'Admin User', N'Enterprise Owner & CEO');

-- Standard 'admin' alias user for immediate standard login
IF NOT EXISTS (SELECT * FROM [admin].[users] WHERE [Username] = N'admin')
    INSERT INTO [admin].[users] ([Id], [CompanyId], [BranchId], [RoleId], [Username], [Email], [PasswordHash], [IsActive], [FullName], [Designation])
    VALUES (4, 1, 1, 1, N'admin', N'admin@sdksolutions.com', N'c63b46eb0c2a21e40ebad34ff7f9aa5bb9f48ca4fc11409ae0012bbecf5d22d2', 1, N'System Administrator', N'Administrator');

IF NOT EXISTS (SELECT * FROM [admin].[users] WHERE [Id] = 2)
    INSERT INTO [admin].[users] ([Id], [CompanyId], [BranchId], [RoleId], [Username], [Email], [PasswordHash], [IsActive], [LastLoginAt], [FullName], [Designation], [PhoneNumber])
    VALUES (2, 3, 2, 1002, N'kamal', N'kamal@gmail.com', N'7b86f47f8a82dfdc96efc3c61262966997f8b141f152cd0879a5c72ccb671987', 1, '2026-09-22 17:31:46.404', N'Kamal Adhikari', N'Managing Director / Owner', N'9832508859');

IF NOT EXISTS (SELECT * FROM [admin].[users] WHERE [Id] = 3)
    INSERT INTO [admin].[users] ([Id], [CompanyId], [BranchId], [RoleId], [Username], [Email], [PasswordHash], [IsActive], [FullName], [Designation])
    VALUES (3, 1, 1, 3, N'testuser', N'testuser2@sdksolutions.com', N'16adc0ce4d6a19677ec00b0af77418aeb5ce68b6811ce1b9a00191e8a6e6a6d3', 1, N'Test User', N'Team Member');

IF NOT EXISTS (SELECT * FROM [admin].[users] WHERE [Id] = 5)
    INSERT INTO [admin].[users] ([Id], [CompanyId], [BranchId], [RoleId], [Username], [Email], [PasswordHash], [IsActive], [FullName], [Designation])
    VALUES (5, 3, 2, 2, N'sita.sharma', N'sita@binarysolution.com', N'16adc0ce4d6a19677ec00b0af77418aeb5ce68b6811ce1b9a00191e8a6e6a6d3', 1, N'Sita Sharma', N'Team Member');
SET IDENTITY_INSERT [admin].[users] OFF;
GO

-- 5.5 Standard GST Tax Rates
SET IDENTITY_INSERT [TaxRates] ON;
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 1)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (1, N'GST 0% (Exempt)', 0.00, 0.00, 0.00, 0.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 2)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (2, N'GST 5%', 5.00, 2.50, 2.50, 5.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 3)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (3, N'GST 12%', 12.00, 6.00, 6.00, 12.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 4)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (4, N'GST 18%', 18.00, 9.00, 9.00, 18.00, 1);
IF NOT EXISTS (SELECT * FROM [TaxRates] WHERE [Id] = 5)
    INSERT INTO [TaxRates] ([Id], [TaxName], [RatePercentage], [CgstPercentage], [SgstPercentage], [IgstPercentage], [IsActive]) VALUES (5, N'GST 28%', 28.00, 14.00, 14.00, 28.00, 1);
SET IDENTITY_INSERT [TaxRates] OFF;
GO

-- 5.6 Standard Item Units
SET IDENTITY_INSERT [ItemUnits] ON;
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 1)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (1, N'NOS', N'Numbers', 0);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 2)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (2, N'BOX', N'Boxes', 0);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 3)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (3, N'PKT', N'Packets', 0);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 4)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (4, N'HRS', N'Hours', 1);
IF NOT EXISTS (SELECT * FROM [ItemUnits] WHERE [Id] = 5)
    INSERT INTO [ItemUnits] ([Id], [UnitCode], [UnitName], [IsDecimalAllowed]) VALUES (5, N'KGS', N'Kilograms', 1);
SET IDENTITY_INSERT [ItemUnits] OFF;
GO

-- 5.7 Standard Item Categories
SET IDENTITY_INSERT [ItemCategories] ON;
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 1)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (1, N'Hardware & Equipment', 1);
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 2)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (2, N'Cables & Networking', 1);
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 3)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (3, N'Office Consumables', 1);
IF NOT EXISTS (SELECT * FROM [ItemCategories] WHERE [Id] = 4)
    INSERT INTO [ItemCategories] ([Id], [CategoryName], [IsActive]) VALUES (4, N'Tools & Safety Gear', 1);
SET IDENTITY_INSERT [ItemCategories] OFF;
GO

-- 5.8 Standard Project Types
SET IDENTITY_INSERT [ProjectTypes] ON;
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 1)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (1, N'STANDARD', N'Standard Turnkey Contract', N'Fixed-price supply, installation, testing and commissioning contracts', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 2)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (2, N'AMC', N'Annual Maintenance Contract (AMC)', N'Ongoing operations, service level maintenance and support agreements', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 3)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (3, N'CONSULTING', N'Consulting & Advisory', N'Professional technical advisory, system design and project management', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 4)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (4, N'SUPPLY_INSTALL', N'Supply & Installation', N'Material delivery with onsite installation and sign-off', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 5)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (5, N'MANPOWER', N'Manpower & Managed Services', N'Time and material / rate card based deployment', 1);
IF NOT EXISTS (SELECT * FROM [ProjectTypes] WHERE [Id] = 6)
    INSERT INTO [ProjectTypes] ([Id], [Code], [Name], [Description], [IsActive]) VALUES (6, N'INTERNAL', N'Internal R&D / Capital Project', N'Internal organizational infrastructure or R&D initiatives', 1);
SET IDENTITY_INSERT [ProjectTypes] OFF;
GO

-- 5.9 Chart of Account Groups
SET IDENTITY_INSERT [AccountGroups] ON;
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 1)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (1, N'1000', N'Current Assets', N'ASSET', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 2)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (2, N'2000', N'Current Liabilities', N'LIABILITY', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 3)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (3, N'3000', N'Equity & Reserves', N'EQUITY', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 4)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (4, N'4000', N'Project Revenue', N'REVENUE', NULL);
IF NOT EXISTS (SELECT * FROM [AccountGroups] WHERE [Id] = 5)
    INSERT INTO [AccountGroups] ([Id], [GroupCode], [GroupName], [AccountCategory], [ParentGroupId]) VALUES (5, N'5000', N'Direct Expenses', N'EXPENSE', NULL);
SET IDENTITY_INSERT [AccountGroups] OFF;
GO

-- 5.10 Financial Year
SET IDENTITY_INSERT [FinancialYears] ON;
IF NOT EXISTS (SELECT * FROM [FinancialYears] WHERE [Id] = 1)
    INSERT INTO [FinancialYears] ([Id], [CompanyId], [FyCode], [StartDate], [EndDate], [IsClosed]) VALUES (1, 1, N'FY-2026-27', '2026-04-01', '2027-03-31', 0);
SET IDENTITY_INSERT [FinancialYears] OFF;
GO

-- 5.11 Demo Master Clients
SET IDENTITY_INSERT [master].[clients] ON;
IF NOT EXISTS (SELECT * FROM [master].[clients] WHERE [Id] = 1)
    INSERT INTO [master].[clients] (
        [Id], [CompanyId], [ClientCode], [ClientName], [ContactPerson], [Phone],
        [BillingAddress], [StateCode], [Gstin], [Pan], [CreditLimit], [CreditDays], [IsActive]
    ) VALUES (
        1, 1, N'CL-09101443', N'Finance Department', N'Bhawani Prasad Bhattarai', N'+917797606232',
        N'Test Headquarters, New Delhi', N'07', N'DJD5585DJF', N'CEFHFH6823K', 0.00, 30, 1
    );
SET IDENTITY_INSERT [master].[clients] OFF;
GO

-- 5.12 Demo Master Vendors
SET IDENTITY_INSERT [master].[vendors] ON;
IF NOT EXISTS (SELECT * FROM [master].[vendors] WHERE [Id] = 1)
    INSERT INTO [master].[vendors] (
        [Id], [CompanyId], [VendorCode], [VendorName], [VendorCategory],
        [Gstin], [Pan], [MsmeType], [PaymentTermsDays], [IsActive]
    ) VALUES (
        1, 1, N'VEN-09122210', N'TEJPAL AND SONS', N'HARDWARE',
        N'VVVVVVVVVVV', N'MNHDFUY14H', N'MEDIUM', 30, 1
    );
SET IDENTITY_INSERT [master].[vendors] OFF;
GO

-- 5.13 Demo Active Projects
SET IDENTITY_INSERT [project].[projects] ON;
IF NOT EXISTS (SELECT * FROM [project].[projects] WHERE [Id] = 1)
    INSERT INTO [project].[projects] (
        [Id], [CompanyId], [BranchId], [ClientId], [ProjectCode], [ProjectName], [ProjectType],
        [ManagerId], [ContractValue], [BudgetCost], [StartDate], [ExpectedEndDate], [Status],
        [ClosureNotes], [CreatedAt], [UpdatedAt]
    ) VALUES (
        1, 1, 1, 1, N'PRJ-2026-TEST', N'Metro Line Surveillance Network', N'STANDARD',
        1, 2500000.00, 1800000.00, '2026-09-10', '2027-03-10', N'ACTIVE',
        N'All deliverable testing and client handover completed.', '2026-09-10 09:25:42.431', '2026-09-23 18:26:50.816'
    );
SET IDENTITY_INSERT [project].[projects] OFF;
GO

-- 5.14 Demo Document Attachments
SET IDENTITY_INSERT [DocumentAttachments] ON;
IF NOT EXISTS (SELECT * FROM [DocumentAttachments] WHERE [Id] = 3)
    INSERT INTO [DocumentAttachments] (
        [Id], [EntityType], [EntityId], [FileName], [FilePath], [FileSizeBytes], [MimeType],
        [FileHashSha256], [UploadedBy], [UploadedAt], [VersionNumber], [UploaderId]
    ) VALUES (
        3, N'DeliveryChallan', 1, N'InvoiceSS_2025-26_5_INV (20).pdf',
        N'/uploads/tenants/org_1/deliveries/dc_1_639257848103623721_InvoiceSS_2025-26_5_INV (20).pdf',
        66625, N'application/pdf', N'8c77f80a335b4148a48f54316c1bfd3c', 1, '2026-09-23 18:26:50.521', 1, 1
    );
SET IDENTITY_INSERT [DocumentAttachments] OFF;
GO

-- 5.15 Demo Project Delivery Challans
SET IDENTITY_INSERT [ProjectDeliveries] ON;
IF NOT EXISTS (SELECT * FROM [ProjectDeliveries] WHERE [Id] = 1)
    INSERT INTO [ProjectDeliveries] (
        [Id], [ProjectId], [DcNumber], [DeliveryDate], [DispatchMode], [TrackingRefNo],
        [RecipientName], [Status], [MaterialSummary], [AttachmentDocId], [AttachmentId]
    ) VALUES (
        1, 1, N'DC-PRJ-2026-TEST-20260923-2354', '2026-09-23', N'Company Vehicle / Van', N'1555',
        N'Bhawani Prasad Bhattarai', N'DISPATCHED', N'Network Switches, Cat6 Drums, Mounting Racks', 3, 3
    );
SET IDENTITY_INSERT [ProjectDeliveries] OFF;
GO

-- 5.16 Demo Purchase Orders
SET IDENTITY_INSERT [PurchaseOrders] ON;
IF NOT EXISTS (SELECT * FROM [PurchaseOrders] WHERE [Id] = 1)
    INSERT INTO [PurchaseOrders] (
        [Id], [CompanyId], [BranchId], [VendorId], [ProjectId], [PoNumber], [PoDate],
        [TaxableAmount], [GstAmount], [TotalPoValue], [ApprovalStatus]
    ) VALUES (
        1, 1, 1, 1, 1, N'PO/26-09/221121', '2026-09-12',
        500000.00, 18000.00, 518000.00, N'APPROVED'
    );
SET IDENTITY_INSERT [PurchaseOrders] OFF;
GO

-- 5.17 Demo Sales Invoices
SET IDENTITY_INSERT [sales].[sales_invoices] ON;
IF NOT EXISTS (SELECT * FROM [sales].[sales_invoices] WHERE [Id] = 1)
    INSERT INTO [sales].[sales_invoices] (
        [Id], [CompanyId], [BranchId], [ClientId], [ProjectId], [InvoiceNumber], [InvoiceDate],
        [DueDate], [PlaceOfSupply], [IsReverseCharge], [TaxableAmount], [CgstAmount], [SgstAmount],
        [IgstAmount], [TotalInvoiceValue], [PaidAmount], [OutstandingBalance], [Status]
    ) VALUES (
        1, 1, 1, 1, 1, N'INV-2026-TEST-01', '2026-09-10',
        '2026-10-10', N'07', 0, 150000.00, 13500.00, 13500.00, 0.00, 177000.00, 0.00, 177000.00, N'SENT'
    );

IF NOT EXISTS (SELECT * FROM [sales].[sales_invoices] WHERE [Id] = 2)
    INSERT INTO [sales].[sales_invoices] (
        [Id], [CompanyId], [BranchId], [ClientId], [ProjectId], [InvoiceNumber], [InvoiceDate],
        [DueDate], [PlaceOfSupply], [IsReverseCharge], [TaxableAmount], [CgstAmount], [SgstAmount],
        [IgstAmount], [TotalInvoiceValue], [PaidAmount], [OutstandingBalance], [Status]
    ) VALUES (
        2, 1, 1, 1, 1, N'INV/26-09/215056', '2026-09-12',
        '2026-10-12', N'07', 0, 1000000.00, 90000.00, 90000.00, 0.00, 1180000.00, 0.00, 1180000.00, N'SENT'
    );
SET IDENTITY_INSERT [sales].[sales_invoices] OFF;
GO

-- 5.18 Genesis Audit Logs
IF NOT EXISTS (SELECT * FROM [AuditLogs] WHERE [TableName] = N'SYSTEM_INITIALIZATION')
BEGIN
    INSERT INTO [AuditLogs] ([TableName], [RecordId], [ActionType], [CurrentHash], [PreviousHash], [CreatedAt], [IpAddress], [UserAgent])
    VALUES (
        N'SYSTEM_INITIALIZATION', 0, N'GENESIS_BOOTSTRAP',
        N'a1b2c3d4e5f60718293a4b5c6d7e8f90a1b2c3d4e5f60718293a4b5c6d7e8f90',
        N'0000000000000000000000000000000000000000000000000000000000000000',
        GETUTCDATE(), N'127.0.0.1', N'SDK_ERP_BOOTSTRAP'
    );
END
GO

PRINT '=============================================================================';
PRINT '  SDK SOLUTIONS ERP COMPLETE DATABASE SETUP WITH DATA COMPLETED!             ';
PRINT '  - Database Name: SDK_ERP_DB                                               ';
PRINT '  - Organizations Seeded: SDK Solution, Vani ERP, Binary Solution           ';
PRINT '  - Admin Login 1: sabir  | Password: admin123 (or Admin@123)                ';
PRINT '  - Admin Login 2: admin  | Password: Admin@123 (or admin123)                ';
PRINT '  - User 3: kamal         | Password: admin123 (or Admin@123)                ';
PRINT '  - Demo Masters: Clients, Vendors, Projects, POs, Delivery Challans, Invoices';
PRINT '=============================================================================';
"""
    return header + schema + patches + seed

def main():
    print("Generating SDK_ERP_Database_Complete.sql...")
    complete_sql = generate_complete_with_data_script()
    with open("SDK_ERP_Database_Complete.sql", "w", encoding="utf-8") as f:
        f.write(complete_sql)
    print(f"Generated SDK_ERP_Database_Complete.sql ({len(complete_sql)} bytes)")

    print("Generating SDK_ERP_Database_Fresh_Install.sql...")
    fresh_sql = generate_fresh_install_script()
    with open("SDK_ERP_Database_Fresh_Install.sql", "w", encoding="utf-8") as f:
        f.write(fresh_sql)
    print(f"Generated SDK_ERP_Database_Fresh_Install.sql ({len(fresh_sql)} bytes)")

if __name__ == "__main__":
    main()
