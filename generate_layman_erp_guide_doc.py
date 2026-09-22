import docx
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT
from docx.oxml import OxmlElement, parse_xml
from docx.oxml.ns import nsdecls, qn

def create_element(name):
    return OxmlElement(name)

def set_cell_background(cell, fill_hex):
    shading_elm = parse_xml(f'<w:shd {nsdecls("w")} w:fill="{fill_hex}"/>')
    cell._tc.get_or_add_tcPr().append(shading_elm)

def set_cell_margins(cell, top=140, bottom=140, left=180, right=180):
    tcPr = cell._tc.get_or_add_tcPr()
    tcMar = OxmlElement('w:tcMar')
    for m_name, m_val in [('top', top), ('bottom', bottom), ('left', left), ('right', right)]:
        node = OxmlElement(f'w:{m_name}')
        node.set(qn('w:w'), str(m_val))
        node.set(qn('w:type'), 'dxa')
        tcMar.append(node)
    tcPr.append(tcMar)

def set_table_borders(table, color="D1D5DB", sz="4", val="single"):
    tblPr = table._tbl.tblPr
    borders = parse_xml(f'''
        <w:tblBorders {nsdecls("w")}>
            <w:top w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:bottom w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:insideH w:val="{val}" w:sz="{sz}" w:space="0" w:color="{color}"/>
            <w:insideV w:val="none"/>
            <w:left w:val="none"/>
            <w:right w:val="none"/>
        </w:tblBorders>
    ''')
    tblPr.append(borders)

def make_callout(doc, text, title="KEY BUSINESS TAKEAWAY", fill_hex="EFF6FF", border_hex="0284C7"):
    tbl = doc.add_table(rows=1, cols=1)
    tbl.alignment = WD_TABLE_ALIGNMENT.CENTER
    tbl.autofit = False
    cell = tbl.cell(0, 0)
    cell.width = Inches(6.5)
    set_cell_background(cell, fill_hex)
    set_cell_margins(cell, top=140, bottom=140, left=200, right=200)
    
    # Left border highlight
    tcPr = cell._tc.get_or_add_tcPr()
    tcBorders = parse_xml(f'''
        <w:tcBorders {nsdecls("w")}>
            <w:left w:val="single" w:sz="24" w:space="0" w:color="{border_hex}"/>
            <w:top w:val="none"/>
            <w:right w:val="none"/>
            <w:bottom w:val="none"/>
        </w:tcBorders>
    ''')
    tcPr.append(tcBorders)
    
    p = cell.paragraphs[0]
    p.paragraph_format.space_before = Pt(2)
    p.paragraph_format.space_after = Pt(2)
    r_title = p.add_run(f"★ {title}: ")
    r_title.bold = True
    r_title.font.name = "Calibri"
    r_title.font.size = Pt(10)
    r_title.font.color.rgb = RGBColor(2, 132, 199)
    
    r_text = p.add_run(text)
    r_text.font.name = "Calibri"
    r_text.font.size = Pt(9.5)
    r_text.font.color.rgb = RGBColor(30, 41, 59)
    
    doc.add_paragraph().paragraph_format.space_after = Pt(4)

