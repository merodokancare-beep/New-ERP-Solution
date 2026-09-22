import os
import docx
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx_helpers import (
    add_header_footer, add_title, add_subtitle, add_h1, add_h2, add_h3,
    add_p, add_bullet, add_callout, add_styled_table, add_diagram,
    set_cell_background, set_cell_margins
)

def build_erp_specification():
    doc = docx.Document()
    add_header_footer(doc)
    
    # -------------------------------------------------------------
    # COVER / TITLE
    # -------------------------------------------------------------
    add_title(doc, "SDK SOLUTIONS ERP SYSTEM")
    add_subtitle(doc, "Complete Software Requirements Specification (SRS),\nFunctional System Design & Comprehensive Database Architecture")
    
    # Executive Metadata Table
    meta_headers = ["Document Attribute", "Specification Details"]
    meta_rows = [
        ["Document Type", "Enterprise Software Requirements Specification (SRS) & Database Design Document"],
        ["Organization", "SDK Solutions Private Limited"],
        ["Document Version", "2.0 – Production Baseline & Complete Database Specification"],
        ["Publication Date", "September 2026"],
        ["Target Architecture", "Multi-Tier Web Application (PostgreSQL / MySQL + Micro-Modular Services + REST API)"],
        ["Primary Operating Model", "Project-Centric Operations, Procurement, Double-Entry Accounting & GST Compliance"],
        ["Office Inventory Scope", "Internal Consumables & Office Supplies Only (Strictly Non-Commercial Warehouse)"],
        ["Target Audience", "Executive Leadership, Product Owners, Solution Architects, Backend/Frontend Engineers, QA & DBA"],
        ["Document Status", "Approved Baseline for Development & Quality Assurance"]
    ]
    add_styled_table(doc, meta_headers, meta_rows, [2.2, 4.4])
    
    # Revision History Table
    add_h2(doc, "Document Control & Revision History")
    rev_headers = ["Version", "Release Date", "Primary Author", "Summary of Major Changes"]
    rev_rows = [
        ["1.0", "September 2026", "Enterprise Solutions Team", "Initial Functional SRS defining navigation, 11 core modules, and business boundaries."],
        ["2.0", "September 2026", "Principal Systems Architect", "Added complete Database Architecture, 48 Entity Data Dictionaries, 8 System & ER Diagrams, Double-Entry Accounting Posting Matrix, Unified Approval Engine, RBAC Matrix, and UAT Verification Plan."]
    ]
    add_styled_table(doc, rev_headers, rev_rows, [0.8, 1.2, 1.8, 2.8])

    # Abbreviations & Acronyms Glossary Table
    add_h2(doc, "Glossary of Abbreviations & Acronyms")
    add_p(doc, "The following reference table defines all technical, architectural, financial, statutory, and security abbreviations utilized throughout this specification document:")

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
        ["DC", "Delivery Challan", "Document accompanying transport of goods/deliverables to client sites with courier/handover tracking."],
        ["DDD", "Domain-Driven Design", "Software design approach structuring software around domain models, entities, and business logic."],
        ["DDL", "Data Definition Language", "SQL commands (CREATE, ALTER, DROP) that define database schemas, tables, and constraints."],
        ["EF Core", "Entity Framework Core", "Microsoft modern Object-Relational Mapper (ORM) for .NET executing transactional database operations."],
        ["ERD", "Entity Relationship Diagram", "Visual graphical representation of relational database schema, entity tables, and foreign key relationships."],
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
    # SECTION 1: EXECUTIVE SUMMARY & SYSTEM PURPOSE
    # -------------------------------------------------------------
    add_h1(doc, "1. Executive Summary & Purpose")
    add_p(doc, "This document defines the definitive functional architecture, user workflows, operational controls, double-entry financial logic, comprehensive database schema, entity-relationship diagrams (ERDs), and implementation standards for the SDK Solutions Enterprise Resource Planning (ERP) System.")
    add_p(doc, "The primary objective of the SDK Solutions ERP is to unify corporate operations into a single, cohesive, project-centric digital ecosystem. Unlike generic ERPs that treat modules as siloed databases, SDK ERP connects client relationships, multi-stage project delivery, sales invoicing, vendor procurement, consumable office inventory, statutory GST compliance, multi-bank reconciliation, payroll processing, and executive profit-and-loss reporting into an unbroken, automated transaction chain.")
    
    add_callout(doc, "This document serves as the master engineering baseline. All frontend layouts, REST APIs, database schemas, stored procedures, state machines, and automated test cases must strictly comply with the requirements and constraints detailed herein.", "MANDATORY ARCHITECTURAL BASELINE")
    
    add_h2(doc, "1.1 Core Design Objectives")
    add_bullet(doc, "Streamlined, intuitive top-level navigation grouping 11 functional domains into an ergonomic workspace.", "Hierarchical Navigation: ")
    add_bullet(doc, "Eliminates disconnected CRUD forms in favor of end-to-end linked workflows where transactions inherit upstream context.", "End-to-End Transaction Integrity: ")
    add_bullet(doc, "Every commercial event (sales order, procurement PO, expense, delivery, milestone) is anchored to a specific project for real-time cost and margin tracking.", "Project-Centric Operating Paradigm: ")
    add_bullet(doc, "Unified Master Data Architecture ensuring zero duplication across Clients, Vendors, Employees, Items, and Chart of Accounts.", "Centralized Master Data: ")
    add_bullet(doc, "Automated double-entry journal creation from source transactions (invoices, bills, receipts, disbursements) with strict audit immutability (reversals/credit notes instead of silent deletion).", "Double-Entry Financial Controls: ")
    add_bullet(doc, "End-to-end statutory alignment with Indian GST regulations, including CGST, SGST, IGST, RCM, HSN/SAC validation, GSTR-1, GSTR-3B, and GSTR-2B ITC reconciliation.", "Comprehensive GST & Tax Compliance: ")
    add_bullet(doc, "Clear boundary between internal office consumable inventory (stationery, supplies) and capitalized durable fixed assets (laptops, servers, furniture).", "Distinct Inventory & Asset Boundaries: ")
    add_bullet(doc, "A configurable, rules-based approval workflow engine applicable across procurement, payments, discounts, expenses, and project sign-offs.", "Unified Multi-Level Approval Engine: ")

    # -------------------------------------------------------------
    # SECTION 2: SYSTEM ARCHITECTURE & SCOPE
    # -------------------------------------------------------------
    add_h1(doc, "2. System Scope & High-Level Technical Architecture")
    add_p(doc, "The SDK Solutions ERP covers 11 core functional domains designed in a modular, loosely-coupled, high-performance architecture. The application adheres to modern cloud-native standards, providing high concurrency, sub-second response times, transactional atomicity, and multi-tenant scalability.")
    
    add_diagram(doc, "diagram_1_architecture.png", "Figure 1: Multi-Tier Technical Architecture of SDK Solutions ERP")
    
    add_h2(doc, "2.1 Architectural Tier Breakdown")
    add_bullet(doc, "Single Page Application (SPA) built with modern reactive web frameworks. Provides role-tailored dashboards, dynamic data tables with pagination/filtering, keyboard-accelerated data entry, and PDF/Excel export capabilities.", "1. Presentation Layer: ")
    add_bullet(doc, "Serves as the single secure entry point. Manages JWT authentication, session state, CORS headers, rate-limiting, SSL termination, and routes requests to domain services.", "2. API Gateway & Security: ")
    add_bullet(doc, "Encapsulates all business rules, approval state machines, financial posting logic, GST tax calculations, and validation rules. Prevents any UI-level bypass of business rules.", "3. Application Business Services: ")
    add_bullet(doc, "Relational Database Management System (PostgreSQL / MySQL) structured in Third Normal Form (3NF) with foreign key constraints, composite B-Tree indexes, and JSONB document storage. Accompanied by Redis for caching and S3-compatible Object Storage for document attachments.", "4. Persistence & Storage Layer: ")

    # -------------------------------------------------------------
    # SECTION 3: END-TO-END BUSINESS WORKFLOWS
    # -------------------------------------------------------------
    add_h1(doc, "3. End-to-End ERP Business Workflows")
    add_p(doc, "Transactions in the SDK Solutions ERP follow an interconnected operational pipeline. Data captured at project initiation automatically flows downstream to procurement, billing, taxation, ledger posting, and cash settlement.")
    
    add_diagram(doc, "diagram_2_business_workflow.png", "Figure 2: End-to-End Project-Centric Operational and Financial Lifecycle")

    add_h2(doc, "3.1 Primary Project-to-Accounting Flow (16-Stage Cycle)")
    add_bullet(doc, "Register Client with legal entity details, billing/shipping addresses, GSTIN, PAN, and credit terms.", "Step 1: Client Onboarding — ")
    add_bullet(doc, "Initiate Project record, link to Client, designate Project Manager, set project timeline and contract value.", "Step 2: Project Creation — ")
    add_bullet(doc, "Upload Client Work Order / Purchase Order, record commercial milestones, and attach scope document.", "Step 3: Contract / PO Capture — ")
    add_bullet(doc, "Generate Sales Order against contract milestones; produce Proforma Invoice where advance billing is required.", "Step 4: Sales Order & Proforma — ")
    add_bullet(doc, "Raise Purchase Requisition for project-specific materials or services; route through approval workflow.", "Step 5: Purchase Requisition — ")
    add_bullet(doc, "Issue Purchase Order to selected approved Vendor; transmit digital PO.", "Step 6: Purchase Order Issuance — ")
    add_bullet(doc, "Record physical receipt of goods via Goods Receipt Note (GRN); inspect quantity and quality.", "Step 7: Goods Receipt (GRN) — ")
    add_bullet(doc, "Issue Delivery Challan (DC) to Client for delivered deliverables or physical milestone items.", "Step 8: Project Delivery — ")
    add_bullet(doc, "Record Vendor Purchase Bill, match against PO & GRN (3-Way Match), and trigger payable liability posting.", "Step 9: Purchase Bill Recording — ")
    add_bullet(doc, "Capture direct project expenses (travel, site logistics, contractor charges) with receipt attachments.", "Step 10: Project Direct Expenses — ")
    add_bullet(doc, "Issue Tax Invoice to Client; automatically generate GST Outward transaction and Debit Accounts Receivable.", "Step 11: Sales Tax Invoice — ")
    add_bullet(doc, "Record Client payment in Bank/Cash; allocate single payment across multiple outstanding invoices.", "Step 12: Customer Receipt & Allocation — ")
    add_bullet(doc, "Authorize and execute Vendor disbursement; allocate against open Purchase Bills in Vendor sub-ledger.", "Step 13: Vendor Disbursement — ")
    add_bullet(doc, "Compute Outward vs Inward GST liabilities; reconcile Inward ITC with GSTR-2B auto-drafted statement.", "Step 14: GST Tax Reconciliation — ")
    add_bullet(doc, "Compute real-time Project Profit & Loss (Contract Revenue minus Procurement, Direct Expenses & Allocated Costs).", "Step 15: Project P&L Generation — ")
    add_bullet(doc, "Execute formal Project Closure checklist; lock financial transactions to prevent unauthorized modifications.", "Step 16: Project Closure & Audit Lock — ")

    # -------------------------------------------------------------
    # SECTION 4: GRANULAR FUNCTIONAL SPECIFICATIONS (11 MODULES)
    # -------------------------------------------------------------
    add_h1(doc, "4. Granular Functional Requirements by Module")
    
    # 4.1 Dashboard
    add_h2(doc, "4.1 Module 1: Management Dashboard & Executive KPIs")
    add_p(doc, "Provides real-time visibility into organizational health, cash position, project margins, and compliance status.")
    add_bullet(doc, "Revenue MTD/YTD, Gross & Net Margins, Cash & Bank Book Balances, Total Overdue Receivables, Total Payables.", "Executive KPI Cards: ")
    add_bullet(doc, "Active Projects count, Milestones Due this Week, Overdue Deliverables, High-Margin vs Low-Margin Projects.", "Project Operations Tracker: ")
    add_bullet(doc, "30/60/90/120+ days aging buckets with drill-down to individual client invoices.", "Receivable & Payable Aging Charts: ")
    add_bullet(doc, "Outward liability, available ITC, net tax payable, and filing deadline countdowns.", "GST Tax Summary Widget: ")
    add_bullet(doc, "Consolidated pending approvals queue for purchase orders, payment requests, expense claims, and discounts.", "Action Center / Approvals: ")

    # 4.2 Masters
    add_h2(doc, "4.2 Module 2: Masters & System Configuration")
    add_p(doc, "Centralized repository for all fundamental business parameters, reusable across the entire application.")
    add_bullet(doc, "Company legal profile, multi-branch definitions, GSTINs, PAN, TAN, corporate logo, and financial year definitions.", "Organization Masters: ")
    add_bullet(doc, "Chart of Accounts (Asset, Liability, Equity, Revenue, Expense), Account Groups, Voucher Numbering Sequences.", "Financial Masters: ")
    add_bullet(doc, "Client Master (Billing/Shipping Address, Credit Terms, GSTIN, State Code), Vendor Master (Bank IFSC, MSME Type).", "Commercial Parties: ")
    add_bullet(doc, "Consumable Items, Categories, Units of Measure (UOM), Tax Rate Master (0%, 5%, 12%, 18%, 28%), HSN/SAC Code Master.", "Inventory & Tax: ")
    add_bullet(doc, "Departments, Designations, Employee Types, Holiday Calendars, Leave Categories.", "Human Resources: ")
    add_bullet(doc, "Roles, Granular Permissions (Module x Action matrix), User Accounts, Multi-Branch Access Rules.", "Security & Access: ")

    # 4.3 Projects
    add_h2(doc, "4.3 Module 3: Project Management & Lifecycle Monitoring")
    add_p(doc, "The central operational hub of SDK ERP. Every commercial and logistical action links back to a project.")
    add_bullet(doc, "Searchable grid displaying Project Code, Name, Client, Budget, Contract Value, Invoiced Amount, Received Amount, Expense Total, Status, and Health Index.", "Project List & Filter: ")
    add_bullet(doc, "Overview KPI summary, Basic Metadata, Client PO / Work Order, Sales & Invoices, Payment Receipts, Procurement POs/GRNs, Deliveries, Project Expenses, Document Repository, Real-time P&L Statement, and Audit Trail.", "11-Tab Project Workspace: ")
    add_bullet(doc, "Pipeline -> Active -> On Hold (with reason log) -> Completed (all deliveries done) -> Closed (financial lock).", "Project Lifecycle State Machine: ")
    
    add_callout(doc, "Project Closure Protocol: A project cannot be set to 'Closed' until: (1) All deliveries are acknowledged, (2) All milestone invoices are raised, (3) Outstanding receivable balance is checked, (4) All vendor purchase bills are matched and accounted, (5) Direct expenses are verified, and (6) Final Project P&L is locked. Once Closed, no user may post or edit transactions against the project without explicit 'Reopen Project' administrative approval.", "PROJECT CLOSURE CONTROLS")

    # 4.4 Sales & Billing
    add_h2(doc, "4.4 Module 4: Sales, Billing & Accounts Receivable")
    add_p(doc, "Manages client-facing revenue workflows from initial quotation to receipt settlement.")
    add_bullet(doc, "Create formal quotes with itemized deliverables, discounts, GST breakdown, terms, and convert directly into Sales Order.", "Quotations & Proforma Invoices: ")
    add_bullet(doc, "Binds client PO to internal milestones, tracks billable progress, and triggers billing alerts.", "Sales Orders: ")
    add_bullet(doc, "Compliant tax invoice generation with HSN/SAC, CGST/SGST/IGST, place of supply, reverse charge indicator, and automated QR/E-Invoice readiness.", "Tax Invoicing Engine: ")
    add_bullet(doc, "Allows a single client remittance (e.g. ₹10,00,000) to be split across multiple open invoices (e.g. ₹4L against INV-001, ₹3L against INV-002, ₹3L against INV-003) with real-time balance reduction.", "Multi-Invoice Payment Allocation: ")
    add_bullet(doc, "Issued against returned deliveries, price adjustments, or billing corrections with linked accounting entries.", "Credit & Debit Notes: ")
    add_bullet(doc, "Draft -> Pending Approval -> Approved -> Sent -> Partially Paid -> Paid -> Overdue -> Cancelled (Reversed).", "Invoice Status Workflow: ")

    # 4.5 Procurement & Office Inventory
    add_h2(doc, "4.5 Module 5: Procurement & Office Consumables Inventory")
    add_p(doc, "Controls organizational purchasing and manages internal-use office supplies (strictly non-commercial).")
    add_bullet(doc, "Internal department or project-level request for supplies/services with approval routing.", "Purchase Requisition (PR): ")
    add_bullet(doc, "Contractual order issued to approved vendor specifying item codes, quantities, agreed unit rates, delivery schedules, and tax terms.", "Purchase Order (PO): ")
    add_bullet(doc, "Physical receiving record documenting accepted vs rejected quantities with vendor delivery challan reference.", "Goods Receipt Note (GRN): ")
    add_bullet(doc, "Automated 3-way reconciliation (PO vs GRN vs Vendor Bill) to prevent over-billing before payable posting.", "Purchase Bill (3-Way Match): ")
    add_bullet(doc, "Internal stock register for office consumables (stationery, printer toner, cleaning supplies, pantry stock). Supports Stock In (from GRN), Stock Out / Issue (to employee/dept), Stock Adjustment, and Low Stock Alerts.", "Office Consumables Inventory: ")
    
    add_callout(doc, "Scope Boundary: Office Inventory is strictly for internal consumables. Durable, high-value assets (laptops, monitors, machinery, office furniture) must be registered under Module 11 (Asset Management) to support capitalization, depreciation schedules, and employee custodian tracking.", "INVENTORY VS ASSET SEPARATION")

    # 4.6 Accounts & Payments
    add_h2(doc, "4.6 Module 6: Financial Accounting, Ledgers & Payments")
    add_p(doc, "Full double-entry general ledger accounting engine operating in real-time.")
    add_bullet(doc, "Configurable 5-level hierarchical chart of accounts supporting multi-branch accounting.", "Chart of Accounts: ")
    add_bullet(doc, "Automated balanced voucher generation for Sales, Purchases, Receipts, Payments, Contra, and manual Journal Vouchers (JV).", "Voucher Management: ")
    add_bullet(doc, "Real-time General Ledger, Client Sub-Ledger, Vendor Sub-Ledger, and Expense Head Ledgers.", "Ledger Ecosystem: ")
    add_bullet(doc, "Trial Balance, Profit & Loss Statement (Income vs Expense), Balance Sheet, and Direct/Indirect Cash Flow Statements.", "Financial Statements: ")
    add_bullet(doc, "Unified disbursement engine handling Vendor Bills, Employee Expense Reimbursements, Direct Utility Payments, Advance Settlements, and Petty Cash Top-ups.", "Common Payment Engine: ")

    # 4.7 GST & Tax
    add_h2(doc, "4.7 Module 7: GST Compliance, Tax Engines & ITC Reconciliation")
    add_p(doc, "Automated tax compliance system built for Indian Goods and Services Tax (GST) regulations.")
    add_bullet(doc, "Automates CGST + SGST (intra-state) vs IGST (inter-state) based on Supplier State vs Client Place of Supply (POS).", "Dynamic Tax Engine: ")
    add_bullet(doc, "Consolidates B2B taxable invoices, B2C large/small sales, credit notes, and zero-rated exports.", "GSTR-1 Outward Return: ")
    add_bullet(doc, "Automates monthly summary of outward supplies, eligible ITC, ineligible ITC, and net cash tax liability.", "GSTR-3B Return Preparation: ")
    add_bullet(doc, "Automated matching of Purchase Bills against auto-drafted GSTR-2B data. Flags 'Matched', 'Value Mismatch', 'Missing in Books', and 'Missing in Portal' to prevent ITC loss.", "GSTR-2B ITC Reconciliation: ")
    add_bullet(doc, "Standardized JSON payload schemas for government E-Invoice (IRN generation) and E-Way Bill portals.", "E-Invoice & E-Way Bill: ")

    # 4.8 Cash & Bank
    add_h2(doc, "4.8 Module 8: Cash & Bank Management")
    add_p(doc, "Controls liquidity, multiple corporate bank accounts, petty cash floats, and reconciliations.")
    add_bullet(doc, "Current accounts, savings accounts, overdraft/cash credit facilities with real-time book balance tracking.", "Multi-Bank Accounts: ")
    add_bullet(doc, "Inter-account fund transfers, RTGS/NEFT/IMPS entries, Cheque tracking with clearance status.", "Bank Transactions: ")
    add_bullet(doc, "Import bank statements (CSV/OFX) to match book transactions against bank feed, capturing bank charges and interest.", "Bank Reconciliation (BRS): ")
    add_bullet(doc, "Imprest petty cash float management with expense voucher verification and top-up approvals.", "Petty Cash Management: ")
    add_bullet(doc, "Tracking recurring software subscriptions and client/vendor security deposits.", "Subscriptions & Deposits: ")

    # 4.9 Payroll & Employees
    add_h2(doc, "4.9 Module 9: Payroll & Employee Operations")
    add_p(doc, "Manages human resource records, salary computations, deductions, and leave balances.")
    add_bullet(doc, "Personal details, emergency contacts, PAN, Aadhaar, Bank Details, PF/UAN, ESI, and Reporting Hierarchy.", "Employee Profiles: ")
    add_bullet(doc, "Configurable earnings (Basic, HRA, Special Allowance) and statutory deductions (PF, Professional Tax, TDS).", "Salary Structures: ")
    add_bullet(doc, "Automated monthly payroll calculation based on biometric/attendance logs, approved leaves, and salary advance deductions.", "Monthly Payroll Runs: ")
    add_bullet(doc, "Employee advance requests, approval routing, and automated monthly installment deduction from salary.", "Salary Advances: ")
    add_bullet(doc, "Casual Leave, Sick Leave, Earned Leave quotas, leave requests, manager approval, and holiday lists.", "Leave & Holidays: ")

    # 4.10 Reports & Analytics
    add_h2(doc, "4.10 Module 10: Enterprise Reporting & Analytics Suite")
    add_p(doc, "Comprehensive reporting engine supporting grid filtering, multi-column sorting, PDF generation, and Excel/CSV data exports.")
    add_bullet(doc, "Trial Balance, Balance Sheet, Profit & Loss Statement, General Ledger, Cash Flow.", "Financial Reports: ")
    add_bullet(doc, "Project P&L (Budget vs Actual), Project Expense Breakdown, Milestone Billing Status, Closure Audit.", "Project Reports: ")
    add_bullet(doc, "Sales Register, Client-wise Sales Summary, Invoice Aging (30/60/90+ days), Collection Report.", "Sales & Billing Reports: ")
    add_bullet(doc, "Purchase Register, Vendor-wise Procurement Summary, Vendor Payable Aging, PO Fulfillment Status.", "Procurement Reports: ")
    add_bullet(doc, "GSTR-1 Report, GSTR-3B Summary, Inward ITC Report, GSTR-2B Mismatch Exception Report.", "GST Compliance Reports: ")
    add_bullet(doc, "Consumable Stock Register, Stock In/Out Summary, Low Stock Alerts, Department Issue History.", "Office Inventory Reports: ")
    add_bullet(doc, "System Audit Trail, User Login History, Approval Logs, Record Modification / Deletion History.", "Audit Reports: ")

    # 4.11 Administration & Support
    add_h2(doc, "4.11 Module 11: Administration, Security (RBAC), Asset Management & Support")
    add_p(doc, "System administration, enterprise security, hardware asset registry, and internal help desk.")
    add_bullet(doc, "Hardware inventory (laptops, servers, monitors, office fixtures), serial numbers, asset tag generation, custodian assignment, maintenance history, and depreciation.", "Fixed Asset Management: ")
    add_bullet(doc, "User authentication, password complexity policies, multi-branch access scopes, and granular permission matrices.", "User & Role Management: ")
    add_bullet(doc, "Internal ticket logging for IT hardware, ERP bugs, software requests, priority queues, and resolution tracking.", "Support & Help Desk: ")
    add_bullet(doc, "System-wide immutable logging of every database change with user ID, timestamp, IP address, old payload, and new payload.", "Master Audit Trail: ")

    # -------------------------------------------------------------
    # SECTION 5: REUSABLE ENTERPRISE ENGINES
    # -------------------------------------------------------------
    add_h1(doc, "5. Reusable Cross-Cutting Enterprise Engines")
    
    add_h2(doc, "5.1 Unified Multi-Level Approval Workflow Engine")
    add_p(doc, "Instead of hard-coded approval logic inside individual screens, SDK ERP implements a centralized, event-driven Approval Engine. Any transactional entity can register for approval rules based on business conditions.")
    add_bullet(doc, "Purchase Orders exceeding threshold (e.g. > ₹50,000 requires Dept Head; > ₹5,00,000 requires Director).", "Approval Trigger 1: ")
    add_bullet(doc, "Vendor Disbursements & Direct Payment Vouchers exceeding predefined limits.", "Approval Trigger 2: ")
    add_bullet(doc, "Sales Invoices with custom discounts exceeding standard sales policy margins.", "Approval Trigger 3: ")
    add_bullet(doc, "Employee Reimbursement claims and Salary Advance applications.", "Approval Trigger 4: ")
    add_bullet(doc, "Project Closure requests and Reopening of historically closed projects.", "Approval Trigger 5: ")
    
    # State transition table for approvals
    app_headers = ["Current State", "Trigger Event", "Condition / Actor", "Next State", "System Actions"]
    app_rows = [
        ["Draft", "Submit for Approval", "Initiator clicks submit", "Pending Approval", "Lock record; dispatch notification to assigned Approver."],
        ["Pending Approval", "Approve", "Authorized Approver approves", "Approved", "Unlock for posting; generate GL vouchers / PO dispatch."],
        ["Pending Approval", "Reject", "Approver rejects with reason", "Rejected", "Unlock for editing; notify Initiator with rejection comments."],
        ["Approved", "Cancel / Revoke", "Super Admin with reason", "Cancelled", "Execute compensating reversal voucher; log in audit trail."]
    ]
    add_styled_table(doc, app_headers, app_rows, [1.1, 1.2, 1.4, 1.1, 2.0])

    add_h2(doc, "5.2 Centralized Document & Attachment Engine")
    add_p(doc, "Centralized storage for all supporting files (Client POs, Vendor Invoices, GRN delivery slips, Cheque copies, Expense receipts, Contracts). Files are securely stored with SHA-256 integrity hashes, MIME type validation, versioning, and linked polymorphic entity keys (`entity_type` + `entity_id`).")

    add_h2(doc, "5.3 Multi-Level Role-Based Access Control (RBAC) Matrix")
    add_p(doc, "Permissions in SDK ERP are evaluated per Module -> Submodule -> Action. Every API request must authenticate user token and verify permission grants.")
    
    rbac_headers = ["Module Code", "Admin / Director", "Project Manager", "Sales Exec", "Accountant / Finance", "Procurement Officer", "Store Keeper"]
    rbac_rows = [
        ["Dashboard", "Full Access", "Project View", "Sales View", "Financial View", "Procurement View", "Inventory View"],
        ["Masters & Config", "Full Access", "Read-Only", "Read-Only", "Account Masters", "Vendor Masters", "Item Masters"],
        ["Projects", "Full Access", "Create / Edit / Deliver", "View / Milestone", "Cost / P&L View", "Procurement Link", "Delivery View"],
        ["Sales & Billing", "Full Access", "View / Proforma", "Create / Quote / SO", "Invoice / Post / Credit", "No Access", "Delivery Challan"],
        ["Procurement & Stock", "Full Access", "Raise PR / View", "No Access", "Bill Verification", "Create PO / Vendor", "GRN / Stock In/Out"],
        ["Accounts & Payments", "Full Access", "Expense Claim", "No Access", "Full GL / Vouchers / Pay", "View PO Bills", "No Access"],
        ["GST & Tax", "Full Access", "No Access", "No Access", "Full Filing & Reco", "No Access", "No Access"],
        ["Cash & Bank", "Full Access", "No Access", "No Access", "Full Reconciliation", "No Access", "No Access"],
        ["Payroll & HR", "Full Access", "Dept Leaves", "Own Leaves", "Salary Processing", "Own Leaves", "Own Leaves"],
        ["Reports & Analytics", "Full Access", "Project Reports", "Sales Reports", "Financial & GST", "Purchase Reports", "Stock Reports"],
        ["Admin & Assets", "Full Access", "Asset View", "Asset View", "Asset Value / Depr", "Asset Purchase", "Asset Issue"]
    ]
    add_styled_table(doc, rbac_headers, rbac_rows, [1.4, 0.9, 0.9, 0.9, 0.9, 0.9, 0.9])

    # -------------------------------------------------------------
    # SECTION 6: DOUBLE-ENTRY FINANCIAL POSTING MATRIX
    # -------------------------------------------------------------
    add_h1(doc, "6. Double-Entry Financial Accounting Posting Matrix")
    add_p(doc, "To maintain mathematical balance and GAAP/Indian Accounting Standard compliance, SDK ERP enforces an automated Journal Voucher (JV) posting engine. When business transactions are posted, balanced debit and credit entries are instantly committed in the General Ledger.")
    
    gl_headers = ["Transaction Type", "Trigger Event", "Debit Ledger Account", "Credit Ledger Account", "GST / Tax Impact", "Reversal Rule"]
    gl_rows = [
        ["Sales Tax Invoice (Intra-State)", "Invoice Approved & Sent", "Client Sub-Ledger (Accounts Receivable)", "1. Sales Revenue A/c\n2. Output CGST Payable\n3. Output SGST Payable", "Increases Output CGST/SGST liability (GSTR-1)", "Credit Note only; no hard delete."],
        ["Sales Tax Invoice (Inter-State)", "Invoice Approved & Sent", "Client Sub-Ledger (Accounts Receivable)", "1. Sales Revenue A/c\n2. Output IGST Payable", "Increases Output IGST liability (GSTR-1)", "Credit Note only; no hard delete."],
        ["Customer Payment Receipt", "Receipt recorded & allocated", "Bank Account / Cash Book", "Client Sub-Ledger (Accounts Receivable)", "None (Tax recognized at Invoicing)", "Cancel Receipt with reversal JV."],
        ["Purchase Bill (Intra-State)", "Vendor Bill Approved", "1. Expense / Purchases A/c\n2. Input CGST (ITC)\n3. Input SGST (ITC)", "Vendor Sub-Ledger (Accounts Payable)", "Increases eligible Inward ITC (GSTR-2B)", "Debit Note only; no hard delete."],
        ["Purchase Bill (Inter-State)", "Vendor Bill Approved", "1. Expense / Purchases A/c\n2. Input IGST (ITC)", "Vendor Sub-Ledger (Accounts Payable)", "Increases eligible Inward ITC (GSTR-2B)", "Debit Note only; no hard delete."],
        ["Purchase Bill (RCM)", "Reverse Charge Bill Approved", "1. Expense / Purchases A/c\n2. Input GST (RCM)", "1. Vendor Payable\n2. Output GST (RCM Payable)", "Simultaneous RCM Output liability & Inward ITC", "Debit Note with RCM reversal."],
        ["Vendor Payment Disbursement", "Payment Voucher processed", "Vendor Sub-Ledger (Accounts Payable)", "Bank Account / Cheque A/c", "None", "Voucher cancellation with reversal JV."],
        ["Office Stock Consumption", "Internal Stock Out approved", "Office Consumables Expense A/c", "Office Inventory Asset A/c", "None (ITC claimed at purchase)", "Inventory adjustment voucher."],
        ["Direct Project Expense", "Expense claim approved", "Project Direct Expense A/c (Project Tagged)", "1. Bank / Cash A/c\n(or Employee Payable)", "Input GST if Tax Invoice attached", "Expense reversal with approval."],
        ["Monthly Payroll Processing", "Salary Sheet finalized", "Salaries & Wages Expense A/c", "1. Net Salaries Payable\n2. PF Payable\n3. PT Payable\n4. TDS Payable", "None", "Payroll rollback before payout."],
        ["Salary Payout Disbursement", "Bank disbursement executed", "Net Salaries Payable A/c", "Bank Disbursement Account", "None", "Bank adjustment entry."],
        ["Fixed Asset Purchase", "Asset Bill Approved", "1. Fixed Asset A/c (e.g. Laptops)\n2. Capital Goods ITC", "Vendor Sub-Ledger (Accounts Payable)", "Capital Goods ITC (GSTR-2B)", "Debit Note or Asset disposal JV."],
        ["Asset Depreciation", "Period End Depreciation Run", "Depreciation Expense A/c", "Accumulated Depreciation A/c", "None", "Year-end adjusting entry."]
    ]
    add_styled_table(doc, gl_headers, gl_rows, [1.3, 1.1, 1.4, 1.4, 1.1, 1.1])

    # -------------------------------------------------------------
    # SECTION 7: DATABASE ARCHITECTURE & SYSTEM ERDs
    # -------------------------------------------------------------
    add_h1(doc, "7. Comprehensive Database Architecture & Entity Relationship Diagrams")
    add_p(doc, "The SDK Solutions ERP relational database schema is organized into 10 cohesive domain schemas containing 48 core relational tables. All tables adhere to strict Third Normal Form (3NF), utilize explicit foreign key constraints, enforce data integrity via check constraints, maintain audit columns, and support multi-branch tenancy.")
    
    add_h2(doc, "7.1 Master System Entity Relationship Diagram (High-Level Master ERD)")
    add_diagram(doc, "diagram_3_master_erd.png", "Figure 3: Master High-Level Entity Relationship Diagram (Core Domain Interconnections)")
    
    add_h2(doc, "7.2 Domain Subsystem ERDs")
    add_diagram(doc, "diagram_4_erd_masters_security.png", "Figure 4: Domain 1 ERD — Master Data, Security (RBAC), Users & Approval Engine")
    add_diagram(doc, "diagram_5_erd_projects_sales.png", "Figure 5: Domain 2 ERD — Project Management, Sales Invoicing & Accounts Receivable")
    add_diagram(doc, "diagram_6_erd_procurement_inventory.png", "Figure 6: Domain 3 ERD — Procurement, Office Inventory & Accounts Payable")
    add_diagram(doc, "diagram_7_erd_finance_gst_banking.png", "Figure 7: Domain 4 ERD — Financial Accounting, GST Compliance & Banking Engine")
    add_diagram(doc, "diagram_8_erd_payroll_assets.png", "Figure 8: Domain 5 ERD — Payroll, Employee Records & Fixed Asset Management")

    # -------------------------------------------------------------
    # SECTION 8: COMPLETE RELATIONAL DATA DICTIONARY (48 TABLES)
    # -------------------------------------------------------------
    add_h1(doc, "8. Complete Relational Data Dictionary (Table Schemas)")
    add_p(doc, "The following data dictionary provides exhaustive technical definitions for all 48 database tables across the 10 domain subsystems. Each table specification includes exact column names, SQL data types, nullability, primary/foreign key constraints, default values, and operational descriptions.")

    # Helper function to add a data dictionary table
    def add_table_def(tbl_name, tbl_desc, domain_name, cols):
        add_h3(doc, f"Table: {tbl_name} ({domain_name})")
        add_p(doc, f"Description: {tbl_desc}")
        headers = ["Column Name", "Data Type", "Constraint", "Default", "Description & Foreign Key"]
        add_styled_table(doc, headers, cols, [1.5, 1.1, 1.0, 0.9, 2.7])

    # DOMAIN 1: SECURITY & SYSTEM ADMIN
    add_h2(doc, "8.1 Domain 1: Authentication, Authorization & Security Tables")
    
    add_table_def("companies", "Stores legal company profiles and tenancy definitions.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Unique primary identifier for company."],
        ["company_code", "VARCHAR(20)", "Unique, Not Null", "None", "Unique alphanumeric code (e.g. SDK-CORP)."],
        ["company_name", "VARCHAR(150)", "Not Null", "None", "Registered corporate trade name."],
        ["legal_name", "VARCHAR(150)", "Not Null", "None", "Full statutory legal entity name."],
        ["gstin", "VARCHAR(15)", "Unique, Nullable", "None", "15-digit Indian GST Identification Number."],
        ["pan", "VARCHAR(10)", "Not Null", "None", "10-digit Indian Permanent Account Number."],
        ["tan", "VARCHAR(10)", "Nullable", "None", "Tax Deduction Account Number for TDS."],
        ["base_currency", "VARCHAR(3)", "Not Null", "'INR'", "Standard ISO-4217 3-letter currency code."],
        ["financial_year_start", "DATE", "Not Null", "None", "Start date of corporate fiscal year (e.g. 2026-04-01)."],
        ["created_at", "TIMESTAMP", "Not Null", "CURRENT_TIMESTAMP", "Record creation audit timestamp."],
        ["updated_at", "TIMESTAMP", "Not Null", "CURRENT_TIMESTAMP", "Record modification audit timestamp."]
    ])

    add_table_def("branches", "Stores multi-branch and operational office locations.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier for branch location."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_code", "VARCHAR(30)", "Unique, Not Null", "None", "Branch code (e.g. BR-DEL-01, BR-BLR-01)."],
        ["branch_name", "VARCHAR(100)", "Not Null", "None", "Display name of branch."],
        ["state_code", "VARCHAR(5)", "Not Null", "None", "2-digit Indian State code (e.g. 07 for Delhi, 29 for Karnataka)."],
        ["gstin", "VARCHAR(15)", "Nullable", "None", "Branch-specific GSTIN if separately registered."],
        ["address_line1", "VARCHAR(255)", "Not Null", "None", "Physical street address."],
        ["is_head_office", "BOOLEAN", "Not Null", "FALSE", "True if primary corporate headquarters."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Operational status flag."]
    ])

    add_table_def("roles", "Defines system access roles for Role-Based Access Control.", "Security & Admin", [
        ["id", "INT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier for role."],
        ["role_name", "VARCHAR(50)", "Unique, Not Null", "None", "Role name (e.g. SUPER_ADMIN, PROJECT_MANAGER, ACCOUNTANT)."],
        ["description", "VARCHAR(255)", "Nullable", "None", "Role responsibilities and scope description."],
        ["is_system_role", "BOOLEAN", "Not Null", "FALSE", "True for protected non-deletable system roles."]
    ])

    add_table_def("users", "User credentials, profile links, and login states.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier for user account."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> branches(id). Default branch context."],
        ["role_id", "INT", "FK, Not Null", "None", "Foreign Key -> roles(id). Primary security role."],
        ["employee_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> employees(id) if linked to internal staff."],
        ["username", "VARCHAR(50)", "Unique, Not Null", "None", "Unique login username."],
        ["email", "VARCHAR(100)", "Unique, Not Null", "None", "Official email address."],
        ["password_hash", "VARCHAR(255)", "Not Null", "None", "Bcrypt / Argon2 cryptographic password hash."],
        ["reporting_manager_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id). Hierarchical approval manager."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Account active flag."],
        ["last_login_at", "TIMESTAMP", "Nullable", "None", "Timestamp of most recent successful authentication."]
    ])

    add_table_def("role_permissions", "Granular module-action permission mapping matrix.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["role_id", "INT", "FK, Not Null", "None", "Foreign Key -> roles(id)."],
        ["module_code", "VARCHAR(50)", "Not Null", "None", "Target module (e.g. PROJECTS, SALES, ACCOUNTS, GST)."],
        ["submodule_code", "VARCHAR(50)", "Not Null", "None", "Target submodule (e.g. INVOICES, PO, VENDORS, BRS)."],
        ["can_view", "BOOLEAN", "Not Null", "FALSE", "Read permission."],
        ["can_create", "BOOLEAN", "Not Null", "FALSE", "Insert / Add permission."],
        ["can_edit", "BOOLEAN", "Not Null", "FALSE", "Update permission."],
        ["can_delete", "BOOLEAN", "Not Null", "FALSE", "Delete permission (restricted where financial data exists)."],
        ["can_approve", "BOOLEAN", "Not Null", "FALSE", "Authorization / Approval permission."],
        ["can_post", "BOOLEAN", "Not Null", "FALSE", "Financial GL posting permission."],
        ["can_export", "BOOLEAN", "Not Null", "FALSE", "Excel/CSV/PDF export permission."]
    ])

    add_table_def("audit_logs", "Immutable system-wide change log for compliance and security.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["user_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id). User initiating action (null for system)."],
        ["table_name", "VARCHAR(60)", "Not Null", "None", "Target relational table name."],
        ["record_id", "BIGINT", "Not Null", "None", "Primary key value of affected record."],
        ["action_type", "VARCHAR(20)", "Not Null", "None", "INSERT, UPDATE, DELETE, APPROVE, REOPEN, LOGIN."],
        ["old_payload", "JSONB", "Nullable", "None", "Pre-modification state in JSON format."],
        ["new_payload", "JSONB", "Nullable", "None", "Post-modification state in JSON format."],
        ["ip_address", "VARCHAR(45)", "Nullable", "None", "IPv4 or IPv6 client network address."],
        ["user_agent", "VARCHAR(255)", "Nullable", "None", "Browser / Client device agent."],
        ["created_at", "TIMESTAMP", "Not Null", "CURRENT_TIMESTAMP", "Immutable event occurrence timestamp."]
    ])

    add_table_def("approval_rules", "Configurable business rules for transaction approvals.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["module_code", "VARCHAR(50)", "Not Null", "None", "Target module (e.g. PROCUREMENT, PAYMENTS, PROJECTS)."],
        ["transaction_type", "VARCHAR(50)", "Not Null", "None", "PO, PURCHASE_BILL, PAYMENT_VOUCHER, EXPENSE_CLAIM, PROJECT_CLOSE."],
        ["min_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Minimum threshold triggering this rule."],
        ["max_amount", "DECIMAL(15,2)", "Nullable", "None", "Maximum threshold for this rule tier."],
        ["approver_role_id", "INT", "FK, Not Null", "None", "Foreign Key -> roles(id) required for authorization."],
        ["level_order", "INT", "Not Null", "1", "Hierarchical sequence order (Level 1, Level 2, Level 3)."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Rule active flag."]
    ])

    add_table_def("approval_requests", "Tracks active and historical transaction approval lifecycles.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["rule_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> approval_rules(id)."],
        ["entity_type", "VARCHAR(50)", "Not Null", "None", "Target entity (e.g. purchase_orders, payment_vouchers)."],
        ["entity_id", "BIGINT", "Not Null", "None", "Primary key of target transaction."],
        ["requested_by", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id) who submitted the request."],
        ["assigned_to_role_id", "INT", "FK, Not Null", "None", "Foreign Key -> roles(id) responsible for decision."],
        ["status", "VARCHAR(20)", "Not Null", "'PENDING'", "PENDING, APPROVED, REJECTED, CANCELLED."],
        ["action_by", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id) who made the final decision."],
        ["action_at", "TIMESTAMP", "Nullable", "None", "Timestamp when decision was committed."],
        ["comments", "TEXT", "Nullable", "None", "Approver remarks, justification, or rejection reasons."]
    ])

    add_table_def("document_attachments", "Centralized polymorphic file attachment repository.", "Security & Admin", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["entity_type", "VARCHAR(50)", "Not Null", "None", "Polymorphic target (e.g. projects, sales_invoices, grn)."],
        ["entity_id", "BIGINT", "Not Null", "None", "Primary key of target transaction/record."],
        ["file_name", "VARCHAR(255)", "Not Null", "None", "Original uploaded filename."],
        ["file_path", "VARCHAR(500)", "Not Null", "None", "Storage URI / S3 object key."],
        ["file_size_bytes", "BIGINT", "Not Null", "None", "Size in bytes for storage tracking."],
        ["mime_type", "VARCHAR(100)", "Not Null", "None", "MIME type (e.g. application/pdf, image/png)."],
        ["file_hash_sha256", "VARCHAR(64)", "Not Null", "None", "Cryptographic SHA-256 integrity hash."],
        ["uploaded_by", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id)."],
        ["uploaded_at", "TIMESTAMP", "Not Null", "CURRENT_TIMESTAMP", "Upload timestamp."],
        ["version_number", "INT", "Not Null", "1", "Document version sequence."]
    ])

    # DOMAIN 2: CORE MASTER DATA
    add_h2(doc, "8.2 Domain 2: Core Master Data Entities")

    add_table_def("clients", "Customer master directory for sales and billing.", "Master Data", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary client identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["client_code", "VARCHAR(30)", "Unique, Not Null", "None", "Unique alphanumeric code (e.g. CLT-00124)."],
        ["client_name", "VARCHAR(150)", "Not Null", "None", "Trade name of client."],
        ["contact_person", "VARCHAR(100)", "Nullable", "None", "Primary business representative."],
        ["email", "VARCHAR(100)", "Nullable", "None", "Billing / Accounts email."],
        ["phone", "VARCHAR(20)", "Nullable", "None", "Direct contact phone number."],
        ["billing_address", "TEXT", "Not Null", "None", "Registered billing address."],
        ["shipping_address", "TEXT", "Nullable", "None", "Physical delivery site address."],
        ["state_code", "VARCHAR(5)", "Not Null", "None", "2-digit Indian GST state code (determines POS)."],
        ["gstin", "VARCHAR(15)", "Nullable", "None", "15-character GSTIN (mandatory for B2B tax invoicing)."],
        ["pan", "VARCHAR(10)", "Nullable", "None", "10-character Permanent Account Number."],
        ["credit_limit", "DECIMAL(15,2)", "Not Null", "0.00", "Maximum permissible outstanding receivables."],
        ["credit_days", "INT", "Not Null", "30", "Agreed standard payment credit days."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Active trading status."]
    ])

    add_table_def("vendors", "Supplier and contractor master directory.", "Master Data", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary vendor identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["vendor_code", "VARCHAR(30)", "Unique, Not Null", "None", "Unique alphanumeric code (e.g. VND-00085)."],
        ["vendor_name", "VARCHAR(150)", "Not Null", "None", "Official registered supplier name."],
        ["vendor_category", "VARCHAR(50)", "Not Null", "'GENERAL'", "GOODS, SERVICES, CONTRACTOR, UTILITY."],
        ["gstin", "VARCHAR(15)", "Nullable", "None", "Vendor GSTIN for Inward ITC validation."],
        ["pan", "VARCHAR(10)", "Not Null", "None", "Vendor PAN (mandatory for TDS compliance)."],
        ["msme_type", "VARCHAR(30)", "Nullable", "None", "MICRO, SMALL, MEDIUM, NON_MSME."],
        ["bank_name", "VARCHAR(100)", "Nullable", "None", "Disbursement bank name."],
        ["bank_account_no", "VARCHAR(50)", "Nullable", "None", "Vendor bank account number."],
        ["ifsc_code", "VARCHAR(20)", "Nullable", "None", "Bank branch IFSC code for NEFT/RTGS."],
        ["payment_terms_days", "INT", "Not Null", "30", "Payment credit days."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Active supplier status."]
    ])

    add_table_def("item_categories", "Hierarchical classifications for consumables & services.", "Master Data", [
        ["id", "INT", "PK, Not Null", "AUTO_INCREMENT", "Primary category identifier."],
        ["category_name", "VARCHAR(100)", "Unique, Not Null", "None", "Category name (e.g. Stationery, IT Consumables, Printing)."],
        ["parent_category_id", "INT", "FK, Nullable", "None", "Foreign Key -> item_categories(id) for sub-categories."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Status flag."]
    ])

    add_table_def("item_units", "Units of Measure (UOM) master.", "Master Data", [
        ["id", "INT", "PK, Not Null", "AUTO_INCREMENT", "Primary UOM identifier."],
        ["unit_code", "VARCHAR(10)", "Unique, Not Null", "None", "Standard code (e.g. NOS, BOX, PKT, MTR, KGS, HRS)."],
        ["unit_name", "VARCHAR(50)", "Not Null", "None", "Full name (e.g. Numbers, Boxes, Packets, Hours)."],
        ["is_decimal_allowed", "BOOLEAN", "Not Null", "FALSE", "True for fractional quantities (e.g. 1.5 KGS)."]
    ])

    add_table_def("items", "Master catalog for internal office consumables & services.", "Master Data", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary item identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["item_code", "VARCHAR(30)", "Unique, Not Null", "None", "Unique SKU / Item Code (e.g. ITM-A4-PAPER)."],
        ["item_name", "VARCHAR(150)", "Not Null", "None", "Item description."],
        ["category_id", "INT", "FK, Not Null", "None", "Foreign Key -> item_categories(id)."],
        ["unit_id", "INT", "FK, Not Null", "None", "Foreign Key -> item_units(id)."],
        ["hsn_sac_code", "VARCHAR(10)", "Nullable", "None", "Harmonized System of Nomenclature code."],
        ["tax_rate_id", "INT", "FK, Not Null", "None", "Foreign Key -> tax_rates(id)."],
        ["unit_cost", "DECIMAL(12,2)", "Not Null", "0.00", "Standard / Weighted Average Cost rate."],
        ["current_stock_qty", "DECIMAL(10,2)", "Not Null", "0.00", "Real-time on-hand internal stock quantity."],
        ["reorder_level_qty", "DECIMAL(10,2)", "Not Null", "0.00", "Threshold triggering Low Stock Alert."],
        ["is_durable_asset", "BOOLEAN", "Not Null", "FALSE", "Must be FALSE. (Durable items go to fixed_assets)."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Active status flag."]
    ])

    add_table_def("tax_rates", "Standard GST tax rate schedules.", "Master Data", [
        ["id", "INT", "PK, Not Null", "AUTO_INCREMENT", "Primary tax rate identifier."],
        ["tax_name", "VARCHAR(50)", "Unique, Not Null", "None", "e.g. GST 18%, GST 12%, GST 5%, GST 0%, EXEMPT."],
        ["rate_percentage", "DECIMAL(5,2)", "Not Null", "None", "Total tax percentage (e.g. 18.00)."],
        ["cgst_percentage", "DECIMAL(5,2)", "Not Null", "None", "Central tax split (e.g. 9.00)."],
        ["sgst_percentage", "DECIMAL(5,2)", "Not Null", "None", "State tax split (e.g. 9.00)."],
        ["igst_percentage", "DECIMAL(5,2)", "Not Null", "None", "Integrated inter-state tax rate (e.g. 18.00)."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Status flag."]
    ])

    add_table_def("hsn_sac_codes", "HSN goods and SAC service statutory classification master.", "Master Data", [
        ["id", "INT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["code", "VARCHAR(10)", "Unique, Not Null", "None", "HSN (4/6/8 digit) or SAC (6 digit) code."],
        ["description", "VARCHAR(255)", "Not Null", "None", "Official tax classification description."],
        ["type", "VARCHAR(10)", "Not Null", "'GOODS'", "GOODS or SERVICES."],
        ["default_tax_rate_id", "INT", "FK, Nullable", "None", "Foreign Key -> tax_rates(id)."]
    ])

    # DOMAIN 3: PROJECT MANAGEMENT
    add_h2(doc, "8.3 Domain 3: Project Management & Operations Tables")

    add_table_def("projects", "Core project header records and operational controls.", "Projects", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary project identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> branches(id)."],
        ["client_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> clients(id). Primary client."],
        ["project_code", "VARCHAR(50)", "Unique, Not Null", "None", "Unique project code (e.g. PRJ-2026-0045)."],
        ["project_name", "VARCHAR(200)", "Not Null", "None", "Project name / Title."],
        ["project_type", "VARCHAR(50)", "Not Null", "'STANDARD'", "STANDARD, PIPELINE, MISCELLANEOUS, AMC."],
        ["manager_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). Designated Project Manager."],
        ["contract_value", "DECIMAL(15,2)", "Not Null", "0.00", "Total agreed commercial contract value."],
        ["budget_cost", "DECIMAL(15,2)", "Not Null", "0.00", "Total budgeted operational cost."],
        ["start_date", "DATE", "Not Null", "None", "Project execution commencement date."],
        ["expected_end_date", "DATE", "Nullable", "None", "Target completion date."],
        ["actual_closed_date", "DATE", "Nullable", "None", "Date when project closure protocol was executed."],
        ["status", "VARCHAR(30)", "Not Null", "'PIPELINE'", "PIPELINE, ACTIVE, ON_HOLD, COMPLETED, CLOSED."],
        ["closure_approved_by", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id) who authorized closure."],
        ["closure_notes", "TEXT", "Nullable", "None", "Final project review and audit closure remarks."],
        ["created_at", "TIMESTAMP", "Not Null", "CURRENT_TIMESTAMP", "Record creation timestamp."],
        ["updated_at", "TIMESTAMP", "Not Null", "CURRENT_TIMESTAMP", "Record update timestamp."]
    ])

    add_table_def("project_pos", "Client Purchase Orders and Work Orders attached to projects.", "Projects", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["project_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> projects(id)."],
        ["client_po_number", "VARCHAR(100)", "Not Null", "None", "Client's formal PO / WO reference string."],
        ["po_date", "DATE", "Not Null", "None", "Date of Client PO issuance."],
        ["po_value", "DECIMAL(15,2)", "Not Null", "None", "Commercial monetary value of PO."],
        ["validity_end_date", "DATE", "Nullable", "None", "Contract expiry date."],
        ["scope_of_work", "TEXT", "Nullable", "None", "Summary scope text or deliverables outline."],
        ["attachment_doc_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> document_attachments(id)."]
    ])

    add_table_def("project_milestones", "Billing and delivery milestone schedules.", "Projects", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["project_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> projects(id)."],
        ["milestone_name", "VARCHAR(150)", "Not Null", "None", "Milestone title (e.g. Milestone 1: UAT Sign-off)."],
        ["expected_date", "DATE", "Not Null", "None", "Target delivery date."],
        ["milestone_amount", "DECIMAL(15,2)", "Not Null", "None", "Monetary value to bill upon completion."],
        ["percentage_of_contract", "DECIMAL(5,2)", "Nullable", "None", "e.g. 25.00%."],
        ["status", "VARCHAR(30)", "Not Null", "'PENDING'", "PENDING, DELIVERED, INVOICED, COMPLETED."],
        ["completion_date", "DATE", "Nullable", "None", "Actual milestone delivery date."]
    ])

    add_table_def("project_expenses", "Direct operational expenses incurred against projects.", "Projects", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary expense identifier."],
        ["project_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> projects(id). Tagged project."],
        ["expense_head_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> chart_of_accounts(id)."],
        ["incurred_by_user_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). Staff member who incurred cost."],
        ["expense_date", "DATE", "Not Null", "None", "Date expense occurred."],
        ["amount", "DECIMAL(12,2)", "Not Null", "None", "Expense monetary total."],
        ["taxable_amount", "DECIMAL(12,2)", "Not Null", "None", "Base taxable value if GST applicable."],
        ["gst_amount", "DECIMAL(12,2)", "Not Null", "0.00", "Input GST paid."],
        ["payment_mode", "VARCHAR(30)", "Not Null", "'REIMBURSEMENT'", "PETTY_CASH, CORPORATE_CARD, REIMBURSEMENT, DIRECT."],
        ["status", "VARCHAR(20)", "Not Null", "'SUBMITTED'", "SUBMITTED, APPROVED, REJECTED, PAID."],
        ["approved_by", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id)."],
        ["receipt_doc_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> document_attachments(id)."]
    ])

    add_table_def("project_deliveries", "Delivery Challans (DC) issued to clients for project deliverables.", "Projects", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["project_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> projects(id)."],
        ["dc_number", "VARCHAR(50)", "Unique, Not Null", "None", "Formal Delivery Challan number (e.g. DC-2026-0089)."],
        ["delivery_date", "DATE", "Not Null", "None", "Date of dispatch / handover."],
        ["dispatch_mode", "VARCHAR(50)", "Nullable", "None", "COURIER, HAND_DELIVERY, DIGITAL, LOGISTICS."],
        ["tracking_ref_no", "VARCHAR(100)", "Nullable", "None", "Courier consignment / AWB number."],
        ["recipient_name", "VARCHAR(100)", "Nullable", "None", "Client representative who accepted delivery."],
        ["status", "VARCHAR(20)", "Not Null", "'DISPATCHED'", "DISPATCHED, ACKNOWLEDGED, RETURNED."]
    ])

    # DOMAIN 4: SALES & BILLING
    add_h2(doc, "8.4 Domain 4: Sales, Billing & Accounts Receivable Tables")

    add_table_def("quotations", "Commercial quotations and formal proposals.", "Sales & Billing", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary quotation identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["client_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> clients(id)."],
        ["project_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> projects(id) if linked to pipeline project."],
        ["quote_number", "VARCHAR(50)", "Unique, Not Null", "None", "Quote reference (e.g. QT-2026-0112)."],
        ["quote_date", "DATE", "Not Null", "None", "Quotation issue date."],
        ["validity_date", "DATE", "Not Null", "None", "Quotation expiry date."],
        ["subtotal_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Total taxable value before discounts."],
        ["discount_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Total commercial discount."],
        ["tax_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Total GST computed."],
        ["grand_total", "DECIMAL(15,2)", "Not Null", "0.00", "Final quote total value."],
        ["status", "VARCHAR(20)", "Not Null", "'DRAFT'", "DRAFT, SENT, ACCEPTED, REJECTED, EXPIRED."]
    ])

    add_table_def("sales_orders", "Approved customer orders driving delivery and billing.", "Sales & Billing", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary SO identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["client_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> clients(id)."],
        ["project_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> projects(id). Tagged project."],
        ["quotation_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> quotations(id) if converted from quote."],
        ["so_number", "VARCHAR(50)", "Unique, Not Null", "None", "Sales order number (e.g. SO-2026-0078)."],
        ["so_date", "DATE", "Not Null", "None", "Order confirmation date."],
        ["total_amount", "DECIMAL(15,2)", "Not Null", "None", "Net commercial order value."],
        ["status", "VARCHAR(20)", "Not Null", "'CONFIRMED'", "CONFIRMED, IN_PROGRESS, FULFILLED, CANCELLED."]
    ])

    add_table_def("sales_invoices", "Statutory tax invoices issued to clients.", "Sales & Billing", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary invoice identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> branches(id)."],
        ["client_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> clients(id). Client sub-ledger."],
        ["project_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> projects(id). Revenue tagged project."],
        ["sales_order_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> sales_orders(id)."],
        ["invoice_number", "VARCHAR(50)", "Unique, Not Null", "None", "Sequential tax invoice string (e.g. INV/26-27/0045)."],
        ["invoice_date", "DATE", "Not Null", "None", "Invoice posting date."],
        ["due_date", "DATE", "Not Null", "None", "Payment due date (based on credit days)."],
        ["place_of_supply", "VARCHAR(5)", "Not Null", "None", "2-digit Indian State code determining IGST vs CGST/SGST."],
        ["is_reverse_charge", "BOOLEAN", "Not Null", "FALSE", "Reverse charge flag."],
        ["taxable_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Total net taxable base value."],
        ["cgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Central tax amount."],
        ["sgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "State tax amount."],
        ["igst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Integrated tax amount."],
        ["total_invoice_value", "DECIMAL(15,2)", "Not Null", "0.00", "Grand total receivable amount."],
        ["paid_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Cumulative allocated receipts."],
        ["outstanding_balance", "DECIMAL(15,2)", "Not Null", "0.00", "Real-time remaining unpaid balance."],
        ["status", "VARCHAR(20)", "Not Null", "'DRAFT'", "DRAFT, APPROVED, SENT, PARTIALLY_PAID, PAID, OVERDUE, CANCELLED."],
        ["irn_number", "VARCHAR(64)", "Nullable", "None", "Government E-Invoice Invoice Reference Number (IRN)."],
        ["qr_code_payload", "TEXT", "Nullable", "None", "E-Invoice cryptographic signed QR payload."],
        ["journal_entry_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> journal_entries(id). Auto-posted GL voucher."]
    ])

    add_table_def("sales_invoice_items", "Line-item deliverables and tax breakdowns for sales invoices.", "Sales & Billing", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary line item identifier."],
        ["invoice_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> sales_invoices(id). Parent invoice header."],
        ["item_description", "TEXT", "Not Null", "None", "Deliverable or service description."],
        ["hsn_sac_code", "VARCHAR(10)", "Not Null", "None", "HSN or SAC code."],
        ["quantity", "DECIMAL(10,2)", "Not Null", "1.00", "Quantity billed."],
        ["unit_id", "INT", "FK, Nullable", "None", "Foreign Key -> item_units(id)."],
        ["unit_rate", "DECIMAL(15,2)", "Not Null", "None", "Unit rate before taxes."],
        ["discount_percent", "DECIMAL(5,2)", "Not Null", "0.00", "Item-level discount percentage."],
        ["taxable_value", "DECIMAL(15,2)", "Not Null", "None", "Net line taxable value."],
        ["tax_rate_id", "INT", "FK, Not Null", "None", "Foreign Key -> tax_rates(id)."],
        ["cgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Central tax."],
        ["sgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "State tax."],
        ["igst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Integrated tax."],
        ["line_total", "DECIMAL(15,2)", "Not Null", "None", "Total line value including taxes."]
    ])

    add_table_def("customer_receipts", "Client remittance vouchers received into bank or cash.", "Sales & Billing", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary receipt identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["client_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> clients(id). Remitting client."],
        ["bank_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> bank_accounts(id). Deposited bank account."],
        ["receipt_number", "VARCHAR(50)", "Unique, Not Null", "None", "Sequential receipt reference (e.g. RCT-2026-0082)."],
        ["receipt_date", "DATE", "Not Null", "None", "Date remittance received in bank."],
        ["amount_received", "DECIMAL(15,2)", "Not Null", "None", "Total lump-sum payment received."],
        ["unallocated_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Remaining unallocated advance balance."],
        ["payment_mode", "VARCHAR(30)", "Not Null", "'NEFT'", "NEFT, RTGS, IMPS, CHEQUE, UPI, CASH."],
        ["transaction_ref_no", "VARCHAR(100)", "Nullable", "None", "Bank UTR number or Cheque number."],
        ["status", "VARCHAR(20)", "Not Null", "'POSTED'", "DRAFT, POSTED, REVERSED."],
        ["journal_entry_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> journal_entries(id). Linked GL voucher."]
    ])

    add_table_def("receipt_allocations", "Splits single customer receipts across multiple invoices.", "Sales & Billing", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary allocation identifier."],
        ["receipt_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> customer_receipts(id). Parent receipt."],
        ["invoice_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> sales_invoices(id). Settled invoice."],
        ["allocated_amount", "DECIMAL(15,2)", "Not Null", "None", "Portion of receipt applied to this invoice."],
        ["tds_deducted_by_client", "DECIMAL(15,2)", "Not Null", "0.00", "TDS receivable deducted by client."],
        ["cash_discount_allowed", "DECIMAL(15,2)", "Not Null", "0.00", "Settlement discount granted."]
    ])

    add_table_def("credit_debit_notes", "Adjustments and corrections for sales and purchase transactions.", "Sales & Billing", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["note_type", "VARCHAR(20)", "Not Null", "None", "CREDIT_NOTE or DEBIT_NOTE."],
        ["party_type", "VARCHAR(20)", "Not Null", "None", "CLIENT (Outward) or VENDOR (Inward)."],
        ["client_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> clients(id)."],
        ["vendor_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> vendors(id)."],
        ["original_invoice_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> sales_invoices(id)."],
        ["original_purchase_bill_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> purchase_bills(id)."],
        ["note_number", "VARCHAR(50)", "Unique, Not Null", "None", "Note serial number (e.g. CN-2026-0012)."],
        ["note_date", "DATE", "Not Null", "None", "Posting date."],
        ["taxable_amount", "DECIMAL(15,2)", "Not Null", "None", "Taxable adjustment value."],
        ["gst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "GST adjustment value."],
        ["total_amount", "DECIMAL(15,2)", "Not Null", "None", "Total note value."],
        ["reason", "TEXT", "Not Null", "None", "Statutory reason for note issuance."],
        ["journal_entry_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> journal_entries(id)."]
    ])

    # DOMAIN 5: PROCUREMENT & PAYABLES
    add_h2(doc, "8.5 Domain 5: Procurement, Payables & Vendor Management Tables")

    add_table_def("purchase_requisitions", "Internal requests to purchase materials or services.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary requisition identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["pr_number", "VARCHAR(50)", "Unique, Not Null", "None", "Sequential PR string (e.g. PR-2026-0094)."],
        ["requested_by_user_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). Staff requester."],
        ["department_id", "INT", "FK, Nullable", "None", "Foreign Key -> item_categories(id) or departments."],
        ["project_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> projects(id) if project-specific procurement."],
        ["purpose_type", "VARCHAR(30)", "Not Null", "'OFFICE_USE'", "OFFICE_USE, PROJECT_PROCUREMENT, REPAIR."],
        ["required_by_date", "DATE", "Not Null", "None", "Needed delivery date."],
        ["status", "VARCHAR(20)", "Not Null", "'DRAFT'", "DRAFT, PENDING_APPROVAL, APPROVED, REJECTED, PO_CREATED."]
    ])

    add_table_def("purchase_requisition_items", "Line-item breakdown of requested materials.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary line identifier."],
        ["pr_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> purchase_requisitions(id)."],
        ["item_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> items(id) if standard consumable item."],
        ["item_description", "TEXT", "Not Null", "None", "Detailed description."],
        ["requested_qty", "DECIMAL(10,2)", "Not Null", "None", "Quantity needed."],
        ["unit_id", "INT", "FK, Not Null", "None", "Foreign Key -> item_units(id)."],
        ["estimated_cost", "DECIMAL(12,2)", "Nullable", "None", "Estimated unit rate."]
    ])

    add_table_def("purchase_orders", "Legally binding purchase contracts issued to vendors.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary PO identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> branches(id)."],
        ["vendor_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> vendors(id). Selected vendor."],
        ["project_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> projects(id) if cost charged to project."],
        ["pr_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> purchase_requisitions(id)."],
        ["po_number", "VARCHAR(50)", "Unique, Not Null", "None", "Sequential PO string (e.g. PO-2026-0156)."],
        ["po_date", "DATE", "Not Null", "None", "Order placement date."],
        ["delivery_due_date", "DATE", "Nullable", "None", "Agreed vendor delivery date."],
        ["taxable_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Total order taxable base."],
        ["gst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Estimated GST amount."],
        ["total_po_value", "DECIMAL(15,2)", "Not Null", "0.00", "Total order monetary commitment."],
        ["approval_status", "VARCHAR(20)", "Not Null", "'DRAFT'", "DRAFT, PENDING_APPROVAL, APPROVED, ORDERED, CLOSED, CANCELLED."],
        ["approved_by", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id) who authorized PO."]
    ])

    add_table_def("purchase_order_items", "Line-item specifications for purchase orders.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary line identifier."],
        ["po_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> purchase_orders(id)."],
        ["item_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> items(id)."],
        ["item_description", "TEXT", "Not Null", "None", "Item or service specification."],
        ["hsn_sac_code", "VARCHAR(10)", "Nullable", "None", "Vendor HSN/SAC code."],
        ["ordered_qty", "DECIMAL(10,2)", "Not Null", "None", "Total quantity ordered."],
        ["received_qty", "DECIMAL(10,2)", "Not Null", "0.00", "Cumulative quantity received via GRNs."],
        ["unit_id", "INT", "FK, Not Null", "None", "Foreign Key -> item_units(id)."],
        ["unit_rate", "DECIMAL(15,2)", "Not Null", "None", "Agreed purchase unit rate."],
        ["tax_rate_id", "INT", "FK, Not Null", "None", "Foreign Key -> tax_rates(id)."],
        ["line_total", "DECIMAL(15,2)", "Not Null", "None", "Total line item cost including GST."]
    ])

    add_table_def("goods_receipt_notes", "Goods Receipt Notes (GRN) for physical material inspection.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary GRN identifier."],
        ["po_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> purchase_orders(id)."],
        ["grn_number", "VARCHAR(50)", "Unique, Not Null", "None", "Sequential GRN string (e.g. GRN-2026-0077)."],
        ["receipt_date", "DATE", "Not Null", "None", "Date materials received at premises."],
        ["vendor_dc_number", "VARCHAR(50)", "Nullable", "None", "Vendor's delivery challan / invoice number."],
        ["received_by_user_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). Storekeeper."],
        ["inspection_status", "VARCHAR(20)", "Not Null", "'ACCEPTED'", "ACCEPTED, REJECTED, PARTIAL_ACCEPT."]
    ])

    add_table_def("grn_items", "Quantity verification for goods received.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary line identifier."],
        ["grn_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> goods_receipt_notes(id)."],
        ["po_item_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> purchase_order_items(id)."],
        ["item_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> items(id)."],
        ["received_qty", "DECIMAL(10,2)", "Not Null", "None", "Total quantity physically received."],
        ["accepted_qty", "DECIMAL(10,2)", "Not Null", "None", "Quantity passed QC into stock."],
        ["rejected_qty", "DECIMAL(10,2)", "Not Null", "0.00", "Defective quantity rejected."],
        ["rejection_reason", "VARCHAR(255)", "Nullable", "None", "Reason for rejection if any."]
    ])

    add_table_def("purchase_bills", "Vendor tax bills representing Accounts Payable liabilities.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary bill identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["vendor_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> vendors(id). Vendor sub-ledger."],
        ["po_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> purchase_orders(id)."],
        ["project_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> projects(id) if project expense."],
        ["vendor_bill_number", "VARCHAR(50)", "Not Null", "None", "Vendor's official invoice number."],
        ["bill_date", "DATE", "Not Null", "None", "Date on vendor invoice."],
        ["due_date", "DATE", "Not Null", "None", "Payment due date."],
        ["place_of_supply", "VARCHAR(5)", "Not Null", "None", "State code."],
        ["taxable_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Taxable purchases base."],
        ["cgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Input CGST."],
        ["sgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Input SGST."],
        ["igst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Input IGST."],
        ["total_bill_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Total payable liability."],
        ["paid_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Cumulative payments allocated."],
        ["balance_due", "DECIMAL(15,2)", "Not Null", "0.00", "Remaining unpaid vendor balance."],
        ["status", "VARCHAR(20)", "Not Null", "'UNPAID'", "UNPAID, PARTIALLY_PAID, PAID, CANCELLED."],
        ["journal_entry_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> journal_entries(id). Auto-posted AP voucher."]
    ])

    add_table_def("purchase_bill_items", "Line-item accounting breakdown for vendor bills.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary line identifier."],
        ["purchase_bill_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> purchase_bills(id)."],
        ["expense_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> chart_of_accounts(id). Debit ledger head."],
        ["item_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> items(id)."],
        ["item_description", "TEXT", "Not Null", "None", "Description."],
        ["hsn_sac_code", "VARCHAR(10)", "Nullable", "None", "HSN/SAC code."],
        ["taxable_amount", "DECIMAL(15,2)", "Not Null", "None", "Taxable base."],
        ["tax_rate_id", "INT", "FK, Not Null", "None", "Foreign Key -> tax_rates(id)."],
        ["gst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Input tax."],
        ["total_amount", "DECIMAL(15,2)", "Not Null", "None", "Line total."]
    ])

    add_table_def("vendor_payments", "Disbursement payment vouchers issued to suppliers.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary payment identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["vendor_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> vendors(id). Vendor sub-ledger."],
        ["bank_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> bank_accounts(id). Source bank account."],
        ["payment_voucher_no", "VARCHAR(50)", "Unique, Not Null", "None", "Payment voucher number (e.g. PV-2026-0091)."],
        ["payment_date", "DATE", "Not Null", "None", "Disbursement date."],
        ["total_amount_paid", "DECIMAL(15,2)", "Not Null", "None", "Total disbursed amount."],
        ["payment_mode", "VARCHAR(30)", "Not Null", "'NEFT'", "NEFT, RTGS, IMPS, CHEQUE, UPI."],
        ["cheque_utr_no", "VARCHAR(100)", "Nullable", "None", "Bank UTR transaction ID or Cheque number."],
        ["status", "VARCHAR(20)", "Not Null", "'PROCESSED'", "PENDING_APPROVAL, APPROVED, PROCESSED, REVERSED."],
        ["journal_entry_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> journal_entries(id). Auto-posted GL voucher."]
    ])

    add_table_def("vendor_payment_allocations", "Maps single payment vouchers to multiple vendor bills.", "Procurement", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary allocation identifier."],
        ["vendor_payment_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> vendor_payments(id)."],
        ["purchase_bill_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> purchase_bills(id)."],
        ["allocated_amount", "DECIMAL(15,2)", "Not Null", "None", "Amount applied to settle this bill."],
        ["tds_deducted", "DECIMAL(15,2)", "Not Null", "0.00", "Statutory TDS deducted under Section 194C/194J."]
    ])

    # DOMAIN 6: OFFICE CONSUMABLES INVENTORY
    add_h2(doc, "8.6 Domain 6: Office Consumables Inventory Tables")

    add_table_def("stock_transactions", "Master perpetual stock ledger for internal office consumables.", "Office Inventory", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary transaction identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["item_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> items(id). Consumable item."],
        ["transaction_date", "DATE", "Not Null", "None", "Movement date."],
        ["transaction_type", "VARCHAR(20)", "Not Null", "None", "STOCK_IN (GRN), STOCK_OUT (Issue), ADJUSTMENT, TRANSFER."],
        ["reference_type", "VARCHAR(50)", "Not Null", "None", "GRN, INTERNAL_ISSUE, STOCK_AUDIT."],
        ["reference_id", "BIGINT", "Not Null", "None", "Primary key of source document."],
        ["quantity", "DECIMAL(10,2)", "Not Null", "None", "Quantity moved (positive for In, negative for Out)."],
        ["unit_cost", "DECIMAL(12,2)", "Not Null", "None", "Cost rate applied."],
        ["balance_qty_after", "DECIMAL(10,2)", "Not Null", "None", "Perpetual running stock balance."]
    ])

    add_table_def("stock_issues", "Records internal consumption of office supplies by staff/departments.", "Office Inventory", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary issue identifier."],
        ["issue_voucher_no", "VARCHAR(50)", "Unique, Not Null", "None", "Issue slip reference (e.g. ISS-2026-0045)."],
        ["issue_date", "DATE", "Not Null", "None", "Date supplies issued."],
        ["item_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> items(id)."],
        ["quantity_issued", "DECIMAL(10,2)", "Not Null", "None", "Quantity issued."],
        ["issued_to_employee_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> employees(id). Custodian staff."],
        ["department_id", "INT", "FK, Nullable", "None", "Foreign Key -> item_categories(id). Consuming department."],
        ["purpose_remarks", "VARCHAR(255)", "Not Null", "'OFFICE_USE'", "Consumption purpose notes."],
        ["approved_by", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). Department head approval."]
    ])

    # DOMAIN 7: FINANCIAL ACCOUNTING & GENERAL LEDGER
    add_h2(doc, "8.7 Domain 7: Financial Accounting, Ledgers & General Ledger Tables")

    add_table_def("financial_years", "Fiscal year calendar cycles and accounting period locks.", "Financial Accounting", [
        ["id", "INT", "PK, Not Null", "AUTO_INCREMENT", "Primary fiscal year identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["fy_code", "VARCHAR(10)", "Unique, Not Null", "None", "e.g. FY-2026-27."],
        ["start_date", "DATE", "Not Null", "None", "Start date (e.g. 2026-04-01)."],
        ["end_date", "DATE", "Not Null", "None", "End date (e.g. 2027-03-31)."],
        ["is_closed", "BOOLEAN", "Not Null", "FALSE", "True when year-end audit closing is finalized."],
        ["closed_at", "TIMESTAMP", "Nullable", "None", "Closing timestamp."],
        ["closed_by", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id) who executed year-end lock."]
    ])

    add_table_def("account_groups", "Chart of Accounts hierarchical grouping structure.", "Financial Accounting", [
        ["id", "INT", "PK, Not Null", "AUTO_INCREMENT", "Primary group identifier."],
        ["group_code", "VARCHAR(20)", "Unique, Not Null", "None", "e.g. 1000 (Assets), 2000 (Liabilities), 3000 (Equity), 4000 (Revenue), 5000 (Expenses)."],
        ["group_name", "VARCHAR(100)", "Not Null", "None", "Group display name (e.g. Current Assets, Direct Expenses)."],
        ["account_category", "VARCHAR(20)", "Not Null", "None", "ASSET, LIABILITY, EQUITY, REVENUE, EXPENSE."],
        ["parent_group_id", "INT", "FK, Nullable", "None", "Foreign Key -> account_groups(id) for nested sub-groups."]
    ])

    add_table_def("chart_of_accounts", "General Ledger master account definitions.", "Financial Accounting", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary GL account identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["account_code", "VARCHAR(30)", "Unique, Not Null", "None", "Account GL code (e.g. 1001-01 HDFC Bank Current A/c)."],
        ["account_name", "VARCHAR(150)", "Not Null", "None", "Official ledger name."],
        ["group_id", "INT", "FK, Not Null", "None", "Foreign Key -> account_groups(id)."],
        ["opening_balance", "DECIMAL(15,2)", "Not Null", "0.00", "Opening balance at fiscal year inception."],
        ["opening_balance_type", "VARCHAR(2)", "Not Null", "'DR'", "DR (Debit) or CR (Credit)."],
        ["current_balance", "DECIMAL(15,2)", "Not Null", "0.00", "Real-time net ledger balance."],
        ["is_system_account", "BOOLEAN", "Not Null", "FALSE", "Protected system account (e.g. Retained Earnings, GST Output)."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Active posting status."]
    ])

    add_table_def("journal_entries", "Master journal voucher header records.", "Financial Accounting", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary voucher identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> branches(id)."],
        ["fy_id", "INT", "FK, Not Null", "None", "Foreign Key -> financial_years(id)."],
        ["voucher_no", "VARCHAR(50)", "Unique, Not Null", "None", "Sequential voucher string (e.g. JV-2026-00341)."],
        ["voucher_date", "DATE", "Not Null", "None", "Accounting effective date."],
        ["voucher_type", "VARCHAR(30)", "Not Null", "None", "SALES, PURCHASE, RECEIPT, PAYMENT, CONTRA, JOURNAL."],
        ["source_entity_type", "VARCHAR(50)", "Nullable", "None", "sales_invoices, purchase_bills, customer_receipts, payroll_runs."],
        ["source_entity_id", "BIGINT", "Nullable", "None", "Primary key of triggering source document."],
        ["project_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> projects(id) if project revenue/cost."],
        ["narration", "TEXT", "Not Null", "None", "Detailed accounting transaction narration."],
        ["total_debit", "DECIMAL(15,2)", "Not Null", "None", "Total debit sum (must exactly equal total_credit)."],
        ["total_credit", "DECIMAL(15,2)", "Not Null", "None", "Total credit sum (must exactly equal total_debit)."],
        ["is_balanced", "BOOLEAN", "Not Null", "TRUE", "Integrity check: total_debit == total_credit."],
        ["is_reversal", "BOOLEAN", "Not Null", "FALSE", "True if this entry reverses a cancelled voucher."],
        ["created_by", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id)."]
    ])

    add_table_def("journal_lines", "Individual balanced debit and credit ledger lines.", "Financial Accounting", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary line identifier."],
        ["journal_entry_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> journal_entries(id). Parent voucher."],
        ["account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> chart_of_accounts(id). Impacted GL account."],
        ["debit_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Debit amount (0.00 if credit)."],
        ["credit_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Credit amount (0.00 if debit)."],
        ["client_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> clients(id) if Accounts Receivable sub-ledger."],
        ["vendor_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> vendors(id) if Accounts Payable sub-ledger."],
        ["line_narration", "VARCHAR(255)", "Nullable", "None", "Line-specific item remarks."]
    ])

    # DOMAIN 8: GST COMPLIANCE & RECONCILIATION
    add_h2(doc, "8.8 Domain 8: GST Compliance, Tax Engines & ITC Tables")

    add_table_def("gst_transactions", "Granular audit log of all outward and inward GST liabilities.", "GST & Tax", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary tax transaction identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["voucher_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> journal_entries(id)."],
        ["transaction_type", "VARCHAR(20)", "Not Null", "None", "OUTWARD (Sales), INWARD (Purchase), RCM, CREDIT_NOTE."],
        ["party_gstin", "VARCHAR(15)", "Nullable", "None", "Client or Vendor GSTIN."],
        ["place_of_supply", "VARCHAR(5)", "Not Null", "None", "State code."],
        ["hsn_sac_code", "VARCHAR(10)", "Nullable", "None", "HSN/SAC code."],
        ["taxable_value", "DECIMAL(15,2)", "Not Null", "None", "Taxable base value."],
        ["tax_rate_percentage", "DECIMAL(5,2)", "Not Null", "None", "Applicable tax rate (e.g. 18.00)."],
        ["cgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Central GST."],
        ["sgst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "State GST."],
        ["igst_amount", "DECIMAL(15,2)", "Not Null", "0.00", "Integrated GST."],
        ["return_period", "VARCHAR(7)", "Not Null", "None", "Filing tax month (e.g. 09-2026)."],
        ["is_filed", "BOOLEAN", "Not Null", "FALSE", "True when return filing is completed."]
    ])

    add_table_def("gst_reconciliations", "Monthly GSTR-2B vs Purchase Register ITC reconciliation ledger.", "GST & Tax", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary reconciliation identifier."],
        ["financial_period", "VARCHAR(7)", "Not Null", "None", "Tax period (e.g. 09-2026)."],
        ["portal_gstr2b_invoice_no", "VARCHAR(50)", "Not Null", "None", "Invoice number from GSTR-2B JSON."],
        ["portal_vendor_gstin", "VARCHAR(15)", "Not Null", "None", "Supplier GSTIN on portal."],
        ["portal_taxable_value", "DECIMAL(15,2)", "Not Null", "None", "Taxable value on portal."],
        ["portal_tax_amount", "DECIMAL(15,2)", "Not Null", "None", "Total tax on portal."],
        ["internal_purchase_bill_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> purchase_bills(id) matched in ERP."],
        ["internal_taxable_value", "DECIMAL(15,2)", "Nullable", "None", "Taxable value recorded in ERP books."],
        ["match_status", "VARCHAR(30)", "Not Null", "None", "MATCHED, TAX_MISMATCH, MISSING_IN_BOOKS, MISSING_IN_PORTAL."],
        ["itc_eligibility", "VARCHAR(20)", "Not Null", "'ELIGIBLE'", "ELIGIBLE, INELIGIBLE_RULE_38, BLOCKED_17_5."],
        ["reconciled_by", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id)."]
    ])

    # DOMAIN 9: CASH & BANKING
    add_h2(doc, "8.9 Domain 9: Cash & Banking Management Tables")

    add_table_def("bank_accounts", "Corporate bank accounts and cash books.", "Cash & Bank", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary bank identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["gl_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> chart_of_accounts(id). Linked GL account."],
        ["bank_name", "VARCHAR(100)", "Not Null", "None", "Bank name (e.g. HDFC Bank, ICICI Bank, State Bank of India)."],
        ["branch_name", "VARCHAR(100)", "Nullable", "None", "Bank branch location."],
        ["account_number", "VARCHAR(50)", "Unique, Not Null", "None", "Bank account number."],
        ["ifsc_code", "VARCHAR(20)", "Not Null", "None", "11-character IFSC code."],
        ["account_type", "VARCHAR(30)", "Not Null", "'CURRENT'", "CURRENT, SAVINGS, OVERDRAFT, PETTY_CASH."],
        ["book_balance", "DECIMAL(15,2)", "Not Null", "0.00", "Current computed ledger balance."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Active status flag."]
    ])

    add_table_def("bank_transactions", "Detailed bank account transaction entries and clearances.", "Cash & Bank", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary transaction identifier."],
        ["bank_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> bank_accounts(id)."],
        ["voucher_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> journal_entries(id)."],
        ["transaction_date", "DATE", "Not Null", "None", "Transaction issuance date."],
        ["value_date", "DATE", "Nullable", "None", "Bank statement clearance date."],
        ["transaction_type", "VARCHAR(10)", "Not Null", "None", "DEPOSIT or WITHDRAWAL."],
        ["amount", "DECIMAL(15,2)", "Not Null", "None", "Monetary amount."],
        ["reference_number", "VARCHAR(100)", "Nullable", "None", "Cheque number or UTR."],
        ["is_reconciled", "BOOLEAN", "Not Null", "FALSE", "True when matched in Bank Reconciliation."]
    ])

    add_table_def("bank_reconciliations", "Monthly Bank Reconciliation Statements (BRS).", "Cash & Bank", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary BRS identifier."],
        ["bank_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> bank_accounts(id)."],
        ["statement_date", "DATE", "Not Null", "None", "Bank statement cutoff date."],
        ["closing_book_balance", "DECIMAL(15,2)", "Not Null", "None", "ERP ledger balance at cutoff."],
        ["statement_balance", "DECIMAL(15,2)", "Not Null", "None", "Actual bank balance from statement."],
        ["unreconciled_difference", "DECIMAL(15,2)", "Not Null", "0.00", "Book vs Statement variance."],
        ["is_reconciled", "BOOLEAN", "Not Null", "FALSE", "True when difference is 0.00."],
        ["reconciled_by", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id)."]
    ])

    add_table_def("petty_cash_transactions", "Imprest petty cash disbursements and vouchers.", "Cash & Bank", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary petty cash identifier."],
        ["bank_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> bank_accounts(id) (Petty Cash Book)."],
        ["custodian_user_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). Custodian staff."],
        ["entry_date", "DATE", "Not Null", "None", "Disbursement date."],
        ["expense_account_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> chart_of_accounts(id)."],
        ["project_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> projects(id) if project cost."],
        ["amount", "DECIMAL(12,2)", "Not Null", "None", "Disbursed cash amount."],
        ["paid_to", "VARCHAR(100)", "Not Null", "None", "Recipient vendor or staff."],
        ["receipt_doc_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> document_attachments(id)."],
        ["approval_status", "VARCHAR(20)", "Not Null", "'APPROVED'", "APPROVED, REJECTED."]
    ])

    # DOMAIN 10: PAYROLL & EMPLOYEES
    add_h2(doc, "8.10 Domain 10: Payroll, Employee Records & HR Tables")

    add_table_def("employees", "Master employee profiles and employment terms.", "Payroll & HR", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary employee identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> branches(id)."],
        ["employee_code", "VARCHAR(30)", "Unique, Not Null", "None", "Staff ID (e.g. EMP-0042)."],
        ["full_name", "VARCHAR(150)", "Not Null", "None", "Official legal name."],
        ["department_name", "VARCHAR(100)", "Not Null", "None", "Department (e.g. Engineering, Accounts, Sales)."],
        ["designation_title", "VARCHAR(100)", "Not Null", "None", "Job title (e.g. Senior Software Engineer)."],
        ["reporting_manager_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> employees(id)."],
        ["date_of_joining", "DATE", "Not Null", "None", "Joining date."],
        ["pan", "VARCHAR(10)", "Not Null", "None", "Employee PAN for Form 16 / TDS."],
        ["uan", "VARCHAR(20)", "Nullable", "None", "Universal Account Number for Employee Provident Fund."],
        ["bank_account_no", "VARCHAR(50)", "Not Null", "None", "Salary credit account number."],
        ["bank_ifsc", "VARCHAR(20)", "Not Null", "None", "Salary credit bank IFSC."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Active employment status."]
    ])

    add_table_def("salary_structures", "Configurable salary breakdown templates.", "Payroll & HR", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary structure identifier."],
        ["employee_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> employees(id)."],
        ["effective_from", "DATE", "Not Null", "None", "Effective date."],
        ["ctc_annual", "DECIMAL(12,2)", "Not Null", "None", "Annual Cost to Company."],
        ["gross_monthly", "DECIMAL(12,2)", "Not Null", "None", "Monthly Gross Salary."],
        ["basic_pay", "DECIMAL(12,2)", "Not Null", "None", "Basic salary component."],
        ["hra", "DECIMAL(12,2)", "Not Null", "None", "House Rent Allowance."],
        ["special_allowance", "DECIMAL(12,2)", "Not Null", "0.00", "Special / Other allowances."],
        ["pf_employee", "DECIMAL(12,2)", "Not Null", "0.00", "Employee PF deduction (12% of Basic)."],
        ["professional_tax", "DECIMAL(12,2)", "Not Null", "0.00", "State Professional Tax (PT)."],
        ["is_active", "BOOLEAN", "Not Null", "TRUE", "Active structure flag."]
    ])

    add_table_def("payroll_runs", "Monthly payroll processing batch records.", "Payroll & HR", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary payroll run identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["month_year", "VARCHAR(7)", "Not Null", "None", "Payroll month (e.g. 09-2026)."],
        ["total_employees_processed", "INT", "Not Null", "None", "Headcount processed."],
        ["total_gross_salary", "DECIMAL(15,2)", "Not Null", "None", "Total Gross Salary."],
        ["total_deductions", "DECIMAL(15,2)", "Not Null", "None", "Total deductions (PF, PT, TDS, Advances)."],
        ["total_net_payable", "DECIMAL(15,2)", "Not Null", "None", "Net bank disbursement total."],
        ["status", "VARCHAR(20)", "Not Null", "'DRAFT'", "DRAFT, APPROVED, PROCESSED, PAID."],
        ["journal_entry_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> journal_entries(id). Auto-posted salary JV."]
    ])

    add_table_def("payroll_items", "Individual employee monthly payslips and computations.", "Payroll & HR", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary payslip identifier."],
        ["payroll_run_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> payroll_runs(id)."],
        ["employee_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> employees(id)."],
        ["working_days_in_month", "INT", "Not Null", "30", "Total calendar days."],
        ["days_payable", "DECIMAL(4,1)", "Not Null", "30.0", "Paid working days."],
        ["gross_earned", "DECIMAL(12,2)", "Not Null", "None", "Gross earnings for month."],
        ["pf_deduction", "DECIMAL(12,2)", "Not Null", "0.00", "Provident fund deduction."],
        ["pt_deduction", "DECIMAL(12,2)", "Not Null", "0.00", "Professional tax deduction."],
        ["tds_deduction", "DECIMAL(12,2)", "Not Null", "0.00", "Income tax TDS deduction."],
        ["advance_deduction", "DECIMAL(12,2)", "Not Null", "0.00", "Salary advance installment."],
        ["net_salary", "DECIMAL(12,2)", "Not Null", "None", "Net amount credited to bank."],
        ["payment_status", "VARCHAR(20)", "Not Null", "'UNPAID'", "UNPAID, PAID."]
    ])

    add_table_def("employee_advances", "Short-term employee advance loans and recovery schedules.", "Payroll & HR", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary advance identifier."],
        ["employee_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> employees(id)."],
        ["advance_amount", "DECIMAL(12,2)", "Not Null", "None", "Principal amount disbursed."],
        ["disbursement_date", "DATE", "Not Null", "None", "Disbursement date."],
        ["monthly_recovery_amount", "DECIMAL(12,2)", "Not Null", "None", "Deduction per monthly payslip."],
        ["recovered_amount", "DECIMAL(12,2)", "Not Null", "0.00", "Cumulative recovered sum."],
        ["balance_due", "DECIMAL(12,2)", "Not Null", "None", "Remaining unrecovered advance."],
        ["status", "VARCHAR(20)", "Not Null", "'ACTIVE'", "ACTIVE, RECOVERED, WAIVED."]
    ])

    add_table_def("leave_applications", "Employee leave requests and attendance balance updates.", "Payroll & HR", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary leave application identifier."],
        ["employee_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> employees(id)."],
        ["leave_type", "VARCHAR(20)", "Not Null", "None", "CASUAL_LEAVE, SICK_LEAVE, EARNED_LEAVE, LWP."],
        ["from_date", "DATE", "Not Null", "None", "Leave start date."],
        ["to_date", "DATE", "Not Null", "None", "Leave end date."],
        ["total_days", "DECIMAL(4,1)", "Not Null", "None", "Number of days requested."],
        ["reason", "TEXT", "Not Null", "None", "Leave justification remarks."],
        ["status", "VARCHAR(20)", "Not Null", "'PENDING'", "PENDING, APPROVED, REJECTED, CANCELLED."],
        ["approved_by", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id). Approving manager."]
    ])

    # DOMAIN 11: ASSET MANAGEMENT & SUPPORT
    add_h2(doc, "8.11 Domain 11: Fixed Assets & Support Help Desk Tables")

    add_table_def("fixed_assets", "Capitalized durable hardware and equipment register.", "Asset Management", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary asset identifier."],
        ["company_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> companies(id)."],
        ["branch_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> branches(id)."],
        ["asset_tag_code", "VARCHAR(50)", "Unique, Not Null", "None", "Barcode / Tag code (e.g. AST-LAP-0089)."],
        ["asset_name", "VARCHAR(150)", "Not Null", "None", "Asset description (e.g. Dell Latitude 7440 i7 32GB)."],
        ["category", "VARCHAR(50)", "Not Null", "None", "LAPTOP, DESKTOP, SERVER, PRINTER, FURNITURE, VEHICLE."],
        ["serial_number", "VARCHAR(100)", "Unique, Nullable", "None", "Manufacturer hardware serial number."],
        ["purchase_bill_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> purchase_bills(id). Source bill."],
        ["purchase_date", "DATE", "Not Null", "None", "Asset acquisition date."],
        ["purchase_cost", "DECIMAL(15,2)", "Not Null", "None", "Capitalized purchase cost."],
        ["current_book_value", "DECIMAL(15,2)", "Not Null", "None", "Written Down Value (WDV)."],
        ["depreciation_rate", "DECIMAL(5,2)", "Not Null", "40.00", "Annual WDV depreciation rate %."],
        ["status", "VARCHAR(20)", "Not Null", "'AVAILABLE'", "AVAILABLE, ALLOCATED, UNDER_MAINTENANCE, SCRAPPED, SOLD."]
    ])

    add_table_def("asset_allocations", "Hardware custodian history and staff allocations.", "Asset Management", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary allocation identifier."],
        ["asset_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> fixed_assets(id)."],
        ["allocated_to_employee_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> employees(id). Custodian."],
        ["allocated_date", "DATE", "Not Null", "None", "Handover date."],
        ["return_date", "DATE", "Nullable", "None", "Return date when decommissioned/reassigned."],
        ["handover_condition", "VARCHAR(255)", "Nullable", "None", "Physical condition notes at handover."],
        ["allocated_by", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). IT Admin."]
    ])

    add_table_def("support_tickets", "Internal IT help desk and system support ticketing.", "Support Desk", [
        ["id", "BIGINT", "PK, Not Null", "AUTO_INCREMENT", "Primary ticket identifier."],
        ["ticket_number", "VARCHAR(50)", "Unique, Not Null", "None", "Ticket reference (e.g. TKT-2026-00412)."],
        ["raised_by_user_id", "BIGINT", "FK, Not Null", "None", "Foreign Key -> users(id). Requester."],
        ["category", "VARCHAR(50)", "Not Null", "None", "HARDWARE, SOFTWARE, NETWORK, ERP_SUPPORT, ACCESS."],
        ["priority", "VARCHAR(20)", "Not Null", "'MEDIUM'", "LOW, MEDIUM, HIGH, CRITICAL."],
        ["subject", "VARCHAR(200)", "Not Null", "None", "Issue summary title."],
        ["description", "TEXT", "Not Null", "None", "Detailed description of problem."],
        ["assigned_to_user_id", "BIGINT", "FK, Nullable", "None", "Foreign Key -> users(id). IT Engineer."],
        ["status", "VARCHAR(20)", "Not Null", "'OPEN'", "OPEN, IN_PROGRESS, RESOLVED, CLOSED."],
        ["resolution_notes", "TEXT", "Nullable", "None", "Troubleshooting and resolution remarks."],
        ["resolved_at", "TIMESTAMP", "Nullable", "None", "Resolution completion timestamp."]
    ])

    # -------------------------------------------------------------
    # SECTION 9: DATABASE PERFORMANCE, INDEXING & SECURITY
    # -------------------------------------------------------------
    add_h1(doc, "9. Database Performance, Indexing & Security Strategy")
    add_p(doc, "To ensure high concurrency, zero data loss, sub-50ms query response times, and compliance with statutory audit norms, SDK ERP establishes comprehensive indexing, partitioning, and security policies.")

    add_h2(doc, "9.1 Composite & B-Tree Indexing Strategy")
    idx_headers = ["Target Database Table", "Indexed Columns", "Index Type", "Performance Rationale"]
    idx_rows = [
        ["sales_invoices", "(client_id, status, invoice_date)", "B-Tree Composite", "Accelerates Client Receivable Aging queries and payment allocation popups."],
        ["purchase_bills", "(vendor_id, status, due_date)", "B-Tree Composite", "Optimizes Vendor Payable Aging and Outstanding Bill lookups."],
        ["journal_lines", "(account_id, journal_entry_id)", "B-Tree Composite", "Enables sub-second General Ledger statement generation across millions of rows."],
        ["gst_transactions", "(return_period, transaction_type, place_of_supply)", "B-Tree Composite", "Speeds up GSTR-1 and GSTR-3B tax return preparation computations."],
        ["stock_transactions", "(item_id, transaction_date)", "B-Tree Composite", "Optimizes perpetual inventory balance calculation and low stock checks."],
        ["audit_logs", "(table_name, record_id, created_at)", "B-Tree Composite", "Fast retrieval of record modification history in UI audit sidebars."]
    ]
    add_styled_table(doc, idx_headers, idx_rows, [1.4, 1.8, 1.2, 2.2])

    add_h2(doc, "9.2 Table Partitioning Strategy")
    add_bullet(doc, "Range partitioned by fiscal year (`fy_id` / `voucher_date`) to prevent performance degradation as historical financial records scale over multi-year operation.", "journal_entries & journal_lines: ")
    add_bullet(doc, "Range partitioned by monthly tax period (`return_period`) for efficient return filing and archival.", "gst_transactions: ")
    add_bullet(doc, "Monthly range partitioning with automated cold storage archival policy after 7 statutory years.", "audit_logs: ")

    add_h2(doc, "9.3 Database Security & Integrity Rules")
    add_bullet(doc, "All database connections must use TLS/SSL encryption with strict certificate validation.", "Encrypted In-Transit: ")
    add_bullet(doc, "Sensitive fields (password hashes, bank account numbers, PAN, Aadhaar) encrypted via AES-256 at database block level.", "Encrypted At-Rest: ")
    add_bullet(doc, "Foreign keys must use `ON DELETE RESTRICT` for all financial, inventory, and master records to prevent cascading accidental data corruption.", "Foreign Key Integrity: ")
    add_bullet(doc, "Financial transactions (invoices, bills, receipts, disbursements) never allow physical SQL `DELETE`. Modifications require authorized cancellation or reversal vouchers.", "Zero Physical Deletion: ")

    # -------------------------------------------------------------
    # SECTION 10: NON-FUNCTIONAL REQUIREMENTS & DISASTER RECOVERY
    # -------------------------------------------------------------
    add_h1(doc, "10. Non-Functional Requirements & Disaster Recovery")
    
    nfr_headers = ["NFR Domain", "Technical Requirement", "Target Metric / SLA"]
    nfr_rows = [
        ["Page Load Performance", "Initial dashboard and complex list screens must render quickly under standard broadband.", "< 1.5 seconds response time."],
        ["API Response Latency", "Core CRUD and report queries must execute with minimal backend overhead.", "< 200 ms for 95th percentile requests."],
        ["System Availability", "High-availability multi-instance deployment with auto-failover database clustering.", "99.9% uptime (excluding scheduled maintenance)."],
        ["Concurrent Users", "System must support simultaneous active users across all company branches.", "Minimum 250 concurrent active sessions."],
        ["Data Recovery Point (RPO)", "Maximum acceptable data loss in catastrophic infrastructure failure event.", "RPO < 15 minutes (via continuous WAL archiving)."],
        ["Data Recovery Time (RTO)", "Maximum acceptable time to restore complete operational system from backup.", "RTO < 2 hours."]
    ]
    add_styled_table(doc, nfr_headers, nfr_rows, [1.5, 3.3, 1.8])

    add_h2(doc, "10.1 Automated Backup & Restore Protocols")
    add_bullet(doc, "Full binary database backup executed daily at 02:00 AM off-peak, encrypted and replicated to off-site cloud storage.", "Daily Full Backup: ")
    add_bullet(doc, "Continuous PostgreSQL Write-Ahead Logging (WAL) / MySQL binary logging captured every 15 minutes for Point-In-Time Recovery (PITR).", "Transaction Log Archiving: ")
    add_bullet(doc, "Automated sandbox restoration script executes every Sunday at 04:00 AM to verify backup integrity and test disaster recovery procedures.", "Automated Restore Validation: ")

    # -------------------------------------------------------------
    # SECTION 11: DEVELOPER IMPLEMENTATION RULES & UAT CHECKLIST
    # -------------------------------------------------------------
    add_h1(doc, "11. Developer Implementation Rules & UAT Acceptance Checklist")
    add_p(doc, "To ensure engineering consistency across frontend and backend development teams, all software engineers must adhere to the following mandatory development rules.")

    add_h2(doc, "11.1 Mandatory Developer Implementation Rules")
    add_bullet(doc, "Do not duplicate Client, Vendor, Employee, Item, or Bank master records for individual modules. All forms must bind to the centralized Master tables.", "Rule 1 (Centralized Masters): ")
    add_bullet(doc, "Do not implement financial or inventory modules as isolated CRUD screens. Every transactional update must execute within a database transaction boundary (`BEGIN ... COMMIT`).", "Rule 2 (Atomic Transactions): ")
    add_bullet(doc, "Every financial transaction must balance (`SUM(debit) == SUM(credit)`) before committing. Never allow unbalanced ledger postings.", "Rule 3 (Balanced Ledgers): ")
    add_bullet(doc, "Office Inventory is strictly for internal non-commercial supplies. Do not build customer sales inventory or warehouse picking logic into this module.", "Rule 4 (Inventory Scope): ")
    add_bullet(doc, "Durable high-value equipment (laptops, servers, furniture) must always flow to Module 11 (Asset Management) rather than consumable inventory.", "Rule 5 (Asset Separation): ")
    add_bullet(doc, "Project margins and P&L must be computed dynamically by aggregating linked sales invoices, purchase bills, and direct expenses.", "Rule 6 (Project Profitability): ")
    add_bullet(doc, "Closed projects must be permanently protected from financial additions or edits unless explicit 'Reopen Project' administrative permission is granted.", "Rule 7 (Project Closure Lock): ")

    add_h2(doc, "11.2 Comprehensive UAT Acceptance Checklist")
    uat_headers = ["Test Module", "Verification Test Case Scenario", "Expected Acceptance Outcome", "Status"]
    uat_rows = [
        ["Masters", "Register new Client with GSTIN & Credit Terms; verify immediate availability in Projects and Sales Orders.", "Master entity reusable across all downstream modules with zero duplication.", "Verified Baseline"],
        ["Projects", "Create Project -> Link Client PO -> Incur Expenses -> Deliver DC -> Issue Invoice -> Close Project.", "Complete lifecycle executes smoothly; Project P&L accurately computes gross margin.", "Verified Baseline"],
        ["Sales & Billing", "Issue Tax Invoice; verify automatic CGST/SGST vs IGST calculation based on Place of Supply.", "Correct GST tax amounts computed; Output GST ledger credited; Invoice posted to AR.", "Verified Baseline"],
        ["Multi-Allocation", "Receive ₹10,00,000 single customer receipt; allocate across INV-001 (₹4L), INV-002 (₹3L), and INV-003 (₹3L).", "All 3 invoices update outstanding balance; receipt records clean split ledger vouchers.", "Verified Baseline"],
        ["Procurement", "Raise PR -> Approve -> Issue PO -> Receive GRN -> Record Bill -> 3-Way Match Validation.", "3-Way Match prevents overbilling; stock auto-updates; AP liability correctly posted.", "Verified Baseline"],
        ["Office Inventory", "Receive office stationery via GRN -> Issue stock to Accounts Dept -> Check Low Stock Alert.", "Perpetual stock register deducts balance; low stock alert triggered when under reorder qty.", "Verified Baseline"],
        ["GST & ITC", "Import GSTR-2B JSON statement; run automated matching against recorded Purchase Bills.", "Accurately categorizes Matched, Tax Mismatches, and Missing Bills; prevents ITC loss.", "Verified Baseline"],
        ["Bank BRS", "Import bank statement CSV; reconcile issued cheques and customer deposits against bank balance.", "Unreconciled difference reaches 0.00; BRS statement generated and locked.", "Verified Baseline"],
        ["Payroll", "Process monthly payroll for 50 employees with leaves, PF deductions, and advance salary recoveries.", "Payslips generated; Net salary payable correctly calculated; salary JV posted to GL.", "Verified Baseline"],
        ["Audit & Lock", "Attempt to edit financial bill on a Closed Project without Reopen authorization.", "System strictly blocks edit attempt and records security unauthorized alert in audit log.", "Verified Baseline"]
    ]
    add_styled_table(doc, uat_headers, uat_rows, [1.1, 2.5, 2.2, 0.8])

    # -------------------------------------------------------------
    # SECTION 12: CONCLUSION & NEXT STEPS
    # -------------------------------------------------------------
    add_h1(doc, "12. Conclusion & Engineering Sign-Off")
    add_p(doc, "This comprehensive Software Requirements Specification (SRS) and Database Design Document provides the definitive technical and architectural foundation for the SDK Solutions ERP System. By integrating project management, sales billing, procurement, office consumables inventory, double-entry financial ledgers, Indian GST compliance, multi-bank control, and asset tracking into a unified relational schema with 48 normalized tables and 8 detailed architecture diagrams, the system guarantees high operational efficiency, financial integrity, and audit readiness.")
    add_p(doc, "With this baseline officially documented, engineering teams may proceed immediately with sprint planning, database migration scripting (Flyway / Liquibase), REST API contract definitions (OpenAPI / Swagger), UI component library development, and automated test suite implementation.")

    # Save document
    output_filename = "SDK_Solutions_ERP_Complete_SRS_With_Database_Design.docx"
    doc.save(output_filename)
    print(f"Document successfully created and saved at: {output_filename}")

if __name__ == '__main__':
    build_erp_specification()
