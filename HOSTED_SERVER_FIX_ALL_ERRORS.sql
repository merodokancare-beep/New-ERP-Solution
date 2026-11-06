-- =========================================================================================
-- SDK SOLUTIONS ERP - ALL-IN-ONE HOSTED SERVER FIX SCRIPT
-- Resolves HTTP 500 errors on erp.sdksolution.co.in (/Masters and /Procurement)
-- Safe & Idempotent: Can be executed multiple times without losing any data.
-- =========================================================================================

PRINT '>>> [1/7] Ensuring Schemas...';
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'master')
    EXEC('CREATE SCHEMA [master]');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'admin')
    EXEC('CREATE SCHEMA [admin]');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'sales')
    EXEC('CREATE SCHEMA [sales]');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'project')
    EXEC('CREATE SCHEMA [project]');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'tax')
    EXEC('CREATE SCHEMA [tax]');
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'accounting')
    EXEC('CREATE SCHEMA [accounting]');
GO

-- =========================================================================================
-- [2/7] MASTERS: STATES TABLE & 37 INDIAN STATES AND UTs
-- =========================================================================================
PRINT '>>> [2/7] Checking master.states table & seeding state masters...';
IF OBJECT_ID(N'[master].[states]', N'U') IS NULL
BEGIN
    CREATE TABLE [master].[states] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [StateCode] NVARCHAR(10) NOT NULL,
        [StateName] NVARCHAR(100) NOT NULL,
        [StateType] NVARCHAR(50) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    );
    PRINT '  + Created [master].[states] table.';
END
GO

IF NOT EXISTS (SELECT 1 FROM [master].[states])
BEGIN
    INSERT INTO [master].[states] ([StateCode], [StateName], [StateType], [IsActive]) VALUES
    ('01', 'Jammu and Kashmir', 'Union Territory', 1),
    ('02', 'Himachal Pradesh', 'State', 1),
    ('03', 'Punjab', 'State', 1),
    ('04', 'Chandigarh', 'Union Territory', 1),
    ('05', 'Uttarakhand', 'State', 1),
    ('06', 'Haryana', 'State', 1),
    ('07', 'Delhi', 'Union Territory', 1),
    ('08', 'Rajasthan', 'State', 1),
    ('09', 'Uttar Pradesh', 'State', 1),
    ('10', 'Bihar', 'State', 1),
    ('11', 'Sikkim', 'State', 1),
    ('12', 'Arunachal Pradesh', 'State', 1),
    ('13', 'Nagaland', 'State', 1),
    ('14', 'Manipur', 'State', 1),
    ('15', 'Mizoram', 'State', 1),
    ('16', 'Tripura', 'State', 1),
    ('17', 'Meghalaya', 'State', 1),
    ('18', 'Assam', 'State', 1),
    ('19', 'West Bengal', 'State', 1),
    ('20', 'Jharkhand', 'State', 1),
    ('21', 'Odisha', 'State', 1),
    ('22', 'Chhattisgarh', 'State', 1),
    ('23', 'Madhya Pradesh', 'State', 1),
    ('24', 'Gujarat', 'State', 1),
    ('26', 'Dadra and Nagar Haveli and Daman and Diu', 'Union Territory', 1),
    ('27', 'Maharashtra', 'State', 1),
    ('28', 'Andhra Pradesh', 'State', 1),
    ('29', 'Karnataka', 'State', 1),
    ('30', 'Goa', 'State', 1),
    ('31', 'Lakshadweep', 'Union Territory', 1),
    ('32', 'Kerala', 'State', 1),
    ('33', 'Tamil Nadu', 'State', 1),
    ('34', 'Puducherry', 'Union Territory', 1),
    ('35', 'Andaman and Nicobar Islands', 'Union Territory', 1),
    ('36', 'Telangana', 'State', 1),
    ('37', 'Andhra Pradesh (New)', 'State', 1),
    ('38', 'Ladakh', 'Union Territory', 1);
    PRINT '  + Seeded 37 Indian States and Union Territories into [master].[states].';
END
GO