def build_document():
    doc = Document()
    
    # Page setup
    sections = doc.sections
    for section in sections:
        section.top_margin = Inches(0.8)
        section.bottom_margin = Inches(0.8)
        section.left_margin = Inches(0.8)
        section.right_margin = Inches(0.8)
        section.header_distance = Inches(0.4)
        section.footer_distance = Inches(0.4)
        
        # Header & Footer
        header = section.header
        hp = header.paragraphs[0]
        hp.alignment = WD_ALIGN_PARAGRAPH.RIGHT
        hrun = hp.add_run("SDK Solutions ERP • Stepwise Layman User Guide & Menu Manual")
        hrun.font.name = "Calibri"
        hrun.font.size = Pt(8.5)
        hrun.font.color.rgb = RGBColor(148, 163, 184)
        
        footer = section.footer
        fp = footer.paragraphs[0]
        fp.alignment = WD_ALIGN_PARAGRAPH.CENTER
        frun = fp.add_run("Confidential • SDK Solutions Private Limited • Internal Operational Guide")
        frun.font.name = "Calibri"
        frun.font.size = Pt(8.5)
        frun.font.color.rgb = RGBColor(148, 163, 184)

    # Document Header Title
    p_badge = doc.add_paragraph()
    p_badge.paragraph_format.space_before = Pt(0)
    p_badge.paragraph_format.space_after = Pt(4)
    r_badge = p_badge.add_run("ENTERPRISE RESOURCE PLANNING (ERP) • END-USER OPERATIONAL GUIDE")
    r_badge.font.name = "Calibri"
    r_badge.font.size = Pt(9)
    r_badge.bold = True
    r_badge.font.color.rgb = RGBColor(2, 132, 199)
    
    p_title = doc.add_paragraph()
    p_title.paragraph_format.space_before = Pt(0)
    p_title.paragraph_format.space_after = Pt(4)
    r_title = p_title.add_run("SDK Solutions ERP — Simple Stepwise User Guide & Menu Linkage Manual")
    r_title.font.name = "Segoe UI"
    r_title.font.size = Pt(22)
    r_title.bold = True
    r_title.font.color.rgb = RGBColor(15, 23, 42)
    
    p_sub = doc.add_paragraph()
    p_sub.paragraph_format.space_before = Pt(0)
    p_sub.paragraph_format.space_after = Pt(14)
    r_sub = p_sub.add_run("A complete, easy-to-understand reference explaining every navigation menu, business purpose, interconnected data flows, and end-to-end lifecycle.")
    r_sub.font.name = "Calibri"
    r_sub.font.size = Pt(11)
    r_sub.font.color.rgb = RGBColor(71, 85, 105)

    def add_sec_heading(title, num=""):
        p = doc.add_paragraph()
        p.paragraph_format.space_before = Pt(16)
        p.paragraph_format.space_after = Pt(6)
        p.paragraph_format.keep_with_next = True
        if num:
            r_n = p.add_run(f"{num}. ")
            r_n.font.name = "Segoe UI"
            r_n.font.size = Pt(14)
            r_n.bold = True
            r_n.font.color.rgb = RGBColor(2, 132, 199)
        r = p.add_run(title)
        r.font.name = "Segoe UI"
        r.font.size = Pt(14)
        r.bold = True
        r.font.color.rgb = RGBColor(15, 23, 42)

    def add_sub_heading(title):
        p = doc.add_paragraph()
        p.paragraph_format.space_before = Pt(12)
        p.paragraph_format.space_after = Pt(4)
        p.paragraph_format.keep_with_next = True
        r = p.add_run(title)
        r.font.name = "Segoe UI"
        r.font.size = Pt(11.5)
        r.bold = True
        r.font.color.rgb = RGBColor(30, 41, 59)

    def add_body(text, space_after=6):
        p = doc.add_paragraph()
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(space_after)
        r = p.add_run(text)
        r.font.name = "Calibri"
        r.font.size = Pt(10)
        r.font.color.rgb = RGBColor(51, 65, 85)
        return p

    # 1. Executive Summary
    add_sec_heading("Executive Overview: What is SDK Solutions ERP?", "1")
    add_body(
        "In most engineering and service contracting businesses, different departments operate in disconnected silos. "
        "Sales teams create quotations in Word, project managers track site progress in Excel, accountants manage invoices in Tally, "
        "and procurement officers negotiate vendor purchases over email. This fragmentation leads to lost billing milestones, untracked site expenses, "
        "delayed customer payments, and inaccurate tax filings."
    )
    add_body(
        "SDK Solutions ERP replaces all disconnected spreadsheets and standalone tools with one single, intelligent system. "
        "The entire software is built around a Project-Centric Architecture: every action taken by any employee—whether buying hardware, "
        "claiming travel expenses, or delivering equipment—automatically links to the relevant project, updates double-entry accounting books, "
        "calculates GST liabilities, and recalculates live project profitability in real time."
    )
    make_callout(
        doc,
        "No Double Entry! When a user creates a Sales Invoice or Purchase Bill, the ERP automatically writes the balanced Journal Voucher (JV) into the General Ledger and posts the GST transaction without requiring any manual accounting knowledge.",
        "CORE VALUE PROPOSITION"
    )

    # 2. Layman Glossary
    add_sec_heading("Glossary of Key Terms in Plain English", "2")
    add_body("Before exploring the menus, here are the most common terms explained in simple, everyday language:")
    
    glossary_data = [
        ("Project (PRJ)", "A specific client work order (e.g., 'Metro Line Surveillance Installation') with an agreed contract value, timeline, and delivery milestones."),
        ("Client vs Vendor", "Client is who pays you (your customer). Vendor is whom you pay (supplier of hardware, cables, subcontractors)."),
        ("Milestone", "A predefined stage of project completion (e.g., '30% upon Hardware Delivery') that unlocks your legal right to raise a tax invoice."),
        ("Sales Tax Invoice", "The formal legal bill sent to the client demanding payment, including mandatory GST tax splits and HSN/SAC statutory codes."),
        ("Tally Invoice Format", "The standard Indian accounting invoice layout with double-bordered header boxes, dispatch details, item-level GST breakdowns, bank details, and legal declarations."),
        ("Purchase Order (PO)", "A formal order placed with a supplier committing to purchase equipment or services at an agreed rate."),
        ("Goods Receipt Note (GRN)", "A warehouse check verifying that physical materials delivered by a vendor match what was ordered in the PO before payment is approved."),
        ("Delivery Challan (DC)", "A transport document accompanying goods dispatched from your warehouse to the client's project site."),
        ("General Ledger (GL)", "The master financial record of all company income, expenses, assets, and liabilities using standard double-entry accounting."),
        ("Journal Voucher (JV)", "A financial record with equal Debit and Credit entries ensuring the accounting balance sheet always stays mathematically balanced."),
        ("Input Tax Credit (ITC)", "GST paid on purchases that reduces the amount of GST you owe the Government on your sales invoices."),
        ("Audit Trail (SHA-256)", "An unalterable digital logbook that records who created, changed, or deleted any record, secured by cryptographic math so records cannot be forged.")
    ]
    
    tbl_g = doc.add_table(rows=len(glossary_data) + 1, cols=2)
    tbl_g.alignment = WD_TABLE_ALIGNMENT.CENTER
    tbl_g.autofit = False
    set_table_borders(tbl_g)
    
    hdr_cells = tbl_g.rows[0].cells
    hdr_cells[0].text = "Term / Concept"
    hdr_cells[1].text = "Plain English Meaning & Everyday Purpose"
    hdr_cells[0].width = Inches(2.0)
    hdr_cells[1].width = Inches(4.5)
    for c in hdr_cells:
        set_cell_background(c, "1E293B")
        set_cell_margins(c, 120, 120, 140, 140)
        for r in c.paragraphs[0].runs:
            r.font.name = "Segoe UI"
            r.font.size = Pt(9.5)
            r.bold = True
            r.font.color.rgb = RGBColor(255, 255, 255)
            
    for idx, (term, desc) in enumerate(glossary_data):
        row_cells = tbl_g.rows[idx + 1].cells
        row_cells[0].text = term
        row_cells[1].text = desc
        row_cells[0].width = Inches(2.0)
        row_cells[1].width = Inches(4.5)
        bg = "F8FAFC" if idx % 2 == 1 else "FFFFFF"
        for i, c in enumerate(row_cells):
            set_cell_background(c, bg)
            set_cell_margins(c, 100, 100, 140, 140)
            p = c.paragraphs[0]
            p.paragraph_format.space_before = Pt(1)
            p.paragraph_format.space_after = Pt(1)
            if i == 0:
                p.runs[0].bold = True
                p.runs[0].font.color.rgb = RGBColor(2, 132, 199)
            else:
                p.runs[0].font.color.rgb = RGBColor(51, 65, 85)
            p.runs[0].font.name = "Calibri"
            p.runs[0].font.size = Pt(9)
            
    doc.add_paragraph().paragraph_format.space_after = Pt(8)

    # 3. All Menus Explained
    add_sec_heading("The 11 Menus & Modules Explained Step-by-Step", "3")
    add_body("The ERP sidebar is categorized into 7 operational sections. Below is each menu, what it does, and how it connects to the rest of the software:")

    menus = [
        {
            "num": "Menu 1",
            "name": "Executive Dashboard",
            "section": "Core Navigation",
            "url": "http://localhost:5140/",
            "purpose": "The main control center for business owners, managing directors, and operations leads.",
            "what_it_does": "Shows summary cards of Total Billed Revenue, Total Direct Project Costs, Realized Gross Margin (%), and Total Active Turnkey Projects. It also displays a 30/60/90-day Customer Receivables Aging bar chart, recent project milestone alerts, and quick-action buttons.",
            "how_it_links": "Pulls data in real-time from Projects, Sales Invoices, and Purchase Bills. Whenever an invoice is raised or a vendor bill is booked, dashboard totals update immediately."
        },
        {
            "num": "Menu 2",
            "name": "Master Catalogs",
            "section": "Master Data",
            "url": "http://localhost:5140/Masters",
            "purpose": "The master address book and configuration center for the entire company.",
            "what_it_does": "Allows you to add and edit Clients (with full PAN, GSTIN, credit days, and billing/shipping addresses), Vendors (with MSME category and bank details), GST Slabs (0%, 5%, 12%, 18%, 28%), Measurement Units (NOS, PKT, BOX, KGS), and Item Categories.",
            "how_it_links": "Feeds dropdown selection menus everywhere in the system. When creating a project, you pick from the Client Master. When issuing a Purchase Order, you pick from the Vendor Master."
        },
        {
            "num": "Menu 3",
            "name": "Project Management",
            "section": "Operations & Projects",
            "url": "http://localhost:5140/Projects",
            "purpose": "The central operational hub where every contract is created, monitored, and executed.",
            "what_it_does": "Each project gets a dedicated 11-Tab Workspace: 1. Overview, 2. Details, 3. Client PO, 4. Sales & Invoices, 5. Receipts, 6. Procurement POs, 7. Deliveries (DC), 8. Expenses, 9. Documents, 10. Real-time Project P&L, 11. Audit Trail. Shows live financial position: Contract Value, Total Invoiced, Direct PO/Expense Costs, and Net Realized Gross Profit.",
            "how_it_links": "The master container for the business! Invoices generated in Sales link to the project; POs issued in Procurement link to the project; site technician claims link to the project."
        },
        {
            "num": "Menu 4",
            "name": "Sales & Tax Invoices",
            "section": "Sales & Receivables",
            "url": "http://localhost:5140/Sales",
            "purpose": "Client billing, statutory GST tax invoices, and revenue tracking.",
            "what_it_does": "Lists all raised client tax invoices with payment status (DRAFT, SENT, PAID, OVERDUE). Lets you create new invoices with automatic CGST, SGST, or IGST calculation based on place of supply. Includes a 'Tally PDF' button that generates a print-ready invoice matching authentic TallyPrime layout.",
            "how_it_links": "Whenever a Sales Invoice is saved: 1. It updates the Project's Invoiced total. 2. It auto-creates a double-entry Journal Voucher (Debit Accounts Receivable, Credit Revenue & GST Output). 3. It adds an output tax line into the GST module."
        },
        {
            "num": "Menu 5",
            "name": "Receipts & Allocation",
            "section": "Sales & Receivables",
            "url": "http://localhost:5140/Sales/AllocatePayment",
            "purpose": "Recording customer payments and reconciling unpaid client bills.",
            "what_it_does": "When a client pays via NEFT, RTGS, IMPS, or Cheque, you enter the remittance details here and allocate the money to one or more outstanding invoices.",
            "how_it_links": "Reduces the Outstanding Balance on the specific Sales Invoice, increases the Company Bank Account balance, and posts a credit receipt to the General Ledger."
        },
        {
            "num": "Menu 6",
            "name": "Procurement & POs",
            "section": "Procurement & Payables",
            "url": "http://localhost:5140/Procurement",
            "purpose": "Supplier purchasing, vendor order tracking, and purchase bills.",
            "what_it_does": "Allows project engineers to raise Purchase Requisitions (PR) and procurement managers to approve and issue formal Purchase Orders (PO) to vendors. Also logs Goods Receipt Notes (GRN) when items arrive at the site or warehouse.",
            "how_it_links": "Links directly to the Project Direct Cost ledger and automatically populates items into Office Inventory upon GRN receipt."
        },
        {
            "num": "Menu 7",
            "name": "Office Inventory",
            "section": "Procurement & Payables",
            "url": "http://localhost:5140/Procurement/Inventory",
            "purpose": "Warehouse stock control and material issuance tracking.",
            "what_it_does": "Tracks stock levels of cables, cameras, network switches, connectors, and tools. Allows storekeepers to issue stock directly to specific site engineers and generates low-stock reorder warnings.",
            "how_it_links": "Receives stock increases when Procurement GRNs are approved; records stock reductions when materials are issued to project sites via Delivery Challans."
        },
        {
            "num": "Menu 8",
            "name": "General Ledger & COA",
            "section": "Financials & Taxation",
            "url": "http://localhost:5140/Accounts",
            "purpose": "Statutory double-entry accounting engine and Chart of Accounts.",
            "what_it_does": "Maintains the standard 5-Tier Chart of Accounts (Assets, Liabilities, Equity, Revenue, Expense) and records auto-balanced Journal Vouchers (JVs). Lets accountants inspect debit/credit entries, trial balance positions, and ledger summaries.",
            "how_it_links": "The central receiver of all financial events! Receives auto-posted JVs from Sales, Customer Receipts, Vendor Bills, Site Expenses, and Payroll runs."
        },
        {
            "num": "Menu 9",
            "name": "GST & Tax Compliance",
            "section": "Financials & Taxation",
            "url": "http://localhost:5140/Gst",
            "purpose": "Indian statutory taxation, return filing preparation, and ITC reconciliation.",
            "what_it_does": "Automatically categorizes taxes into CGST, SGST, and IGST for GSTR-1 (Outward Supplies/Sales) and GSTR-3B (Monthly Summary). Computes Net GST Payable = Output GST (from Sales) - Input Tax Credit (from Purchases).",
            "how_it_links": "Pulls output tax automatically from Sales Invoices and input tax credit from Procurement Bills."
        },
        {
            "num": "Menu 10",
            "name": "Cash & Banking / BRS",
            "section": "Financials & Taxation",
            "url": "http://localhost:5140/Banking",
            "purpose": "Bank account balances, petty cash floats, and bank reconciliation.",
            "what_it_does": "Manages company bank accounts (HDFC, ICICI, SBI) and site petty cash floats. Supports Bank Reconciliation Statements (BRS) to match ERP accounting entries against bank statement PDFs/Excel sheets.",
            "how_it_links": "Increases balance upon Customer Receipts; decreases balance upon Vendor Payments and Petty Cash withdrawals."
        },
        {
            "num": "Menu 11",
            "name": "Payroll & HR",
            "section": "Human Resources & Assets",
            "url": "http://localhost:5140/Payroll",
            "purpose": "Employee records, salary structures, attendance, and payroll processing.",
            "what_it_does": "Maintains employee master details, monthly attendance, salary components (Basic, HRA, Conveyance, PF, ESIC, Professional Tax), and generates monthly payroll runs with payslips.",
            "how_it_links": "Posts total monthly salary expense and statutory liabilities (PF/ESIC payable) into the General Ledger upon payroll approval."
        },
        {
            "num": "Menu 12",
            "name": "Fixed Assets & Support",
            "section": "Human Resources & Assets",
            "url": "http://localhost:5140/Assets",
            "purpose": "Company asset tracking, depreciation calculation, and internal IT helpdesk.",
            "what_it_does": "Tracks company laptops, fusion splicers, OTDR testing kits, and vehicles. Computes Straight-Line or WDV depreciation and tracks internal support helpdesk tickets.",
            "how_it_links": "Asset purchases link from Procurement; annual depreciation charges post to General Ledger."
        },
        {
            "num": "Menu 13",
            "name": "Reports & Analytics",
            "section": "MIS & Governance",
            "url": "http://localhost:5140/Reports",
            "purpose": "Executive MIS, financial statements, and business performance analytics.",
            "what_it_does": "Provides one-click generation and PDF/Excel export of: Profit & Loss Statement, Balance Sheet, Trial Balance, Project Profitability Summary, Customer Receivables Aging, and Vendor Payables Aging.",
            "how_it_links": "Reads aggregated data from across the entire database to produce accurate executive reports."
        },
        {
            "num": "Menu 14",
            "name": "Admin, Audit & RBAC",
            "section": "MIS & Governance",
            "url": "http://localhost:5140/Admin",
            "purpose": "Security, user logins, role permissions, and tamper-proof audit trail.",
            "what_it_does": "Allows Super Admins to create users, assign roles (Project Manager, Accountant, Procurement Officer), enforce password policies, and inspect the Cryptographic SHA-256 Audit Log of every action taken in the ERP.",
            "how_it_links": "Governs access to every screen and logs every database change across all modules."
        }
    ]

    for m in menus:
        add_sub_heading(f"{m['num']}: {m['name']} ({m['section']})")
        add_body(f"• URL Path: {m['url']}")
        add_body(f"• Purpose: {m['purpose']}")
        add_body(f"• What It Does: {m['what_it_does']}")
        add_body(f"• How It Connects: {m['how_it_links']}")
        doc.add_paragraph().paragraph_format.space_after = Pt(4)

    # 4. Step-by-Step Lifecycle Walkthrough
    add_sec_heading("End-to-End Business Lifecycle Walkthrough (Step-by-Step)", "4")
    add_body("To see how these menus work together in real daily operations, let us follow a real-world example from start to finish:")
    
    steps = [
        ("Step 1: Onboard Client in Master Catalogs", "Go to Master Catalogs -> Add Client. Enter 'Delhi Metro Rail Corp (DMRC)', GSTIN: '07AAAAA0000A1Z5', PAN: 'AAAAA0000A', and 30-Day Credit Terms."),
        ("Step 2: Initialize Project in Project Management", "Go to Project Management -> Create Project. Enter Code 'PRJ-DMRC-01', Name 'Metro Surveillance Network Phase 1', Contract Value '₹25,00,000.00', and link Client 'DMRC'."),
        ("Step 3: Define Billing Milestones", "Open the Project Detail page -> Milestones Tab -> Add Milestones:\n- Milestone 1: 30% on Site Material Delivery (₹7,50,000)\n- Milestone 2: 50% on Installation & Testing (₹12,50,000)\n- Milestone 3: 20% on Handover & Signoff (₹5,00,000)"),
        ("Step 4: Issue Purchase Orders for Materials", "Go to Procurement -> Create PO. Select Vendor 'Hikvision India Ltd', link Project 'PRJ-DMRC-01', and order 50 IP Cameras & 5 NVRs for ₹4,00,000."),
        ("Step 5: Receive Materials (GRN) & Dispatch to Site (DC)", "When materials arrive, mark GRN Received in Procurement. Items enter Office Inventory. Open Project -> Deliveries Tab -> Create Delivery Challan (DC) to dispatch equipment to site."),
        ("Step 6: Raise Sales Tax Invoice (Tally Format)", "Upon completing Milestone 1, open Project -> Sales Tab -> Raise Tax Invoice for ₹7,50,000 + 18% GST (₹1,35,000) = ₹8,85,000 total.\n- System generates an authentic Tally-style printable Tax Invoice PDF.\n- System auto-posts a ₹8,85,000 Journal Voucher in the General Ledger.\n- System records ₹1,35,000 Output GST in the GST Compliance module."),
        ("Step 7: Record Customer Payment", "When DMRC wires ₹8,85,000 via NEFT, go to Receipts & Allocation -> Allocate Payment. Select the DMRC invoice -> Mark as PAID. Money enters Bank Account and clears the receivable balance."),
        ("Step 8: Review Project Profit & Loss (P&L)", "Open Project Workspace -> Project P&L Tab. View live financials:\n- Billed Revenue: ₹7,50,000\n- Direct Material Costs: ₹4,00,000\n- Site Expenses: ₹25,000\n- Net Gross Profit: ₹3,25,000 (43.3% Margin)"),
        ("Step 9: Project Closure", "Once all 3 milestones are billed, fully paid, and site expenses settled, click 'Execute Project Closure' to archive the project with a permanent audit certificate.")
    ]

    for title, desc in steps:
        add_sub_heading(title)
        add_body(desc)

    # 5. Cross-Reference Linkage Matrix Table
    add_sec_heading("Inter-Module Linkage & Automation Matrix", "5")
    add_body("The table below shows what happens behind the scenes when you perform an action in one module:")

    matrix_data = [
        ("Create Project", "Project Management", "Generates unique project code, initializes P&L ledger, prepares milestone register.", "Master Catalogs (Client selection), Reports (Active project counts)"),
        ("Issue Purchase Order", "Procurement", "Locks committed vendor costs against the project budget.", "Project Detail (Tab 6 - Procurement POs), General Ledger (Committed cost)"),
        ("Approve GRN (Goods Receipt)", "Procurement", "Verifies physical goods received, adds items to warehouse inventory.", "Office Inventory (Stock count increase), Project Deliveries (Available for DC)"),
        ("Generate Sales Invoice", "Sales & Invoicing", "Generates Tally-compliant statutory invoice PDF with HSN tax summary.", "Project Detail (Tab 4), General Ledger (Debit AR, Credit Revenue), GST Module (Output Tax)"),
        ("Record Customer Receipt", "Receipts & Allocation", "Clears unpaid invoice balance, matches bank transaction.", "Sales Invoices (Marks PAID), Banking (Bank balance increase), General Ledger (Receipt JV)"),
        ("Record Site Expense", "Project Management", "Logs technician travel/food vouchers with GST bills.", "Project P&L (Direct cost increase), Banking (Petty cash reduction), General Ledger (Expense JV)"),
        ("Run Monthly Payroll", "Payroll & HR", "Calculates net employee salaries, PF, ESIC, and PT statutory deductions.", "General Ledger (Salary expense & statutory liability entries), Banking (Salary disbursement)")
    ]

    tbl_m = doc.add_table(rows=len(matrix_data) + 1, cols=4)
    tbl_m.alignment = WD_TABLE_ALIGNMENT.CENTER
    tbl_m.autofit = False
    set_table_borders(tbl_m)
    
    m_headers = ["User Action", "Originating Menu", "What the System Does Automatically", "Other Modules Updated"]
    m_widths = [Inches(1.3), Inches(1.3), Inches(2.2), Inches(1.7)]
    for i, title in enumerate(m_headers):
        c = tbl_m.rows[0].cells[i]
        c.text = title
        c.width = m_widths[i]
        set_cell_background(c, "1E293B")
        set_cell_margins(c, 120, 120, 120, 120)
        for r in c.paragraphs[0].runs:
            r.font.name = "Segoe UI"
            r.font.size = Pt(9)
            r.bold = True
            r.font.color.rgb = RGBColor(255, 255, 255)

    for idx, (action, orig, auto, links) in enumerate(matrix_data):
        row_cells = tbl_m.rows[idx + 1].cells
        row_cells[0].text = action
        row_cells[1].text = orig
        row_cells[2].text = auto
        row_cells[3].text = links
        bg = "F8FAFC" if idx % 2 == 1 else "FFFFFF"
        for i, c in enumerate(row_cells):
            c.width = m_widths[i]
            set_cell_background(c, bg)
            set_cell_margins(c, 100, 100, 120, 120)
            p = c.paragraphs[0]
            p.paragraph_format.space_before = Pt(1)
            p.paragraph_format.space_after = Pt(1)
            p.runs[0].font.name = "Calibri"
            p.runs[0].font.size = Pt(8.5)
            if i == 0:
                p.runs[0].bold = True
                p.runs[0].font.color.rgb = RGBColor(2, 132, 199)
            elif i == 1:
                p.runs[0].bold = True
                p.runs[0].font.color.rgb = RGBColor(30, 41, 59)
            else:
                p.runs[0].font.color.rgb = RGBColor(51, 65, 85)

    doc.add_paragraph().paragraph_format.space_after = Pt(8)

    # 6. FAQ Section
    add_sec_heading("Frequently Asked Questions (FAQ) for Daily Users", "6")
    faqs = [
        ("Q1: Do I need accounting experience to create invoices in this ERP?", "No. When you raise an invoice against a project milestone, the ERP automatically selects the correct GST rates, formats the invoice according to Tally guidelines, and writes the balanced debit/credit Journal Voucher into the General Ledger in the background."),
        ("Q2: How do I print an invoice for the client?", "Go to Sales & Tax Invoices (or the Sales tab inside any Project), find the invoice, and click the blue 'Tally PDF' button. It will open a print-ready A4 Tax Invoice formatted exactly like TallyPrime."),
        ("Q3: Why is Budget Cost no longer visible on the Project Overview?", "Following standard engineering practice, the ERP calculates your project margin directly from Realized Contract Revenue minus Actual Purchase Orders & Site Expenses. This gives an exact, tamper-proof profit figure without guesswork."),
        ("Q4: What if an invoice is partially paid by the client?", "Go to Receipts & Allocation, enter the partial amount received, and allocate it to the invoice. The ERP will update the invoice status to PARTIAL, update the outstanding balance, and keep the remaining amount open for future collection."),
        ("Q5: Where do I check how much profit our company made on a specific project?", "Open the Project Management menu, click 'Manage Workspace' on the project, and click Tab 10 ('10. Project P&L'). You will see an instant, itemized breakdown of Total Invoiced vs Direct Costs and Net Margin.")
    ]

    for q, a in faqs:
        add_sub_heading(q)
        add_body(a)

    doc_path = r"d:\Bhawani Works\Project All\Website\New ERP Solution\SDK_Solutions_ERP_Simple_User_Guide_and_Menu_Reference.docx"
    doc.save(doc_path)
    print(f"Document successfully created at: {doc_path}")

if __name__ == "__main__":
    build_document()
