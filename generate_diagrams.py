import os
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
import matplotlib.patches as patches

def set_diagram_style():
    plt.rcParams['font.sans-serif'] = 'DejaVu Sans'
    plt.rcParams['font.family'] = 'sans-serif'

def create_architecture_diagram():
    fig, ax = plt.subplots(figsize=(12, 7.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    # Title
    ax.text(50, 96, "SDK Solutions ERP - Multi-Tier Technical Architecture", 
            ha='center', va='center', fontsize=15, fontweight='bold', color='#1B365D')
    
    # Layer 1: Client / Presentation
    rect1 = patches.FancyBboxPatch((5, 78), 90, 14, boxstyle="round,pad=1", ec="#1B365D", fc="#EAEFF5", lw=1.5)
    ax.add_patch(rect1)
    ax.text(7, 89, "1. PRESENTATION & USER INTERFACE LAYER", fontsize=10, fontweight='bold', color='#1B365D')
    
    ui_boxes = [
        ("Web Portal (SPA)", 8, 80, 19, 6),
        ("Project & Operations UI", 29, 80, 21, 6),
        ("Accounts & GST Desk", 52, 80, 20, 6),
        ("Admin & Mobile Web", 74, 80, 19, 6)
    ]
    for name, x, y, w, h in ui_boxes:
        r = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.5", ec="#2B547E", fc="#FFFFFF", lw=1)
        ax.add_patch(r)
        ax.text(x + w/2, y + h/2, name, ha='center', va='center', fontsize=8, fontweight='bold', color='#2C3E50')

    # Layer 2: API Gateway & Security
    rect2 = patches.FancyBboxPatch((5, 61), 90, 13, boxstyle="round,pad=1", ec="#2B547E", fc="#F0F4F8", lw=1.5)
    ax.add_patch(rect2)
    ax.text(7, 71, "2. API GATEWAY, AUTHENTICATION & CROSS-CUTTING LAYER", fontsize=10, fontweight='bold', color='#1B365D')
    
    gw_boxes = [
        ("REST / GraphQL API", 8, 63, 19, 5.5),
        ("JWT Auth & Session RBAC", 29, 63, 21, 5.5),
        ("Approval Workflow Engine", 52, 63, 20, 5.5),
        ("Audit Logger & Dispatcher", 74, 63, 19, 5.5)
    ]
    for name, x, y, w, h in gw_boxes:
        r = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.5", ec="#3182CE", fc="#FFFFFF", lw=1)
        ax.add_patch(r)
        ax.text(x + w/2, y + h/2, name, ha='center', va='center', fontsize=8, fontweight='bold', color='#2B6CB0')

    # Layer 3: Application Core Business Services
    rect3 = patches.FancyBboxPatch((5, 27), 90, 30, boxstyle="round,pad=1", ec="#0D5C75", fc="#E6F4F8", lw=1.5)
    ax.add_patch(rect3)
    ax.text(7, 54, "3. APPLICATION SERVICES & CORE BUSINESS DOMAINS LAYER", fontsize=10, fontweight='bold', color='#0D5C75')
    
    services = [
        ("Project Lifecycle & P&L", 8, 45, 26, 6),
        ("Sales, Invoicing & Allocations", 37, 45, 26, 6),
        ("Procurement & Office Stock", 66, 45, 26, 6),
        ("General Ledger & Payments", 8, 37, 26, 6),
        ("GST Engine & ITC Reconciliation", 37, 37, 26, 6),
        ("Cash & Multi-Bank Control", 66, 37, 26, 6),
        ("Payroll & Employee Mgmt", 8, 29, 26, 6),
        ("Asset Register & Allocation", 37, 29, 26, 6),
        ("Report & Analytics Suite", 66, 29, 26, 6)
    ]
    for name, x, y, w, h in services:
        r = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.5", ec="#008080", fc="#FFFFFF", lw=1)
        ax.add_patch(r)
        ax.text(x + w/2, y + h/2, name, ha='center', va='center', fontsize=8, fontweight='bold', color='#005B5C')

    # Layer 4: Data & Storage Infrastructure
    rect4 = patches.FancyBboxPatch((5, 5), 90, 18, boxstyle="round,pad=1", ec="#234E52", fc="#E2E8F0", lw=1.5)
    ax.add_patch(rect4)
    ax.text(7, 20, "4. DATA ACCESS, PERSISTENCE & STORAGE INFRASTRUCTURE", fontsize=10, fontweight='bold', color='#1A202C')
    
    data_boxes = [
        ("Relational Database\n(PostgreSQL / MySQL 3NF)", 8, 7, 26, 10.5),
        ("Redis Memory Cache\n(Sessions & Key Performance)", 37, 7, 26, 10.5),
        ("Document / Object Store\n(Secure Files & S3 Blob)", 66, 7, 26, 10.5)
    ]
    for name, x, y, w, h in data_boxes:
        r = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.5", ec="#4A5568", fc="#FFFFFF", lw=1)
        ax.add_patch(r)
        ax.text(x + w/2, y + h/2, name, ha='center', va='center', fontsize=8, fontweight='bold', color='#2D3748')

    plt.tight_layout()
    plt.savefig('diagram_1_architecture.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_1_architecture.png")

def create_business_workflow_diagram():
    fig, ax = plt.subplots(figsize=(12, 8.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 97, "SDK Solutions ERP - End-to-End Business & Accounting Lifecycle", 
            ha='center', va='center', fontsize=14, fontweight='bold', color='#1B365D')
    
    # 5 Major Stages
    stages = [
        ("STAGE 1: Master Setup & Project Onboarding", 90, "#1B365D", [
            ("Client & Vendor Registration", 6, 81, 26, 6),
            ("Project Creation & Contract/PO", 37, 81, 26, 6),
            ("Budget & Resource Allocation", 68, 81, 26, 6)
        ]),
        ("STAGE 2: Commercial Sales & Procurement Operations", 74, "#2B547E", [
            ("Sales Order & Milestones", 6, 65, 26, 6),
            ("Purchase Requisition -> PO", 37, 65, 26, 6),
            ("GRN -> Office Stock In / Delivery", 68, 65, 26, 6)
        ]),
        ("STAGE 3: Billing, Invoicing & Cost Recording", 58, "#008080", [
            ("Sales Invoice Generation", 6, 49, 26, 6),
            ("Purchase Bill Validation", 37, 49, 26, 6),
            ("Project Direct Expenses", 68, 49, 26, 6)
        ]),
        ("STAGE 4: Financial Settlements, GST & Ledgers", 42, "#805AD5", [
            ("Multi-Invoice Customer Receipt", 6, 33, 26, 6),
            ("Approved Vendor Payments", 37, 33, 26, 6),
            ("GST Engine & ITC Reconciliation", 68, 33, 26, 6)
        ]),
        ("STAGE 5: Project P&L, Financial Audit & Project Closure", 26, "#2C3E50", [
            ("Double-Entry Ledger Posting", 6, 17, 26, 6),
            ("Real-Time Project P&L & Margin", 37, 17, 26, 6),
            ("Audit Review & Project Closure Lock", 68, 17, 26, 6)
        ])
    ]
    
    for title, y_head, col, boxes in stages:
        ax.text(6, y_head, title, fontsize=9.5, fontweight='bold', color=col)
        for name, x, y, w, h in boxes:
            r = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.5", ec=col, fc="#F8FAFC", lw=1.2)
            ax.add_patch(r)
            ax.text(x + w/2, y + h/2, name, ha='center', va='center', fontsize=7.5, fontweight='bold', color='#1A202C')

    # Arrows connecting stages
    ax.annotate('', xy=(19, 73), xytext=(19, 81), arrowprops=dict(arrowstyle="->", color="#1B365D", lw=1.5))
    ax.annotate('', xy=(50, 73), xytext=(50, 81), arrowprops=dict(arrowstyle="->", color="#1B365D", lw=1.5))
    ax.annotate('', xy=(81, 73), xytext=(81, 81), arrowprops=dict(arrowstyle="->", color="#1B365D", lw=1.5))

    ax.annotate('', xy=(19, 57), xytext=(19, 65), arrowprops=dict(arrowstyle="->", color="#2B547E", lw=1.5))
    ax.annotate('', xy=(50, 57), xytext=(50, 65), arrowprops=dict(arrowstyle="->", color="#2B547E", lw=1.5))
    ax.annotate('', xy=(81, 57), xytext=(81, 65), arrowprops=dict(arrowstyle="->", color="#2B547E", lw=1.5))

    ax.annotate('', xy=(19, 41), xytext=(19, 49), arrowprops=dict(arrowstyle="->", color="#008080", lw=1.5))
    ax.annotate('', xy=(50, 41), xytext=(50, 49), arrowprops=dict(arrowstyle="->", color="#008080", lw=1.5))
    ax.annotate('', xy=(81, 41), xytext=(81, 49), arrowprops=dict(arrowstyle="->", color="#008080", lw=1.5))

    ax.annotate('', xy=(19, 25), xytext=(19, 33), arrowprops=dict(arrowstyle="->", color="#805AD5", lw=1.5))
    ax.annotate('', xy=(50, 25), xytext=(50, 33), arrowprops=dict(arrowstyle="->", color="#805AD5", lw=1.5))
    ax.annotate('', xy=(81, 25), xytext=(81, 33), arrowprops=dict(arrowstyle="->", color="#805AD5", lw=1.5))

    # Summary bar at bottom
    r_foot = patches.FancyBboxPatch((6, 4), 88, 7, boxstyle="round,pad=0.5", ec="#2C3E50", fc="#EDF2F7", lw=1)
    ax.add_patch(r_foot)
    ax.text(50, 7.5, "CORE PRINCIPLE: Single Centralized Master Record -> Linked Transaction Chain -> Automated Balanced Postings -> Audit Integrity", 
            ha='center', va='center', fontsize=8, fontweight='bold', color='#2D3748')

    plt.tight_layout()
    plt.savefig('diagram_2_business_workflow.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_2_business_workflow.png")

def draw_entity_card(ax, title, x, y, w, h, header_color, fields):
    # Main box
    box = patches.FancyBboxPatch((x, y), w, h, boxstyle="round,pad=0.2", ec=header_color, fc="#FFFFFF", lw=1.2)
    ax.add_patch(box)
    
    # Header bar
    hh = min(3.8, h * 0.22)
    head = patches.Rectangle((x, y + h - hh), w, hh, ec=header_color, fc=header_color)
    ax.add_patch(head)
    ax.text(x + w/2, y + h - hh/2, title, ha='center', va='center', fontsize=8, fontweight='bold', color='#FFFFFF')
    
    # Fields
    y_pos = y + h - hh - 1.2
    for f in fields:
        is_pk = f.startswith("PK")
        is_fk = f.startswith("FK")
        f_color = "#C53030" if is_pk else ("#2B6CB0" if is_fk else "#2D3748")
        ax.text(x + 1, y_pos, f, ha='left', va='center', fontsize=6.8, color=f_color)
        y_pos -= 2.0

def create_master_erd():
    fig, ax = plt.subplots(figsize=(13, 9), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 97.5, "SDK Solutions ERP - Master High-Level Entity Relationship Diagram (ERD)", 
            ha='center', va='center', fontsize=14, fontweight='bold', color='#1B365D')

    # Draw major entities
    draw_entity_card(ax, "COMPANIES", 4, 76, 26, 17, "#1B365D", [
        "PK id : BIGINT",
        "name : VARCHAR(150)",
        "gstin : VARCHAR(15)",
        "pan : VARCHAR(10)",
        "financial_year_start : DATE"
    ])
    
    draw_entity_card(ax, "USERS & ROLES", 37, 76, 26, 17, "#2B547E", [
        "PK id : BIGINT",
        "FK role_id : INT",
        "username / email : VARCHAR",
        "password_hash : VARCHAR",
        "reporting_manager_id : BIGINT"
    ])

    draw_entity_card(ax, "CLIENTS", 70, 76, 26, 17, "#0D5C75", [
        "PK id : BIGINT",
        "client_code : VARCHAR(30)",
        "name : VARCHAR(150)",
        "gstin : VARCHAR(15)",
        "state_code : VARCHAR(5)"
    ])

    draw_entity_card(ax, "PROJECTS", 70, 48, 26, 22, "#008080", [
        "PK id : BIGINT",
        "FK client_id : BIGINT",
        "project_code : VARCHAR(50)",
        "project_name : VARCHAR(200)",
        "contract_value : DECIMAL(15,2)",
        "status : VARCHAR(30)",
        "manager_id : BIGINT"
    ])

    draw_entity_card(ax, "SALES_INVOICES", 70, 20, 26, 22, "#2B6CB0", [
        "PK id : BIGINT",
        "FK project_id : BIGINT",
        "FK client_id : BIGINT",
        "invoice_number : VARCHAR(50)",
        "total_taxable : DECIMAL(15,2)",
        "total_gst : DECIMAL(15,2)",
        "status : VARCHAR(20)"
    ])

    draw_entity_card(ax, "VENDORS", 4, 48, 26, 18, "#744210", [
        "PK id : BIGINT",
        "vendor_code : VARCHAR(30)",
        "name : VARCHAR(150)",
        "gstin : VARCHAR(15)",
        "payment_terms : VARCHAR(50)"
    ])

    draw_entity_card(ax, "PURCHASE_ORDERS", 4, 20, 26, 22, "#8C3B00", [
        "PK id : BIGINT",
        "FK vendor_id : BIGINT",
        "FK project_id : BIGINT",
        "po_number : VARCHAR(50)",
        "total_amount : DECIMAL(15,2)",
        "approval_status : VARCHAR(20)"
    ])

    draw_entity_card(ax, "CHART_OF_ACCOUNTS", 37, 48, 26, 22, "#4A5568", [
        "PK id : BIGINT",
        "account_code : VARCHAR(30)",
        "account_name : VARCHAR(100)",
        "FK group_id : INT",
        "account_type : VARCHAR(30)",
        "current_balance : DECIMAL(15,2)"
    ])

    draw_entity_card(ax, "JOURNAL_ENTRIES", 37, 20, 26, 22, "#553C9A", [
        "PK id : BIGINT",
        "voucher_no : VARCHAR(50)",
        "voucher_date : DATE",
        "voucher_type : VARCHAR(30)",
        "FK project_id : BIGINT",
        "total_debit : DECIMAL(15,2)",
        "total_credit : DECIMAL(15,2)"
    ])

    draw_entity_card(ax, "CUSTOMER_RECEIPTS & ALLOCATIONS", 37, 2, 40, 14, "#2C7A7B", [
        "PK id : BIGINT | FK client_id : BIGINT | FK bank_account_id : BIGINT",
        "receipt_number : VARCHAR(50) | amount_received : DECIMAL(15,2)",
        "FK invoice_id -> allocated_amount : DECIMAL(15,2) (Multi-Allocation)"
    ])

    # Connectors
    ax.annotate('', xy=(83, 70), xytext=(83, 76), arrowprops=dict(arrowstyle="->", color="#008080", lw=1.5))
    ax.annotate('', xy=(83, 42), xytext=(83, 48), arrowprops=dict(arrowstyle="->", color="#2B6CB0", lw=1.5))
    ax.annotate('', xy=(17, 42), xytext=(17, 48), arrowprops=dict(arrowstyle="->", color="#8C3B00", lw=1.5))
    ax.annotate('', xy=(30, 31), xytext=(70, 55), arrowprops=dict(arrowstyle="->", color="#8C3B00", lw=1.2, ls='--'))
    ax.annotate('', xy=(63, 31), xytext=(70, 31), arrowprops=dict(arrowstyle="->", color="#553C9A", lw=1.5))
    ax.annotate('', xy=(50, 42), xytext=(50, 48), arrowprops=dict(arrowstyle="->", color="#553C9A", lw=1.5))

    plt.tight_layout()
    plt.savefig('diagram_3_master_erd.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_3_master_erd.png")

def create_domain_erd_masters():
    fig, ax = plt.subplots(figsize=(12, 8), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 97, "Domain 1: Master Data, Security (RBAC) & Approvals ERD", 
            ha='center', va='center', fontsize=13, fontweight='bold', color='#1B365D')

    draw_entity_card(ax, "COMPANIES", 5, 68, 26, 24, "#1B365D", [
        "PK id : BIGINT",
        "company_name : VARCHAR(150)",
        "legal_name : VARCHAR(150)",
        "gstin : VARCHAR(15)",
        "pan : VARCHAR(10)",
        "tan : VARCHAR(10)",
        "email / phone : VARCHAR",
        "base_currency : VARCHAR(3)"
    ])

    draw_entity_card(ax, "BRANCHES", 37, 68, 26, 24, "#2B547E", [
        "PK id : BIGINT",
        "FK company_id : BIGINT",
        "branch_code : VARCHAR(30)",
        "branch_name : VARCHAR(100)",
        "state_code : VARCHAR(5)",
        "gstin : VARCHAR(15)",
        "is_head_office : BOOLEAN"
    ])

    draw_entity_card(ax, "USERS", 69, 68, 26, 24, "#2B6CB0", [
        "PK id : BIGINT",
        "FK company_id : BIGINT",
        "FK branch_id : BIGINT",
        "FK role_id : INT",
        "username : VARCHAR(50)",
        "email : VARCHAR(100)",
        "password_hash : VARCHAR(255)",
        "is_active : BOOLEAN"
    ])

    draw_entity_card(ax, "ROLES & PERMISSIONS", 69, 36, 26, 26, "#0D5C75", [
        "PK id : INT (Roles)",
        "role_name : VARCHAR(50)",
        "description : VARCHAR(255)",
        "--- ROLE_PERMISSIONS ---",
        "FK role_id : INT",
        "module_code : VARCHAR(50)",
        "submodule_code : VARCHAR(50)",
        "can_view / can_create : BOOL",
        "can_edit / can_delete : BOOL",
        "can_approve / can_post : BOOL"
    ])

    draw_entity_card(ax, "APPROVAL_RULES & LOGS", 37, 36, 26, 26, "#805AD5", [
        "PK id : BIGINT (Rules)",
        "module : VARCHAR(50)",
        "min_amount / max_amount : NUM",
        "FK approver_role_id : INT",
        "--- APPROVAL_REQUESTS ---",
        "FK record_id : BIGINT",
        "status : PENDING/APPROVED",
        "action_by : BIGINT",
        "action_date : TIMESTAMP",
        "comments : TEXT"
    ])

    draw_entity_card(ax, "AUDIT_LOGS", 5, 36, 26, 26, "#4A5568", [
        "PK id : BIGINT",
        "FK user_id : BIGINT",
        "table_name : VARCHAR(60)",
        "record_id : BIGINT",
        "action : INSERT/UPDATE/DELETE",
        "old_values : JSONB",
        "new_values : JSONB",
        "ip_address : VARCHAR(45)",
        "timestamp : TIMESTAMP"
    ])

    draw_entity_card(ax, "DOCUMENT_ATTACHMENTS", 37, 5, 58, 25, "#2C7A7B", [
        "PK id : BIGINT | FK entity_type : VARCHAR(50) | FK entity_id : BIGINT",
        "file_name : VARCHAR(255) | file_path : VARCHAR(500) | mime_type : VARCHAR(100)",
        "file_size_bytes : BIGINT | uploaded_by : BIGINT | uploaded_at : TIMESTAMP | version : INT"
    ])

    plt.tight_layout()
    plt.savefig('diagram_4_erd_masters_security.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_4_erd_masters_security.png")

def create_domain_erd_projects_sales():
    fig, ax = plt.subplots(figsize=(13, 8.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 97, "Domain 2: Project Management, Sales & Accounts Receivable ERD", 
            ha='center', va='center', fontsize=13, fontweight='bold', color='#1B365D')

    draw_entity_card(ax, "CLIENTS", 4, 68, 26, 24, "#1B365D", [
        "PK id : BIGINT",
        "client_code : VARCHAR(30)",
        "company_name : VARCHAR(150)",
        "gstin : VARCHAR(15)",
        "billing_address : TEXT",
        "state_code : VARCHAR(5)",
        "credit_limit : DECIMAL(15,2)",
        "is_active : BOOLEAN"
    ])

    draw_entity_card(ax, "PROJECTS", 37, 68, 26, 24, "#008080", [
        "PK id : BIGINT",
        "FK client_id : BIGINT",
        "project_code : VARCHAR(50)",
        "project_name : VARCHAR(200)",
        "contract_value : DECIMAL(15,2)",
        "start_date / end_date : DATE",
        "status : PIPELINE/ACTIVE/CLOSED",
        "FK manager_id : BIGINT"
    ])

    draw_entity_card(ax, "PROJECT_POS / WORK_ORDERS", 70, 68, 26, 24, "#2B547E", [
        "PK id : BIGINT",
        "FK project_id : BIGINT",
        "client_po_number : VARCHAR(100)",
        "po_date : DATE",
        "po_value : DECIMAL(15,2)",
        "scope_of_work : TEXT",
        "attachment_id : BIGINT"
    ])

    draw_entity_card(ax, "SALES_ORDERS & ITEMS", 70, 36, 26, 26, "#2B6CB0", [
        "PK id : BIGINT (SO Header)",
        "FK project_id : BIGINT",
        "FK client_id : BIGINT",
        "so_number : VARCHAR(50)",
        "total_amount : DECIMAL(15,2)",
        "--- SALES_ORDER_ITEMS ---",
        "PK id : BIGINT | FK so_id : BIGINT",
        "item_description : TEXT",
        "quantity : DECIMAL(10,2)",
        "unit_price : DECIMAL(15,2)",
        "tax_rate : DECIMAL(5,2)"
    ])

    draw_entity_card(ax, "SALES_INVOICES & ITEMS", 37, 36, 26, 26, "#1A365D", [
        "PK id : BIGINT (Invoice)",
        "FK project_id : BIGINT",
        "FK client_id : BIGINT",
        "invoice_number : VARCHAR(50)",
        "taxable_amount : DECIMAL(15,2)",
        "cgst / sgst / igst : DECIMAL",
        "total_amount : DECIMAL(15,2)",
        "status : DRAFT/SENT/PAID",
        "--- INVOICE_ITEMS ---",
        "FK invoice_id : BIGINT",
        "hsn_sac : VARCHAR(10)"
    ])

    draw_entity_card(ax, "PROJECT_EXPENSES & DELIVERIES", 4, 36, 26, 26, "#C53030", [
        "PK id : BIGINT (Expense)",
        "FK project_id : BIGINT",
        "expense_type_id : INT",
        "amount : DECIMAL(15,2)",
        "paid_by : BIGINT",
        "--- DELIVERIES ---",
        "PK id : BIGINT",
        "dc_number : VARCHAR(50)",
        "delivery_date : DATE",
        "received_by : VARCHAR(100)"
    ])

    draw_entity_card(ax, "CUSTOMER_RECEIPTS & ALLOCATIONS", 20, 5, 60, 26, "#2C7A7B", [
        "PK id : BIGINT (Receipt Header) | FK client_id : BIGINT | FK bank_account_id : BIGINT",
        "receipt_number : VARCHAR(50) | receipt_date : DATE | amount_received : DECIMAL(15,2)",
        "payment_mode : NEFT/RTGS/CHEQUE/UPI | reference_no : VARCHAR(100)",
        "--- RECEIPT_ALLOCATIONS (Multi-Invoice Split) ---",
        "PK id : BIGINT | FK receipt_id : BIGINT | FK invoice_id : BIGINT",
        "allocated_amount : DECIMAL(15,2) | discount_amount : DECIMAL(15,2)"
    ])

    # Connectors
    ax.annotate('', xy=(37, 80), xytext=(30, 80), arrowprops=dict(arrowstyle="->", color="#008080", lw=1.5))
    ax.annotate('', xy=(70, 80), xytext=(63, 80), arrowprops=dict(arrowstyle="->", color="#2B547E", lw=1.5))
    ax.annotate('', xy=(50, 62), xytext=(50, 68), arrowprops=dict(arrowstyle="->", color="#1A365D", lw=1.5))
    ax.annotate('', xy=(50, 31), xytext=(50, 36), arrowprops=dict(arrowstyle="->", color="#2C7A7B", lw=1.5))

    plt.tight_layout()
    plt.savefig('diagram_5_erd_projects_sales.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_5_erd_projects_sales.png")

def create_domain_erd_procurement_inventory():
    fig, ax = plt.subplots(figsize=(13, 8.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 97, "Domain 3: Procurement, Office Inventory & Payables ERD", 
            ha='center', va='center', fontsize=13, fontweight='bold', color='#1B365D')

    draw_entity_card(ax, "VENDORS", 4, 68, 26, 24, "#744210", [
        "PK id : BIGINT",
        "vendor_code : VARCHAR(30)",
        "vendor_name : VARCHAR(150)",
        "gstin : VARCHAR(15)",
        "pan : VARCHAR(10)",
        "bank_account_no : VARCHAR(50)",
        "ifsc_code : VARCHAR(20)",
        "is_active : BOOLEAN"
    ])

    draw_entity_card(ax, "PURCHASE_REQUISITIONS", 37, 68, 26, 24, "#8C3B00", [
        "PK id : BIGINT",
        "pr_number : VARCHAR(50)",
        "requested_by : BIGINT",
        "department_id : INT",
        "FK project_id : BIGINT (Optional)",
        "purpose : OFFICE_USE / PROJECT",
        "status : DRAFT/APPROVED"
    ])

    draw_entity_card(ax, "PURCHASE_ORDERS & ITEMS", 70, 68, 26, 24, "#9C4221", [
        "PK id : BIGINT (PO Header)",
        "FK vendor_id : BIGINT",
        "FK project_id : BIGINT (Optional)",
        "po_number : VARCHAR(50)",
        "po_date : DATE",
        "total_amount : DECIMAL(15,2)",
        "approval_status : VARCHAR(30)",
        "--- PO_ITEMS ---",
        "FK item_id : BIGINT",
        "quantity : DECIMAL(10,2)"
    ])

    draw_entity_card(ax, "GOODS_RECEIPT_NOTES (GRN)", 70, 36, 26, 26, "#2B6CB0", [
        "PK id : BIGINT (GRN Header)",
        "FK po_id : BIGINT",
        "grn_number : VARCHAR(50)",
        "receipt_date : DATE",
        "vendor_dc_number : VARCHAR(50)",
        "received_by : BIGINT",
        "--- GRN_ITEMS ---",
        "FK item_id : BIGINT",
        "received_qty : DECIMAL(10,2)",
        "accepted_qty : DECIMAL(10,2)",
        "rejected_qty : DECIMAL(10,2)"
    ])

    draw_entity_card(ax, "PURCHASE_BILLS & PAYABLES", 37, 36, 26, 26, "#285E61", [
        "PK id : BIGINT (Bill)",
        "FK vendor_id : BIGINT",
        "FK po_id : BIGINT",
        "FK project_id : BIGINT",
        "vendor_bill_no : VARCHAR(50)",
        "bill_date : DATE",
        "taxable_value : DECIMAL(15,2)",
        "cgst / sgst / igst : DECIMAL",
        "total_payable : DECIMAL(15,2)",
        "status : UNPAID/PAID"
    ])

    draw_entity_card(ax, "OFFICE_INVENTORY_ITEMS", 4, 36, 26, 26, "#2C7A7B", [
        "PK id : BIGINT",
        "item_code : VARCHAR(30)",
        "item_name : VARCHAR(150)",
        "category_id : INT",
        "unit_id : INT",
        "current_stock_qty : DECIMAL(10,2)",
        "reorder_level_qty : DECIMAL(10,2)",
        "unit_cost : DECIMAL(12,2)",
        "is_durable_asset : BOOLEAN (False)"
    ])

    draw_entity_card(ax, "STOCK_TRANSACTIONS & INTERNAL ISSUES", 4, 5, 45, 26, "#234E52", [
        "PK id : BIGINT (Stock Register)",
        "FK item_id : BIGINT | transaction_type : IN/OUT/ADJUST/TRANSFER",
        "quantity : DECIMAL(10,2) | unit_rate : DECIMAL(12,2) | balance_after : DECIMAL",
        "--- INTERNAL_STOCK_ISSUES ---",
        "issued_to_employee_id : BIGINT | issued_to_dept_id : INT",
        "purpose : OFFICE_CONSUMPTION | approved_by : BIGINT"
    ])

    draw_entity_card(ax, "VENDOR_PAYMENTS & ALLOCATIONS", 52, 5, 44, 26, "#553C9A", [
        "PK id : BIGINT (Payment) | FK vendor_id : BIGINT | FK bank_id : BIGINT",
        "voucher_no : VARCHAR(50) | amount_paid : DECIMAL(15,2) | mode : RTGS/NEFT",
        "--- PAYMENT_ALLOCATIONS ---",
        "FK purchase_bill_id : BIGINT | allocated_amount : DECIMAL(15,2)"
    ])

    plt.tight_layout()
    plt.savefig('diagram_6_erd_procurement_inventory.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_6_erd_procurement_inventory.png")

def create_domain_erd_finance_gst():
    fig, ax = plt.subplots(figsize=(13, 8.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 97, "Domain 4: Financial Accounting, GST & Banking ERD", 
            ha='center', va='center', fontsize=13, fontweight='bold', color='#1B365D')

    draw_entity_card(ax, "CHART_OF_ACCOUNTS", 4, 68, 26, 24, "#1A365D", [
        "PK id : BIGINT",
        "account_code : VARCHAR(30)",
        "account_name : VARCHAR(150)",
        "FK group_id : INT",
        "account_category : ASSET/LIAB/INC/EXP",
        "opening_balance : DECIMAL(15,2)",
        "current_balance : DECIMAL(15,2)",
        "is_active : BOOLEAN"
    ])

    draw_entity_card(ax, "JOURNAL_ENTRIES & LINES", 37, 68, 26, 24, "#553C9A", [
        "PK id : BIGINT (Voucher Header)",
        "voucher_no : VARCHAR(50)",
        "voucher_date : DATE",
        "voucher_type : SALES/PURCHASE/PAYMENT/RECEIPT/JV",
        "FK project_id : BIGINT (Optional)",
        "--- JOURNAL_LINES ---",
        "FK account_id : BIGINT",
        "debit_amount : DECIMAL(15,2)",
        "credit_amount : DECIMAL(15,2)",
        "narration : TEXT"
    ])

    draw_entity_card(ax, "BANK_ACCOUNTS & RECONCILIATION", 70, 68, 26, 24, "#2C7A7B", [
        "PK id : BIGINT (Bank Master)",
        "bank_name : VARCHAR(100)",
        "account_number : VARCHAR(50)",
        "ifsc_code : VARCHAR(20)",
        "account_type : CURRENT/SAVINGS",
        "book_balance : DECIMAL(15,2)",
        "--- BANK_RECONCILIATION ---",
        "statement_date : DATE",
        "bank_statement_balance : DECIMAL",
        "unreconciled_diff : DECIMAL"
    ])

    draw_entity_card(ax, "GST_TRANSACTIONS", 4, 36, 26, 26, "#0D5C75", [
        "PK id : BIGINT",
        "transaction_type : SALES (OUT) / PURCHASE (IN)",
        "FK voucher_id : BIGINT",
        "party_gstin : VARCHAR(15)",
        "hsn_sac_code : VARCHAR(10)",
        "place_of_supply : VARCHAR(5)",
        "taxable_value : DECIMAL(15,2)",
        "cgst / sgst / igst : DECIMAL(15,2)",
        "reverse_charge_applicable : BOOLEAN",
        "return_period : VARCHAR(7) (e.g. 09-2026)"
    ])

    draw_entity_card(ax, "GST_RECONCILIATION & ITC", 37, 36, 26, 26, "#805AD5", [
        "PK id : BIGINT",
        "financial_period : VARCHAR(7)",
        "gstr2b_invoice_no : VARCHAR(50)",
        "gstr2b_taxable_val : DECIMAL(15,2)",
        "gstr2b_tax_val : DECIMAL(15,2)",
        "internal_bill_id : BIGINT",
        "match_status : MATCHED / MISMATCH / MISSING",
        "itc_eligibility : ELIGIBLE / INELIGIBLE",
        "reconciled_by : BIGINT"
    ])

    draw_entity_card(ax, "PETTY_CASH & EXPENSES", 70, 36, 26, 26, "#C53030", [
        "PK id : BIGINT (Petty Cash Entry)",
        "FK custodian_user_id : BIGINT",
        "entry_date : DATE",
        "amount : DECIMAL(12,2)",
        "expense_head_id : INT",
        "FK project_id : BIGINT (Optional)",
        "receipt_proof_doc_id : BIGINT",
        "approval_status : APPROVED"
    ])

    draw_entity_card(ax, "FINANCIAL_PERIODS & YEAR_CLOSING_LOCK", 20, 5, 60, 26, "#2D3748", [
        "PK id : INT (Financial Year) | start_date : DATE | end_date : DATE | fy_code : VARCHAR(10)",
        "is_closed : BOOLEAN | locked_at : TIMESTAMP | locked_by : BIGINT",
        "--- ACCOUNT_PERIOD_BALANCES (Historical Snapshot) ---",
        "FK account_id : BIGINT | period_name : VARCHAR(20) | closing_debit : DECIMAL | closing_credit : DECIMAL"
    ])

    plt.tight_layout()
    plt.savefig('diagram_7_erd_finance_gst_banking.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_7_erd_finance_gst_banking.png")

def create_domain_erd_payroll_assets():
    fig, ax = plt.subplots(figsize=(13, 8.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    ax.text(50, 97, "Domain 5: Payroll, Employees & Asset Management ERD", 
            ha='center', va='center', fontsize=13, fontweight='bold', color='#1B365D')

    draw_entity_card(ax, "EMPLOYEES", 4, 68, 26, 24, "#1B365D", [
        "PK id : BIGINT",
        "employee_code : VARCHAR(30)",
        "full_name : VARCHAR(150)",
        "department_id : INT",
        "designation_id : INT",
        "FK reporting_manager_id : BIGINT",
        "pan : VARCHAR(10) | uan : VARCHAR(20)",
        "bank_account_no : VARCHAR(50)",
        "is_active : BOOLEAN"
    ])

    draw_entity_card(ax, "SALARY_STRUCTURES & COMPONENTS", 37, 68, 26, 24, "#2B547E", [
        "PK id : BIGINT (Structure Header)",
        "FK employee_id : BIGINT",
        "effective_from : DATE",
        "gross_salary : DECIMAL(12,2)",
        "--- SALARY_COMPONENTS ---",
        "component_name : BASIC / HRA / PF",
        "component_type : EARNING / DEDUCTION",
        "amount : DECIMAL(12,2)"
    ])

    draw_entity_card(ax, "PAYROLL_RUNS & SLIPS", 70, 68, 26, 24, "#2B6CB0", [
        "PK id : BIGINT (Payroll Month)",
        "month_year : VARCHAR(7)",
        "total_gross : DECIMAL(15,2)",
        "total_net : DECIMAL(15,2)",
        "status : DRAFT/APPROVED/PAID",
        "--- PAYROLL_SLIPS ---",
        "FK employee_id : BIGINT",
        "days_worked / days_leave : INT",
        "net_payable : DECIMAL(12,2)"
    ])

    draw_entity_card(ax, "LEAVES & HOLIDAYS", 70, 36, 26, 26, "#008080", [
        "PK id : BIGINT (Leave App)",
        "FK employee_id : BIGINT",
        "leave_type : CL/SL/EL",
        "from_date / to_date : DATE",
        "days_count : DECIMAL(4,1)",
        "status : PENDING/APPROVED",
        "--- HOLIDAYS ---",
        "holiday_name : VARCHAR(100)",
        "holiday_date : DATE"
    ])

    draw_entity_card(ax, "FIXED_ASSETS REGISTER", 37, 36, 26, 26, "#744210", [
        "PK id : BIGINT",
        "asset_tag_code : VARCHAR(50)",
        "asset_name : VARCHAR(150)",
        "category : LAPTOP/SERVER/FURNITURE",
        "purchase_date : DATE",
        "purchase_cost : DECIMAL(15,2)",
        "serial_number : VARCHAR(100)",
        "depreciation_rate : DECIMAL(5,2)",
        "status : IN_USE/SCRAPPED/SOLD"
    ])

    draw_entity_card(ax, "ASSET_ALLOCATIONS & MAINTENANCE", 4, 36, 26, 26, "#8C3B00", [
        "PK id : BIGINT (Allocation)",
        "FK asset_id : BIGINT",
        "FK allocated_employee_id : BIGINT",
        "allocation_date : DATE",
        "return_date : DATE",
        "--- MAINTENANCE_LOGS ---",
        "service_date : DATE",
        "maintenance_cost : DECIMAL(12,2)",
        "vendor_id : BIGINT"
    ])

    draw_entity_card(ax, "SUPPORT_TICKETS & HELP DESK", 20, 5, 60, 26, "#2D3748", [
        "PK id : BIGINT | ticket_number : VARCHAR(50) | FK raised_by_user_id : BIGINT",
        "category : IT_SUPPORT / SOFTWARE / FACILITY | priority : LOW / MEDIUM / CRITICAL",
        "subject : VARCHAR(200) | description : TEXT | status : OPEN / IN_PROGRESS / RESOLVED",
        "assigned_to_user_id : BIGINT | resolution_notes : TEXT | closed_at : TIMESTAMP"
    ])

    plt.tight_layout()
    plt.savefig('diagram_8_erd_payroll_assets.png', dpi=300, bbox_inches='tight')
    plt.close()
    print("Generated diagram_8_erd_payroll_assets.png")

if __name__ == '__main__':
    set_diagram_style()
    create_architecture_diagram()
    create_business_workflow_diagram()
    create_master_erd()
    create_domain_erd_masters()
    create_domain_erd_projects_sales()
    create_domain_erd_procurement_inventory()
    create_domain_erd_finance_gst()
    create_domain_erd_payroll_assets()
    print("All 8 Diagrams Generated Successfully!")
