-- ============================================================================
-- PROJECT: SDK SOLUTIONS ENTERPRISE ERP
-- FILE: SDK_ERP_Database_Fresh_Install.sql
-- DESCRIPTION: Clean production installation script with full schema, tables, indexes, constraints, and standard foundational seed data.
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

IF SCHEMA_ID(N'admin') IS NULL EXEC(N'CREATE SCHEMA [admin];');
GO

IF SCHEMA_ID(N'master') IS NULL EXEC(N'CREATE SCHEMA [master];');
GO

IF SCHEMA_ID(N'accounting') IS NULL EXEC(N'CREATE SCHEMA [accounting];');
GO

IF SCHEMA_ID(N'project') IS NULL EXEC(N'CREATE SCHEMA [project];');
GO

IF SCHEMA_ID(N'procurement') IS NULL EXEC(N'CREATE SCHEMA [procurement];');
GO

IF SCHEMA_ID(N'sales') IS NULL EXEC(N'CREATE SCHEMA [sales];');
GO

IF OBJECT_ID(N'[AccountGroups]', N'U') IS NULL
BEGIN
CREATE TABLE [AccountGroups] (
    [Id] int NOT NULL IDENTITY,
    [GroupCode] nvarchar(max) NOT NULL,
    [GroupName] nvarchar(max) NOT NULL,
    [AccountCategory] nvarchar(max) NOT NULL,
    [ParentGroupId] int NULL,
    CONSTRAINT [PK_AccountGroups] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AccountGroups_AccountGroups_ParentGroupId] FOREIGN KEY ([ParentGroupId]) REFERENCES [AccountGroups] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[admin].[companies]', N'U') IS NULL
BEGIN
CREATE TABLE [admin].[companies] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyCode] nvarchar(20) NOT NULL,
    [CompanyName] nvarchar(150) NOT NULL,
    [LegalName] nvarchar(150) NOT NULL,
    [Gstin] nvarchar(15) NULL,
    [Pan] nvarchar(10) NOT NULL,
    [Tan] nvarchar(10) NULL,
    [BaseCurrency] nvarchar(3) NOT NULL DEFAULT N'INR',
    [FinancialYearStart] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [LogoUrl] nvarchar(max) NULL,
    [BrandShortName] nvarchar(max) NULL,
    [Tagline] nvarchar(max) NULL,
    [Industry] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    [Website] nvarchar(max) NULL,
    [AddressLine1] nvarchar(max) NULL,
    [AddressLine2] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [State] nvarchar(max) NULL,
    [StateCode] nvarchar(max) NULL,
    [Pincode] nvarchar(max) NULL,
    [Cin] nvarchar(max) NULL,
    [MsmeUdyamNo] nvarchar(max) NULL,
    [BankName] nvarchar(max) NULL,
    [BankAccountNumber] nvarchar(max) NULL,
    [BankIfsc] nvarchar(max) NULL,
    [BankBranch] nvarchar(max) NULL,
    [UpiId] nvarchar(max) NULL,
    [AuthorizedSignatoryName] nvarchar(max) NULL,
    [AuthorizedSignatoryDesignation] nvarchar(max) NULL,
    [TermsAndConditions] nvarchar(max) NULL,
    CONSTRAINT [PK_companies] PRIMARY KEY ([Id])
);
END
GO

IF OBJECT_ID(N'[ItemCategories]', N'U') IS NULL
BEGIN
CREATE TABLE [ItemCategories] (
    [Id] int NOT NULL IDENTITY,
    [CategoryName] nvarchar(max) NOT NULL,
    [ParentCategoryId] int NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ItemCategories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ItemCategories_ItemCategories_ParentCategoryId] FOREIGN KEY ([ParentCategoryId]) REFERENCES [ItemCategories] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ItemUnits]', N'U') IS NULL
BEGIN
CREATE TABLE [ItemUnits] (
    [Id] int NOT NULL IDENTITY,
    [UnitCode] nvarchar(max) NOT NULL,
    [UnitName] nvarchar(max) NOT NULL,
    [IsDecimalAllowed] bit NOT NULL,
    CONSTRAINT [PK_ItemUnits] PRIMARY KEY ([Id])
);
END
GO

IF OBJECT_ID(N'[ProjectTypes]', N'U') IS NULL
BEGIN
CREATE TABLE [ProjectTypes] (
    [Id] bigint NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ProjectTypes] PRIMARY KEY ([Id])
);
END
GO

IF OBJECT_ID(N'[admin].[roles]', N'U') IS NULL
BEGIN
CREATE TABLE [admin].[roles] (
    [Id] int NOT NULL IDENTITY,
    [RoleName] nvarchar(50) NOT NULL,
    [Description] nvarchar(255) NULL,
    [IsSystemRole] bit NOT NULL,
    CONSTRAINT [PK_roles] PRIMARY KEY ([Id])
);
END
GO

IF OBJECT_ID(N'[TaxRates]', N'U') IS NULL
BEGIN
CREATE TABLE [TaxRates] (
    [Id] int NOT NULL IDENTITY,
    [TaxName] nvarchar(max) NOT NULL,
    [RatePercentage] decimal(18,2) NOT NULL,
    [CgstPercentage] decimal(18,2) NOT NULL,
    [SgstPercentage] decimal(18,2) NOT NULL,
    [IgstPercentage] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_TaxRates] PRIMARY KEY ([Id])
);
END
GO

IF OBJECT_ID(N'[admin].[branches]', N'U') IS NULL
BEGIN
CREATE TABLE [admin].[branches] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchCode] nvarchar(30) NOT NULL,
    [BranchName] nvarchar(100) NOT NULL,
    [StateCode] nvarchar(5) NOT NULL,
    [Gstin] nvarchar(15) NULL,
    [AddressLine1] nvarchar(255) NOT NULL,
    [IsHeadOffice] bit NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_branches] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_branches_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ChartOfAccounts]', N'U') IS NULL
BEGIN
CREATE TABLE [ChartOfAccounts] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [AccountCode] nvarchar(max) NOT NULL,
    [AccountName] nvarchar(max) NOT NULL,
    [GroupId] int NOT NULL,
    [OpeningBalance] decimal(18,2) NOT NULL,
    [OpeningBalanceType] nvarchar(max) NOT NULL,
    [CurrentBalance] decimal(18,2) NOT NULL,
    [IsSystemAccount] bit NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ChartOfAccounts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChartOfAccounts_AccountGroups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [AccountGroups] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ChartOfAccounts_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[master].[clients]', N'U') IS NULL
BEGIN
CREATE TABLE [master].[clients] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ClientCode] nvarchar(50) NOT NULL,
    [ClientName] nvarchar(200) NOT NULL,
    [ContactPerson] nvarchar(max) NULL,
    [Email] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    [BillingAddress] nvarchar(max) NOT NULL,
    [ShippingAddress] nvarchar(max) NULL,
    [StateCode] nvarchar(10) NOT NULL,
    [Gstin] nvarchar(30) NULL,
    [Pan] nvarchar(30) NULL,
    [CreditLimit] decimal(15,2) NOT NULL,
    [CreditDays] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_clients] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_clients_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[FinancialYears]', N'U') IS NULL
BEGIN
CREATE TABLE [FinancialYears] (
    [Id] int NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [FyCode] nvarchar(max) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsClosed] bit NOT NULL,
    [ClosedAt] datetime2 NULL,
    [ClosedBy] bigint NULL,
    CONSTRAINT [PK_FinancialYears] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FinancialYears_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PayrollRuns]', N'U') IS NULL
BEGIN
CREATE TABLE [PayrollRuns] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [MonthYear] nvarchar(max) NOT NULL,
    [TotalEmployeesProcessed] int NOT NULL,
    [TotalGrossSalary] decimal(18,2) NOT NULL,
    [TotalDeductions] decimal(18,2) NOT NULL,
    [TotalNetPayable] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [JournalEntryId] bigint NULL,
    CONSTRAINT [PK_PayrollRuns] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PayrollRuns_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[master].[vendors]', N'U') IS NULL
BEGIN
CREATE TABLE [master].[vendors] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [VendorCode] nvarchar(50) NOT NULL,
    [VendorName] nvarchar(200) NOT NULL,
    [VendorCategory] nvarchar(max) NOT NULL,
    [Gstin] nvarchar(30) NULL,
    [Pan] nvarchar(30) NOT NULL,
    [MsmeType] nvarchar(50) NULL,
    [BankName] nvarchar(max) NULL,
    [BankAccountNo] nvarchar(max) NULL,
    [IfscCode] nvarchar(30) NULL,
    [PaymentTermsDays] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_vendors] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_vendors_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ApprovalRules]', N'U') IS NULL
BEGIN
CREATE TABLE [ApprovalRules] (
    [Id] bigint NOT NULL IDENTITY,
    [ModuleCode] nvarchar(max) NOT NULL,
    [TransactionType] nvarchar(max) NOT NULL,
    [MinAmount] decimal(18,2) NOT NULL,
    [MaxAmount] decimal(18,2) NULL,
    [ApproverRoleId] int NOT NULL,
    [LevelOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_ApprovalRules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ApprovalRules_roles_ApproverRoleId] FOREIGN KEY ([ApproverRoleId]) REFERENCES [admin].[roles] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[RolePermissions]', N'U') IS NULL
BEGIN
CREATE TABLE [RolePermissions] (
    [Id] bigint NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ModuleCode] nvarchar(max) NOT NULL,
    [SubmoduleCode] nvarchar(max) NOT NULL,
    [CanView] bit NOT NULL,
    [CanCreate] bit NOT NULL,
    [CanEdit] bit NOT NULL,
    [CanDelete] bit NOT NULL,
    [CanApprove] bit NOT NULL,
    [CanPost] bit NOT NULL,
    [CanExport] bit NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RolePermissions_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [admin].[roles] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[HsnSacCodes]', N'U') IS NULL
BEGIN
CREATE TABLE [HsnSacCodes] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [DefaultTaxRateId] int NULL,
    CONSTRAINT [PK_HsnSacCodes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HsnSacCodes_TaxRates_DefaultTaxRateId] FOREIGN KEY ([DefaultTaxRateId]) REFERENCES [TaxRates] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[master].[items]', N'U') IS NULL
BEGIN
CREATE TABLE [master].[items] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ItemCode] nvarchar(30) NOT NULL,
    [ItemName] nvarchar(150) NOT NULL,
    [CategoryId] int NOT NULL,
    [UnitId] int NOT NULL,
    [HsnSacCode] nvarchar(10) NULL,
    [TaxRateId] int NOT NULL,
    [UnitCost] decimal(12,2) NOT NULL,
    [CurrentStockQty] decimal(10,2) NOT NULL,
    [ReorderLevelQty] decimal(10,2) NOT NULL,
    [IsDurableAsset] bit NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_items] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_items_ItemCategories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [ItemCategories] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_items_ItemUnits_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [ItemUnits] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_items_TaxRates_TaxRateId] FOREIGN KEY ([TaxRateId]) REFERENCES [TaxRates] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_items_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[Employees]', N'U') IS NULL
