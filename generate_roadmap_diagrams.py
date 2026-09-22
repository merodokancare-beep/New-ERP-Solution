import os
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import matplotlib.patches as patches

def create_roadmap_phases_diagram():
    fig, ax = plt.subplots(figsize=(13, 9.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    # Main Header
    ax.text(50, 97, "SDK Solutions ERP — Phase-by-Phase Technical Roadmap & Delivery Dependencies", 
            ha='center', va='center', fontsize=14, fontweight='bold', color='#1B365D')
    ax.text(50, 94.5, "12 Sequential Implementation Phases with Embedded Security-by-Design & Multi-Tier Gates", 
            ha='center', va='center', fontsize=9.5, fontstyle='italic', color='#4A5568')
    
    # Helper to draw phase box
    def draw_phase(x, y, w, h, p_num, title, items, bg_col, border_col, header_col, sec_badge=None):
        r = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.6", ec=border_col, fc=bg_col, lw=1.5)
        ax.add_patch(r)
        
        # Header bar
        hr = patches.Rectangle((x+0.5, y+h-3.2), w-1, 2.8, ec='none', fc=header_col)
        ax.add_patch(hr)
        
        ax.text(x + 1.2, y + h - 1.8, p_num, fontsize=8.5, fontweight='bold', color='#FFFFFF')
        ax.text(x + w - 1.2, y + h - 1.8, title, fontsize=8.5, fontweight='bold', color='#FFFFFF', ha='right')
        
        # Bullet items
        for idx, itm in enumerate(items):
            ax.text(x + 1.2, y + h - 4.5 - (idx * 1.7), "• " + itm, fontsize=7.2, color='#2D3748')
            
        # Security badge if present
        if sec_badge:
            sb = patches.FancyBboxPatch((x + w - 24, y + 0.8), 23, 2.2, boxstyle="round,pad=0.2", ec="#C53030", fc="#FFF5F5", lw=0.8)
            ax.add_patch(sb)
            ax.text(x + w - 12.5, y + 1.9, "[SEC] " + sec_badge, fontsize=6.2, fontweight='bold', color='#C53030', ha='center', va='center')

    # Row 1: Foundational Tier (Y = 76)
    draw_phase(4, 76, 43, 15, "PHASE 0", "Solution & DB DDL (48 Tables)", [
        "SDK.ERP.sln Multi-Project Scaffolding",
        "48 POCO Entities across 10 Schemas",
        "EF Core Fluent API & SQL Server 2022 Migrations",
        "Composite B-Tree Indexes & Partitioning",
        "Master Seeding (Roles, Slabs, standard COA)"
    ], "#F0F4F8", "#2B547E", "#1B365D", "Roslyn Analyzers & Gitleaks")

    draw_phase(53, 76, 43, 15, "PHASE 1", "Security, Identity & Core Engines", [
        "ASP.NET Core Identity & Cookie Session",
        "MFA (TOTP) for Admin & Approver Roles",
        "[BranchContextFilter] & Global Query Filter",
        "Granular RBAC Authorization Handlers",
        "Unified Multi-Level Approval State Machine",
        "Tamper-Evident SHA-256 Audit Interceptor"
    ], "#FEFCBF", "#D69E2E", "#B7791F", "IDOR Defense & ASVS Auth")

    # Connector Arrow Row 1 -> Row 2
    ax.annotate('', xy=(25, 71), xytext=(25, 76),
                arrowprops=dict(arrowstyle="->", color="#1B365D", lw=2))
    ax.annotate('', xy=(75, 71), xytext=(75, 76),
                arrowprops=dict(arrowstyle="->", color="#B7791F", lw=2))

    # Row 2: Master Data & Accounting Core (Y = 56)
    draw_phase(4, 56, 43, 15, "PHASE 2", "Master Data Management", [
        "Company, Branch & Period Lock Controllers",
        "Client Master (GSTIN/PAN strict regex)",
        "Vendor Master (MSME type, Bank IFSC)",
        "Item Catalog (Consumable vs Asset flag)",
        "Voucher Alphanumeric Sequence Generator"
    ], "#EBF8FF", "#3182CE", "#2B6CB0", "Strict Input Regex & Anti-XSS")

    draw_phase(53, 56, 43, 15, "PHASE 3", "General Ledger & Double-Entry", [
        "5-Level Chart of Accounts Hierarchy",
        "IAccountingPostingService (Balanced JVs)",
        "Automatic Posting Adapters (Invoices, Bills)",
        "Immutable Ledgers (Reversals only)",
        "Dapper High-Speed Trial Balance & GL"
    ], "#E6FFFA", "#319795", "#234E52", "Balanced Ledger Guarantee")

    # Connector Arrow Row 2 -> Row 3
    ax.annotate('', xy=(25, 51), xytext=(25, 56),
                arrowprops=dict(arrowstyle="->", color="#2B6CB0", lw=2))
    ax.annotate('', xy=(75, 51), xytext=(75, 56),
                arrowprops=dict(arrowstyle="->", color="#234E52", lw=2))

    # Row 3: Operational Core (Y = 36)
    draw_phase(4, 36, 29, 15, "PHASE 4", "Projects & 11-Tab Hub", [
        "11-Tab Razor ViewComponents",
        "Client POs & Billing Milestones",
        "Direct Project Expenses Claims",
        "Delivery Challans Handover",
        "Real-Time Project P&L Engine",
        "6-Point Project Closure Lock"
    ], "#FAF5FF", "#805AD5", "#553C9A", "Closure Audit Immutability")

    draw_phase(36, 36, 29, 15, "PHASE 5", "Sales, Billing & AR", [
        "Quotation to Sales Order Pipeline",
        "Tax Invoicing Engine (CGST/SGST/IGST)",
        "QuestPDF Statutory Invoice Generator",
        "Multi-Invoice Receipt Allocation Grid",
        "Credit / Debit Notes & Reversals",
        "Receivables Aging Analytics"
    ], "#F0FFF4", "#38A169", "#22543D", "Anti-CSRF & Parameterized")

    draw_phase(68, 36, 28, 15, "PHASE 6", "Procurement & Office Stock", [
        "PR & Purchase Orders Routing",
        "Goods Receipt Note (GRN) QC",
        "3-Way Match Verification (PO-GRN-Bill)",
        "Vendor Payment & TDS Deduction",
        "Office Consumables Inventory Register",
        "Low Stock Alerts & Internal Issue"
    ], "#FFF5F5", "#E53E3E", "#9B2C2C", "3-Way Match Validation")

    # Connector Arrow Row 3 -> Row 4
    ax.annotate('', xy=(18, 31), xytext=(18, 36),
                arrowprops=dict(arrowstyle="->", color="#553C9A", lw=2))
    ax.annotate('', xy=(50, 31), xytext=(50, 36),
                arrowprops=dict(arrowstyle="->", color="#22543D", lw=2))
    ax.annotate('', xy=(82, 31), xytext=(82, 36),
                arrowprops=dict(arrowstyle="->", color="#9B2C2C", lw=2))

    # Row 4: Compliance, Treasury & HR (Y = 16)
    draw_phase(4, 16, 29, 15, "PHASE 7", "Cash, Banking & BRS", [
        "Current, Overdraft & Cash Accounts",
        "Inter-Bank Contra Transfers",
        "CSV/OFX Bank Statement Parser",
        "BRS Reconciliation Statement",
        "Imprest Petty Cash & Floats"
    ], "#EDF2F7", "#4A5568", "#2D3748", "Audit Reconciled Locks")

    draw_phase(36, 16, 29, 15, "PHASE 8", "GST & ITC Reconciliation", [
        "GST Audit Register (Outward/Inward)",
        "GSTR-1 (T4, T7, T9, T12) Summaries",
        "GSTR-3B Tax Liability Computation",
        "GSTR-2B JSON 4-Way ITC Matcher",
        "E-Invoice (IRN) & E-Way Bill Ready"
    ], "#EBF8FF", "#3182CE", "#1A365D", "Cryptographic JSON Verify")

    draw_phase(68, 16, 28, 15, "PHASE 9", "Payroll & Fixed Assets", [
        "Employee Profiles & Salary Structure",
        "Monthly Payroll Batch Processing",
        "QuestPDF Monthly Payslips",
        "Employee Advance Loans Recovery",
        "Fixed Asset Register & WDV Depr."
    ], "#FFFAF0", "#DD6B20", "#7B341E", "AES-256 PII Encrypted")

    # Final Row: Executive Intelligence & Dedicated Security Audit Gate (Y = 1)
    # Connector Arrow to Final Row
    ax.annotate('', xy=(30, 11), xytext=(30, 16),
                arrowprops=dict(arrowstyle="->", color="#1A365D", lw=2))
    ax.annotate('', xy=(70, 11), xytext=(70, 16),
                arrowprops=dict(arrowstyle="->", color="#7B341E", lw=2))

    # Final Phase 10 & 11
    draw_phase(4, 1, 43, 10, "PHASE 10", "Executive Dashboards & Enterprise Reports", [
        "KPI Cards (Revenue, Margins, Cash Position, Receivables/Payables)",
        "20+ Financial, Project, Sales, Purchase & GST Reports (ClosedXML & QuestPDF)",
        "Exhaustive UAT Verification against SRS 10-Scenario Matrix"
    ], "#EAEFF5", "#2B547E", "#1B365D", "Role-Tailored Scope")

    draw_phase(53, 1, 43, 10, "PHASE 11 (FINAL GATE)", "Security Audit, VAPT & Certification", [
        "OWASP ASVS Level 2 Verification & Privilege Escalation Audit",
        "Automated OWASP ZAP & Manual Burp Suite Pro Penetration Testing",
        "Zero-SQLi, Anti-CSRF, SHA-256 Tamper Chaining & STRIDE Threat Model"
    ], "#FFF5F5", "#E53E3E", "#9B2C2C", "CERT-In / VAPT Sign-Off")

    plt.tight_layout()
    plt.savefig('diagram_roadmap_phases.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_roadmap_phases.png successfully.")

def create_security_audit_flow_diagram():
    fig, ax = plt.subplots(figsize=(12, 6), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 93, "SDK Solutions ERP — Phase 11 Enterprise Security Audit & VAPT Lifecycle", 
            ha='center', va='center', fontsize=13, fontweight='bold', color='#1B365D')
    ax.text(50, 87, "Multi-Stage Verification Framework for OWASP ASVS, Penetration Testing & Compliance Sign-Off", 
            ha='center', va='center', fontsize=8.5, fontstyle='italic', color='#4A5568')
    
    stages = [
        ("1. Static Analysis & Code Review", "SonarQube & Roslyn Analyzers\nGitleaks Secret Scanning\nDependency CVE (Snyk)", 5, 25, 16, 50, "#EBF8FF", "#3182CE", "#1A365D"),
        ("2. Automated DAST & Vuln Scan", "OWASP ZAP Active Scan\nSQLMap Injection Testing\nHeader & SSL/TLS 1.3 Audit", 24, 25, 16, 50, "#FEFCBF", "#D69E2E", "#744210"),
        ("3. Manual VAPT Penetration", "Burp Suite Pro Suite\nIDOR & Privilege Escalation\nSession Hijacking & Replay\nRace Condition on Payments", 43, 25, 17, 50, "#FFF5F5", "#E53E3E", "#742A2A"),
        ("4. Cryptographic & Data Audit", "AES-256 Column Encryption\nAzure Key Vault Keys\nTamper-Evident SHA-256 Audit\nZero Plaintext PII/Passwords", 63, 25, 16, 50, "#FAF5FF", "#805AD5", "#44337A"),
        ("5. Remediation & Clean Sign-Off", "0 Critical & 0 High Findings\nSTRIDE Threat Model Dossier\nFormal VAPT Closure Report\nAudit Compliance Certificate", 82, 25, 15, 50, "#F0FFF4", "#38A169", "#1C4532")
    ]
    
    for title, desc, x, y, w, h, bg_col, border_col, header_col in stages:
        r = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.5", ec=border_col, fc=bg_col, lw=1.5)
        ax.add_patch(r)
        
        # Header bar
        hr = patches.Rectangle((x+0.3, y+h-5), w-0.6, 4.6, ec='none', fc=header_col)
        ax.add_patch(hr)
        ax.text(x + w/2, y + h - 2.7, title, fontsize=7.2, fontweight='bold', color='#FFFFFF', ha='center', va='center')
        
        # Body text
        ax.text(x + 1, y + h - 9, desc, fontsize=7.0, color='#2D3748', va='top', linespacing=1.6)
        
    # Flow arrows
    for i in range(len(stages) - 1):
        x_start = stages[i][2] + stages[i][4]
        x_end = stages[i+1][2]
        y_pos = 50
        ax.annotate('', xy=(x_end, y_pos), xytext=(x_start, y_pos),
                    arrowprops=dict(arrowstyle="->", color="#1B365D", lw=2))
        
    plt.tight_layout()
    plt.savefig('diagram_security_audit_flow.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_security_audit_flow.png successfully.")

if __name__ == '__main__':
    create_roadmap_phases_diagram()
    create_security_audit_flow_diagram()