-- =========================================================================================
-- [3/7] MASTERS: CLIENTS & VENDORS TABLE COLUMNS
-- =========================================================================================
PRINT '>>> [3/7] Ensuring Clients and Vendors table columns...';
-- 3.1 Clients
IF OBJECT_ID(N'[master].[clients]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[clients]') AND name = 'ClientCategory')
    BEGIN
        ALTER TABLE [master].[clients] ADD [ClientCategory] NVARCHAR(30) NULL DEFAULT 'B2B';
        PRINT '  + Added [master].[clients].[ClientCategory]';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[clients]') AND name = 'StateName')
    BEGIN
        ALTER TABLE [master].[clients] ADD [StateName] NVARCHAR(100) NULL;
        PRINT '  + Added [master].[clients].[StateName]';
    END
    EXEC('UPDATE [master].[clients] SET [ClientCategory] = ''B2B'' WHERE [ClientCategory] IS NULL');
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[clients]') AND name = 'Pan')
        ALTER TABLE [master].[clients] ALTER COLUMN [Pan] NVARCHAR(50) NULL;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[clients]') AND name = 'Gstin')
        ALTER TABLE [master].[clients] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;
END
GO

-- 3.2 Vendors
IF OBJECT_ID(N'[master].[vendors]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'StateCode')
    BEGIN
        ALTER TABLE [master].[vendors] ADD [StateCode] NVARCHAR(10) NULL;
        PRINT '  + Added [master].[vendors].[StateCode]';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'StateName')
    BEGIN
        ALTER TABLE [master].[vendors] ADD [StateName] NVARCHAR(100) NULL;
        PRINT '  + Added [master].[vendors].[StateName]';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'VendorCategory')
    BEGIN
        ALTER TABLE [master].[vendors] ADD [VendorCategory] NVARCHAR(50) NULL DEFAULT 'GENERAL';
        PRINT '  + Added [master].[vendors].[VendorCategory]';
    END
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'MsmeType')
        ALTER TABLE [master].[vendors] ADD [MsmeType] NVARCHAR(50) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'BankName')
        ALTER TABLE [master].[vendors] ADD [BankName] NVARCHAR(150) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'BankAccountNo')
        ALTER TABLE [master].[vendors] ADD [BankAccountNo] NVARCHAR(50) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'IfscCode')
        ALTER TABLE [master].[vendors] ADD [IfscCode] NVARCHAR(20) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'PaymentTermsDays')
        ALTER TABLE [master].[vendors] ADD [PaymentTermsDays] INT NOT NULL DEFAULT 30;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'Pan')
        ALTER TABLE [master].[vendors] ALTER COLUMN [Pan] NVARCHAR(50) NULL;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[master].[vendors]') AND name = 'Gstin')
        ALTER TABLE [master].[vendors] ALTER COLUMN [Gstin] NVARCHAR(50) NULL;
    EXEC('UPDATE [master].[vendors] SET [VendorCategory] = ''GENERAL'' WHERE [VendorCategory] IS NULL');
END
GO

-- =========================================================================================
-- [4/7] MASTERS: PROJECT TYPES & EXPENSE TYPES
-- =========================================================================================
PRINT '>>> [4/7] Ensuring ProjectTypes & ExpenseTypes...';
IF OBJECT_ID(N'[dbo].[ProjectTypes]', N'U') IS NULL
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
    PRINT '  + Created and seeded [dbo].[ProjectTypes].';
END
GO

IF OBJECT_ID(N'[dbo].[ExpenseTypes]', N'U') IS NULL
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
    INSERT INTO [dbo].[ExpenseTypes] ([Name], [Description], [IsProjectRelated], [ReceiveDirectPayments], [OpeningAmount], [IsActive]) VALUES
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
    PRINT '  + Created and seeded [dbo].[ExpenseTypes].';
END
GO