BEGIN
CREATE TABLE [Employees] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchId] bigint NOT NULL,
    [EmployeeCode] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [DepartmentName] nvarchar(max) NOT NULL,
    [DesignationTitle] nvarchar(max) NOT NULL,
    [ReportingManagerId] bigint NULL,
    [DateOfJoining] datetime2 NOT NULL,
    [Pan] nvarchar(max) NOT NULL,
    [Uan] nvarchar(max) NULL,
    [BankAccountNo] nvarchar(max) NOT NULL,
    [BankIfsc] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Employees_ReportingManagerId] FOREIGN KEY ([ReportingManagerId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [admin].[branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[admin].[users]', N'U') IS NULL
BEGIN
CREATE TABLE [admin].[users] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchId] bigint NOT NULL,
    [RoleId] int NOT NULL,
    [EmployeeId] bigint NULL,
    [Username] nvarchar(50) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(255) NOT NULL,
    [ReportingManagerId] bigint NULL,
    [IsActive] bit NOT NULL,
    [LastLoginAt] datetime2 NULL,
    [FullName] nvarchar(max) NULL,
    [Designation] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [AvatarUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_users_branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [admin].[branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_users_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_users_roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [admin].[roles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_users_users_ReportingManagerId] FOREIGN KEY ([ReportingManagerId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[BankAccounts]', N'U') IS NULL
BEGIN
CREATE TABLE [BankAccounts] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [GlAccountId] bigint NOT NULL,
    [BankName] nvarchar(max) NOT NULL,
    [BranchName] nvarchar(max) NULL,
    [AccountNumber] nvarchar(max) NOT NULL,
    [IfscCode] nvarchar(max) NOT NULL,
    [AccountType] nvarchar(max) NOT NULL,
    [BookBalance] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_BankAccounts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BankAccounts_ChartOfAccounts_GlAccountId] FOREIGN KEY ([GlAccountId]) REFERENCES [ChartOfAccounts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BankAccounts_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[CustomerReceipts]', N'U') IS NULL
BEGIN
CREATE TABLE [CustomerReceipts] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ClientId] bigint NOT NULL,
    [BankAccountId] bigint NOT NULL,
    [ReceiptNumber] nvarchar(max) NOT NULL,
    [ReceiptDate] datetime2 NOT NULL,
    [AmountReceived] decimal(18,2) NOT NULL,
    [UnallocatedAmount] decimal(18,2) NOT NULL,
    [PaymentMode] nvarchar(max) NOT NULL,
    [TransactionRefNo] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [JournalEntryId] bigint NULL,
    CONSTRAINT [PK_CustomerReceipts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CustomerReceipts_clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [master].[clients] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CustomerReceipts_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[VendorPayments]', N'U') IS NULL
BEGIN
CREATE TABLE [VendorPayments] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [VendorId] bigint NOT NULL,
    [BankAccountId] bigint NOT NULL,
    [PaymentVoucherNo] nvarchar(max) NOT NULL,
    [PaymentDate] datetime2 NOT NULL,
    [TotalAmountPaid] decimal(18,2) NOT NULL,
    [PaymentMode] nvarchar(max) NOT NULL,
    [ChequeUtrNo] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [JournalEntryId] bigint NULL,
    CONSTRAINT [PK_VendorPayments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VendorPayments_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_VendorPayments_vendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [master].[vendors] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[StockTransactions]', N'U') IS NULL
BEGIN
CREATE TABLE [StockTransactions] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ItemId] bigint NOT NULL,
    [TransactionDate] datetime2 NOT NULL,
    [TransactionType] nvarchar(max) NOT NULL,
    [ReferenceType] nvarchar(max) NOT NULL,
    [ReferenceId] bigint NOT NULL,
    [Quantity] decimal(18,2) NOT NULL,
    [UnitCost] decimal(18,2) NOT NULL,
    [BalanceQtyAfter] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_StockTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StockTransactions_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StockTransactions_items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [master].[items] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[EmployeeAdvances]', N'U') IS NULL
BEGIN
CREATE TABLE [EmployeeAdvances] (
    [Id] bigint NOT NULL IDENTITY,
    [EmployeeId] bigint NOT NULL,
    [AdvanceAmount] decimal(18,2) NOT NULL,
    [DisbursementDate] datetime2 NOT NULL,
    [MonthlyRecoveryAmount] decimal(18,2) NOT NULL,
    [RecoveredAmount] decimal(18,2) NOT NULL,
    [BalanceDue] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_EmployeeAdvances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeAdvances_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PayrollItems]', N'U') IS NULL
BEGIN
CREATE TABLE [PayrollItems] (
    [Id] bigint NOT NULL IDENTITY,
    [PayrollRunId] bigint NOT NULL,
    [EmployeeId] bigint NOT NULL,
    [WorkingDaysInMonth] int NOT NULL,
    [DaysPayable] decimal(18,2) NOT NULL,
    [GrossEarned] decimal(18,2) NOT NULL,
    [PfDeduction] decimal(18,2) NOT NULL,
    [PtDeduction] decimal(18,2) NOT NULL,
    [TdsDeduction] decimal(18,2) NOT NULL,
    [AdvanceDeduction] decimal(18,2) NOT NULL,
    [NetSalary] decimal(18,2) NOT NULL,
    [PaymentStatus] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PayrollItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PayrollItems_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PayrollItems_PayrollRuns_PayrollRunId] FOREIGN KEY ([PayrollRunId]) REFERENCES [PayrollRuns] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[SalaryStructures]', N'U') IS NULL
BEGIN
CREATE TABLE [SalaryStructures] (
    [Id] bigint NOT NULL IDENTITY,
    [EmployeeId] bigint NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [CtcAnnual] decimal(18,2) NOT NULL,
    [GrossMonthly] decimal(18,2) NOT NULL,
    [BasicPay] decimal(18,2) NOT NULL,
    [Hra] decimal(18,2) NOT NULL,
    [SpecialAllowance] decimal(18,2) NOT NULL,
    [PfEmployee] decimal(18,2) NOT NULL,
    [ProfessionalTax] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_SalaryStructures] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SalaryStructures_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ApprovalRequests]', N'U') IS NULL
BEGIN
CREATE TABLE [ApprovalRequests] (
    [Id] bigint NOT NULL IDENTITY,
    [RuleId] bigint NOT NULL,
    [EntityType] nvarchar(max) NOT NULL,
    [EntityId] bigint NOT NULL,
    [RequestedBy] bigint NOT NULL,
    [AssignedToRoleId] int NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [ActionBy] bigint NULL,
    [ActionAt] datetime2 NULL,
    [Comments] nvarchar(max) NULL,
    [RequesterId] bigint NOT NULL,
    [AssignedRoleId] int NOT NULL,
    [ActionUserId] bigint NULL,
    CONSTRAINT [PK_ApprovalRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ApprovalRequests_ApprovalRules_RuleId] FOREIGN KEY ([RuleId]) REFERENCES [ApprovalRules] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalRequests_roles_AssignedRoleId] FOREIGN KEY ([AssignedRoleId]) REFERENCES [admin].[roles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalRequests_users_ActionUserId] FOREIGN KEY ([ActionUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ApprovalRequests_users_RequesterId] FOREIGN KEY ([RequesterId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[AuditLogs]', N'U') IS NULL
BEGIN
CREATE TABLE [AuditLogs] (
    [Id] bigint NOT NULL IDENTITY,
    [UserId] bigint NULL,
    [TableName] nvarchar(max) NOT NULL,
    [RecordId] bigint NOT NULL,
    [ActionType] nvarchar(max) NOT NULL,
    [OldPayload] nvarchar(max) NULL,
    [NewPayload] nvarchar(max) NULL,
    [IpAddress] nvarchar(max) NULL,
    [UserAgent] nvarchar(max) NULL,
    [PreviousHash] nvarchar(max) NULL,
    [CurrentHash] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuditLogs_users_UserId] FOREIGN KEY ([UserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[DocumentAttachments]', N'U') IS NULL
BEGIN
CREATE TABLE [DocumentAttachments] (
    [Id] bigint NOT NULL IDENTITY,
    [EntityType] nvarchar(max) NOT NULL,
    [EntityId] bigint NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [FilePath] nvarchar(max) NOT NULL,
    [FileSizeBytes] bigint NOT NULL,
    [MimeType] nvarchar(max) NOT NULL,
    [FileHashSha256] nvarchar(max) NOT NULL,
    [UploadedBy] bigint NULL,
    [UploaderId] bigint NULL,
    [UploadedAt] datetime2 NOT NULL,
    [VersionNumber] int NOT NULL,
    CONSTRAINT [PK_DocumentAttachments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DocumentAttachments_users_UploaderId] FOREIGN KEY ([UploaderId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[LeaveApplications]', N'U') IS NULL
BEGIN
CREATE TABLE [LeaveApplications] (
    [Id] bigint NOT NULL IDENTITY,
    [EmployeeId] bigint NOT NULL,
    [LeaveType] nvarchar(max) NOT NULL,
    [FromDate] datetime2 NOT NULL,
    [ToDate] datetime2 NOT NULL,
    [TotalDays] decimal(18,2) NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [ApprovedBy] bigint NULL,
    [ApproverId] bigint NULL,
    CONSTRAINT [PK_LeaveApplications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LeaveApplications_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_LeaveApplications_users_ApproverId] FOREIGN KEY ([ApproverId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[project].[projects]', N'U') IS NULL
BEGIN
CREATE TABLE [project].[projects] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchId] bigint NOT NULL,
    [ClientId] bigint NOT NULL,
    [ProjectCode] nvarchar(50) NOT NULL,
    [ProjectName] nvarchar(200) NOT NULL,
    [ProjectType] nvarchar(max) NOT NULL,
    [ManagerId] bigint NULL,
    [ContractValue] decimal(15,2) NOT NULL,
    [BudgetCost] decimal(15,2) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [ExpectedEndDate] datetime2 NULL,
    [ActualClosedDate] datetime2 NULL,
    [Status] nvarchar(30) NOT NULL,
    [ClosureApprovedBy] bigint NULL,
    [ClosureNotes] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [ClosureApproverId] bigint NULL,
    CONSTRAINT [PK_projects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_projects_branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [admin].[branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_projects_clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [master].[clients] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_projects_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_projects_users_ClosureApproverId] FOREIGN KEY ([ClosureApproverId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_projects_users_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[StockIssues]', N'U') IS NULL
BEGIN
CREATE TABLE [StockIssues] (
    [Id] bigint NOT NULL IDENTITY,
    [IssueVoucherNo] nvarchar(max) NOT NULL,
    [IssueDate] datetime2 NOT NULL,
    [ItemId] bigint NOT NULL,
    [QuantityIssued] decimal(18,2) NOT NULL,
    [IssuedToEmployeeId] bigint NULL,
    [DepartmentId] int NULL,
    [PurposeRemarks] nvarchar(max) NOT NULL,
    [ApprovedBy] bigint NOT NULL,
    [ApproverId] bigint NOT NULL,
    CONSTRAINT [PK_StockIssues] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StockIssues_items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [master].[items] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StockIssues_users_ApproverId] FOREIGN KEY ([ApproverId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[SupportTickets]', N'U') IS NULL
BEGIN
CREATE TABLE [SupportTickets] (
    [Id] bigint NOT NULL IDENTITY,
    [TicketNumber] nvarchar(max) NOT NULL,
    [RaisedByUserId] bigint NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [Priority] nvarchar(max) NOT NULL,
    [Subject] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [AssignedToUserId] bigint NULL,
    [Status] nvarchar(max) NOT NULL,
    [ResolutionNotes] nvarchar(max) NULL,
    [ResolvedAt] datetime2 NULL,
    CONSTRAINT [PK_SupportTickets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SupportTickets_users_AssignedToUserId] FOREIGN KEY ([AssignedToUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SupportTickets_users_RaisedByUserId] FOREIGN KEY ([RaisedByUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[BankReconciliations]', N'U') IS NULL
BEGIN
CREATE TABLE [BankReconciliations] (
    [Id] bigint NOT NULL IDENTITY,
    [BankAccountId] bigint NOT NULL,
    [StatementDate] datetime2 NOT NULL,
    [ClosingBookBalance] decimal(18,2) NOT NULL,
    [StatementBalance] decimal(18,2) NOT NULL,
    [UnreconciledDifference] decimal(18,2) NOT NULL,
    [IsReconciled] bit NOT NULL,
    [ReconciledBy] bigint NOT NULL,
    [ReconcilerId] bigint NOT NULL,
    CONSTRAINT [PK_BankReconciliations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BankReconciliations_BankAccounts_BankAccountId] FOREIGN KEY ([BankAccountId]) REFERENCES [BankAccounts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BankReconciliations_users_ReconcilerId] FOREIGN KEY ([ReconcilerId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[accounting].[journal_entries]', N'U') IS NULL
BEGIN
CREATE TABLE [accounting].[journal_entries] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchId] bigint NOT NULL,
    [FyId] int NOT NULL,
    [VoucherNo] nvarchar(50) NOT NULL,
    [VoucherDate] datetime2 NOT NULL,
    [VoucherType] nvarchar(30) NOT NULL,
    [SourceEntityType] nvarchar(max) NULL,
    [SourceEntityId] bigint NULL,
    [ProjectId] bigint NULL,
    [Narration] nvarchar(max) NOT NULL,
    [TotalDebit] decimal(15,2) NOT NULL,
    [TotalCredit] decimal(15,2) NOT NULL,
    [IsBalanced] bit NOT NULL,
    [IsReversal] bit NOT NULL,
    [CreatedBy] bigint NOT NULL,
    CONSTRAINT [PK_journal_entries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_journal_entries_FinancialYears_FyId] FOREIGN KEY ([FyId]) REFERENCES [FinancialYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_journal_entries_branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [admin].[branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_journal_entries_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_journal_entries_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_journal_entries_users_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PettyCashTransactions]', N'U') IS NULL
BEGIN
CREATE TABLE [PettyCashTransactions] (
    [Id] bigint NOT NULL IDENTITY,
    [BankAccountId] bigint NOT NULL,
    [CustodianUserId] bigint NOT NULL,
    [EntryDate] datetime2 NOT NULL,
    [ExpenseAccountId] bigint NOT NULL,
    [ProjectId] bigint NULL,
    [Amount] decimal(18,2) NOT NULL,
    [PaidTo] nvarchar(max) NOT NULL,
    [ReceiptDocId] bigint NULL,
    [ApprovalStatus] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PettyCashTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PettyCashTransactions_BankAccounts_BankAccountId] FOREIGN KEY ([BankAccountId]) REFERENCES [BankAccounts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PettyCashTransactions_ChartOfAccounts_ExpenseAccountId] FOREIGN KEY ([ExpenseAccountId]) REFERENCES [ChartOfAccounts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PettyCashTransactions_DocumentAttachments_ReceiptDocId] FOREIGN KEY ([ReceiptDocId]) REFERENCES [DocumentAttachments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PettyCashTransactions_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PettyCashTransactions_users_CustodianUserId] FOREIGN KEY ([CustodianUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ProjectDeliveries]', N'U') IS NULL
BEGIN
CREATE TABLE [ProjectDeliveries] (
    [Id] bigint NOT NULL IDENTITY,
    [ProjectId] bigint NOT NULL,
    [DcNumber] nvarchar(max) NOT NULL,
    [DeliveryDate] datetime2 NOT NULL,
    [DispatchMode] nvarchar(max) NULL,
    [TrackingRefNo] nvarchar(max) NULL,
    [RecipientName] nvarchar(max) NULL,
    [Status] nvarchar(max) NOT NULL,
    [MaterialSummary] nvarchar(max) NULL,
    [AttachmentDocId] bigint NULL,
    [AttachmentId] bigint NULL,
    CONSTRAINT [PK_ProjectDeliveries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectDeliveries_DocumentAttachments_AttachmentDocId] FOREIGN KEY ([AttachmentDocId]) REFERENCES [DocumentAttachments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProjectDeliveries_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ProjectExpenses]', N'U') IS NULL
BEGIN
CREATE TABLE [ProjectExpenses] (
    [Id] bigint NOT NULL IDENTITY,
    [ProjectId] bigint NOT NULL,
    [ExpenseHeadId] bigint NOT NULL,
    [IncurredByUserId] bigint NOT NULL,
    [ExpenseDate] datetime2 NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [TaxableAmount] decimal(18,2) NOT NULL,
    [GstAmount] decimal(18,2) NOT NULL,
    [PaymentMode] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [ApprovedBy] bigint NULL,
    [ReceiptDocId] bigint NULL,
    [Description] nvarchar(max) NULL,
    [ApproverId] bigint NULL,
    CONSTRAINT [PK_ProjectExpenses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectExpenses_DocumentAttachments_ReceiptDocId] FOREIGN KEY ([ReceiptDocId]) REFERENCES [DocumentAttachments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProjectExpenses_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProjectExpenses_users_ApproverId] FOREIGN KEY ([ApproverId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProjectExpenses_users_IncurredByUserId] FOREIGN KEY ([IncurredByUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ProjectMilestones]', N'U') IS NULL
BEGIN
CREATE TABLE [ProjectMilestones] (
    [Id] bigint NOT NULL IDENTITY,
    [ProjectId] bigint NOT NULL,
    [MilestoneName] nvarchar(max) NOT NULL,
    [ExpectedDate] datetime2 NOT NULL,
    [MilestoneAmount] decimal(18,2) NOT NULL,
    [PercentageOfContract] decimal(18,2) NULL,
    [Status] nvarchar(max) NOT NULL,
    [CompletionDate] datetime2 NULL,
    CONSTRAINT [PK_ProjectMilestones] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectMilestones_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ProjectPos]', N'U') IS NULL
BEGIN
CREATE TABLE [ProjectPos] (
    [Id] bigint NOT NULL IDENTITY,
    [ProjectId] bigint NOT NULL,
    [ClientPoNumber] nvarchar(max) NOT NULL,
    [PoDate] datetime2 NOT NULL,
    [PoValue] decimal(18,2) NOT NULL,
    [ValidityEndDate] datetime2 NULL,
    [ScopeOfWork] nvarchar(max) NULL,
    [AttachmentDocId] bigint NULL,
    [AttachmentId] bigint NULL,
    CONSTRAINT [PK_ProjectPos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ProjectPos_DocumentAttachments_AttachmentId] FOREIGN KEY ([AttachmentId]) REFERENCES [DocumentAttachments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ProjectPos_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PurchaseRequisitions]', N'U') IS NULL
BEGIN
CREATE TABLE [PurchaseRequisitions] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [PrNumber] nvarchar(max) NOT NULL,
    [RequestedByUserId] bigint NOT NULL,
    [DepartmentId] int NULL,
    [ProjectId] bigint NULL,
    [PurposeType] nvarchar(max) NOT NULL,
    [RequiredByDate] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_PurchaseRequisitions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseRequisitions_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseRequisitions_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseRequisitions_users_RequestedByUserId] FOREIGN KEY ([RequestedByUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[Quotations]', N'U') IS NULL
BEGIN
CREATE TABLE [Quotations] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ClientId] bigint NOT NULL,
    [ProjectId] bigint NULL,
    [QuoteNumber] nvarchar(max) NOT NULL,
    [QuoteDate] datetime2 NOT NULL,
    [ValidityDate] datetime2 NOT NULL,
    [SubtotalAmount] decimal(18,2) NOT NULL,
    [DiscountAmount] decimal(18,2) NOT NULL,
    [TaxAmount] decimal(18,2) NOT NULL,
    [GrandTotal] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Quotations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Quotations_clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [master].[clients] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Quotations_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Quotations_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[BankTransactions]', N'U') IS NULL
BEGIN
CREATE TABLE [BankTransactions] (
    [Id] bigint NOT NULL IDENTITY,
    [BankAccountId] bigint NOT NULL,
    [VoucherId] bigint NULL,
    [TransactionDate] datetime2 NOT NULL,
    [ValueDate] datetime2 NULL,
    [TransactionType] nvarchar(max) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [ReferenceNumber] nvarchar(max) NULL,
    [IsReconciled] bit NOT NULL,
    CONSTRAINT [PK_BankTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BankTransactions_BankAccounts_BankAccountId] FOREIGN KEY ([BankAccountId]) REFERENCES [BankAccounts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BankTransactions_journal_entries_VoucherId] FOREIGN KEY ([VoucherId]) REFERENCES [accounting].[journal_entries] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[GstTransactions]', N'U') IS NULL
BEGIN
CREATE TABLE [GstTransactions] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [VoucherId] bigint NULL,
    [TransactionType] nvarchar(max) NOT NULL,
    [PartyGstin] nvarchar(max) NULL,
    [PlaceOfSupply] nvarchar(max) NOT NULL,
    [HsnSacCode] nvarchar(max) NULL,
    [TaxableValue] decimal(18,2) NOT NULL,
    [TaxRatePercentage] decimal(18,2) NOT NULL,
    [CgstAmount] decimal(18,2) NOT NULL,
    [SgstAmount] decimal(18,2) NOT NULL,
    [IgstAmount] decimal(18,2) NOT NULL,
    [ReturnPeriod] nvarchar(max) NOT NULL,
    [IsFiled] bit NOT NULL,
    CONSTRAINT [PK_GstTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GstTransactions_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GstTransactions_journal_entries_VoucherId] FOREIGN KEY ([VoucherId]) REFERENCES [accounting].[journal_entries] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[accounting].[journal_lines]', N'U') IS NULL
BEGIN
CREATE TABLE [accounting].[journal_lines] (
    [Id] bigint NOT NULL IDENTITY,
    [JournalEntryId] bigint NOT NULL,
    [AccountId] bigint NOT NULL,
    [DebitAmount] decimal(15,2) NOT NULL,
    [CreditAmount] decimal(15,2) NOT NULL,
    [ClientId] bigint NULL,
    [VendorId] bigint NULL,
    [LineNarration] nvarchar(max) NULL,
    CONSTRAINT [PK_journal_lines] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_journal_lines_ChartOfAccounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [ChartOfAccounts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_journal_lines_clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [master].[clients] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_journal_lines_journal_entries_JournalEntryId] FOREIGN KEY ([JournalEntryId]) REFERENCES [accounting].[journal_entries] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_journal_lines_vendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [master].[vendors] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PurchaseOrders]', N'U') IS NULL
BEGIN
CREATE TABLE [PurchaseOrders] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchId] bigint NOT NULL,
    [VendorId] bigint NOT NULL,
    [ProjectId] bigint NULL,
    [PrId] bigint NULL,
    [PoNumber] nvarchar(max) NOT NULL,
    [PoDate] datetime2 NOT NULL,
    [DeliveryDueDate] datetime2 NULL,
    [TaxableAmount] decimal(18,2) NOT NULL,
    [GstAmount] decimal(18,2) NOT NULL,
    [TotalPoValue] decimal(18,2) NOT NULL,
    [ApprovalStatus] nvarchar(max) NOT NULL,
    [ApprovedBy] bigint NULL,
    [RequisitionId] bigint NULL,
    [ApproverId] bigint NULL,
    CONSTRAINT [PK_PurchaseOrders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseOrders_PurchaseRequisitions_RequisitionId] FOREIGN KEY ([RequisitionId]) REFERENCES [PurchaseRequisitions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrders_branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [admin].[branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrders_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrders_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrders_users_ApproverId] FOREIGN KEY ([ApproverId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrders_vendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [master].[vendors] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PurchaseRequisitionItems]', N'U') IS NULL
BEGIN
CREATE TABLE [PurchaseRequisitionItems] (
    [Id] bigint NOT NULL IDENTITY,
    [PrId] bigint NOT NULL,
    [ItemId] bigint NULL,
    [ItemDescription] nvarchar(max) NOT NULL,
    [RequestedQty] decimal(18,2) NOT NULL,
    [UnitId] int NOT NULL,
    [EstimatedCost] decimal(18,2) NULL,
    [RequisitionId] bigint NOT NULL,
    CONSTRAINT [PK_PurchaseRequisitionItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseRequisitionItems_ItemUnits_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [ItemUnits] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseRequisitionItems_PurchaseRequisitions_RequisitionId] FOREIGN KEY ([RequisitionId]) REFERENCES [PurchaseRequisitions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseRequisitionItems_items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [master].[items] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[SalesOrders]', N'U') IS NULL
BEGIN
CREATE TABLE [SalesOrders] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [ClientId] bigint NOT NULL,
    [ProjectId] bigint NOT NULL,
    [QuotationId] bigint NULL,
    [SoNumber] nvarchar(max) NOT NULL,
    [SoDate] datetime2 NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_SalesOrders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SalesOrders_Quotations_QuotationId] FOREIGN KEY ([QuotationId]) REFERENCES [Quotations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SalesOrders_clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [master].[clients] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SalesOrders_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SalesOrders_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[GoodsReceiptNotes]', N'U') IS NULL
BEGIN
CREATE TABLE [GoodsReceiptNotes] (
    [Id] bigint NOT NULL IDENTITY,
    [PoId] bigint NOT NULL,
    [GrnNumber] nvarchar(max) NOT NULL,
    [ReceiptDate] datetime2 NOT NULL,
    [VendorDcNumber] nvarchar(max) NULL,
    [ReceivedByUserId] bigint NOT NULL,
    [InspectionStatus] nvarchar(max) NOT NULL,
    [PurchaseOrderId] bigint NOT NULL,
    CONSTRAINT [PK_GoodsReceiptNotes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GoodsReceiptNotes_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GoodsReceiptNotes_users_ReceivedByUserId] FOREIGN KEY ([ReceivedByUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[procurement].[purchase_bills]', N'U') IS NULL
BEGIN
CREATE TABLE [procurement].[purchase_bills] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [VendorId] bigint NOT NULL,
    [PoId] bigint NULL,
    [ProjectId] bigint NULL,
    [VendorBillNumber] nvarchar(50) NOT NULL,
    [BillDate] datetime2 NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [PlaceOfSupply] nvarchar(5) NOT NULL,
    [TaxableAmount] decimal(15,2) NOT NULL,
    [CgstAmount] decimal(15,2) NOT NULL,
    [SgstAmount] decimal(15,2) NOT NULL,
    [IgstAmount] decimal(15,2) NOT NULL,
    [TotalBillAmount] decimal(15,2) NOT NULL,
    [PaidAmount] decimal(15,2) NOT NULL,
    [BalanceDue] decimal(15,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [JournalEntryId] bigint NULL,
    [PurchaseOrderId] bigint NULL,
    CONSTRAINT [PK_purchase_bills] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_purchase_bills_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_purchase_bills_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_purchase_bills_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_purchase_bills_vendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [master].[vendors] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PurchaseOrderItems]', N'U') IS NULL
BEGIN
CREATE TABLE [PurchaseOrderItems] (
    [Id] bigint NOT NULL IDENTITY,
    [PoId] bigint NOT NULL,
    [ItemId] bigint NULL,
    [ItemDescription] nvarchar(max) NOT NULL,
    [HsnSacCode] nvarchar(max) NULL,
    [OrderedQty] decimal(18,2) NOT NULL,
    [ReceivedQty] decimal(18,2) NOT NULL,
    [UnitId] int NOT NULL,
    [UnitRate] decimal(18,2) NOT NULL,
    [TaxRateId] int NOT NULL,
    [LineTotal] decimal(18,2) NOT NULL,
    [PurchaseOrderId] bigint NOT NULL,
    CONSTRAINT [PK_PurchaseOrderItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseOrderItems_ItemUnits_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [ItemUnits] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId] FOREIGN KEY ([PurchaseOrderId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrderItems_TaxRates_TaxRateId] FOREIGN KEY ([TaxRateId]) REFERENCES [TaxRates] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseOrderItems_items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [master].[items] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[sales].[sales_invoices]', N'U') IS NULL
BEGIN
CREATE TABLE [sales].[sales_invoices] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchId] bigint NOT NULL,
    [ClientId] bigint NOT NULL,
    [ProjectId] bigint NOT NULL,
    [SalesOrderId] bigint NULL,
    [InvoiceNumber] nvarchar(50) NOT NULL,
    [InvoiceDate] datetime2 NOT NULL,
    [DueDate] datetime2 NOT NULL,
    [PlaceOfSupply] nvarchar(5) NOT NULL,
    [IsReverseCharge] bit NOT NULL,
    [TaxableAmount] decimal(15,2) NOT NULL,
    [CgstAmount] decimal(15,2) NOT NULL,
    [SgstAmount] decimal(15,2) NOT NULL,
    [IgstAmount] decimal(15,2) NOT NULL,
    [TotalInvoiceValue] decimal(15,2) NOT NULL,
    [PaidAmount] decimal(15,2) NOT NULL,
    [OutstandingBalance] decimal(15,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [IrnNumber] nvarchar(max) NULL,
    [QrCodePayload] nvarchar(max) NULL,
    [JournalEntryId] bigint NULL,
    CONSTRAINT [PK_sales_invoices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_sales_invoices_SalesOrders_SalesOrderId] FOREIGN KEY ([SalesOrderId]) REFERENCES [SalesOrders] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_sales_invoices_branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [admin].[branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_sales_invoices_clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [master].[clients] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_sales_invoices_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_sales_invoices_projects_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [project].[projects] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[FixedAssets]', N'U') IS NULL
BEGIN
CREATE TABLE [FixedAssets] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [BranchId] bigint NOT NULL,
    [AssetTagCode] nvarchar(max) NOT NULL,
    [AssetName] nvarchar(max) NOT NULL,
    [Category] nvarchar(max) NOT NULL,
    [SerialNumber] nvarchar(max) NULL,
    [PurchaseBillId] bigint NULL,
    [PurchaseDate] datetime2 NOT NULL,
    [PurchaseCost] decimal(18,2) NOT NULL,
    [CurrentBookValue] decimal(18,2) NOT NULL,
    [DepreciationRate] decimal(18,2) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_FixedAssets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FixedAssets_branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [admin].[branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FixedAssets_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FixedAssets_purchase_bills_PurchaseBillId] FOREIGN KEY ([PurchaseBillId]) REFERENCES [procurement].[purchase_bills] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[GstReconciliations]', N'U') IS NULL
BEGIN
CREATE TABLE [GstReconciliations] (
    [Id] bigint NOT NULL IDENTITY,
    [FinancialPeriod] nvarchar(max) NOT NULL,
    [PortalGstr2bInvoiceNo] nvarchar(max) NOT NULL,
    [PortalVendorGstin] nvarchar(max) NOT NULL,
    [PortalTaxableValue] decimal(18,2) NOT NULL,
    [PortalTaxAmount] decimal(18,2) NOT NULL,
    [InternalPurchaseBillId] bigint NULL,
    [InternalTaxableValue] decimal(18,2) NULL,
    [MatchStatus] nvarchar(max) NOT NULL,
    [ItcEligibility] nvarchar(max) NOT NULL,
    [ReconciledBy] bigint NULL,
    [ReconcilerId] bigint NULL,
    CONSTRAINT [PK_GstReconciliations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GstReconciliations_purchase_bills_InternalPurchaseBillId] FOREIGN KEY ([InternalPurchaseBillId]) REFERENCES [procurement].[purchase_bills] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GstReconciliations_users_ReconcilerId] FOREIGN KEY ([ReconcilerId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[PurchaseBillItems]', N'U') IS NULL
BEGIN
CREATE TABLE [PurchaseBillItems] (
    [Id] bigint NOT NULL IDENTITY,
    [PurchaseBillId] bigint NOT NULL,
    [ExpenseAccountId] bigint NOT NULL,
    [ItemId] bigint NULL,
    [ItemDescription] nvarchar(max) NOT NULL,
    [HsnSacCode] nvarchar(max) NULL,
    [TaxableAmount] decimal(18,2) NOT NULL,
    [TaxRateId] int NOT NULL,
    [GstAmount] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_PurchaseBillItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PurchaseBillItems_TaxRates_TaxRateId] FOREIGN KEY ([TaxRateId]) REFERENCES [TaxRates] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseBillItems_items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [master].[items] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PurchaseBillItems_purchase_bills_PurchaseBillId] FOREIGN KEY ([PurchaseBillId]) REFERENCES [procurement].[purchase_bills] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[VendorPaymentAllocations]', N'U') IS NULL
BEGIN
CREATE TABLE [VendorPaymentAllocations] (
    [Id] bigint NOT NULL IDENTITY,
    [VendorPaymentId] bigint NOT NULL,
    [PurchaseBillId] bigint NOT NULL,
    [AllocatedAmount] decimal(18,2) NOT NULL,
    [TdsDeducted] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_VendorPaymentAllocations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VendorPaymentAllocations_VendorPayments_VendorPaymentId] FOREIGN KEY ([VendorPaymentId]) REFERENCES [VendorPayments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_VendorPaymentAllocations_purchase_bills_PurchaseBillId] FOREIGN KEY ([PurchaseBillId]) REFERENCES [procurement].[purchase_bills] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[GrnItems]', N'U') IS NULL
BEGIN
CREATE TABLE [GrnItems] (
    [Id] bigint NOT NULL IDENTITY,
    [GrnId] bigint NOT NULL,
    [PoItemId] bigint NOT NULL,
    [ItemId] bigint NULL,
    [ReceivedQty] decimal(18,2) NOT NULL,
    [AcceptedQty] decimal(18,2) NOT NULL,
    [RejectedQty] decimal(18,2) NOT NULL,
    [RejectionReason] nvarchar(max) NULL,
    [GoodsReceiptNoteId] bigint NOT NULL,
    CONSTRAINT [PK_GrnItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GrnItems_GoodsReceiptNotes_GoodsReceiptNoteId] FOREIGN KEY ([GoodsReceiptNoteId]) REFERENCES [GoodsReceiptNotes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GrnItems_PurchaseOrderItems_PoItemId] FOREIGN KEY ([PoItemId]) REFERENCES [PurchaseOrderItems] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_GrnItems_items_ItemId] FOREIGN KEY ([ItemId]) REFERENCES [master].[items] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[CreditDebitNotes]', N'U') IS NULL
BEGIN
CREATE TABLE [CreditDebitNotes] (
    [Id] bigint NOT NULL IDENTITY,
    [CompanyId] bigint NOT NULL,
    [NoteType] nvarchar(max) NOT NULL,
    [PartyType] nvarchar(max) NOT NULL,
    [ClientId] bigint NULL,
    [VendorId] bigint NULL,
    [OriginalInvoiceId] bigint NULL,
    [OriginalPurchaseBillId] bigint NULL,
    [NoteNumber] nvarchar(max) NOT NULL,
    [NoteDate] datetime2 NOT NULL,
    [TaxableAmount] decimal(18,2) NOT NULL,
    [GstAmount] decimal(18,2) NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [Reason] nvarchar(max) NOT NULL,
    [JournalEntryId] bigint NULL,
    CONSTRAINT [PK_CreditDebitNotes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CreditDebitNotes_clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [master].[clients] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CreditDebitNotes_companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [admin].[companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CreditDebitNotes_sales_invoices_OriginalInvoiceId] FOREIGN KEY ([OriginalInvoiceId]) REFERENCES [sales].[sales_invoices] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CreditDebitNotes_vendors_VendorId] FOREIGN KEY ([VendorId]) REFERENCES [master].[vendors] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[ReceiptAllocations]', N'U') IS NULL
BEGIN
CREATE TABLE [ReceiptAllocations] (
    [Id] bigint NOT NULL IDENTITY,
    [ReceiptId] bigint NOT NULL,
    [InvoiceId] bigint NOT NULL,
    [AllocatedAmount] decimal(18,2) NOT NULL,
    [TdsDeductedByClient] decimal(18,2) NOT NULL,
    [CashDiscountAllowed] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_ReceiptAllocations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ReceiptAllocations_CustomerReceipts_ReceiptId] FOREIGN KEY ([ReceiptId]) REFERENCES [CustomerReceipts] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ReceiptAllocations_sales_invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [sales].[sales_invoices] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[SalesInvoiceItems]', N'U') IS NULL
BEGIN
CREATE TABLE [SalesInvoiceItems] (
    [Id] bigint NOT NULL IDENTITY,
    [InvoiceId] bigint NOT NULL,
    [ItemDescription] nvarchar(max) NOT NULL,
    [HsnSacCode] nvarchar(max) NOT NULL,
    [Quantity] decimal(18,2) NOT NULL,
    [UnitId] int NULL,
    [UnitRate] decimal(18,2) NOT NULL,
    [DiscountPercent] decimal(18,2) NOT NULL,
    [TaxableValue] decimal(18,2) NOT NULL,
    [TaxRateId] int NOT NULL,
    [CgstAmount] decimal(18,2) NOT NULL,
    [SgstAmount] decimal(18,2) NOT NULL,
    [IgstAmount] decimal(18,2) NOT NULL,
    [LineTotal] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_SalesInvoiceItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SalesInvoiceItems_ItemUnits_UnitId] FOREIGN KEY ([UnitId]) REFERENCES [ItemUnits] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SalesInvoiceItems_TaxRates_TaxRateId] FOREIGN KEY ([TaxRateId]) REFERENCES [TaxRates] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SalesInvoiceItems_sales_invoices_InvoiceId] FOREIGN KEY ([InvoiceId]) REFERENCES [sales].[sales_invoices] ([Id]) ON DELETE NO ACTION
);
END
GO

IF OBJECT_ID(N'[AssetAllocations]', N'U') IS NULL
BEGIN
CREATE TABLE [AssetAllocations] (
    [Id] bigint NOT NULL IDENTITY,
    [AssetId] bigint NOT NULL,
    [AllocatedToEmployeeId] bigint NOT NULL,
    [AllocatedDate] datetime2 NOT NULL,
    [ReturnDate] datetime2 NULL,
    [HandoverCondition] nvarchar(max) NULL,
    [AllocatedBy] bigint NOT NULL,
    [AllocatedByUserId] bigint NOT NULL,
    CONSTRAINT [PK_AssetAllocations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AssetAllocations_Employees_AllocatedToEmployeeId] FOREIGN KEY ([AllocatedToEmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AssetAllocations_FixedAssets_AssetId] FOREIGN KEY ([AssetId]) REFERENCES [FixedAssets] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AssetAllocations_users_AllocatedByUserId] FOREIGN KEY ([AllocatedByUserId]) REFERENCES [admin].[users] ([Id]) ON DELETE NO ACTION
);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_AccountGroups_ParentGroupId' AND object_id = OBJECT_ID(N'[AccountGroups]'))
BEGIN
CREATE INDEX [IX_AccountGroups_ParentGroupId] ON [AccountGroups] ([ParentGroupId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ApprovalRequests_ActionUserId' AND object_id = OBJECT_ID(N'[ApprovalRequests]'))
BEGIN
CREATE INDEX [IX_ApprovalRequests_ActionUserId] ON [ApprovalRequests] ([ActionUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ApprovalRequests_AssignedRoleId' AND object_id = OBJECT_ID(N'[ApprovalRequests]'))
BEGIN
CREATE INDEX [IX_ApprovalRequests_AssignedRoleId] ON [ApprovalRequests] ([AssignedRoleId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ApprovalRequests_RequesterId' AND object_id = OBJECT_ID(N'[ApprovalRequests]'))
BEGIN
CREATE INDEX [IX_ApprovalRequests_RequesterId] ON [ApprovalRequests] ([RequesterId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ApprovalRequests_RuleId' AND object_id = OBJECT_ID(N'[ApprovalRequests]'))
BEGIN
CREATE INDEX [IX_ApprovalRequests_RuleId] ON [ApprovalRequests] ([RuleId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ApprovalRules_ApproverRoleId' AND object_id = OBJECT_ID(N'[ApprovalRules]'))
BEGIN
CREATE INDEX [IX_ApprovalRules_ApproverRoleId] ON [ApprovalRules] ([ApproverRoleId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_AssetAllocations_AllocatedByUserId' AND object_id = OBJECT_ID(N'[AssetAllocations]'))
BEGIN
CREATE INDEX [IX_AssetAllocations_AllocatedByUserId] ON [AssetAllocations] ([AllocatedByUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_AssetAllocations_AllocatedToEmployeeId' AND object_id = OBJECT_ID(N'[AssetAllocations]'))
BEGIN
CREATE INDEX [IX_AssetAllocations_AllocatedToEmployeeId] ON [AssetAllocations] ([AllocatedToEmployeeId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_AssetAllocations_AssetId' AND object_id = OBJECT_ID(N'[AssetAllocations]'))
BEGIN
CREATE INDEX [IX_AssetAllocations_AssetId] ON [AssetAllocations] ([AssetId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_AuditLogs_UserId' AND object_id = OBJECT_ID(N'[AuditLogs]'))
BEGIN
CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_BankAccounts_CompanyId' AND object_id = OBJECT_ID(N'[BankAccounts]'))
BEGIN
CREATE INDEX [IX_BankAccounts_CompanyId] ON [BankAccounts] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_BankAccounts_GlAccountId' AND object_id = OBJECT_ID(N'[BankAccounts]'))
BEGIN
CREATE INDEX [IX_BankAccounts_GlAccountId] ON [BankAccounts] ([GlAccountId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_BankReconciliations_BankAccountId' AND object_id = OBJECT_ID(N'[BankReconciliations]'))
BEGIN
CREATE INDEX [IX_BankReconciliations_BankAccountId] ON [BankReconciliations] ([BankAccountId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_BankReconciliations_ReconcilerId' AND object_id = OBJECT_ID(N'[BankReconciliations]'))
BEGIN
CREATE INDEX [IX_BankReconciliations_ReconcilerId] ON [BankReconciliations] ([ReconcilerId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_BankTransactions_BankAccountId' AND object_id = OBJECT_ID(N'[BankTransactions]'))
BEGIN
CREATE INDEX [IX_BankTransactions_BankAccountId] ON [BankTransactions] ([BankAccountId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_BankTransactions_VoucherId' AND object_id = OBJECT_ID(N'[BankTransactions]'))
BEGIN
CREATE INDEX [IX_BankTransactions_VoucherId] ON [BankTransactions] ([VoucherId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_branches_BranchCode' AND object_id = OBJECT_ID(N'[admin].[branches]'))
BEGIN
CREATE UNIQUE INDEX [IX_branches_BranchCode] ON [admin].[branches] ([BranchCode]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_branches_CompanyId' AND object_id = OBJECT_ID(N'[admin].[branches]'))
BEGIN
CREATE INDEX [IX_branches_CompanyId] ON [admin].[branches] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ChartOfAccounts_CompanyId' AND object_id = OBJECT_ID(N'[ChartOfAccounts]'))
BEGIN
CREATE INDEX [IX_ChartOfAccounts_CompanyId] ON [ChartOfAccounts] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ChartOfAccounts_GroupId' AND object_id = OBJECT_ID(N'[ChartOfAccounts]'))
BEGIN
CREATE INDEX [IX_ChartOfAccounts_GroupId] ON [ChartOfAccounts] ([GroupId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_clients_ClientCode' AND object_id = OBJECT_ID(N'[master].[clients]'))
BEGIN
CREATE UNIQUE INDEX [IX_clients_ClientCode] ON [master].[clients] ([ClientCode]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_clients_CompanyId' AND object_id = OBJECT_ID(N'[master].[clients]'))
BEGIN
CREATE INDEX [IX_clients_CompanyId] ON [master].[clients] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_companies_CompanyCode' AND object_id = OBJECT_ID(N'[admin].[companies]'))
BEGIN
CREATE UNIQUE INDEX [IX_companies_CompanyCode] ON [admin].[companies] ([CompanyCode]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_CreditDebitNotes_ClientId' AND object_id = OBJECT_ID(N'[CreditDebitNotes]'))
BEGIN
CREATE INDEX [IX_CreditDebitNotes_ClientId] ON [CreditDebitNotes] ([ClientId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_CreditDebitNotes_CompanyId' AND object_id = OBJECT_ID(N'[CreditDebitNotes]'))
BEGIN
CREATE INDEX [IX_CreditDebitNotes_CompanyId] ON [CreditDebitNotes] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_CreditDebitNotes_OriginalInvoiceId' AND object_id = OBJECT_ID(N'[CreditDebitNotes]'))
BEGIN
CREATE INDEX [IX_CreditDebitNotes_OriginalInvoiceId] ON [CreditDebitNotes] ([OriginalInvoiceId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_CreditDebitNotes_VendorId' AND object_id = OBJECT_ID(N'[CreditDebitNotes]'))
BEGIN
CREATE INDEX [IX_CreditDebitNotes_VendorId] ON [CreditDebitNotes] ([VendorId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_CustomerReceipts_ClientId' AND object_id = OBJECT_ID(N'[CustomerReceipts]'))
BEGIN
CREATE INDEX [IX_CustomerReceipts_ClientId] ON [CustomerReceipts] ([ClientId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_CustomerReceipts_CompanyId' AND object_id = OBJECT_ID(N'[CustomerReceipts]'))
BEGIN
CREATE INDEX [IX_CustomerReceipts_CompanyId] ON [CustomerReceipts] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_DocumentAttachments_UploaderId' AND object_id = OBJECT_ID(N'[DocumentAttachments]'))
BEGIN
CREATE INDEX [IX_DocumentAttachments_UploaderId] ON [DocumentAttachments] ([UploaderId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_EmployeeAdvances_EmployeeId' AND object_id = OBJECT_ID(N'[EmployeeAdvances]'))
BEGIN
CREATE INDEX [IX_EmployeeAdvances_EmployeeId] ON [EmployeeAdvances] ([EmployeeId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Employees_BranchId' AND object_id = OBJECT_ID(N'[Employees]'))
BEGIN
CREATE INDEX [IX_Employees_BranchId] ON [Employees] ([BranchId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Employees_CompanyId' AND object_id = OBJECT_ID(N'[Employees]'))
BEGIN
CREATE INDEX [IX_Employees_CompanyId] ON [Employees] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Employees_ReportingManagerId' AND object_id = OBJECT_ID(N'[Employees]'))
BEGIN
CREATE INDEX [IX_Employees_ReportingManagerId] ON [Employees] ([ReportingManagerId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_FinancialYears_CompanyId' AND object_id = OBJECT_ID(N'[FinancialYears]'))
BEGIN
CREATE INDEX [IX_FinancialYears_CompanyId] ON [FinancialYears] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_FixedAssets_BranchId' AND object_id = OBJECT_ID(N'[FixedAssets]'))
BEGIN
CREATE INDEX [IX_FixedAssets_BranchId] ON [FixedAssets] ([BranchId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_FixedAssets_CompanyId' AND object_id = OBJECT_ID(N'[FixedAssets]'))
BEGIN
CREATE INDEX [IX_FixedAssets_CompanyId] ON [FixedAssets] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_FixedAssets_PurchaseBillId' AND object_id = OBJECT_ID(N'[FixedAssets]'))
BEGIN
CREATE INDEX [IX_FixedAssets_PurchaseBillId] ON [FixedAssets] ([PurchaseBillId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GoodsReceiptNotes_PurchaseOrderId' AND object_id = OBJECT_ID(N'[GoodsReceiptNotes]'))
BEGIN
CREATE INDEX [IX_GoodsReceiptNotes_PurchaseOrderId] ON [GoodsReceiptNotes] ([PurchaseOrderId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GoodsReceiptNotes_ReceivedByUserId' AND object_id = OBJECT_ID(N'[GoodsReceiptNotes]'))
BEGIN
CREATE INDEX [IX_GoodsReceiptNotes_ReceivedByUserId] ON [GoodsReceiptNotes] ([ReceivedByUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GrnItems_GoodsReceiptNoteId' AND object_id = OBJECT_ID(N'[GrnItems]'))
BEGIN
CREATE INDEX [IX_GrnItems_GoodsReceiptNoteId] ON [GrnItems] ([GoodsReceiptNoteId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GrnItems_ItemId' AND object_id = OBJECT_ID(N'[GrnItems]'))
BEGIN
CREATE INDEX [IX_GrnItems_ItemId] ON [GrnItems] ([ItemId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GrnItems_PoItemId' AND object_id = OBJECT_ID(N'[GrnItems]'))
BEGIN
CREATE INDEX [IX_GrnItems_PoItemId] ON [GrnItems] ([PoItemId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GstReconciliations_InternalPurchaseBillId' AND object_id = OBJECT_ID(N'[GstReconciliations]'))
BEGIN
CREATE INDEX [IX_GstReconciliations_InternalPurchaseBillId] ON [GstReconciliations] ([InternalPurchaseBillId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GstReconciliations_ReconcilerId' AND object_id = OBJECT_ID(N'[GstReconciliations]'))
BEGIN
CREATE INDEX [IX_GstReconciliations_ReconcilerId] ON [GstReconciliations] ([ReconcilerId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GstTransactions_CompanyId' AND object_id = OBJECT_ID(N'[GstTransactions]'))
BEGIN
CREATE INDEX [IX_GstTransactions_CompanyId] ON [GstTransactions] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_GstTransactions_VoucherId' AND object_id = OBJECT_ID(N'[GstTransactions]'))
BEGIN
CREATE INDEX [IX_GstTransactions_VoucherId] ON [GstTransactions] ([VoucherId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_HsnSacCodes_DefaultTaxRateId' AND object_id = OBJECT_ID(N'[HsnSacCodes]'))
BEGIN
CREATE INDEX [IX_HsnSacCodes_DefaultTaxRateId] ON [HsnSacCodes] ([DefaultTaxRateId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ItemCategories_ParentCategoryId' AND object_id = OBJECT_ID(N'[ItemCategories]'))
BEGIN
CREATE INDEX [IX_ItemCategories_ParentCategoryId] ON [ItemCategories] ([ParentCategoryId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_items_CategoryId' AND object_id = OBJECT_ID(N'[master].[items]'))
BEGIN
CREATE INDEX [IX_items_CategoryId] ON [master].[items] ([CategoryId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_items_CompanyId' AND object_id = OBJECT_ID(N'[master].[items]'))
BEGIN
CREATE INDEX [IX_items_CompanyId] ON [master].[items] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_items_ItemCode' AND object_id = OBJECT_ID(N'[master].[items]'))
BEGIN
CREATE UNIQUE INDEX [IX_items_ItemCode] ON [master].[items] ([ItemCode]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_items_TaxRateId' AND object_id = OBJECT_ID(N'[master].[items]'))
BEGIN
CREATE INDEX [IX_items_TaxRateId] ON [master].[items] ([TaxRateId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_items_UnitId' AND object_id = OBJECT_ID(N'[master].[items]'))
BEGIN
CREATE INDEX [IX_items_UnitId] ON [master].[items] ([UnitId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_entries_BranchId' AND object_id = OBJECT_ID(N'[accounting].[journal_entries]'))
BEGIN
CREATE INDEX [IX_journal_entries_BranchId] ON [accounting].[journal_entries] ([BranchId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_entries_CompanyId' AND object_id = OBJECT_ID(N'[accounting].[journal_entries]'))
BEGIN
CREATE INDEX [IX_journal_entries_CompanyId] ON [accounting].[journal_entries] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_entries_CreatedBy' AND object_id = OBJECT_ID(N'[accounting].[journal_entries]'))
BEGIN
CREATE INDEX [IX_journal_entries_CreatedBy] ON [accounting].[journal_entries] ([CreatedBy]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_entries_FyId' AND object_id = OBJECT_ID(N'[accounting].[journal_entries]'))
BEGIN
CREATE INDEX [IX_journal_entries_FyId] ON [accounting].[journal_entries] ([FyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_entries_ProjectId' AND object_id = OBJECT_ID(N'[accounting].[journal_entries]'))
BEGIN
CREATE INDEX [IX_journal_entries_ProjectId] ON [accounting].[journal_entries] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_entries_VoucherNo' AND object_id = OBJECT_ID(N'[accounting].[journal_entries]'))
BEGIN
CREATE UNIQUE INDEX [IX_journal_entries_VoucherNo] ON [accounting].[journal_entries] ([VoucherNo]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_lines_AccountId_JournalEntryId' AND object_id = OBJECT_ID(N'[accounting].[journal_lines]'))
BEGIN
CREATE INDEX [IX_journal_lines_AccountId_JournalEntryId] ON [accounting].[journal_lines] ([AccountId], [JournalEntryId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_lines_ClientId' AND object_id = OBJECT_ID(N'[accounting].[journal_lines]'))
BEGIN
CREATE INDEX [IX_journal_lines_ClientId] ON [accounting].[journal_lines] ([ClientId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_lines_JournalEntryId' AND object_id = OBJECT_ID(N'[accounting].[journal_lines]'))
BEGIN
CREATE INDEX [IX_journal_lines_JournalEntryId] ON [accounting].[journal_lines] ([JournalEntryId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_journal_lines_VendorId' AND object_id = OBJECT_ID(N'[accounting].[journal_lines]'))
BEGIN
CREATE INDEX [IX_journal_lines_VendorId] ON [accounting].[journal_lines] ([VendorId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_LeaveApplications_ApproverId' AND object_id = OBJECT_ID(N'[LeaveApplications]'))
BEGIN
CREATE INDEX [IX_LeaveApplications_ApproverId] ON [LeaveApplications] ([ApproverId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_LeaveApplications_EmployeeId' AND object_id = OBJECT_ID(N'[LeaveApplications]'))
BEGIN
CREATE INDEX [IX_LeaveApplications_EmployeeId] ON [LeaveApplications] ([EmployeeId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PayrollItems_EmployeeId' AND object_id = OBJECT_ID(N'[PayrollItems]'))
BEGIN
CREATE INDEX [IX_PayrollItems_EmployeeId] ON [PayrollItems] ([EmployeeId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PayrollItems_PayrollRunId' AND object_id = OBJECT_ID(N'[PayrollItems]'))
BEGIN
CREATE INDEX [IX_PayrollItems_PayrollRunId] ON [PayrollItems] ([PayrollRunId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PayrollRuns_CompanyId' AND object_id = OBJECT_ID(N'[PayrollRuns]'))
BEGIN
CREATE INDEX [IX_PayrollRuns_CompanyId] ON [PayrollRuns] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PettyCashTransactions_BankAccountId' AND object_id = OBJECT_ID(N'[PettyCashTransactions]'))
BEGIN
CREATE INDEX [IX_PettyCashTransactions_BankAccountId] ON [PettyCashTransactions] ([BankAccountId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PettyCashTransactions_CustodianUserId' AND object_id = OBJECT_ID(N'[PettyCashTransactions]'))
BEGIN
CREATE INDEX [IX_PettyCashTransactions_CustodianUserId] ON [PettyCashTransactions] ([CustodianUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PettyCashTransactions_ExpenseAccountId' AND object_id = OBJECT_ID(N'[PettyCashTransactions]'))
BEGIN
CREATE INDEX [IX_PettyCashTransactions_ExpenseAccountId] ON [PettyCashTransactions] ([ExpenseAccountId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PettyCashTransactions_ProjectId' AND object_id = OBJECT_ID(N'[PettyCashTransactions]'))
BEGIN
CREATE INDEX [IX_PettyCashTransactions_ProjectId] ON [PettyCashTransactions] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PettyCashTransactions_ReceiptDocId' AND object_id = OBJECT_ID(N'[PettyCashTransactions]'))
BEGIN
CREATE INDEX [IX_PettyCashTransactions_ReceiptDocId] ON [PettyCashTransactions] ([ReceiptDocId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectDeliveries_AttachmentDocId' AND object_id = OBJECT_ID(N'[ProjectDeliveries]'))
BEGIN
CREATE INDEX [IX_ProjectDeliveries_AttachmentDocId] ON [ProjectDeliveries] ([AttachmentDocId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectDeliveries_ProjectId' AND object_id = OBJECT_ID(N'[ProjectDeliveries]'))
BEGIN
CREATE INDEX [IX_ProjectDeliveries_ProjectId] ON [ProjectDeliveries] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectExpenses_ApproverId' AND object_id = OBJECT_ID(N'[ProjectExpenses]'))
BEGIN
CREATE INDEX [IX_ProjectExpenses_ApproverId] ON [ProjectExpenses] ([ApproverId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectExpenses_IncurredByUserId' AND object_id = OBJECT_ID(N'[ProjectExpenses]'))
BEGIN
CREATE INDEX [IX_ProjectExpenses_IncurredByUserId] ON [ProjectExpenses] ([IncurredByUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectExpenses_ProjectId' AND object_id = OBJECT_ID(N'[ProjectExpenses]'))
BEGIN
CREATE INDEX [IX_ProjectExpenses_ProjectId] ON [ProjectExpenses] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectExpenses_ReceiptDocId' AND object_id = OBJECT_ID(N'[ProjectExpenses]'))
BEGIN
CREATE INDEX [IX_ProjectExpenses_ReceiptDocId] ON [ProjectExpenses] ([ReceiptDocId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectMilestones_ProjectId' AND object_id = OBJECT_ID(N'[ProjectMilestones]'))
BEGIN
CREATE INDEX [IX_ProjectMilestones_ProjectId] ON [ProjectMilestones] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectPos_AttachmentId' AND object_id = OBJECT_ID(N'[ProjectPos]'))
BEGIN
CREATE INDEX [IX_ProjectPos_AttachmentId] ON [ProjectPos] ([AttachmentId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ProjectPos_ProjectId' AND object_id = OBJECT_ID(N'[ProjectPos]'))
BEGIN
CREATE INDEX [IX_ProjectPos_ProjectId] ON [ProjectPos] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_projects_BranchId' AND object_id = OBJECT_ID(N'[project].[projects]'))
BEGIN
CREATE INDEX [IX_projects_BranchId] ON [project].[projects] ([BranchId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_projects_ClientId' AND object_id = OBJECT_ID(N'[project].[projects]'))
BEGIN
CREATE INDEX [IX_projects_ClientId] ON [project].[projects] ([ClientId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_projects_ClosureApproverId' AND object_id = OBJECT_ID(N'[project].[projects]'))
BEGIN
CREATE INDEX [IX_projects_ClosureApproverId] ON [project].[projects] ([ClosureApproverId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_projects_CompanyId' AND object_id = OBJECT_ID(N'[project].[projects]'))
BEGIN
CREATE INDEX [IX_projects_CompanyId] ON [project].[projects] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_projects_ManagerId' AND object_id = OBJECT_ID(N'[project].[projects]'))
BEGIN
CREATE INDEX [IX_projects_ManagerId] ON [project].[projects] ([ManagerId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_projects_ProjectCode' AND object_id = OBJECT_ID(N'[project].[projects]'))
BEGIN
CREATE UNIQUE INDEX [IX_projects_ProjectCode] ON [project].[projects] ([ProjectCode]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_purchase_bills_CompanyId' AND object_id = OBJECT_ID(N'[procurement].[purchase_bills]'))
BEGIN
CREATE INDEX [IX_purchase_bills_CompanyId] ON [procurement].[purchase_bills] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_purchase_bills_ProjectId' AND object_id = OBJECT_ID(N'[procurement].[purchase_bills]'))
BEGIN
CREATE INDEX [IX_purchase_bills_ProjectId] ON [procurement].[purchase_bills] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_purchase_bills_PurchaseOrderId' AND object_id = OBJECT_ID(N'[procurement].[purchase_bills]'))
BEGIN
CREATE INDEX [IX_purchase_bills_PurchaseOrderId] ON [procurement].[purchase_bills] ([PurchaseOrderId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_purchase_bills_VendorId_Status_DueDate' AND object_id = OBJECT_ID(N'[procurement].[purchase_bills]'))
BEGIN
CREATE INDEX [IX_purchase_bills_VendorId_Status_DueDate] ON [procurement].[purchase_bills] ([VendorId], [Status], [DueDate]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseBillItems_ItemId' AND object_id = OBJECT_ID(N'[PurchaseBillItems]'))
BEGIN
CREATE INDEX [IX_PurchaseBillItems_ItemId] ON [PurchaseBillItems] ([ItemId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseBillItems_PurchaseBillId' AND object_id = OBJECT_ID(N'[PurchaseBillItems]'))
BEGIN
CREATE INDEX [IX_PurchaseBillItems_PurchaseBillId] ON [PurchaseBillItems] ([PurchaseBillId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseBillItems_TaxRateId' AND object_id = OBJECT_ID(N'[PurchaseBillItems]'))
BEGIN
CREATE INDEX [IX_PurchaseBillItems_TaxRateId] ON [PurchaseBillItems] ([TaxRateId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrderItems_ItemId' AND object_id = OBJECT_ID(N'[PurchaseOrderItems]'))
BEGIN
CREATE INDEX [IX_PurchaseOrderItems_ItemId] ON [PurchaseOrderItems] ([ItemId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrderItems_PurchaseOrderId' AND object_id = OBJECT_ID(N'[PurchaseOrderItems]'))
BEGIN
CREATE INDEX [IX_PurchaseOrderItems_PurchaseOrderId] ON [PurchaseOrderItems] ([PurchaseOrderId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrderItems_TaxRateId' AND object_id = OBJECT_ID(N'[PurchaseOrderItems]'))
BEGIN
CREATE INDEX [IX_PurchaseOrderItems_TaxRateId] ON [PurchaseOrderItems] ([TaxRateId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrderItems_UnitId' AND object_id = OBJECT_ID(N'[PurchaseOrderItems]'))
BEGIN
CREATE INDEX [IX_PurchaseOrderItems_UnitId] ON [PurchaseOrderItems] ([UnitId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrders_ApproverId' AND object_id = OBJECT_ID(N'[PurchaseOrders]'))
BEGIN
CREATE INDEX [IX_PurchaseOrders_ApproverId] ON [PurchaseOrders] ([ApproverId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrders_BranchId' AND object_id = OBJECT_ID(N'[PurchaseOrders]'))
BEGIN
CREATE INDEX [IX_PurchaseOrders_BranchId] ON [PurchaseOrders] ([BranchId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrders_CompanyId' AND object_id = OBJECT_ID(N'[PurchaseOrders]'))
BEGIN
CREATE INDEX [IX_PurchaseOrders_CompanyId] ON [PurchaseOrders] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrders_ProjectId' AND object_id = OBJECT_ID(N'[PurchaseOrders]'))
BEGIN
CREATE INDEX [IX_PurchaseOrders_ProjectId] ON [PurchaseOrders] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrders_RequisitionId' AND object_id = OBJECT_ID(N'[PurchaseOrders]'))
BEGIN
CREATE INDEX [IX_PurchaseOrders_RequisitionId] ON [PurchaseOrders] ([RequisitionId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseOrders_VendorId' AND object_id = OBJECT_ID(N'[PurchaseOrders]'))
BEGIN
CREATE INDEX [IX_PurchaseOrders_VendorId] ON [PurchaseOrders] ([VendorId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseRequisitionItems_ItemId' AND object_id = OBJECT_ID(N'[PurchaseRequisitionItems]'))
BEGIN
CREATE INDEX [IX_PurchaseRequisitionItems_ItemId] ON [PurchaseRequisitionItems] ([ItemId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseRequisitionItems_RequisitionId' AND object_id = OBJECT_ID(N'[PurchaseRequisitionItems]'))
BEGIN
CREATE INDEX [IX_PurchaseRequisitionItems_RequisitionId] ON [PurchaseRequisitionItems] ([RequisitionId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseRequisitionItems_UnitId' AND object_id = OBJECT_ID(N'[PurchaseRequisitionItems]'))
BEGIN
CREATE INDEX [IX_PurchaseRequisitionItems_UnitId] ON [PurchaseRequisitionItems] ([UnitId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseRequisitions_CompanyId' AND object_id = OBJECT_ID(N'[PurchaseRequisitions]'))
BEGIN
CREATE INDEX [IX_PurchaseRequisitions_CompanyId] ON [PurchaseRequisitions] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseRequisitions_ProjectId' AND object_id = OBJECT_ID(N'[PurchaseRequisitions]'))
BEGIN
CREATE INDEX [IX_PurchaseRequisitions_ProjectId] ON [PurchaseRequisitions] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_PurchaseRequisitions_RequestedByUserId' AND object_id = OBJECT_ID(N'[PurchaseRequisitions]'))
BEGIN
CREATE INDEX [IX_PurchaseRequisitions_RequestedByUserId] ON [PurchaseRequisitions] ([RequestedByUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Quotations_ClientId' AND object_id = OBJECT_ID(N'[Quotations]'))
BEGIN
CREATE INDEX [IX_Quotations_ClientId] ON [Quotations] ([ClientId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Quotations_CompanyId' AND object_id = OBJECT_ID(N'[Quotations]'))
BEGIN
CREATE INDEX [IX_Quotations_CompanyId] ON [Quotations] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_Quotations_ProjectId' AND object_id = OBJECT_ID(N'[Quotations]'))
BEGIN
CREATE INDEX [IX_Quotations_ProjectId] ON [Quotations] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ReceiptAllocations_InvoiceId' AND object_id = OBJECT_ID(N'[ReceiptAllocations]'))
BEGIN
CREATE INDEX [IX_ReceiptAllocations_InvoiceId] ON [ReceiptAllocations] ([InvoiceId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_ReceiptAllocations_ReceiptId' AND object_id = OBJECT_ID(N'[ReceiptAllocations]'))
BEGIN
CREATE INDEX [IX_ReceiptAllocations_ReceiptId] ON [ReceiptAllocations] ([ReceiptId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_RolePermissions_RoleId' AND object_id = OBJECT_ID(N'[RolePermissions]'))
BEGIN
CREATE INDEX [IX_RolePermissions_RoleId] ON [RolePermissions] ([RoleId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_roles_RoleName' AND object_id = OBJECT_ID(N'[admin].[roles]'))
BEGIN
CREATE UNIQUE INDEX [IX_roles_RoleName] ON [admin].[roles] ([RoleName]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalaryStructures_EmployeeId' AND object_id = OBJECT_ID(N'[SalaryStructures]'))
BEGIN
CREATE INDEX [IX_SalaryStructures_EmployeeId] ON [SalaryStructures] ([EmployeeId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_sales_invoices_BranchId' AND object_id = OBJECT_ID(N'[sales].[sales_invoices]'))
BEGIN
CREATE INDEX [IX_sales_invoices_BranchId] ON [sales].[sales_invoices] ([BranchId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_sales_invoices_ClientId_Status_InvoiceDate' AND object_id = OBJECT_ID(N'[sales].[sales_invoices]'))
BEGIN
CREATE INDEX [IX_sales_invoices_ClientId_Status_InvoiceDate] ON [sales].[sales_invoices] ([ClientId], [Status], [InvoiceDate]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_sales_invoices_CompanyId' AND object_id = OBJECT_ID(N'[sales].[sales_invoices]'))
BEGIN
CREATE INDEX [IX_sales_invoices_CompanyId] ON [sales].[sales_invoices] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_sales_invoices_InvoiceNumber' AND object_id = OBJECT_ID(N'[sales].[sales_invoices]'))
BEGIN
CREATE UNIQUE INDEX [IX_sales_invoices_InvoiceNumber] ON [sales].[sales_invoices] ([InvoiceNumber]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_sales_invoices_ProjectId' AND object_id = OBJECT_ID(N'[sales].[sales_invoices]'))
BEGIN
CREATE INDEX [IX_sales_invoices_ProjectId] ON [sales].[sales_invoices] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_sales_invoices_SalesOrderId' AND object_id = OBJECT_ID(N'[sales].[sales_invoices]'))
BEGIN
CREATE INDEX [IX_sales_invoices_SalesOrderId] ON [sales].[sales_invoices] ([SalesOrderId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalesInvoiceItems_InvoiceId' AND object_id = OBJECT_ID(N'[SalesInvoiceItems]'))
BEGIN
CREATE INDEX [IX_SalesInvoiceItems_InvoiceId] ON [SalesInvoiceItems] ([InvoiceId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalesInvoiceItems_TaxRateId' AND object_id = OBJECT_ID(N'[SalesInvoiceItems]'))
BEGIN
CREATE INDEX [IX_SalesInvoiceItems_TaxRateId] ON [SalesInvoiceItems] ([TaxRateId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalesInvoiceItems_UnitId' AND object_id = OBJECT_ID(N'[SalesInvoiceItems]'))
BEGIN
CREATE INDEX [IX_SalesInvoiceItems_UnitId] ON [SalesInvoiceItems] ([UnitId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalesOrders_ClientId' AND object_id = OBJECT_ID(N'[SalesOrders]'))
BEGIN
CREATE INDEX [IX_SalesOrders_ClientId] ON [SalesOrders] ([ClientId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalesOrders_CompanyId' AND object_id = OBJECT_ID(N'[SalesOrders]'))
BEGIN
CREATE INDEX [IX_SalesOrders_CompanyId] ON [SalesOrders] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalesOrders_ProjectId' AND object_id = OBJECT_ID(N'[SalesOrders]'))
BEGIN
CREATE INDEX [IX_SalesOrders_ProjectId] ON [SalesOrders] ([ProjectId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SalesOrders_QuotationId' AND object_id = OBJECT_ID(N'[SalesOrders]'))
BEGIN
CREATE INDEX [IX_SalesOrders_QuotationId] ON [SalesOrders] ([QuotationId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_StockIssues_ApproverId' AND object_id = OBJECT_ID(N'[StockIssues]'))
BEGIN
CREATE INDEX [IX_StockIssues_ApproverId] ON [StockIssues] ([ApproverId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_StockIssues_ItemId' AND object_id = OBJECT_ID(N'[StockIssues]'))
BEGIN
CREATE INDEX [IX_StockIssues_ItemId] ON [StockIssues] ([ItemId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_StockTransactions_CompanyId' AND object_id = OBJECT_ID(N'[StockTransactions]'))
BEGIN
CREATE INDEX [IX_StockTransactions_CompanyId] ON [StockTransactions] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_StockTransactions_ItemId' AND object_id = OBJECT_ID(N'[StockTransactions]'))
BEGIN
CREATE INDEX [IX_StockTransactions_ItemId] ON [StockTransactions] ([ItemId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SupportTickets_AssignedToUserId' AND object_id = OBJECT_ID(N'[SupportTickets]'))
BEGIN
CREATE INDEX [IX_SupportTickets_AssignedToUserId] ON [SupportTickets] ([AssignedToUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_SupportTickets_RaisedByUserId' AND object_id = OBJECT_ID(N'[SupportTickets]'))
BEGIN
CREATE INDEX [IX_SupportTickets_RaisedByUserId] ON [SupportTickets] ([RaisedByUserId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_users_BranchId' AND object_id = OBJECT_ID(N'[admin].[users]'))
BEGIN
CREATE INDEX [IX_users_BranchId] ON [admin].[users] ([BranchId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_users_CompanyId' AND object_id = OBJECT_ID(N'[admin].[users]'))
BEGIN
CREATE INDEX [IX_users_CompanyId] ON [admin].[users] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_users_Email' AND object_id = OBJECT_ID(N'[admin].[users]'))
BEGIN
CREATE UNIQUE INDEX [IX_users_Email] ON [admin].[users] ([Email]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_users_ReportingManagerId' AND object_id = OBJECT_ID(N'[admin].[users]'))
BEGIN
CREATE INDEX [IX_users_ReportingManagerId] ON [admin].[users] ([ReportingManagerId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_users_RoleId' AND object_id = OBJECT_ID(N'[admin].[users]'))
BEGIN
CREATE INDEX [IX_users_RoleId] ON [admin].[users] ([RoleId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_users_Username' AND object_id = OBJECT_ID(N'[admin].[users]'))
BEGIN
CREATE UNIQUE INDEX [IX_users_Username] ON [admin].[users] ([Username]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_VendorPaymentAllocations_PurchaseBillId' AND object_id = OBJECT_ID(N'[VendorPaymentAllocations]'))
BEGIN
CREATE INDEX [IX_VendorPaymentAllocations_PurchaseBillId] ON [VendorPaymentAllocations] ([PurchaseBillId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_VendorPaymentAllocations_VendorPaymentId' AND object_id = OBJECT_ID(N'[VendorPaymentAllocations]'))
BEGIN
CREATE INDEX [IX_VendorPaymentAllocations_VendorPaymentId] ON [VendorPaymentAllocations] ([VendorPaymentId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_VendorPayments_CompanyId' AND object_id = OBJECT_ID(N'[VendorPayments]'))
BEGIN
CREATE INDEX [IX_VendorPayments_CompanyId] ON [VendorPayments] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_VendorPayments_VendorId' AND object_id = OBJECT_ID(N'[VendorPayments]'))
BEGIN
CREATE INDEX [IX_VendorPayments_VendorId] ON [VendorPayments] ([VendorId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_vendors_CompanyId' AND object_id = OBJECT_ID(N'[master].[vendors]'))
BEGIN
CREATE INDEX [IX_vendors_CompanyId] ON [master].[vendors] ([CompanyId]);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = N'IX_vendors_VendorCode' AND object_id = OBJECT_ID(N'[master].[vendors]'))
BEGIN
CREATE UNIQUE INDEX [IX_vendors_VendorCode] ON [master].[vendors] ([VendorCode]);
END
GO

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
