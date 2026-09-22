import os
import docx
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx_helpers import (
    add_header_footer, add_title, add_subtitle, add_h1, add_h2, add_h3,
    add_p, add_bullet, add_callout, add_styled_table, add_diagram,
    set_cell_background, set_cell_margins
)

def build_implementation_plan_doc():
    doc = docx.Document()
    
    # Configure custom Header & Footer for Implementation Plan
    for s in doc.sections:
        s.top_margin = Inches(0.8)
        s.bottom_margin = Inches(0.8)
        s.left_margin = Inches(0.8)
        s.right_margin = Inches(0.8)
        
        # Header
        header = s.header
        hp = header.paragraphs[0]
        hp.alignment = WD_ALIGN_PARAGRAPH.RIGHT
        hrun = hp.add_run("SDK SOLUTIONS ERP — .NET Core MVC & SQL Server Implementation Plan")
        hrun.font.name = "Calibri"
        hrun.font.size = Pt(8.5)
        hrun.font.color.rgb = RGBColor(120, 144, 156)
        
        # Footer
        footer = s.footer
        fp = footer.paragraphs[0]
        fp.alignment = WD_ALIGN_PARAGRAPH.CENTER
        frun = fp.add_run("SDK Solutions Confidential  |  Technical Delivery Roadmap & Security Audit Baseline  |  Version 2.0")
        frun.font.name = "Calibri"
        frun.font.size = Pt(8.5)
        frun.font.color.rgb = RGBColor(120, 144, 156)

    # -------------------------------------------------------------
    # COVER / TITLE
    # -------------------------------------------------------------
    add_title(doc, "SDK SOLUTIONS ERP SYSTEM")
    add_subtitle(doc, "Comprehensive Technical Implementation Plan & Delivery Roadmap\nASP.NET Core MVC (C#), Entity Framework Core, SQL Server & Security Audit Baseline")
    
    # Executive Metadata Table
    meta_headers = ["Document Attribute", "Specification Details"]
    meta_rows = [
        ["Document Type", "Technical Implementation Plan & Security Audit Delivery Baseline"],
        ["Target Platform", "ASP.NET Core MVC (.NET 8 / .NET 9 LTS) + C# 12/13"],
        ["Database Tier", "Microsoft SQL Server 2022 (48 Tables in 3NF, 10 Domain Schemas, B-Tree Indexes)"],
        ["Data Access Layer", "Entity Framework Core 8/9 (Code-First Fluent API) + Dapper (High-Speed Reporting)"],
        ["Primary Operating Model", "Project-Centric Operations, Double-Entry Accounting & Indian GST Compliance"],
        ["Security & Compliance Scope", "OWASP Top 10, ASVS Level 2, VAPT Penetration Testing, AES-256 Encryption & Tamper-Evident Audit"],
        ["Document Status", "Approved Baseline for Software Engineering & Quality Assurance"]
    ]
    add_styled_table(doc, meta_headers, meta_rows, [2.2, 4.4])
    
    # Revision History Table
    add_h2(doc, "Document Control & Revision History")
    rev_headers = ["Version", "Release Date", "Primary Author", "Summary of Major Changes"]
    rev_rows = [
        ["1.0", "September 2026", "Enterprise Solutions Team", "Initial implementation plan for core functional modules."],
        ["2.0", "September 2026", "Principal Systems Architect", "Tailored to .NET Core MVC, C#, EF Core, and Microsoft SQL Server. Embedded Security-by-Design across all phases and established Phase 11 for VAPT and Security Audit Certification."]
    ]
    add_styled_table(doc, rev_headers, rev_rows, [0.8, 1.2, 1.8, 2.8])

    # Abbreviations & Acronyms Glossary Table
    add_h2(doc, "Glossary of Abbreviations & Acronyms")
    add_p(doc, "The following reference table defines all technical, architectural, financial, statutory, and security abbreviations utilized throughout this implementation document:")

    abbr_headers = ["Abbreviation", "Full Term / Expansion", "Domain & Contextual Definition in SDK ERP"]
    abbr_rows = [
        ["3NF", "Third Normal Form", "Database normalization standard ensuring zero transitive functional dependencies across 48 tables."],
        ["3-Way Match", "Three-Way Matching", "Automated reconciliation matching Purchase Order (PO), Goods Receipt (GRN), and Vendor Bill quantities/rates."],
        ["AES-256", "Advanced Encryption Standard (256-bit)", "Cryptographic standard used for column-level encryption of sensitive data (PAN, Aadhaar, Bank Details)."],
        ["AP", "Accounts Payable", "Sub-ledger tracking outstanding liabilities owed to vendors and suppliers for goods and services."],
        ["AR", "Accounts Receivable", "Sub-ledger tracking money owed to the company by clients for billed tax invoices."],
        ["ASVS", "Application Security Verification Standard", "OWASP standard (Level 2) defining enterprise application security requirements and verification metrics."],
        ["BRS", "Bank Reconciliation Statement", "System process matching bank statement records (CSV/OFX) against internal general ledger cash books."],
        ["CGST", "Central Goods and Services Tax", "Tax levied by the Central Government of India on intra-state supplies of goods and services."],
        ["COA", "Chart of Accounts", "5-level hierarchical master structure of all general ledger accounts (Assets, Liabilities, Equity, Revenue, Expenses)."],
        ["CSRF", "Cross-Site Request Forgery", "Web security vulnerability mitigated via ASP.NET Core [ValidateAntiForgeryToken] on all state-changing HTTP requests."],
        ["DAST", "Dynamic Application Security Testing", "Runtime black-box automated security vulnerability scanning (e.g., OWASP ZAP active scanning)."],
        ["DDD", "Domain-Driven Design", "Software design approach structuring software around domain models, entities, and business logic."],
        ["DDL", "Data Definition Language", "SQL commands (CREATE, ALTER, DROP) that define database schemas, tables, and constraints."],
        ["EF Core", "Entity Framework Core", "Microsoft modern Object-Relational Mapper (ORM) for .NET executing transactional database operations."],
        ["ERP", "Enterprise Resource Planning", "Integrated management system managing core business processes across projects, finance, sales, and supply chain."],
        ["GL", "General Ledger", "Primary accounting record storing all debit and credit journal entries in permanent double-entry balance."],
        ["GRN", "Goods Receipt Note", "Warehouse document acknowledging receipt and quality inspection of procured items against a Purchase Order."],
        ["GST", "Goods and Services Tax", "Comprehensive indirect tax levied on manufacture, sale, and consumption of goods and services in India."],
        ["GSTIN", "GST Identification Number", "15-digit unique alphanumeric state-wise tax identifier assigned to registered business entities in India."],
        ["GSTR-1", "GST Return 1", "Monthly/Quarterly statutory statement detailing outward supplies and sales tax invoices."],
        ["GSTR-2B", "GST Return 2B", "Auto-drafted read-only Input Tax Credit (ITC) statement generated by the GST Portal from supplier filings."],
        ["GSTR-3B", "GST Return 3B", "Monthly self-declared summary GST return declaring tax liabilities, claiming ITC, and recording net tax paid."],
        ["HSN / SAC", "Harmonized System of Nomenclature / Services Accounting Code", "Standardized commodity/service classification codes mandated for statutory GST invoicing."],
        ["HSTS", "HTTP Strict Transport Security", "Security header forcing browsers to communicate exclusively over encrypted HTTPS connections."],
        ["IDOR", "Insecure Direct Object Reference", "Access control vulnerability prevented by enforcing tenant and branch context filters on all DB queries."],
        ["IFSC", "Indian Financial System Code", "11-character alphanumeric code identifying bank branches for NEFT, RTGS, and IMPS fund transfers."],
        ["IGST", "Integrated Goods and Services Tax", "Tax levied on inter-state supplies of goods and services and imports into India."],
        ["ITC", "Input Tax Credit", "Taxes paid on business purchase inputs that can be legally offset against output tax liability on sales."],
        ["JV", "Journal Voucher", "Fundamental double-entry accounting record containing balanced debit and credit ledger postings."],
        ["MFA / TOTP", "Multi-Factor Authentication / Time-Based One-Time Password", "Two-step verification mechanism generating dynamic 6-digit codes for privileged administrative roles."],
        ["MSME", "Micro, Small and Medium Enterprises", "Government classification determining vendor statutory payment priority and reporting compliance."],
        ["MVC", "Model-View-Controller", "Software architectural pattern separating data models, user interface views, and controller logic."],
        ["OWASP", "Open Web Application Security Project", "International non-profit organization focused on improving software security and publishing top risk standards."],
        ["P&L", "Profit and Loss Statement", "Financial statement summarizing revenues, direct costs, and expenses to determine project/company net profitability."],
        ["PAN", "Permanent Account Number", "10-digit alphanumeric identifier issued by the Indian Income Tax Department to business entities and individuals."],
        ["PF / PT", "Provident Fund / Professional Tax", "Mandatory statutory payroll deductions contributed towards employee retirement and state tax."],
        ["PO", "Purchase Order", "Legally binding commercial document issued by buyer to vendor specifying quantities and agreed prices."],
        ["POCO", "Plain Old CLR Object", "Pure C# entity class unencumbered by inheritance from framework-specific base classes."],
        ["PR", "Purchase Requisition", "Internal document created by project teams to formally request procurement of materials or services."],
        ["RBAC", "Role-Based Access Control", "Security mechanism restricting system access based on user role assignments and granular permissions."],
        ["RCM", "Reverse Charge Mechanism", "GST rule where recipient of goods/services is liable to pay tax instead of the supplier."],
        ["SAST", "Static Application Security Testing", "White-box source code security analysis detecting vulnerabilities and hardcoded secrets (Roslyn, SonarQube)."],
        ["SGST", "State Goods and Services Tax", "Tax levied by Indian State Governments on intra-state supplies of goods and services."],
        ["SHA-256", "Secure Hash Algorithm (256-bit)", "Cryptographic hashing function used in audit logging to generate tamper-evident chained audit records."],
        ["SQLi", "SQL Injection", "Database security vulnerability eliminated via 100% parameterized EF Core LINQ and Dapper @param queries."],
        ["SRS", "Software Requirements Specification", "Definitive technical requirements document detailing functional, database, and system specifications."],
        ["STRIDE", "Threat Modeling Framework", "Security framework analyzing Spoofing, Tampering, Repudiation, Info Disclosure, DoS, and Elevation of Privilege."],
        ["TB", "Trial Balance", "Accounting report listing all general ledger account balances to verify mathematical debit/credit equilibrium."],
        ["TDS", "Tax Deducted at Source", "Direct taxation withholding mechanism (e.g., Section 194C for contractors, Section 194J for professional fees)."],
        ["UAT", "User Acceptance Testing", "Final validation phase where end users verify workflows against formal business test scenarios."],
        ["UTR", "Unique Transaction Reference", "Banking reference number identifying electronic fund transfers (NEFT/RTGS) for payment reconciliation."],
        ["VAPT", "Vulnerability Assessment & Penetration Testing", "Comprehensive security auditing methodology combining automated vulnerability scans and manual ethical hacking."],
        ["WDV", "Written Down Value", "Asset accounting depreciation method calculating book value reduction based on asset lifespan and usage."],
        ["XSS", "Cross-Site Scripting", "Client-side code injection attack mitigated by automatic Razor HTML output encoding."]
    ]
    add_styled_table(doc, abbr_headers, abbr_rows, [1.4, 2.2, 3.0])
    
    doc.add_page_break()

    # -------------------------------------------------------------
    # SECTION 1: EXECUTIVE SUMMARY & ARCHITECTURAL STRATEGY
    # -------------------------------------------------------------
    add_h1(doc, "1. Executive Summary & Architectural Strategy")
    add_p(doc, "This document establishes the definitive, engineering-grade implementation plan to build, test, secure, and deploy the SDK Solutions Enterprise Resource Planning (ERP) System. The development baseline strictly implements the 48-table relational schema and 11 functional modules defined in the Master SRS specification.")
    add_p(doc, "The system is engineered using ASP.NET Core MVC (C#) and Microsoft SQL Server 2022 following Clean Architecture and Domain-Driven Design (DDD) principles. The architecture guarantees transactional atomicity, double-entry mathematical balance, multi-branch tenant isolation, Indian GST statutory compliance, and rigorous security hardening designed to pass third-party Vulnerability Assessment and Penetration Testing (VAPT) and ISO 27001/SOC 2 audits.")

    add_callout(doc, "This implementation plan incorporates Security-by-Design at every layer. Developers must strictly adhere to the parameterized query standards, IDOR tenant isolation filters, cryptographic data protection rules, and tamper-evident audit logging mechanisms detailed herein.", "MANDATORY SECURITY & ENGINEERING BASELINE")

    add_h2(doc, "1.1 Multi-Tier Technical Architecture Matrix (.NET Core & SQL Server)")
    add_p(doc, "The following architectural matrix details the technical implementation specifications, core responsibilities, and security controls across all 7 architectural layers:")

    arch_headers = ["Architecture Tier", "Framework & Technology", "Key Components & Domain Responsibilities", "Security & Reliability Controls"]
    arch_rows = [
        ["Tier 1: Presentation & Web", "ASP.NET Core MVC (C# .NET 8/9)\nRazor Views & ViewComponents\nBootstrap 5 + DataTables.net", "• 11 Functional Module MVC Controllers\n• Dense 11-Tab Project Operational ViewComponent\n• Partial Views for dynamic line items (Quotes, Orders, Invoices)\n• Responsive AJAX modals & Toastr alert notifications", "• Anti-Forgery Tokens ([ValidateAntiForgeryToken])\n• Contextual HTML Output Encoding (Anti-XSS)\n• Client-side unobtrusive validation\n• Secure HTTP-only session cookies"],
        ["Tier 2: Security & Middleware", "ASP.NET Core Identity\nCustom Action Filters\nRate Limiting Middleware", "• Multi-Factor Authentication (TOTP for Admin/Approvers)\n• [BranchContextFilter] for multi-tenant isolation\n• [HasPermission] custom AuthorizationHandler & claims\n• Global Exception Handling & Security Headers Middleware", "• OWASP ASVS Level 2 compliance\n• Brute-force rate limiting on login (5 attempts)\n• Sliding cookies (SameSite=Strict, Secure)\n• Strict HSTS & Content Security Policy (CSP)"],
        ["Tier 3: Business Application", "C# Business Services\nFluentValidation\nAutoMapper DTOs", "• Invoicing & GST Tax Calculation Engine\n• 3-Way Match Verification (PO vs GRN vs Bill)\n• Double-Entry Balanced JV Posting Engine\n• Multi-Level Unified Approval State Machine\n• Bank Statement Reconciliation (BRS) Engine\n• Monthly Batch Payroll Calculation Engine", "• Transaction boundary isolation (BeginTransactionAsync)\n• Strict domain invariant validation rules\n• IDOR defense on all entity lookups\n• Thread-safe voucher sequence generator"],
        ["Tier 4: Domain & Entities", "C# POCO Entities\nDomain Enums & Value Objects", "• 48 POCO Entities organized across 10 Schemas\n• Double-entry posting interfaces & domain events\n• Zero external library dependencies (Pure Domain)", "• Immutable entity audit fields (created_at, created_by)\n• Strict Domain-level status state machines\n• Business invariant validation"],
        ["Tier 5: Persistence & ORM", "EF Core 8/9 (Fluent API)\nDapper ORM\nAudit Interceptor", "• ApplicationDbContext with 48 entity mappings\n• EF Core Global Query Filters for tenant isolation\n• SaveChangesInterceptor for SHA-256 hash chaining\n• Dapper for high-speed financial reporting & aging", "• 100% Parameterized queries (SQLi immune)\n• Cascade delete prevention (DeleteBehavior.Restrict)\n• Tamper-evident ledger record chaining\n• Concurrency tokens on balance modifications"],
        ["Tier 6: Database Storage", "Microsoft SQL Server 2022\n10 Relational Schemas\nB-Tree Indexes", "• 48 Relational Tables structured in 3NF\n• Composite B-Tree Non-Clustered Indexes\n• Temporal Audit Tables & Sequence Objects\n• Least-privilege DB runtime user roles", "• Encrypted connections (TLS 1.3 / SSL)\n• Column-level AES-256 for PII & Bank details\n• Daily automated backup with checksum\n• Period locking constraints"],
        ["Tier 7: Documents & Storage", "QuestPDF\nClosedXML\nAzure Key Vault & Blob", "• QuestPDF statutory Tax Invoices, Payslips, and POs\n• ClosedXML multi-tab financial Excel workbooks\n• Secure binary magic-number file storage service\n• External Key Vault for configuration secrets", "• Zero hardcoded secrets in source code\n• Executable file upload rejection (.exe/.php)\n• Randomized GUID storage keys\n• SHA-256 file checksum validation"]
    ]
    add_styled_table(doc, arch_headers, arch_rows, [1.3, 1.4, 2.5, 1.6])

    add_h2(doc, "1.2 Technical Stack Summary")
    add_bullet(doc, "ASP.NET Core MVC with Razor Views, Partial Views, and reusable ViewComponents for dense multi-tab operational workspaces.", "1. Web & Presentation Layer: ")
    add_bullet(doc, "Encapsulates all domain workflows, approval state machines, financial double-entry posting adapters, GST tax calculations, and FluentValidation rules.", "2. Application Business Services: ")
    add_bullet(doc, "Contains 48 POCO Entities, domain enums, and posting interfaces with zero external infrastructure dependencies.", "3. Domain Entity Layer: ")
    add_bullet(doc, "Entity Framework Core 8/9 with explicit Fluent API mappings for transactional integrity, combined with Dapper for high-speed financial reporting queries.", "4. Persistence & Data Access: ")
    add_bullet(doc, "Microsoft SQL Server 2022 structured into 10 domain schemas with composite B-Tree non-clustered indexes, table partitioning, and least-privilege runtime user roles.", "5. Relational Database: ")
    add_bullet(doc, "QuestPDF for pixel-perfect PDF Tax Invoices, Purchase Orders, and Payslips; ClosedXML for multi-tab Excel financial reporting.", "6. Reporting & Document Engine: ")

    # -------------------------------------------------------------
    # SECTION 2: SOLUTION STRUCTURE & VISUAL ARCHITECTURE
    # -------------------------------------------------------------
    add_h1(doc, "2. Solution Structure & Project Organization")
    add_p(doc, "The codebase is organized into a modular, clean N-Tier Visual Studio Solution (SDK.ERP.sln):")
    
    sol_headers = ["Project Name", "Namespace", "Core Responsibilities & Components"]
    sol_rows = [
        ["SDK.ERP.Domain", "SDK.ERP.Domain", "48 POCO Entities (10 Domain folders), Enums, Value Objects, Domain Exceptions, and Repository Interfaces."],
        ["SDK.ERP.Application", "SDK.ERP.Application", "Business Services (Invoicing, Accounting, Procurement, GST, Payroll), DTOs, ViewModels, FluentValidation, AutoMapper."],
        ["SDK.ERP.Infrastructure", "SDK.ERP.Infrastructure", "ApplicationDbContext, 48 Fluent API Configurations, EF Core Migrations, Dapper queries, AES-256 Encryptors, S3/Azure Blob Storage, TamperEvidentAuditInterceptor."],
        ["SDK.ERP.Web", "SDK.ERP.Web", "11 Module MVC Controllers, Razor Views, ViewComponents, Action Filters ([BranchContextFilter], [HasPermission], [AuditLogFilter]), Security Middleware."],
        ["SDK.ERP.UnitTests", "SDK.ERP.UnitTests", "Business rule, GST Tax calculation, 3-Way Match, and Double-Entry balanced ledger unit tests."],
        ["SDK.ERP.SecurityTests", "SDK.ERP.SecurityTests", "Automated IDOR tenant isolation tests, RBAC privilege escalation tests, Anti-CSRF verification, and SQLi automated scans."]
    ]
    add_styled_table(doc, sol_headers, sol_rows, [1.6, 1.6, 3.4])

    add_h2(doc, "2.1 End-to-End Operational & Financial Lifecycle Workflow Matrix")
    add_p(doc, "The table below establishes the complete, project-centric lifecycle connecting commercial initiation, procurement, project delivery, billing, collections, and final closure:")

    wf_headers = ["Lifecycle Stage", "Initiating Operational Action", "Primary System Workflows & Validations", "Accounting, Inventory & Audit Impact"]
    wf_rows = [
        ["1. Commercial Initiation", "Sales Team receives Client RFP / Inquiry", "• Generate formal Quotation with GST tax split\n• Dynamic line-item pricing and discount controls\n• Convert approved Quotation into confirmed Sales Order\n• Validate Client credit limit and active status", "• Status: DRAFT -> SENT -> CONFIRMED\n• Pre-allocates commercial pipeline\n• Zero immediate General Ledger impact"],
        ["2. Project Workspace Setup", "Operations Lead creates new Project", "• Initialize 11-Tab Project Operational Workspace\n• Link confirmed Sales Order and attach Client PO\n• Configure billing delivery milestones and deadlines\n• Establish project cost baseline and margin target", "• Generates unique Project Code (PRJ-YYYY-XXXX)\n• Sets project status to ACTIVE\n• Establishes baseline for real-time P&L tracking"],
        ["3. Procurement & 3-Way Match", "Project Team initiates Material Requisition", "• Create Purchase Requisition (PR) -> Approval routing\n• Issue Purchase Order (PO) to Vendor via QuestPDF\n• Warehouse records Goods Receipt Note (GRN) with QC\n• System executes automated 3-Way Match (PO-GRN-Bill)", "• Discrepancies block bill booking\n• Auto-posts Accounts Payable (AP) sub-ledger\n• Consumables auto-credited to Inventory Register"],
        ["4. Execution & Fulfillment", "Project Team executes on-site delivery", "• Requisition internal consumables via Stock Issue slip\n• Submit direct project expense claims with receipts\n• Generate Delivery Challan with serial tracking\n• Client signs Delivery Acknowledgment", "• Material cost debited to Project Direct Costs\n• Expense claim auto-posts balanced JV upon approval\n• Perpetual inventory balance decreased in real-time"],
        ["5. Statutory Tax Invoicing", "Delivery Milestone achieved / Billing due", "• Generate Sales Tax Invoice against project milestone\n• Auto-compute CGST/SGST (Intra-state) vs IGST (Inter-state)\n• Render statutory PDF invoice via QuestPDF\n• Dispatch invoice to client with HSN breakdown", "• Auto-posts: Dr Accounts Receivable / Cr Sales Revenue & Cr GST Output Tax\n• Generates real-time GstTransaction entry\n• Increases project Billed Revenue"],
        ["6. Multi-Invoice Receipts", "Finance Team receives Client NEFT/RTGS", "• Record Customer Receipt with UTR bank reference\n• Interactive AJAX grid allocates receipt across invoices\n• Auto-deduct Section 194C/J TDS and cash discounts\n• Update real-time AR aging buckets (<30, 60, 90+ days)", "• Auto-posts: Dr Bank Account & Dr TDS Receivable / Cr Accounts Receivable\n• Closes or reduces open invoice balances\n• Updates Client outstanding balance"],
        ["7. Treasury & Compliance", "Monthly fiscal closing by Tax Accountant", "• Import bank statement (CSV/OFX) for BRS reconciliation\n• Aggregate GSTR-1 outward tax summaries\n• Ingest government GSTR-2B JSON for 4-way ITC match\n• Compute GSTR-3B net tax payable and set-off", "• Locks reconciled bank ledger periods\n• Reconciles Input Tax Credit (ITC) against portal\n• Auto-generates statutory tax payment voucher"],
        ["8. Project Closure & Final P&L", "Project Manager initiates project sign-off", "• Execute 6-Point Pre-Closure Validation Checklist\n• Verify 100% invoices settled, zero pending GRNs/bills\n• Lock project against post-closure modifications\n• Generate consolidated Project Profitability Report", "• Project status locked to CLOSED\n• Direct modifications strictly rejected by API filters\n• Final Project P&L locked for executive review"]
    ]
    add_styled_table(doc, wf_headers, wf_rows, [1.2, 1.4, 2.6, 1.6])

    # -------------------------------------------------------------
    # SECTION 3: STEP-BY-STEP IMPLEMENTATION PHASES (PHASES 0 TO 11)
    # -------------------------------------------------------------
    add_h1(doc, "3. Step-by-Step Implementation Roadmap (Phases 0 to 11)")
    add_p(doc, "The technical delivery of the SDK Solutions ERP is structured into 12 sequential, dependency-ordered phases. Each phase establishes functional modules alongside embedded security-by-design controls, ensuring complete audit readiness upon system completion.")
    
    add_h2(doc, "3.0 Master 12-Phase Implementation Roadmap & Dependency Matrix")
    add_p(doc, "The following editable master roadmap specifies the complete 12-phase technical delivery lifecycle, detailing concrete engineered deliverables, embedded security controls, and dependency gates across all functional tiers:")

    roadmap_headers = ["Phase", "Domain Scope & Focus Tier", "Key Engineered Deliverables & Artifacts", "Embedded Security Controls & Gates", "Prerequisites"]
    roadmap_rows = [
        ["Phase 0", "Solution Scaffolding & DB DDL\n(Foundational Tier)", "• Visual Studio SDK.ERP.sln multi-project scaffolding\n• 48 C# POCO Entities organized across 10 Schemas\n• 48 EF Core Fluent API Configurations (3NF mappings)\n• Composite B-Tree Non-Clustered Indexes & DDL\n• Master Seeding (Roles, Tax Slabs, standard COA)", "• Roslyn Security Analyzers enabled\n• Gitleaks pre-commit hooks configured\n• Cascade delete prevention (DeleteBehavior.Restrict)\n• Zero plaintext DB credentials in code", "None\n(Start Phase)"],
        ["Phase 1", "Identity, RBAC & Core Engines\n(Security Baseline)", "• ASP.NET Core Identity with secure sliding cookie session\n• Multi-Factor Authentication (TOTP) for Admins/Approvers\n• [BranchContextFilter] & Global Tenant Query Filters\n• Granular [HasPermission] custom AuthorizationHandler\n• Unified Multi-Level Approval State Machine\n• Tamper-Evident SaveChangesInterceptor (SHA-256)", "• OWASP ASVS Level 2 Auth checks\n• IDOR Cross-Tenant Isolation validation\n• SHA-256 Chained Hash Audit Trail\n• Secure binary magic-number file validator", "Phase 0"],
        ["Phase 2", "Master Data Management\n(Core Masters Tier)", "• Organization & Branch Masters with Fiscal Period Locking\n• Client Master (DataTables.net server-side pagination)\n• Vendor Master (MSME classification & Bank IFSC)\n• Item Catalog (Consumables vs Durable Asset flag)\n• IVoucherSequenceService thread-safe numbering", "• Strict regex validation (GSTIN, PAN, IFSC)\n• Output encoding against XSS attacks\n• Model validation with FluentValidation\n• Anti-tamper master record concurrency", "Phase 1"],
        ["Phase 3", "General Ledger & Posting\n(Financial Core)", "• 5-Level Hierarchical Chart of Accounts Tree View\n• IAccountingPostingService (Balanced Debit/Credit JVs)\n• Automated Posting Matrix (Invoices, Bills, Receipts, Stock)\n• Immutable Ledger Architecture (Reversing JVs only)\n• Dapper High-Speed Trial Balance & GL Statements", "• Strict balance enforcement (Sum Dr == Sum Cr)\n• Physical SQL DELETE permanently prohibited\n• Period Locking filter prevents backdated entries\n• Decimal(15,2) mathematical rounding precision", "Phase 1, 2"],
        ["Phase 4", "Projects & 11-Tab Hub\n(Operational Core)", "• ProjectController with 11 Razor ViewComponents\n• Client PO Attachment & Delivery Milestone Tracker\n• Direct Project Expense Claims & Receipt Upload\n• Delivery Challan Generator with Courier Tracking\n• Real-Time Project P&L Calculation Engine\n• 6-Point Pre-Closure Checklist & Closure Lock", "• Project-level IDOR access filters\n• Closure immutability (Post-closure edits blocked)\n• Expense receipt magic-number verification\n• Strict separation of billable vs internal costs", "Phase 2, 3"],
        ["Phase 5", "Sales, Billing & AR\n(Commercial Engine)", "• Quotation Line-Item Builder & Sales Order Conversion\n• Statutory Tax Invoicing (CGST/SGST vs IGST Split)\n• QuestPDF Statutory Tax Invoice Template\n• Multi-Invoice Payment Allocation Interactive Grid\n• Credit & Debit Notes Engine with Reversing JVs\n• Dapper Accounts Receivable (AR) Aging Engine", "• Anti-CSRF verification on all financial POSTs\n• Concurrency token on multi-invoice allocation\n• Parameterized SQL queries for AR analytics\n• Auto-generation of GstTransaction audit logs", "Phase 3, 4"],
        ["Phase 6", "Procurement & Stock\n(Supply Chain Tier)", "• Purchase Requisition (PR) & Purchase Order (PO) Engine\n• Goods Receipt Note (GRN) with Quality Inspection QC\n• Automated 3-Way Match Validator (PO vs GRN vs Bill)\n• Vendor Payment Allocation with TDS Deduction (194C/J)\n• Office Consumables Perpetual Inventory Register\n• Stock Issue Slips & Automated Low Stock Alerts", "• 3-Way Match discrepancy blocker on payment\n• Strict scope constraint: Consumables only\n• TDS statutory rate ceiling validation\n• Inventory transaction ledger atomicity", "Phase 2, 3, 4"],
        ["Phase 7", "Treasury & Banking\n(Liquidity Management)", "• Current, Savings, and Overdraft Multi-Bank Accounts\n• Inter-Bank Contra Transfer Processing\n• Bank Statement Parser (CSV & OFX standard formats)\n• Bank Reconciliation Statement (BRS) Matching Engine\n• Petty Cash Imprest Float Management & Top-Up Approvals", "• Reconciled period locks preventing edits\n• Dual-authorization on bank contra transfers\n• Tamper-evident cheque/UTR tracking\n• Imprest float maximum limit controls", "Phase 3"],
        ["Phase 8", "GST Compliance & ITC\n(Tax & Regulatory)", "• Comprehensive Outward/Inward GST Transaction Register\n• GSTR-1 Summary Generator (Tables 4, 7, 9, 12)\n• GSTR-3B Tax Liability Computation & Set-Off Engine\n• Government GSTR-2B JSON 4-Way ITC Matching Engine\n• NIC E-Invoice (v1.03) & E-Way Bill Schema Serializers", "• Cryptographic validation of JSON payloads\n• Mismatched ITC alerting & audit log\n• Reverse Charge Mechanism (RCM) validation\n• Statutory rounding compliance (Section 170)", "Phase 3, 5, 6"],
        ["Phase 9", "Payroll & Fixed Assets\n(HR & Asset Tier)", "• Employee Master & Multi-Component Salary Structures\n• Monthly Automated Payroll Batch Computation Engine\n• QuestPDF Individual Monthly Payslip Generator\n• Employee Advance Loan Recovery Schedule Engine\n• Fixed Asset Register with WDV Depreciation Calculation\n• Internal ERP & IT Hardware Support Ticket Desk", "• Column-level AES-256 for Salary/PAN/Aadhaar\n• Segregation of Duties: HR vs Payroll Approver\n• Asset capitalization rules enforcement\n• Role-based payslip download restriction", "Phase 2, 3"],
        ["Phase 10", "Dashboards & Reports\n(Executive Analytics)", "• Executive Dashboard (MTD/YTD Revenue, Margins, Cash)\n• Dynamic Aging Charts & Multi-Role Approvals Queue\n• 20+ Enterprise Financial, Project & GST Reports\n• ClosedXML Multi-Tab Excel Export Engine\n• QuestPDF Formatted Management Reporting Suite\n• 10-Scenario End-to-End UAT Execution & Sign-Off", "• Strict Role-Based Dashboard data filtering\n• Parameterized reporting queries via Dapper\n• Export rate-limiting & audit logging\n• Full UAT acceptance sign-off across 7 roles", "Phase 4 - 9"],
        ["Phase 11", "Security Audit & VAPT\n(Final Production Gate)", "• OWASP ASVS Level 2 Verification & Remediation\n• Horizontal/Vertical Privilege Escalation Testing\n• Automated OWASP ZAP & Manual Burp Suite Pro VAPT\n• SQLMap Automated Injection Immunity Verification\n• STRIDE Threat Model Dossier & Data Flow Diagrams\n• Formal VAPT Remediation Closure & Audit Certificate", "• 0 Critical & 0 High VAPT findings mandatory\n• Cryptographic key management verification\n• Tamper-evident hash chain verification\n• Executive Security Sign-Off before Go-Live", "Phase 0 - 10\n(Final Gate)"]
    ]
    add_styled_table(doc, roadmap_headers, roadmap_rows, [0.9, 1.3, 2.5, 1.5, 0.6])
    
    # Phase 0
    add_h2(doc, "3.1 Phase 0: Solution Scaffolding, Entity Framework Core & SQL Server DDL")
    add_p(doc, "Establishes the Visual Studio multi-project solution, applies code analysis security analyzers, generates the 48 POCO entities, configures EF Core Fluent API mappings, and deploys the initial SQL Server database schema.")
    add_bullet(doc, "Initialize SDK.ERP.sln with strict C# nullable reference types and enable Roslyn Security Analyzers (Microsoft.CodeAnalysis.NetAnalyzers, SecurityCodeScan).", "Step 1: Solution Setup — ")
    add_bullet(doc, "Create all 48 C# POCO entity classes across 10 domain folders matching the SRS Data Dictionary.", "Step 2: Entity Modeling — ")
    add_bullet(doc, "Implement IEntityTypeConfiguration<T> for all 48 tables enforcing primary keys, decimal(15,2) precisions, unique constraints, and DeleteBehavior.Restrict.", "Step 3: Fluent API Configurations — ")
    add_bullet(doc, "Execute initial EF Core migration; apply composite B-tree non-clustered indexes on high-frequency search paths.", "Step 4: Database Migration — ")
    add_bullet(doc, "Seed default system roles, admin credentials, standard GST tax slabs (0%, 5%, 12%, 18%, 28%), and default Indian Chart of Accounts.", "Step 5: Master Data Seeding — ")

    # Phase 1
    add_h2(doc, "3.2 Phase 1: Authentication, MFA, Tenant Isolation, RBAC & Core Engines")
    add_p(doc, "Builds foundational enterprise services: multi-factor authentication, tenant context filters, custom permission handlers, approval workflow state machine, and tamper-evident audit logging.")
    add_bullet(doc, "ASP.NET Core Identity with PBKDF2 hashing, secure sliding cookies (HttpOnly, SameSite=Strict), and TOTP Multi-Factor Authentication for privileged roles.", "Step 1: Secure Authentication — ")
    add_bullet(doc, "Implement [BranchContextFilter] and EF Core Global Query Filters (builder.HasQueryFilter(e => e.CompanyId == currentCompanyId)) to eliminate cross-tenant data leaks.", "Step 2: Tenant Isolation & IDOR Protection — ")
    add_bullet(doc, "Implement [HasPermission(Module, Submodule, Action)] with custom AuthorizationHandler evaluating claims against cached RolePermission entries.", "Step 3: Granular RBAC Engine — ")
    add_bullet(doc, "Build IApprovalEngineService evaluating configurable business rules with state transitions: Draft -> PendingApproval -> Approved/Rejected -> Cancelled.", "Step 4: Unified Approval Engine — ")
    add_bullet(doc, "File storage service validating binary magic numbers, generating randomized GUID filenames, and checking SHA-256 hashes.", "Step 5: Secure Document Storage — ")
    add_bullet(doc, "EF Core SaveChangesInterceptor calculating SHA-256 hash chaining (Hash(CurrentRecord + PreviousRecordHash)) to detect direct DB tampering.", "Step 6: Tamper-Evident Audit Logger — ")

    # Phase 2
    add_h2(doc, "3.3 Phase 2: Centralized Master Data Management (Controllers & Razor Views)")
    add_p(doc, "Constructs centralized master data controllers and responsive Razor views to guarantee zero data duplication across commercial transactions.")
    add_bullet(doc, "CompanyController, BranchController, and FinancialYearController with Period Locking actions.", "Step 1: Organization & Fiscal Masters — ")
    add_bullet(doc, "ClientController with DataTables.net server-side pagination, strict regex validators for GSTIN/PAN, and credit limit controls.", "Step 2: Client Directory — ")
    add_bullet(doc, "VendorController capturing MSME classification, bank IFSC, PAN, and payment terms.", "Step 3: Vendor Directory — ")
    add_bullet(doc, "ItemController for consumables with Category, UOM, HSN/SAC code, and mandatory is_durable_asset = false constraint.", "Step 4: Item Master & Tax Slabs — ")
    add_bullet(doc, "IVoucherSequenceService generating thread-safe, sequential alphanumeric voucher codes using SQL Server sequences.", "Step 5: Voucher Numbering Engine — ")

    # Phase 3
    add_h2(doc, "3.4 Phase 3: Financial Accounting Core, General Ledger & Double-Entry Posting Engine")
    add_p(doc, "Implements the double-entry general ledger posting engine in C# to automatically commit balanced debit/credit vouchers for all commercial events.")
    add_bullet(doc, "5-level hierarchical tree view of account groups (Asset, Liability, Equity, Revenue, Expense).", "Step 1: Chart of Accounts Hierarchy — ")
    add_bullet(doc, "IAccountingPostingService enforcing mathematical balance (Sum(Debit) == Sum(Credit)) and updating real-time ledger balances.", "Step 2: Balanced Journal Voucher Engine — ")
    add_bullet(doc, "Adapters automatically generating JVs for Sales Invoices, Customer Receipts, Purchase Bills, Vendor Payments, Stock Issues, Expenses, and Payroll.", "Step 3: Automated Posting Matrix — ")
    add_bullet(doc, "Enforce zero physical SQL DELETE; mandate reversing journal entries (is_reversal = true) for audit compliance.", "Step 4: Immutable Ledgers — ")
    add_bullet(doc, "Dapper services for Trial Balance, General Ledger statements, and Client/Vendor sub-ledgers.", "Step 5: High-Speed Financial Reporting — ")

    # Phase 4
    add_h2(doc, "3.5 Phase 4: Project Management & 11-Tab Project Workspace")
    add_p(doc, "Builds the operational hub connecting commercial milestones, purchase orders, expenses, deliveries, and real-time profitability.")
    add_bullet(doc, "ProjectController detail view rendering 11 Razor ViewComponents (Overview, Metadata, Client PO, Sales, Receipts, Procurement, Deliveries, Expenses, Documents, P&L, Audit Trail).", "Step 1: 11-Tab Workspace — ")
    add_bullet(doc, "Attach client purchase orders and define billing milestones with delivery dates.", "Step 2: Client PO & Milestones — ")
    add_bullet(doc, "Expense claim submission with receipt upload, approval routing, and automated accounting JV posting.", "Step 3: Direct Project Expenses — ")
    add_bullet(doc, "Issue Delivery Challans (ProjectDelivery) with courier tracking and recipient acknowledgment.", "Step 4: Delivery Challans — ")
    add_bullet(doc, "Dapper analytics service computing real-time Project Revenue, Direct Costs, Gross Margin, and Margin %.", "Step 5: Real-time Project P&L — ")
    add_bullet(doc, "6-point pre-closure validation checklist locking project against post-closure financial edits unless Reopen authorization is granted.", "Step 6: Strict Project Closure Protocol — ")

    # Phase 5
    add_h2(doc, "3.6 Phase 5: Sales, Tax Invoicing & Accounts Receivable (AR)")
    add_p(doc, "Develops client-facing commercial workflows from quotation generation to sales order fulfillment, tax invoicing, and multi-invoice payment allocation.")
    add_bullet(doc, "QuotationController with dynamic line-item builder, GST calculation, and one-click convert to SalesOrder.", "Step 1: Quotations & Sales Orders — ")
    add_bullet(doc, "SalesInvoiceController automatically splitting CGST/SGST vs IGST based on Place of Supply, posting to AR sub-ledger upon approval.", "Step 2: Statutory Tax Invoicing — ")
    add_bullet(doc, "QuestPDF template generating compliant PDF tax invoices with company branding, GSTIN, HSN summary, and bank details.", "Step 3: QuestPDF Invoice Generator — ")
    add_bullet(doc, "Interactive AJAX grid allocating a single remittance across multiple open invoices with client TDS deduction and discount handling.", "Step 4: Multi-Invoice Payment Allocation — ")
    add_bullet(doc, "CreditDebitNoteController for sales adjustments with auto-posted reversing journal entries.", "Step 5: Credit & Debit Notes — ")
    add_bullet(doc, "Dapper aging service categorizing open receivables into < 30, 31-60, 61-90, and 90+ days buckets.", "Step 6: AR Aging Analytics — ")

    # Phase 6
    add_h2(doc, "3.7 Phase 6: Procurement, 3-Way Match & Office Consumables Inventory")
    add_p(doc, "Implements organizational purchasing controls, vendor bill verification (3-way match), disbursements, and internal office consumable inventory management.")
    add_bullet(doc, "PurchaseRequisitionController and PurchaseOrderController with QuestPDF PO generator and approval routing.", "Step 1: PR & Purchase Orders — ")
    add_bullet(doc, "GoodsReceiptNoteController recording accepted vs rejected quantities, auto-triggering stock-in for consumables.", "Step 2: Goods Receipt & QC Inspection — ")
    add_bullet(doc, "C# validation service matching PO vs GRN vs Vendor Bill quantities and rates, alerting on discrepancies before AP posting.", "Step 3: 3-Way Match Validation — ")
    add_bullet(doc, "VendorPaymentController disbursing funds from bank accounts, applying Section 194C/194J TDS deductions, and settling open purchase bills.", "Step 4: Vendor Disbursements — ")
    add_bullet(doc, "Perpetual inventory register (StockTransaction), internal issue slips (StockIssue), and automated Low Stock Alerts (consumables only).", "Step 5: Office Consumables Inventory — ")

    # Phase 7
    add_h2(doc, "3.8 Phase 7: Cash & Banking Management (Treasury & BRS)")
    add_p(doc, "Manages corporate liquidity, multi-bank accounts, petty cash imprest floats, and bank reconciliation statements.")
    add_bullet(doc, "BankAccountController managing Current, Savings, and Overdraft accounts with inter-bank contra transfers.", "Step 1: Multi-Bank Accounts — ")
    add_bullet(doc, "BankReconciliationController parsing CSV/OFX statements, matching Cheques/UTRs, and locking verified BRS periods.", "Step 2: Bank Reconciliation (BRS) — ")
    add_bullet(doc, "PettyCashController for imprest float management, expense voucher verification, and top-up approvals.", "Step 3: Imprest Petty Cash — ")

    # Phase 8
    add_h2(doc, "3.9 Phase 8: Indian GST Compliance & ITC Reconciliation")
    add_p(doc, "Automates Indian GST statutory returns, GSTR-1 summaries, GSTR-3B liability computation, and automated GSTR-2B ITC matching.")
    add_bullet(doc, "Real-time audit log (GstTransaction) capturing outward/inward tax lines, RCM, credit notes, and HSN breakdowns.", "Step 1: GST Transaction Register — ")
    add_bullet(doc, "GstComplianceController aggregating GSTR-1 (Tables 4, 7, 9, 12) and GSTR-3B (Tables 3.1, 4 ITC summary, Net Tax Payable).", "Step 2: GSTR-1 & GSTR-3B Preparation — ")
    add_bullet(doc, "Upload government GSTR-2B JSON statement; run 4-way matching algorithm flagging Matched, Tax Mismatch, Missing in Books, and Missing in Portal.", "Step 3: GSTR-2B ITC Reconciliation — ")
    add_bullet(doc, "JSON schema serialization complying with NIC E-Invoice (v1.03) and E-Way Bill specifications.", "Step 4: E-Invoice & E-Way Bill Readiness — ")

    # Phase 9
    add_h2(doc, "3.10 Phase 9: Payroll, Employee Management & Fixed Asset Management")
    add_p(doc, "Manages human resource profiles, automated monthly payroll computation, leave management, and capitalized fixed asset tracking.")
    add_bullet(doc, "EmployeeController managing profiles, bank details, and SalaryStructureController configuring earnings and statutory deductions (PF, PT, TDS).", "Step 1: Employee Profiles & Salary Structures — ")
    add_bullet(doc, "PayrollController batch run computing paid working days, advance loan recoveries, QuestPDF payslips, and auto-posting monthly salary JVs.", "Step 2: Monthly Payroll Batch Engine — ")
    add_bullet(doc, "EmployeeAdvanceController tracking short-term employee loans and automated monthly payslip installment deductions.", "Step 3: Employee Advances & Leaves — ")
    add_bullet(doc, "FixedAssetController tracking hardware register, serial numbers, custodian allocations, and Written Down Value (WDV) depreciation.", "Step 4: Fixed Asset Management — ")
    add_bullet(doc, "SupportTicketController for internal hardware, software, and ERP help desk ticketing.", "Step 5: Internal Support Desk — ")

    # Phase 10
    add_h2(doc, "3.11 Phase 10: Executive Dashboards, Enterprise Reports & UAT Acceptance")
    add_p(doc, "Builds role-tailored management dashboards, complete reporting suite with PDF/Excel exports, and executes exhaustive UAT verification.")
    add_bullet(doc, "DashboardController rendering real-time KPI cards (MTD/YTD Revenue, Gross/Net Margins, Cash Balance, Overdue Receivables), aging charts, and approvals queue.", "Step 1: Executive Dashboard — ")
    add_bullet(doc, "Complete suite of 20+ Financial, Project, Sales, Procurement, GST, Inventory, and Audit reports powered by ClosedXML and QuestPDF.", "Step 2: Enterprise Reporting Suite — ")
    add_bullet(doc, "Execute the 10 end-to-end operational verification scenarios detailed in Section 11 of the SRS.", "Step 3: UAT Verification Sign-off — ")

    # Phase 11
    add_h2(doc, "3.12 Phase 11: Enterprise Security Hardening, VAPT & Security Audit Certification")
    add_p(doc, "Executes a formal security audit readiness program covering OWASP ASVS verification, Vulnerability Assessment and Penetration Testing (VAPT), dynamic penetration testing, cryptographic verification, and compliance documentation.")
    
    add_h2(doc, "3.12.1 Security Hardening & VAPT 5-Stage Verification Framework")
    add_p(doc, "The security hardening and audit readiness lifecycle is executed across 5 rigorous, sequential verification stages to ensure complete compliance with OWASP ASVS Level 2 and ISO 27001 audit standards:")

    sec_lifecycle_headers = ["Audit Stage", "Security Focus Domain", "Methodology & Tooling", "Verification Standards & Exit Criteria"]
    sec_lifecycle_rows = [
        ["Stage 1: Static Code SAST", "Source Code & Dependency Security", "• SonarQube / Roslyn Security Analyzers\n• Gitleaks Secret Scanner across Git commit history\n• Snyk / dotnet list package --vulnerable", "• 0 Critical / 0 High static code vulnerabilities\n• Zero hardcoded credentials, JWT secrets, or tokens\n• 100% third-party NuGet packages free of known CVEs"],
        ["Stage 2: Dynamic DAST Scan", "Dynamic Application & Network Security", "• OWASP ZAP Active Scanner in attack mode\n• SQLMap automated injection testing tool\n• SSL Labs & TestSSL transport security tests", "• Zero SQL Injection or Cross-Site Scripting (XSS)\n• TLS 1.3 transport encryption with secure ciphers\n• Strict HTTP Security Headers (HSTS, CSP, X-Frame: DENY)"],
        ["Stage 3: Manual VAPT Testing", "Application Logic & Privilege Escalation", "• Burp Suite Professional proxy & repeater\n• Horizontal & Vertical IDOR tenant bypass testing\n• Session hijacking, fixation, and replay tests\n• Race condition testing on payment allocations", "• Zero unauthorized cross-branch or cross-role data access\n• Anti-CSRF protection validated on all mutating actions\n• Rate-limiting verified on authentication endpoints"],
        ["Stage 4: Cryptographic Audit", "Data-at-Rest & Audit Trail Integrity", "• Column-level encryption inspection in SQL Server\n• Azure Key Vault / KMS master key rotation audit\n• Direct SQL UPDATE simulation on journal tables\n• Binary magic-number inspection on file uploads", "• PAN, Aadhaar, and Bank Accounts stored as AES-256 ciphertext\n• TamperEvidentAuditInterceptor detects hash mismatches\n• Executable / malicious file uploads rejected at storage layer"],
        ["Stage 5: Remediation & Sign-Off", "Audit Dossier & Compliance Certification", "• Formal STRIDE Threat Model documentation\n• Data Flow Diagrams (DFDs) for financial data paths\n• Third-Party VAPT Remediation Closure Report\n• Executive Security & Compliance Sign-Off", "• 100% remediation of all Critical, High, and Medium findings\n• Formal VAPT Certificate issued by security team\n• System certified for enterprise production deployment"]
    ]
    add_styled_table(doc, sec_lifecycle_headers, sec_lifecycle_rows, [1.3, 1.4, 2.3, 1.8])
    
    add_bullet(doc, "Automated test matrix simulating horizontal and vertical privilege escalation across all 7 roles, ensuring zero unauthorized parameter tampering.", "Step 1: Broken Access Control (A01) — ")
    add_bullet(doc, "Verify TLS 1.3 transport, verify AES-256 encryption on sensitive columns (PAN, Aadhaar, Bank Accounts) via Azure Key Vault, and ensure zero plaintext secrets.", "Step 2: Cryptographic Audit (A02) — ")
    add_bullet(doc, "Verify 100% parameterization across EF Core LINQ and Dapper @param queries. Run SQLMap scans against search endpoints to prove SQLi immunity.", "Step 3: Injection & Anti-XSS (A03) — ")
    add_bullet(doc, "Configure strict HTTP security headers (HSTS, CSP, X-Frame-Options: DENY, X-Content-Type-Options: nosniff) and strip server banners.", "Step 4: Security Misconfiguration (A05) — ")
    add_bullet(doc, "Run dotnet list package --vulnerable and Snyk scans; guarantee 0 critical/high CVEs in third-party packages.", "Step 5: Dependency Vulnerability Scan (A06) — ")
    add_bullet(doc, "Verify SHA-256 hash chaining on audit logs, test rate-limiting on login endpoints, and ensure anti-CSRF tokens on all POST/PUT/DELETE actions.", "Step 6: Integrity & Logging Verification (A08/A09) — ")
    add_bullet(doc, "Run automated baseline scan using OWASP ZAP in active mode, followed by manual Burp Suite Professional penetration testing covering session hijacking and race conditions.", "Step 7: Automated & Manual VAPT — ")
    add_bullet(doc, "Produce formal STRIDE Threat Model, Data Flow Diagram (DFD), and VAPT Remediation Report certifying 100% closure of Critical/High/Medium findings.", "Step 8: Audit Dossier & Compliance Sign-Off — ")

    # -------------------------------------------------------------
    # SECTION 4: 13-SPRINT TIMELINE & RESOURCE ALLOCATION SCHEDULE
    # -------------------------------------------------------------
    add_h1(doc, "4. 13-Sprint Timeline & Resource Allocation Schedule")
    add_p(doc, "The project is structured into 13 two-week development sprints (26 Weeks Total), culminating in a comprehensive security audit and production deployment:")
    
    sprint_headers = ["Sprint", "Phase Focus", "Key Deliverables & Milestones", "Security & Quality Checkpoints"]
    sprint_rows = [
        ["Sprint 1", "Phase 0 & 1", "Visual Studio Solution, 48 EF Core POCO Entities, SQL Server DDL, Indexes, Identity, Cookie Auth, Audit Logger.", "Roslyn Security Analyzers, Gitleaks, TLS 1.3."],
        ["Sprint 2", "Phase 1 & 2", "Granular RBAC Authorization Handler, MFA Engine, Unified Approval Engine, Document Storage Service, Company/Branch/Tax Masters.", "ASVS Password Check, Magic-number file validator."],
        ["Sprint 3", "Phase 2 & 3", "Client & Vendor Masters, Item Catalog, Chart of Accounts, General Ledger Core, Voucher Sequence service.", "Strict Regex validators (GSTIN/PAN), Anti-XSS encoding."],
        ["Sprint 4", "Phase 3 & 4", "Double-Entry Posting Engine, Project Controller, 11-Tab Razor ViewComponent Workspace, Client PO & Milestones.", "Balanced JV validator (Sum Dr == Sum Cr)."],
        ["Sprint 5", "Phase 4 & 5", "Project Direct Expenses, Delivery Challans, Real-time Project P&L, Quotation & Sales Order Controllers.", "IDOR Tenant isolation filter on all project endpoints."],
        ["Sprint 6", "Phase 5", "Tax Invoicing Engine (CGST/SGST/IGST), QuestPDF Invoice Template, Credit/Debit Notes, AR Posting.", "Parameterized Dapper queries, Anti-CSRF verification."],
        ["Sprint 7", "Phase 5 & 6", "Multi-Invoice Receipt Allocation Grid, AR Aging Engine, Purchase Requisition & Purchase Order Controllers.", "Concurrency locks on payment allocation."],
        ["Sprint 8", "Phase 6", "Goods Receipt Notes (GRN), Vendor Purchase Bills (3-Way Match), Vendor Payment Allocation, AP Aging.", "3-Way Match discrepancy blocker."],
        ["Sprint 9", "Phase 6 & 7", "Office Consumables Inventory (Stock In/Out/Alerts), Multi-Bank Accounts, Petty Cash, Bank BRS Engine.", "Perpetual inventory boundary checks."],
        ["Sprint 10", "Phase 8", "GST Audit Register, GSTR-1 / GSTR-3B summaries, GSTR-2B JSON Deserializer & 4-Way ITC Matcher.", "Secure JSON deserialization settings."],
        ["Sprint 11", "Phase 9", "Employee Master, Salary Structures, Leave Workflows, Monthly Payroll Batch Run, Fixed Asset Register (WDV).", "Column-level AES-256 for salary/bank/PAN data."],
        ["Sprint 12", "Phase 10", "Executive Dashboards, Complete Reporting Suite (ClosedXML & QuestPDF), UAT Acceptance Execution.", "Full UAT scenario sign-off across 7 roles."],
        ["Sprint 13", "Phase 11", "Complete Security Audit, Automated/Manual VAPT (OWASP ZAP / Burp Suite), Vulnerability Remediation & Audit Dossier.", "0 Critical/High findings, Security Audit Sign-off."]
    ]
    add_styled_table(doc, sprint_headers, sprint_rows, [0.8, 1.0, 3.2, 1.8])

    # -------------------------------------------------------------
    # SECTION 5: MANDATORY DEVELOPER IMPLEMENTATION RULES
    # -------------------------------------------------------------
    add_h1(doc, "5. Mandatory Developer Implementation Rules (.NET & C#)")
    add_p(doc, "To maintain engineering consistency, data integrity, and security across the engineering team, all developers must strictly comply with the following 8 mandatory rules:")
    
    add_bullet(doc, "All forms must bind to centralized EF Core entities (Client, Vendor, Employee, Item, ChartOfAccount). Never create duplicate master records.", "Rule 1 (Centralized Masters): ")
    add_bullet(doc, "All financial postings, inventory movements, and multi-step allocations must execute within an explicit EF Core transaction boundary (BeginTransactionAsync).", "Rule 2 (Atomic Transactions): ")
    add_bullet(doc, "Every journal voucher must enforce Sum(Debit) == Sum(Credit) before committing. Unbalanced entries must throw UnbalancedJournalEntryException.", "Rule 3 (Balanced Ledgers): ")
    add_bullet(doc, "Financial transactions (invoices, bills, receipts, vouchers) must never allow physical SQL DELETE. Use status updates (CANCELLED) and reversing JVs (is_reversal = true).", "Rule 4 (Zero Physical Deletion): ")
    add_bullet(doc, "Office Inventory is strictly for internal consumables. Durable high-value items must always flow to FixedAsset in Asset Management.", "Rule 5 (Inventory vs Asset Scope): ")
    add_bullet(doc, "Database connection strings, JWT keys, and API tokens must reside in Azure Key Vault / Environment Variables. Zero secrets in source code.", "Rule 6 (Zero Hardcoded Secrets): ")
    add_bullet(doc, "All POST/PUT/DELETE actions must enforce [ValidateAntiForgeryToken] and all SQL queries must be 100% parameterized.", "Rule 7 (Anti-CSRF & Parameterization): ")
    add_bullet(doc, "Once a project is marked CLOSED, backend action filters must reject any modifications unless the user holds Projects.Reopen administrative permission.", "Rule 8 (Project Closure Lock): ")

    # -------------------------------------------------------------
    # SECTION 6: SECURITY AUDIT & UAT VERIFICATION MATRIX
    # -------------------------------------------------------------
    add_h1(doc, "6. Security Audit & UAT Verification Matrix")
    
    add_h2(doc, "6.1 Security Audit & Penetration Testing (VAPT) Matrix")
    sec_headers = ["Security Domain", "VAPT Test Case Scenario", "Verification Standard", "Expected Outcome"]
    sec_rows = [
        ["Authentication", "Brute-force password attack on /Account/Login with 50 rapid requests.", "OWASP ASVS A07", "Rate-limiter locks IP after 5 attempts; security alert recorded."],
        ["Authorization", "Standard User attempts to access /FinancialYear/ClosePeriod or edit other branch data.", "OWASP ASVS A01 (IDOR)", "System returns 403 Forbidden; attempt logged in audit trail."],
        ["Data Security", "Inspect database storage blocks for PAN, Aadhaar, and Bank Account columns.", "OWASP ASVS A02", "Sensitive values stored as AES-256 encrypted ciphertext."],
        ["SQL Injection", "Inject ' OR '1'='1 into search filters and reporting query parameters.", "OWASP ASVS A03", "Parameterized queries treat input as literal string; 0 SQLi vulnerability."],
        ["Tamper-Evidence", "Direct SQL UPDATE executed on journal_entries table bypassing application.", "Audit Integrity A09", "TamperEvidentAuditInterceptor flags SHA-256 hash mismatch alert."],
        ["File Uploads", "Upload executable (.exe/.php) disguised with a .pdf extension.", "OWASP ASVS A08", "Magic-number binary header inspection rejects non-PDF file."],
        ["CSRF Defense", "Execute forged cross-origin POST request to /SalesInvoice/Approve.", "OWASP ASVS A08", "Missing/invalid Anti-Forgery Token returns 400 Bad Request."]
    ]
    add_styled_table(doc, sec_headers, sec_rows, [1.1, 2.5, 1.4, 1.8])

    # -------------------------------------------------------------
    # SECTION 7: CONCLUSION & ENGINEERING SIGN-OFF
    # -------------------------------------------------------------
    add_h1(doc, "7. Conclusion & Engineering Sign-Off")
    add_p(doc, "This Technical Implementation Plan provides the complete engineering blueprint for developing the SDK Solutions ERP System in ASP.NET Core MVC (C#) and Microsoft SQL Server 2022. By integrating project management, sales billing, procurement, office consumables inventory, double-entry financial accounting, Indian GST compliance, multi-bank control, and asset tracking into a unified 48-table relational schema with built-in security auditing and VAPT readiness, the system guarantees superior performance, data integrity, and regulatory compliance.")
    add_p(doc, "With this roadmap approved, the engineering team may immediately initiate Sprint 1 activities including Visual Studio solution scaffolding, EF Core entity modeling, SQL Server DDL migrations, and security baseline setup.")

    # Save single canonical document
    output_filename = "SDK_Solutions_ERP_DotNet_Core_Implementation_Plan.docx"
    doc.save(output_filename)
    print(f"Document successfully created and saved at: {output_filename}")

if __name__ == '__main__':
    build_implementation_plan_doc()