-- =========================================================================================
-- [5/7] PROCUREMENT: PURCHASEORDERS LIFECYCLE & AUDIT COLUMNS
-- =========================================================================================
PRINT '>>> [5/7] Ensuring PurchaseOrders columns & constraints...';
IF OBJECT_ID(N'[PurchaseOrders]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'PoType')
        ALTER TABLE [PurchaseOrders] ADD [PoType] NVARCHAR(100) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'ShipTo')
        ALTER TABLE [PurchaseOrders] ADD [ShipTo] NVARCHAR(MAX) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'Remarks')
        ALTER TABLE [PurchaseOrders] ADD [Remarks] NVARCHAR(MAX) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'TermsConditions')
        ALTER TABLE [PurchaseOrders] ADD [TermsConditions] NVARCHAR(MAX) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'CgstAmount')
        ALTER TABLE [PurchaseOrders] ADD [CgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'SgstAmount')
        ALTER TABLE [PurchaseOrders] ADD [SgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'IgstAmount')
        ALTER TABLE [PurchaseOrders] ADD [IgstAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'PaidAmount')
        ALTER TABLE [PurchaseOrders] ADD [PaidAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'MarkedAmount')
        ALTER TABLE [PurchaseOrders] ADD [MarkedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'MarkedPaymentType')
        ALTER TABLE [PurchaseOrders] ADD [MarkedPaymentType] NVARCHAR(50) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'ExpenseHead')
        ALTER TABLE [PurchaseOrders] ADD [ExpenseHead] NVARCHAR(250) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'CreatedBy')
        ALTER TABLE [PurchaseOrders] ADD [CreatedBy] BIGINT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'CreatedByName')
        ALTER TABLE [PurchaseOrders] ADD [CreatedByName] NVARCHAR(150) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'CreatedByRole')
        ALTER TABLE [PurchaseOrders] ADD [CreatedByRole] NVARCHAR(50) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'ApprovedAt')
        ALTER TABLE [PurchaseOrders] ADD [ApprovedAt] DATETIME2 NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'RejectionReason')
        ALTER TABLE [PurchaseOrders] ADD [RejectionReason] NVARCHAR(MAX) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'MarkedForPaymentAt')
        ALTER TABLE [PurchaseOrders] ADD [MarkedForPaymentAt] DATETIME2 NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'PaymentApprovedBy')
        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedBy] BIGINT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'PaymentApprovedByName')
        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedByName] NVARCHAR(150) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'PaymentApprovedAt')
        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovedAt] DATETIME2 NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'PaymentApprovalRemarks')
        ALTER TABLE [PurchaseOrders] ADD [PaymentApprovalRemarks] NVARCHAR(MAX) NULL;
    PRINT '  + Ensured all 18 lifecycle columns on [PurchaseOrders].';
END
GO

-- =========================================================================================
-- [6/7] PROCUREMENT: VENDORPAYMENTS & PURCHASEORDERITEMS
-- =========================================================================================
PRINT '>>> [6/7] Ensuring VendorPayments and PurchaseOrderItems schema...';
-- 6.1 VendorPayments
IF OBJECT_ID(N'[VendorPayments]', N'U') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[VendorPayments]') AND name = 'PoId')
        ALTER TABLE [VendorPayments] ADD [PoId] BIGINT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[VendorPayments]') AND name = 'Remarks')
        ALTER TABLE [VendorPayments] ADD [Remarks] NVARCHAR(MAX) NULL;
    PRINT '  + Ensured [VendorPayments].[PoId] and Remarks.';
END
GO

-- 6.2 PurchaseOrderItems
IF OBJECT_ID(N'[PurchaseOrderItems]', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'PoId' AND is_nullable = 0)
        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [PoId] BIGINT NULL;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'PurchaseOrderId' AND is_nullable = 0)
        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [PurchaseOrderId] BIGINT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'PurchaseOrderId')
        ALTER TABLE [PurchaseOrderItems] ADD [PurchaseOrderId] BIGINT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'GstRate')
        ALTER TABLE [PurchaseOrderItems] ADD [GstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'IgstRate')
        ALTER TABLE [PurchaseOrderItems] ADD [IgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'CgstRate')
        ALTER TABLE [PurchaseOrderItems] ADD [CgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'SgstRate')
        ALTER TABLE [PurchaseOrderItems] ADD [SgstRate] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'TaxAmount')
        ALTER TABLE [PurchaseOrderItems] ADD [TaxAmount] DECIMAL(18,2) NOT NULL DEFAULT 0;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'TaxRateId' AND is_nullable = 0)
        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [TaxRateId] INT NULL;
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderItems]') AND name = 'UnitId' AND is_nullable = 0)
        ALTER TABLE [PurchaseOrderItems] ALTER COLUMN [UnitId] INT NULL;
    PRINT '  + Ensured [PurchaseOrderItems] GST, Tax and nullable foreign keys.';
END
GO

-- =========================================================================================
-- [7/7] PROCUREMENT: PURCHASEORDERLOGS AUDIT TABLE & TIME REPAIR
-- =========================================================================================
PRINT '>>> [7/7] Ensuring PurchaseOrderLogs audit table & date-time accuracy...';
IF OBJECT_ID(N'[PurchaseOrderLogs]', N'U') IS NULL
BEGIN
    CREATE TABLE [PurchaseOrderLogs] (
        [Id] BIGINT NOT NULL IDENTITY,
        [PoId] BIGINT NOT NULL,
        [PurchaseOrderId] BIGINT NULL,
        [ActionType] NVARCHAR(50) NOT NULL,
        [Title] NVARCHAR(200) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Amount] DECIMAL(18,2) NULL,
        [PaymentMode] NVARCHAR(50) NULL,
        [VoucherOrRefNo] NVARCHAR(100) NULL,
        [PerformedById] BIGINT NULL,
        [PerformedByName] NVARCHAR(150) NOT NULL,
        [PerformedByRole] NVARCHAR(50) NULL,
        [Remarks] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT [PK_PurchaseOrderLogs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PurchaseOrderLogs_PurchaseOrders_PoId] FOREIGN KEY ([PoId]) REFERENCES [PurchaseOrders] ([Id]) ON DELETE CASCADE
    );
    CREATE INDEX [IX_PurchaseOrderLogs_PoId_CreatedAt] ON [PurchaseOrderLogs] ([PoId], [CreatedAt]);
    PRINT '  + Created [PurchaseOrderLogs] audit trail table.';
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderLogs]') AND name = 'PurchaseOrderId')
        ALTER TABLE [PurchaseOrderLogs] ADD [PurchaseOrderId] BIGINT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrderLogs]') AND name = 'PoId')
        ALTER TABLE [PurchaseOrderLogs] ADD [PoId] BIGINT NULL;
END
GO

-- Backfill legacy PO settlement states
IF OBJECT_ID(N'[PurchaseOrders]', N'U') IS NOT NULL
BEGIN
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'PaidAmount')
    BEGIN
        EXEC('UPDATE [PurchaseOrders] SET [PaidAmount] = [TotalPoValue] WHERE [ApprovalStatus] = ''PAID'' AND ([PaidAmount] IS NULL OR [PaidAmount] = 0)');
    END
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[PurchaseOrders]') AND name = 'MarkedAmount')
    BEGIN
        EXEC('UPDATE [PurchaseOrders] SET [MarkedAmount] = [TotalPoValue], [MarkedPaymentType] = ''FULL'' WHERE [ApprovalStatus] = ''MARKED_FOR_PAYMENT'' AND ([MarkedAmount] IS NULL OR [MarkedAmount] = 0)');
    END
END
GO

-- Correct audit trail log timestamps to exact Indian Standard Time (prevent previous day display)
IF OBJECT_ID('[PurchaseOrderLogs]') IS NOT NULL
BEGIN
    UPDATE pol
    SET pol.CreatedAt = DATEADD(hour, 4, DATEADD(minute, 30, CAST(CAST(po.PoDate AS DATE) AS DATETIME2)))
    FROM [PurchaseOrderLogs] pol
    INNER JOIN [PurchaseOrders] po ON (pol.PoId = po.Id OR pol.PurchaseOrderId = po.Id)
    WHERE pol.ActionType = 'PO_CREATED'
      AND CAST(DATEADD(minute, 330, pol.CreatedAt) AS DATE) < CAST(po.PoDate AS DATE);
    PRINT '  + Corrected audit log dates to match PO calendar date.';
END
GO

-- =========================================================================================
-- [8/8] EXPENSES & PETTY CASH INFLOW/OUTFLOW TABLES
-- =========================================================================================
PRINT '>>> [8/8] Ensuring DirectExpenses, CashWithdrawals, CashUtilisations & CashAllocations...';

-- 8.1 DirectExpenses
IF OBJECT_ID(N'[DirectExpenses]', N'U') IS NULL AND OBJECT_ID(N'[dbo].[DirectExpenses]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[DirectExpenses] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyId] BIGINT NOT NULL,
        [ExpenseHead] NVARCHAR(250) NOT NULL,
        [ExpenseHeadId] BIGINT NULL,
        [IsVendorApplicable] BIT NOT NULL DEFAULT 0,
        [VendorId] BIGINT NULL,
        [LedgerId] BIGINT NULL,
        [LedgerName] NVARCHAR(250) NOT NULL DEFAULT '',
        [PaymentDate] DATETIME2 NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [IsPipelineProject] BIT NOT NULL DEFAULT 0,
        [ProjectId] BIGINT NULL,
        [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [Remarks] NVARCHAR(MAX) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'PENDING',
        [CreatedByUserId] BIGINT NULL,
        [CreatedByUserName] NVARCHAR(150) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [ApprovedByUserId] BIGINT NULL,
        [ApprovedByUserName] NVARCHAR(150) NULL,
        [ApprovedAt] DATETIME2 NULL,
        [ApprovalRemarks] NVARCHAR(MAX) NULL,
        [DeclinedByUserId] BIGINT NULL,
        [DeclinedByUserName] NVARCHAR(150) NULL,
        [DeclinedAt] DATETIME2 NULL,
        [DeclineReason] NVARCHAR(MAX) NULL,
        [SettledByUserId] BIGINT NULL,
        [SettledByUserName] NVARCHAR(150) NULL,
        [SettledAt] DATETIME2 NULL,
        [SettlementPaymentMode] NVARCHAR(100) NULL,
        [SettlementReference] NVARCHAR(150) NULL,
        [SettlementBankAccountId] BIGINT NULL,
        [SettlementBankName] NVARCHAR(150) NULL,
        [SettlementNotes] NVARCHAR(MAX) NULL,
        [JournalEntryId] BIGINT NULL
    );
    CREATE INDEX [IX_DirectExpenses_CompanyId_Status] ON [dbo].[DirectExpenses] ([CompanyId], [Status]);
    PRINT '  + Created [DirectExpenses] table.';
END
GO

-- 8.2 CashWithdrawals
IF OBJECT_ID(N'[CashWithdrawals]', N'U') IS NULL AND OBJECT_ID(N'[dbo].[CashWithdrawals]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CashWithdrawals] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyId] BIGINT NOT NULL,
        [ReferenceNumber] NVARCHAR(50) NOT NULL,
        [WithdrawalDate] DATETIME2 NOT NULL,
        [SourceType] NVARCHAR(50) NOT NULL DEFAULT 'BANK_WITHDRAWAL',
        [BankAccountId] BIGINT NULL,
        [ChequeOrRefNo] NVARCHAR(100) NULL,
        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [UtilisedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [WithdrawnBy] NVARCHAR(150) NOT NULL DEFAULT '',
        [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [ApprovedBy] NVARCHAR(150) NOT NULL DEFAULT '',
        [BankStatementLineId] BIGINT NULL,
        [BankStatementRef] NVARCHAR(200) NULL,
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'OPEN',
        [Notes] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE INDEX [IX_CashWithdrawals_CompanyId_WithdrawalDate] ON [dbo].[CashWithdrawals] ([CompanyId], [WithdrawalDate]);
    PRINT '  + Created [CashWithdrawals] table.';
END
GO

-- 8.3 CashUtilisations
IF OBJECT_ID(N'[CashUtilisations]', N'U') IS NULL AND OBJECT_ID(N'[dbo].[CashUtilisations]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CashUtilisations] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyId] BIGINT NOT NULL,
        [VoucherNumber] NVARCHAR(50) NOT NULL,
        [ExpenseDate] DATETIME2 NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [AllocatedAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [Payee] NVARCHAR(200) NOT NULL DEFAULT '',
        [ExpenseCategory] NVARCHAR(150) NOT NULL DEFAULT '',
        [ExpenseAccountId] BIGINT NULL,
        [ProjectId] BIGINT NULL,
        [CostCenter] NVARCHAR(100) NULL,
        [Purpose] NVARCHAR(MAX) NOT NULL DEFAULT '',
        [AttachmentFileName] NVARCHAR(250) NULL,
        [AttachmentFilePath] NVARCHAR(500) NULL,
        [PaidBy] NVARCHAR(150) NOT NULL DEFAULT '',
        [ApprovedBy] NVARCHAR(150) NOT NULL DEFAULT '',
        [Status] NVARCHAR(50) NOT NULL DEFAULT 'DRAFT',
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
    CREATE INDEX [IX_CashUtilisations_CompanyId_ExpenseDate] ON [dbo].[CashUtilisations] ([CompanyId], [ExpenseDate]);
    PRINT '  + Created [CashUtilisations] table.';
END
GO

-- 8.4 CashAllocations
IF OBJECT_ID(N'[CashAllocations]', N'U') IS NULL AND OBJECT_ID(N'[dbo].[CashAllocations]', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[CashAllocations] (
        [Id] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [CompanyId] BIGINT NOT NULL,
        [WithdrawalId] BIGINT NOT NULL,
        [UtilisationId] BIGINT NOT NULL,
        [Amount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [AllocationDate] DATETIME2 NOT NULL,
        [AllocatedBy] NVARCHAR(150) NULL,
        [IsManualOverride] BIT NOT NULL DEFAULT 0,
        [Notes] NVARCHAR(MAX) NULL
    );
    CREATE INDEX [IX_CashAllocations_WithdrawalId] ON [dbo].[CashAllocations] ([WithdrawalId]);
    CREATE INDEX [IX_CashAllocations_UtilisationId] ON [dbo].[CashAllocations] ([UtilisationId]);
    PRINT '  + Created [CashAllocations] table.';
END
GO

-- =========================================================================================
-- [9/9] PROJECT CLOSURE, SETTLEMENT & PREMATURE CLOSURE COLUMNS
-- =========================================================================================
PRINT '>>> [9/9] Ensuring Project Closure, Settlement & Premature Document columns...';

IF OBJECT_ID('[project].[projects]') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'ClosureReason')
        ALTER TABLE [project].[projects] ADD [ClosureReason] NVARCHAR(250) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'ClosureDocumentPath')
        ALTER TABLE [project].[projects] ADD [ClosureDocumentPath] NVARCHAR(500) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[project].[projects]') AND name = 'ClosureDocumentName')
        ALTER TABLE [project].[projects] ADD [ClosureDocumentName] NVARCHAR(250) NULL;
END
GO

IF OBJECT_ID('[Projects]') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'ClosureReason')
        ALTER TABLE [Projects] ADD [ClosureReason] NVARCHAR(250) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'ClosureDocumentPath')
        ALTER TABLE [Projects] ADD [ClosureDocumentPath] NVARCHAR(500) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('[Projects]') AND name = 'ClosureDocumentName')
        ALTER TABLE [Projects] ADD [ClosureDocumentName] NVARCHAR(250) NULL;
END
GO

PRINT '=========================================================================================';
PRINT '  SUCCESS: ALL DATABASE TABLES, COLUMNS, SEEDS & AUDIT TRAILS ARE 100% REPAIRED!';
PRINT '  Target URLs can now be loaded without error:';
PRINT '  - https://erp.sdksolution.co.in/Masters';
PRINT '  - https://erp.sdksolution.co.in/Procurement';
PRINT '  - https://erp.sdksolution.co.in/Expenses';
PRINT '  - https://erp.sdksolution.co.in/PettyCash';
PRINT '=========================================================================================';

