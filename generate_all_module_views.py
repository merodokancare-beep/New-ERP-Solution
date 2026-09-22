import os

def create_directory(path):
    if not os.path.exists(path):
        os.makedirs(path)

base_views = "src/SDK.ERP.Web/Views"

# 1. Masters View
create_directory(f"{base_views}/Masters")
with open(f"{base_views}/Masters/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Masters & System Configuration";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Masters &amp; Configuration</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Centralized Master Repository &bull; Zero Data Duplication Architecture</p>
    </div>
    <div class="d-flex gap-2">
        <button class="btn btn-outline-secondary btn-sm px-3"><i class="fa-solid fa-file-import me-1"></i> Bulk Import</button>
        <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary); border-color: var(--erp-primary);"><i class="fa-solid fa-plus me-1"></i> Add Record</button>
    </div>
</div>

<!-- Nav Tabs for Masters -->
<ul class="nav nav-pills mb-4 gap-2" id="masterTabs" role="tablist">
    <li class="nav-item" role="presentation">
        <button class="nav-link active fw-semibold" id="clients-tab" data-bs-toggle="pill" data-bs-target="#clients" type="button" role="tab"><i class="fa-solid fa-user-tie me-2"></i>Clients (12)</button>
    </li>
    <li class="nav-item" role="presentation">
        <button class="nav-link fw-semibold" id="vendors-tab" data-bs-toggle="pill" data-bs-target="#vendors" type="button" role="tab"><i class="fa-solid fa-truck-field me-2"></i>Vendors (18)</button>
    </li>
    <li class="nav-item" role="presentation">
        <button class="nav-link fw-semibold" id="items-tab" data-bs-toggle="pill" data-bs-target="#items" type="button" role="tab"><i class="fa-solid fa-box-archive me-2"></i>Consumable Items (45)</button>
    </li>
    <li class="nav-item" role="presentation">
        <button class="nav-link fw-semibold" id="tax-tab" data-bs-toggle="pill" data-bs-target="#tax" type="button" role="tab"><i class="fa-solid fa-percent me-2"></i>GST Tax Slabs (5)</button>
    </li>
    <li class="nav-item" role="presentation">
        <button class="nav-link fw-semibold" id="coa-tab" data-bs-toggle="pill" data-bs-target="#coa" type="button" role="tab"><i class="fa-solid fa-sitemap me-2"></i>Chart of Accounts (38)</button>
    </li>
</ul>

<div class="tab-content" id="masterTabsContent">
    <!-- Clients Tab -->
    <div class="tab-pane fade show active" id="clients" role="tabpanel">
        <div class="erp-card">
            <div class="erp-card-header d-flex justify-content-between align-items-center">
                <h5 class="erp-card-title"><i class="fa-solid fa-users me-2"></i>Client Master Directory</h5>
                <input type="text" class="form-control form-control-sm" placeholder="Filter clients..." style="width: 240px;" />
            </div>
            <div class="erp-card-body p-0">
                <div class="table-responsive">
                    <table class="table table-hover align-middle mb-0">
                        <thead class="table-light">
                            <tr style="font-size: 12px; text-transform: uppercase;">
                                <th class="ps-4">Client Code</th>
                                <th>Client Name</th>
                                <th>GSTIN</th>
                                <th>State</th>
                                <th>Credit Terms</th>
                                <th>Credit Limit</th>
                                <th>Status</th>
                                <th class="text-end pe-4">Actions</th>
                            </tr>
                        </thead>
                        <tbody style="font-size: 13.5px;">
                            <tr>
                                <td class="ps-4 fw-bold text-primary">CLT-2026-001</td>
                                <td><strong>Reliance Infra Ltd</strong><br><small class="text-muted">contact@relianceinfra.com</small></td>
                                <td><code>27AAACR1234F1Z1</code></td>
                                <td><span class="badge bg-light text-dark">27 (Maharashtra)</span></td>
                                <td>30 Days</td>
                                <td>&#8377;50,00,000</td>
                                <td><span class="badge bg-success-subtle text-success">Active</span></td>
                                <td class="text-end pe-4">
                                    <button class="btn btn-sm btn-light"><i class="fa-solid fa-pen-to-square"></i></button>
                                </td>
                            </tr>
                            <tr>
                                <td class="ps-4 fw-bold text-primary">CLT-2026-002</td>
                                <td><strong>Tata Consultancy Services</strong><br><small class="text-muted">vendor-desk@tcs.com</small></td>
                                <td><code>07AAACT9988H1Z4</code></td>
                                <td><span class="badge bg-light text-dark">07 (Delhi)</span></td>
                                <td>45 Days</td>
                                <td>&#8377;1,20,00,000</td>
                                <td><span class="badge bg-success-subtle text-success">Active</span></td>
                                <td class="text-end pe-4">
                                    <button class="btn btn-sm btn-light"><i class="fa-solid fa-pen-to-square"></i></button>
                                </td>
                            </tr>
                            <tr>
                                <td class="ps-4 fw-bold text-primary">CLT-2026-003</td>
                                <td><strong>Adani Enterprises Ltd</strong><br><small class="text-muted">procurement@adani.com</small></td>
                                <td><code>24AAACA5566K1Z9</code></td>
                                <td><span class="badge bg-light text-dark">24 (Gujarat)</span></td>
                                <td>60 Days</td>
                                <td>&#8377;75,00,000</td>
                                <td><span class="badge bg-success-subtle text-success">Active</span></td>
                                <td class="text-end pe-4">
                                    <button class="btn btn-sm btn-light"><i class="fa-solid fa-pen-to-square"></i></button>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    </div>

    <!-- Vendors Tab -->
    <div class="tab-pane fade" id="vendors" role="tabpanel">
        <div class="erp-card">
            <div class="erp-card-header d-flex justify-content-between align-items-center">
                <h5 class="erp-card-title"><i class="fa-solid fa-truck-field me-2"></i>Approved Vendor Directory</h5>
                <button class="btn btn-sm btn-primary"><i class="fa-solid fa-plus me-1"></i> Register Vendor</button>
            </div>
            <div class="erp-card-body p-0">
                <table class="table table-hover align-middle mb-0">
                    <thead class="table-light" style="font-size: 12px;">
                        <tr>
                            <th class="ps-4">Vendor Code</th>
                            <th>Supplier Name</th>
                            <th>Category</th>
                            <th>GSTIN / PAN</th>
                            <th>MSME Type</th>
                            <th>Bank IFSC</th>
                            <th>Terms</th>
                            <th class="text-end pe-4">Actions</th>
                        </tr>
                    </thead>
                    <tbody style="font-size: 13.5px;">
                        <tr>
                            <td class="ps-4 fw-bold text-primary">VND-001</td>
                            <td><strong>Office World Consumables</strong></td>
                            <td><span class="badge bg-info-subtle text-info">Supplies</span></td>
                            <td><code>07AAAPO1122D1Z0</code><br><small>AAPO1122D</small></td>
                            <td><span class="badge bg-success-subtle text-success">Micro MSME</span></td>
                            <td><code>HDFC0000124</code></td>
                            <td>15 Days</td>
                            <td class="text-end pe-4"><button class="btn btn-sm btn-light"><i class="fa-solid fa-eye"></i></button></td>
                        </tr>
                        <tr>
                            <td class="ps-4 fw-bold text-primary">VND-002</td>
                            <td><strong>Apex Hardware &amp; Networking</strong></td>
                            <td><span class="badge bg-primary-subtle text-primary">Hardware</span></td>
                            <td><code>07BBBA4433E1Z2</code><br><small>BBBA4433E</small></td>
                            <td><span class="badge bg-warning-subtle text-warning">Small MSME</span></td>
                            <td><code>ICIC0001890</code></td>
                            <td>30 Days</td>
                            <td class="text-end pe-4"><button class="btn btn-sm btn-light"><i class="fa-solid fa-eye"></i></button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <!-- Items Tab -->
    <div class="tab-pane fade" id="items" role="tabpanel">
        <div class="erp-card">
            <div class="erp-card-header d-flex justify-content-between align-items-center">
                <h5 class="erp-card-title"><i class="fa-solid fa-box-archive me-2"></i>Consumable Items Catalog</h5>
                <span class="badge bg-info-subtle text-info">Office Consumables Only</span>
            </div>
            <div class="erp-card-body p-0">
                <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
                    <thead class="table-light" style="font-size: 12px;">
                        <tr>
                            <th class="ps-4">Item Code</th>
                            <th>Description</th>
                            <th>Category</th>
                            <th>UOM</th>
                            <th>HSN/SAC</th>
                            <th>Avg Cost</th>
                            <th>Current Stock</th>
                            <th>Reorder Level</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="ps-4 fw-bold text-primary">ITM-PAP-A4</td>
                            <td>JK Copier A4 Paper 75 GSM</td>
                            <td>Stationery</td>
                            <td>BOX</td>
                            <td>4802</td>
                            <td>&#8377;1,250.00</td>
                            <td><span class="badge bg-success">42 BOX</span></td>
                            <td>10 BOX</td>
                        </tr>
                        <tr>
                            <td class="ps-4 fw-bold text-primary">ITM-TNR-88A</td>
                            <td>HP LaserJet 88A Black Toner</td>
                            <td>IT Supplies</td>
                            <td>NOS</td>
                            <td>8443</td>
                            <td>&#8377;3,400.00</td>
                            <td><span class="badge bg-danger">3 NOS (Low)</span></td>
                            <td>5 NOS</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <!-- Tax Slabs Tab -->
    <div class="tab-pane fade" id="tax" role="tabpanel">
        <div class="erp-card">
            <div class="erp-card-header">
                <h5 class="erp-card-title"><i class="fa-solid fa-percent me-2"></i>Standard Indian GST Tax Slabs</h5>
            </div>
            <div class="erp-card-body p-0">
                <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
                    <thead class="table-light" style="font-size: 12px;">
                        <tr>
                            <th class="ps-4">Tax Name</th>
                            <th>Total Rate</th>
                            <th>CGST Rate (Intra)</th>
                            <th>SGST Rate (Intra)</th>
                            <th>IGST Rate (Inter)</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr><td class="ps-4 fw-bold">GST 0% (Exempt)</td><td>0.00%</td><td>0.00%</td><td>0.00%</td><td>0.00%</td><td><span class="badge bg-success">Active</span></td></tr>
                        <tr><td class="ps-4 fw-bold">GST 5%</td><td>5.00%</td><td>2.50%</td><td>2.50%</td><td>5.00%</td><td><span class="badge bg-success">Active</span></td></tr>
                        <tr><td class="ps-4 fw-bold">GST 12%</td><td>12.00%</td><td>6.00%</td><td>6.00%</td><td>12.00%</td><td><span class="badge bg-success">Active</span></td></tr>
                        <tr><td class="ps-4 fw-bold text-primary">GST 18% (Standard)</td><td><strong>18.00%</strong></td><td>9.00%</td><td>9.00%</td><td>18.00%</td><td><span class="badge bg-success">Active</span></td></tr>
                        <tr><td class="ps-4 fw-bold">GST 28%</td><td>28.00%</td><td>14.00%</td><td>14.00%</td><td>28.00%</td><td><span class="badge bg-success">Active</span></td></tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <!-- Chart of Accounts Tab -->
    <div class="tab-pane fade" id="coa" role="tabpanel">
        <div class="erp-card">
            <div class="erp-card-header">
                <h5 class="erp-card-title"><i class="fa-solid fa-sitemap me-2"></i>5-Level Chart of Accounts (COA)</h5>
            </div>
            <div class="erp-card-body">
                <div class="row g-3">
                    <div class="col-md-4">
                        <div class="border rounded p-3 bg-light">
                            <h6 class="fw-bold text-primary"><i class="fa-solid fa-folder me-2"></i>1000 - Assets</h6>
                            <ul class="list-unstyled ps-3 mb-0" style="font-size: 13px;">
                                <li>&bull; 1001 - HDFC Bank Current A/c</li>
                                <li>&bull; 1002 - ICICI Bank Overdraft A/c</li>
                                <li>&bull; 1100 - Accounts Receivable (Client AR)</li>
                                <li>&bull; 1200 - Office Inventory Asset</li>
                                <li>&bull; 1300 - Input GST (ITC Ledger)</li>
                            </ul>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="border rounded p-3 bg-light">
                            <h6 class="fw-bold text-danger"><i class="fa-solid fa-folder me-2"></i>2000 - Liabilities</h6>
                            <ul class="list-unstyled ps-3 mb-0" style="font-size: 13px;">
                                <li>&bull; 2001 - Accounts Payable (Vendor AP)</li>
                                <li>&bull; 2100 - Output CGST Payable</li>
                                <li>&bull; 2101 - Output SGST Payable</li>
                                <li>&bull; 2102 - Output IGST Payable</li>
                                <li>&bull; 2200 - Salaries &amp; Wages Payable</li>
                            </ul>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="border rounded p-3 bg-light">
                            <h6 class="fw-bold text-success"><i class="fa-solid fa-folder me-2"></i>4000 - Revenue &amp; 5000 Expenses</h6>
                            <ul class="list-unstyled ps-3 mb-0" style="font-size: 13px;">
                                <li>&bull; 4001 - Project Billing Revenue</li>
                                <li>&bull; 5001 - Project Direct Procurement Cost</li>
                                <li>&bull; 5002 - Project Logistics &amp; Travel</li>
                                <li>&bull; 5100 - Office Supplies Expense</li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
''')

# 2. Projects View
create_directory(f"{base_views}/Projects")
with open(f"{base_views}/Projects/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Projects Management Hub";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Projects Management Hub</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Project-Centric Operating Paradigm &bull; Lifecycle, Invoicing, Procurement &amp; Real-Time Margins</p>
    </div>
    <div class="d-flex gap-2">
        <button class="btn btn-outline-primary btn-sm px-3"><i class="fa-solid fa-filter me-1"></i> Filter Status</button>
        <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary); border-color: var(--erp-primary);"><i class="fa-solid fa-plus me-1"></i> Create Project</button>
    </div>
</div>

<!-- Project Health Cards -->
<div class="row g-3 mb-4">
    <div class="col-md-3">
        <div class="p-3 bg-white border rounded shadow-sm">
            <small class="text-muted text-uppercase fw-semibold" style="font-size: 11px;">Active Projects</small>
            <div class="fs-4 fw-bold text-primary">18</div>
            <small class="text-success"><i class="fa-solid fa-circle-check me-1"></i>100% On Delivery Schedule</small>
        </div>
    </div>
    <div class="col-md-3">
        <div class="p-3 bg-white border rounded shadow-sm">
            <small class="text-muted text-uppercase fw-semibold" style="font-size: 11px;">Pipeline Projects</small>
            <div class="fs-4 fw-bold text-warning">6</div>
            <small class="text-muted">&#8377;1.85 Cr Estimated Contract</small>
        </div>
    </div>
    <div class="col-md-3">
        <div class="p-3 bg-white border rounded shadow-sm">
            <small class="text-muted text-uppercase fw-semibold" style="font-size: 11px;">Total Contract Value</small>
            <div class="fs-4 fw-bold text-success">&#8377;4.82 Cr</div>
            <small class="text-muted">&#8377;3.15 Cr Billed to Date</small>
        </div>
    </div>
    <div class="col-md-3">
        <div class="p-3 bg-white border rounded shadow-sm">
            <small class="text-muted text-uppercase fw-semibold" style="font-size: 11px;">Average Margin %</small>
            <div class="fs-4 fw-bold text-info">38.4%</div>
            <small class="text-success">+2.1% above target budget</small>
        </div>
    </div>
</div>

<!-- Projects Table -->
<div class="erp-card">
    <div class="erp-card-header d-flex justify-content-between align-items-center">
        <h5 class="erp-card-title"><i class="fa-solid fa-diagram-project me-2"></i>Active Project Register</h5>
        <div class="d-flex gap-2">
            <input type="text" class="form-control form-control-sm" placeholder="Search project code, client..." style="width: 260px;" />
        </div>
    </div>
    <div class="erp-card-body p-0">
        <div class="table-responsive">
            <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
                <thead class="table-light" style="font-size: 12px; text-transform: uppercase;">
                    <tr>
                        <th class="ps-4">Project Code</th>
                        <th>Project Name</th>
                        <th>Client</th>
                        <th>Project Manager</th>
                        <th>Contract Value</th>
                        <th>Billed Amount</th>
                        <th>Direct Cost</th>
                        <th>Gross Margin</th>
                        <th>Status</th>
                        <th class="text-end pe-4">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="ps-4 fw-bold"><a asp-controller="Projects" asp-action="Detail" asp-route-id="1" class="text-primary text-decoration-none">PRJ-2026-0045</a></td>
                        <td><strong>Smart Grid Automation Phase 1</strong><br><small class="text-muted">Start: 01 Apr 2026</small></td>
                        <td>Reliance Infra Ltd</td>
                        <td><span class="badge bg-secondary-subtle text-secondary">A. Sharma</span></td>
                        <td>&#8377;85,00,000</td>
                        <td>&#8377;55,00,000</td>
                        <td>&#8377;31,40,000</td>
                        <td><span class="badge bg-success-subtle text-success fw-bold">42.9% (&#8377;23.6L)</span></td>
                        <td><span class="badge bg-success">Active</span></td>
                        <td class="text-end pe-4">
                            <a asp-controller="Projects" asp-action="Detail" asp-route-id="1" class="btn btn-sm btn-primary"><i class="fa-solid fa-folder-open me-1"></i> Open Workspace</a>
                        </td>
                    </tr>
                    <tr>
                        <td class="ps-4 fw-bold"><a asp-controller="Projects" asp-action="Detail" asp-route-id="2" class="text-primary text-decoration-none">PRJ-2026-0046</a></td>
                        <td><strong>Cloud Migration &amp; Security Setup</strong><br><small class="text-muted">Start: 15 Apr 2026</small></td>
                        <td>Tata Consultancy Services</td>
                        <td><span class="badge bg-secondary-subtle text-secondary">R. Verma</span></td>
                        <td>&#8377;1,20,00,000</td>
                        <td>&#8377;80,00,000</td>
                        <td>&#8377;48,20,000</td>
                        <td><span class="badge bg-success-subtle text-success fw-bold">39.7% (&#8377;31.8L)</span></td>
                        <td><span class="badge bg-success">Active</span></td>
                        <td class="text-end pe-4">
                            <a asp-controller="Projects" asp-action="Detail" asp-route-id="2" class="btn btn-sm btn-primary"><i class="fa-solid fa-folder-open me-1"></i> Open Workspace</a>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>
''')

# Projects Detail 11-Tab Workspace
with open(f"{base_views}/Projects/Detail.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Project Workspace - PRJ-2026-0045";
}

<!-- Project Header Bar -->
<div class="d-flex align-items-center justify-content-between mb-3 bg-white p-3 border rounded shadow-sm">
    <div>
        <div class="d-flex align-items-center gap-2">
            <span class="badge bg-primary fs-6">PRJ-2026-0045</span>
            <h4 class="m-0 fw-bold text-primary">Smart Grid Automation Phase 1</h4>
            <span class="badge bg-success ms-2">ACTIVE</span>
        </div>
        <small class="text-muted">Client: <strong>Reliance Infra Ltd</strong> &bull; Manager: <strong>A. Sharma</strong> &bull; Contract: <strong>&#8377;85,00,000</strong></small>
    </div>
    <div class="d-flex gap-2">
        <a asp-controller="Projects" asp-action="Index" class="btn btn-outline-secondary btn-sm"><i class="fa-solid fa-arrow-left me-1"></i> Back to List</a>
        <button class="btn btn-danger btn-sm"><i class="fa-solid fa-lock me-1"></i> Execute Project Closure</button>
    </div>
</div>

<!-- 11-Tab Workspace Navigation -->
<ul class="nav nav-tabs mb-3" id="projectWorkspaceTabs" role="tablist">
    <li class="nav-item"><button class="nav-link active fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-overview"><i class="fa-solid fa-gauge me-1"></i> 1. Overview</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-metadata"><i class="fa-solid fa-info me-1"></i> 2. Details</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-po"><i class="fa-solid fa-file-contract me-1"></i> 3. Client PO</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-invoices"><i class="fa-solid fa-file-invoice-dollar me-1"></i> 4. Sales &amp; Invoices</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-receipts"><i class="fa-solid fa-receipt me-1"></i> 5. Receipts</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-procurement"><i class="fa-solid fa-cart-shopping me-1"></i> 6. Procurement POs</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-deliveries"><i class="fa-solid fa-truck me-1"></i> 7. Deliveries (DC)</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-expenses"><i class="fa-solid fa-money-bill-transfer me-1"></i> 8. Expenses</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-documents"><i class="fa-solid fa-paperclip me-1"></i> 9. Documents</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold text-success" data-bs-toggle="tab" data-bs-target="#tab-pnl"><i class="fa-solid fa-chart-pie me-1"></i> 10. Project P&amp;L</button></li>
    <li class="nav-item"><button class="nav-link fw-semibold" data-bs-toggle="tab" data-bs-target="#tab-audit"><i class="fa-solid fa-clock-rotate-left me-1"></i> 11. Audit Trail</button></li>
</ul>

<div class="tab-content">
    <div class="tab-pane fade show active" id="tab-overview">
        <div class="row g-3">
            <div class="col-md-3"><div class="p-3 bg-white border rounded"><h6>Contract Value</h6><div class="fs-5 fw-bold">&#8377;85,00,000</div></div></div>
            <div class="col-md-3"><div class="p-3 bg-white border rounded"><h6>Total Invoiced</h6><div class="fs-5 fw-bold text-primary">&#8377;55,00,000</div></div></div>
            <div class="col-md-3"><div class="p-3 bg-white border rounded"><h6>Total Costs</h6><div class="fs-5 fw-bold text-danger">&#8377;31,40,000</div></div></div>
            <div class="col-md-3"><div class="p-3 bg-white border rounded"><h6>Current Margin</h6><div class="fs-5 fw-bold text-success">42.9% (&#8377;23.6L)</div></div></div>
        </div>
    </div>
    <div class="tab-pane fade" id="tab-pnl">
        <div class="erp-card">
            <div class="erp-card-header"><h5 class="erp-card-title text-success"><i class="fa-solid fa-chart-line me-2"></i>Real-Time Project Profit &amp; Loss Statement</h5></div>
            <div class="erp-card-body">
                <table class="table table-bordered">
                    <tr class="table-light"><th colspan="2">A. Contract Revenue</th><th class="text-end">&#8377;55,00,000.00</th></tr>
                    <tr><td class="ps-4">Taxable Invoice Value (INV/26-27/0045)</td><td>Milestone 1 Deliverables</td><td class="text-end">&#8377;55,00,000.00</td></tr>
                    <tr class="table-light"><th colspan="2">B. Direct Operational Costs</th><th class="text-end text-danger">&#8377;31,40,000.00</th></tr>
                    <tr><td class="ps-4">Procurement PO Bills (VND-002)</td><td>Hardware &amp; Switchgears</td><td class="text-end text-danger">&#8377;26,80,000.00</td></tr>
                    <tr><td class="ps-4">Direct Site Expenses</td><td>Field Engineering &amp; Travel</td><td class="text-end text-danger">&#8377;4,60,000.00</td></tr>
                    <tr class="table-success fs-5"><th>Net Project Gross Margin (A - B)</th><th>42.91% Margin</th><th class="text-end">&#8377;23,60,000.00</th></tr>
                </table>
            </div>
        </div>
    </div>
</div>
''')

# 3. Sales View
create_directory(f"{base_views}/Sales")
with open(f"{base_views}/Sales/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Sales & Invoicing";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Sales, Tax Invoicing &amp; Receivables</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Statutory Tax Invoices &bull; CGST/SGST/IGST Automatic Split &bull; Multi-Invoice Allocation</p>
    </div>
    <div class="d-flex gap-2">
        <a asp-controller="Sales" asp-action="AllocatePayment" class="btn btn-outline-success btn-sm px-3"><i class="fa-solid fa-coins me-1"></i> Multi-Receipt Allocation</a>
        <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary);"><i class="fa-solid fa-plus me-1"></i> Create Tax Invoice</button>
    </div>
</div>

<div class="erp-card">
    <div class="erp-card-header d-flex justify-content-between align-items-center">
        <h5 class="erp-card-title"><i class="fa-solid fa-file-invoice-dollar me-2"></i>Sales Tax Invoices</h5>
        <span class="badge bg-success-subtle text-success">Double-Entry AR Posting Ready</span>
    </div>
    <div class="erp-card-body p-0">
        <div class="table-responsive">
            <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
                <thead class="table-light" style="font-size: 12px;">
                    <tr>
                        <th class="ps-4">Invoice No</th>
                        <th>Date</th>
                        <th>Client</th>
                        <th>Project</th>
                        <th>Taxable Value</th>
                        <th>GST Split</th>
                        <th>Grand Total</th>
                        <th>Outstanding</th>
                        <th>Status</th>
                        <th class="text-end pe-4">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        <td class="ps-4 fw-bold text-primary">INV/26-27/0045</td>
                        <td>05 May 2026</td>
                        <td><strong>Reliance Infra Ltd</strong></td>
                        <td>PRJ-2026-0045</td>
                        <td>&#8377;55,00,000</td>
                        <td><small>CGST: &#8377;4.95L<br>SGST: &#8377;4.95L (18%)</small></td>
                        <td>&#8377;64,90,000</td>
                        <td class="text-danger fw-bold">&#8377;64,90,000</td>
                        <td><span class="badge bg-warning text-dark">Sent / Unpaid</span></td>
                        <td class="text-end pe-4">
                            <button class="btn btn-sm btn-outline-primary"><i class="fa-solid fa-file-pdf"></i> PDF</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</div>
''')

with open(f"{base_views}/Sales/AllocatePayment.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Customer Payment Allocation";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Customer Multi-Invoice Payment Allocation</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Split single remittance voucher across multiple outstanding client invoices</p>
    </div>
    <a asp-controller="Sales" asp-action="Index" class="btn btn-outline-secondary btn-sm"><i class="fa-solid fa-arrow-left me-1"></i> Back to Invoices</a>
</div>

<div class="row g-4">
    <div class="col-md-4">
        <div class="erp-card">
            <div class="erp-card-header"><h5 class="erp-card-title"><i class="fa-solid fa-receipt me-2"></i>Receipt Header</h5></div>
            <div class="erp-card-body">
                <div class="mb-3">
                    <label class="form-label fw-semibold">Select Client</label>
                    <select class="form-select"><option>Reliance Infra Ltd</option></select>
                </div>
                <div class="mb-3">
                    <label class="form-label fw-semibold">Amount Received (&#8377;)</label>
                    <input type="text" class="form-control fw-bold fs-5 text-success" value="10,00,000.00" />
                </div>
                <div class="mb-3">
                    <label class="form-label fw-semibold">Deposit Bank Account</label>
                    <select class="form-select"><option>1001 - HDFC Bank Current A/c</option></select>
                </div>
                <div class="mb-3">
                    <label class="form-label fw-semibold">Payment Mode &amp; UTR</label>
                    <input type="text" class="form-control" value="NEFT / UTR-HDFC998822" />
                </div>
            </div>
        </div>
    </div>

    <div class="col-md-8">
        <div class="erp-card">
            <div class="erp-card-header d-flex justify-content-between align-items-center">
                <h5 class="erp-card-title"><i class="fa-solid fa-list-check me-2"></i>Outstanding Invoices Allocation Grid</h5>
                <span class="badge bg-primary">Allocated: &#8377;10,00,000 / Unallocated: &#8377;0.00</span>
            </div>
            <div class="erp-card-body p-0">
                <table class="table table-bordered align-middle mb-0" style="font-size: 13px;">
                    <thead class="table-light">
                        <tr>
                            <th>Invoice Number</th>
                            <th>Total Bill</th>
                            <th>Outstanding</th>
                            <th style="width: 180px;">Allocate Amount (&#8377;)</th>
                            <th>TDS Deducted</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td class="fw-bold">INV/26-27/0041</td>
                            <td>&#8377;4,00,000</td>
                            <td>&#8377;4,00,000</td>
                            <td><input type="text" class="form-control form-control-sm text-end fw-bold" value="4,00,000.00" /></td>
                            <td>&#8377;0.00</td>
                        </tr>
                        <tr>
                            <td class="fw-bold">INV/26-27/0042</td>
                            <td>&#8377;3,00,000</td>
                            <td>&#8377;3,00,000</td>
                            <td><input type="text" class="form-control form-control-sm text-end fw-bold" value="3,00,000.00" /></td>
                            <td>&#8377;0.00</td>
                        </tr>
                        <tr>
                            <td class="fw-bold">INV/26-27/0045</td>
                            <td>&#8377;64,90,000</td>
                            <td>&#8377;64,90,000</td>
                            <td><input type="text" class="form-control form-control-sm text-end fw-bold" value="3,00,000.00" /></td>
                            <td>&#8377;0.00</td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="erp-card-body text-end border-top">
                <button class="btn btn-success px-4 fw-bold"><i class="fa-solid fa-check me-1"></i> Commit Payment Allocation</button>
            </div>
        </div>
    </div>
</div>
''')

# 4. Procurement View
create_directory(f"{base_views}/Procurement")
with open(f"{base_views}/Procurement/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Procurement & 3-Way Match";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Procurement &amp; Office Inventory</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Purchase Orders &bull; Goods Receipt Notes (GRN) &bull; 3-Way Match &bull; Internal Consumable Stock</p>
    </div>
    <div class="d-flex gap-2">
        <button class="btn btn-outline-primary btn-sm px-3"><i class="fa-solid fa-plus me-1"></i> Raise Requisition</button>
        <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary);"><i class="fa-solid fa-file-signature me-1"></i> Issue Purchase Order</button>
    </div>
</div>

<div class="erp-card">
    <div class="erp-card-header d-flex justify-content-between align-items-center">
        <h5 class="erp-card-title"><i class="fa-solid fa-clipboard-check me-2"></i>Vendor Purchase Bills (3-Way Match Enforced)</h5>
        <span class="badge bg-success-subtle text-success">PO vs GRN vs Bill Reconciliation</span>
    </div>
    <div class="erp-card-body p-0">
        <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
            <thead class="table-light" style="font-size: 12px;">
                <tr>
                    <th class="ps-4">Bill No</th>
                    <th>Vendor</th>
                    <th>Linked PO</th>
                    <th>GRN Reference</th>
                    <th>Taxable Value</th>
                    <th>Input GST</th>
                    <th>Grand Total</th>
                    <th>3-Way Match</th>
                    <th class="text-end pe-4">Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class="ps-4 fw-bold text-primary">BILL-9981</td>
                    <td>Apex Hardware &amp; Networking</td>
                    <td><code>PO-2026-0156</code></td>
                    <td><code>GRN-2026-0077</code></td>
                    <td>&#8377;26,80,000</td>
                    <td>&#8377;4,82,400 (18%)</td>
                    <td>&#8377;31,62,400</td>
                    <td><span class="badge bg-success"><i class="fa-solid fa-check me-1"></i> MATCHED</span></td>
                    <td class="text-end pe-4"><button class="btn btn-sm btn-primary">Process Payment</button></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
''')

# 5. Accounts View
create_directory(f"{base_views}/Accounts")
with open(f"{base_views}/Accounts/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Financial Accounting & General Ledger";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">General Ledger &amp; Double-Entry Accounting</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Automated Balanced Journal Vouchers &bull; Real-time Trial Balance &bull; Sub-Ledgers</p>
    </div>
    <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary);"><i class="fa-solid fa-plus me-1"></i> New Journal Voucher (JV)</button>
</div>

<div class="erp-card">
    <div class="erp-card-header"><h5 class="erp-card-title"><i class="fa-solid fa-book-journal-whills me-2"></i>Recent Auto-Posted Journal Vouchers (Balanced)</h5></div>
    <div class="erp-card-body p-0">
        <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
            <thead class="table-light" style="font-size: 12px;">
                <tr>
                    <th class="ps-4">Voucher No</th>
                    <th>Date</th>
                    <th>Type</th>
                    <th>Source Ref</th>
                    <th>Narration</th>
                    <th class="text-end">Debit (&#8377;)</th>
                    <th class="text-end">Credit (&#8377;)</th>
                    <th>Balance Status</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class="ps-4 fw-bold text-primary">JV-2026-00341</td>
                    <td>05 May 2026</td>
                    <td><span class="badge bg-primary-subtle text-primary">SALES</span></td>
                    <td>INV/26-27/0045</td>
                    <td>Tax invoice posted to Reliance Infra Ltd</td>
                    <td class="text-end fw-bold">&#8377;64,90,000.00</td>
                    <td class="text-end fw-bold">&#8377;64,90,000.00</td>
                    <td><span class="badge bg-success">&#10004; Balanced</span></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
''')

# 6. GST View
create_directory(f"{base_views}/Gst")
with open(f"{base_views}/Gst/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "GST & Tax Compliance Desk";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">GST &amp; Tax Compliance Desk</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">GSTR-1 Outward &bull; GSTR-3B Liability &bull; Automated GSTR-2B 4-Way ITC Reconciliation</p>
    </div>
    <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary);"><i class="fa-solid fa-file-arrow-up me-1"></i> Import GSTR-2B JSON</button>
</div>

<div class="row g-3 mb-4">
    <div class="col-md-4">
        <div class="p-3 bg-white border rounded shadow-sm">
            <small class="text-muted fw-semibold">OUTWARD TAX LIABILITY (GSTR-1)</small>
            <div class="fs-4 fw-bold text-primary">&#8377;9,90,000</div>
            <small class="text-muted">CGST: &#8377;4.95L &bull; SGST: &#8377;4.95L</small>
        </div>
    </div>
    <div class="col-md-4">
        <div class="p-3 bg-white border rounded shadow-sm">
            <small class="text-muted fw-semibold">ELIGIBLE INWARD ITC (GSTR-2B)</small>
            <div class="fs-4 fw-bold text-success">&#8377;4,82,400</div>
            <small class="text-success"><i class="fa-solid fa-check me-1"></i> 100% Reconciled with Portal</small>
        </div>
    </div>
    <div class="col-md-4">
        <div class="p-3 bg-white border rounded shadow-sm">
            <small class="text-muted fw-semibold">NET CASH TAX PAYABLE (GSTR-3B)</small>
            <div class="fs-4 fw-bold text-danger">&#8377;5,07,600</div>
            <small class="text-muted">Due Date: 20 June 2026</small>
        </div>
    </div>
</div>
''')

# 7. Banking View
create_directory(f"{base_views}/Banking")
with open(f"{base_views}/Banking/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Cash & Banking Management";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Cash &amp; Banking Management</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Multi-Bank Accounts &bull; Bank Reconciliation (BRS) &bull; Imprest Petty Cash Floats</p>
    </div>
    <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary);"><i class="fa-solid fa-arrow-right-arrow-left me-1"></i> Inter-Bank Transfer</button>
</div>

<div class="erp-card">
    <div class="erp-card-header"><h5 class="erp-card-title"><i class="fa-solid fa-building-columns me-2"></i>Corporate Bank Accounts &amp; Book Balances</h5></div>
    <div class="erp-card-body p-0">
        <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
            <thead class="table-light">
                <tr>
                    <th class="ps-4">Bank Name</th>
                    <th>Account No</th>
                    <th>IFSC</th>
                    <th>Type</th>
                    <th>Book Balance (&#8377;)</th>
                    <th>BRS Status</th>
                    <th class="text-end pe-4">Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class="ps-4 fw-bold">HDFC Bank Current A/c</td>
                    <td><code>50200012345678</code></td>
                    <td><code>HDFC0000124</code></td>
                    <td><span class="badge bg-primary">Current</span></td>
                    <td class="fw-bold text-success fs-6">&#8377;48,20,540.00</td>
                    <td><span class="badge bg-success">Reconciled</span></td>
                    <td class="text-end pe-4"><button class="btn btn-sm btn-outline-primary">Open BRS</button></td>
                </tr>
                <tr>
                    <td class="ps-4 fw-bold">ICICI Bank Overdraft A/c</td>
                    <td><code>000405009988</code></td>
                    <td><code>ICIC0000004</code></td>
                    <td><span class="badge bg-warning text-dark">Overdraft</span></td>
                    <td class="fw-bold text-primary fs-6">&#8377;20,00,000.00</td>
                    <td><span class="badge bg-success">Reconciled</span></td>
                    <td class="text-end pe-4"><button class="btn btn-sm btn-outline-primary">Open BRS</button></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
''')

# 8. Payroll View
create_directory(f"{base_views}/Payroll")
with open(f"{base_views}/Payroll/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Payroll & Employee Operations";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Payroll &amp; Employee Operations</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Monthly Salary Computations &bull; Advance Recoveries &bull; QuestPDF Payslips &bull; Leave Quotas</p>
    </div>
    <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary);"><i class="fa-solid fa-calculator me-1"></i> Execute Monthly Payroll</button>
</div>

<div class="erp-card">
    <div class="erp-card-header d-flex justify-content-between align-items-center">
        <h5 class="erp-card-title"><i class="fa-solid fa-users-gear me-2"></i>Staff Payroll Register (42 Employees)</h5>
        <span class="badge bg-primary">May 2026 Payroll Run</span>
    </div>
    <div class="erp-card-body p-0">
        <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
            <thead class="table-light">
                <tr>
                    <th class="ps-4">Emp Code</th>
                    <th>Full Name</th>
                    <th>Designation</th>
                    <th>Gross Salary</th>
                    <th>PF (12%)</th>
                    <th>PT / TDS</th>
                    <th>Net Payable</th>
                    <th class="text-end pe-4">Payslip</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class="ps-4 fw-bold text-primary">EMP-0042</td>
                    <td><strong>Amit Sharma</strong></td>
                    <td>Project Manager</td>
                    <td>&#8377;1,20,000</td>
                    <td>&#8377;7,200</td>
                    <td>&#8377;10,200</td>
                    <td class="fw-bold text-success">&#8377;1,02,600</td>
                    <td class="text-end pe-4"><button class="btn btn-sm btn-outline-danger"><i class="fa-solid fa-file-pdf"></i> PDF</button></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
''')

# 9. Assets View
create_directory(f"{base_views}/Assets")
with open(f"{base_views}/Assets/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Fixed Asset Management";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Fixed Asset Management</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Capitalized IT Hardware, Servers &amp; Fixtures &bull; Custodian Tracking &bull; WDV Depreciation</p>
    </div>
    <button class="btn btn-primary btn-sm px-3" style="background: var(--erp-primary);"><i class="fa-solid fa-plus me-1"></i> Register Hardware Asset</button>
</div>

<div class="erp-card">
    <div class="erp-card-header"><h5 class="erp-card-title"><i class="fa-solid fa-laptop-code me-2"></i>Capitalized Fixed Assets Register</h5></div>
    <div class="erp-card-body p-0">
        <table class="table table-hover align-middle mb-0" style="font-size: 13.5px;">
            <thead class="table-light">
                <tr>
                    <th class="ps-4">Asset Tag</th>
                    <th>Asset Description</th>
                    <th>Category</th>
                    <th>Serial Number</th>
                    <th>Purchase Cost</th>
                    <th>Current Book Value (WDV)</th>
                    <th>Custodian</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td class="ps-4 fw-bold text-primary">AST-LAP-0089</td>
                    <td>Dell Latitude 7440 i7 32GB 1TB</td>
                    <td><span class="badge bg-info-subtle text-info">LAPTOP</span></td>
                    <td><code>DL7440-889922</code></td>
                    <td>&#8377;1,45,000</td>
                    <td>&#8377;87,000 (WDV 40%)</td>
                    <td>Amit Sharma (EMP-0042)</td>
                    <td><span class="badge bg-success">Allocated</span></td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
''')

# 10. Reports View
create_directory(f"{base_views}/Reports")
with open(f"{base_views}/Reports/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Enterprise Reports Suite";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Enterprise Reports &amp; Analytics Suite</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">20+ Financial, Project, Procurement &amp; Statutory GST Statements with ClosedXML &amp; QuestPDF</p>
    </div>
</div>

<div class="row g-3">
    <div class="col-md-4">
        <div class="erp-card">
            <div class="erp-card-header"><h5 class="erp-card-title text-primary"><i class="fa-solid fa-file-invoice me-2"></i>Financial Statements</h5></div>
            <div class="erp-card-body">
                <ul class="list-group list-group-flush" style="font-size: 13.5px;">
                    <li class="list-group-item d-flex justify-content-between">Trial Balance <button class="btn btn-sm btn-outline-primary"><i class="fa-solid fa-file-excel"></i></button></li>
                    <li class="list-group-item d-flex justify-content-between">Profit &amp; Loss Statement <button class="btn btn-sm btn-outline-primary"><i class="fa-solid fa-file-excel"></i></button></li>
                    <li class="list-group-item d-flex justify-content-between">General Ledger Statement <button class="btn btn-sm btn-outline-primary"><i class="fa-solid fa-file-pdf"></i></button></li>
                </ul>
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="erp-card">
            <div class="erp-card-header"><h5 class="erp-card-title text-success"><i class="fa-solid fa-diagram-project me-2"></i>Project Reports</h5></div>
            <div class="erp-card-body">
                <ul class="list-group list-group-flush" style="font-size: 13.5px;">
                    <li class="list-group-item d-flex justify-content-between">Project Profitability P&amp;L <button class="btn btn-sm btn-outline-success"><i class="fa-solid fa-file-excel"></i></button></li>
                    <li class="list-group-item d-flex justify-content-between">Budget vs Actual Cost <button class="btn btn-sm btn-outline-success"><i class="fa-solid fa-file-excel"></i></button></li>
                    <li class="list-group-item d-flex justify-content-between">Milestone Billing Status <button class="btn btn-sm btn-outline-success"><i class="fa-solid fa-file-pdf"></i></button></li>
                </ul>
            </div>
        </div>
    </div>
    <div class="col-md-4">
        <div class="erp-card">
            <div class="erp-card-header"><h5 class="erp-card-title text-danger"><i class="fa-solid fa-receipt me-2"></i>Statutory GST Reports</h5></div>
            <div class="erp-card-body">
                <ul class="list-group list-group-flush" style="font-size: 13.5px;">
                    <li class="list-group-item d-flex justify-content-between">GSTR-1 Outward Summary <button class="btn btn-sm btn-outline-danger"><i class="fa-solid fa-file-excel"></i></button></li>
                    <li class="list-group-item d-flex justify-content-between">GSTR-3B Tax Computation <button class="btn btn-sm btn-outline-danger"><i class="fa-solid fa-file-excel"></i></button></li>
                    <li class="list-group-item d-flex justify-content-between">GSTR-2B Mismatch Report <button class="btn btn-sm btn-outline-danger"><i class="fa-solid fa-file-excel"></i></button></li>
                </ul>
            </div>
        </div>
    </div>
</div>
''')

# 11. Admin View
create_directory(f"{base_views}/Admin")
with open(f"{base_views}/Admin/Index.cshtml", "w", encoding="utf-8") as f:
    f.write('''@{
    ViewData["Title"] = "Admin & Security Center";
}

<div class="d-flex align-items-center justify-content-between mb-4">
    <div>
        <h3 style="font-weight: 700; color: var(--erp-primary); margin-bottom: 4px;">Administration, Security (RBAC) &amp; Audit</h3>
        <p style="color: var(--erp-text-muted); margin: 0; font-size: 13.5px;">Role-Based Access Control &bull; Multi-Level Approval State Machine &bull; Tamper-Evident SHA-256 Audit Trail</p>
    </div>
    <span class="badge bg-success fs-6"><i class="fa-solid fa-shield-check me-1"></i> Security Hardened</span>
</div>

<div class="row g-4">
    <div class="col-md-6">
        <div class="erp-card">
            <div class="erp-card-header"><h5 class="erp-card-title"><i class="fa-solid fa-user-shield me-2"></i>System User Roles &amp; Permissions (RBAC)</h5></div>
            <div class="erp-card-body p-0">
                <table class="table table-hover align-middle mb-0" style="font-size: 13px;">
                    <thead class="table-light">
                        <tr><th>Role Name</th><th>Scope</th><th>Users</th><th>Permissions</th></tr>
                    </thead>
                    <tbody>
                        <tr><td><strong>SUPER_ADMIN</strong></td><td>Enterprise Full Access</td><td><span class="badge bg-primary">2</span></td><td>Full (All Modules)</td></tr>
                        <tr><td><strong>PROJECT_MANAGER</strong></td><td>Projects &amp; Delivery</td><td><span class="badge bg-primary">6</span></td><td>Projects, DC, Expense</td></tr>
                        <tr><td><strong>ACCOUNTANT</strong></td><td>Finance, GL &amp; Tax</td><td><span class="badge bg-primary">4</span></td><td>Invoices, GL, GST, BRS</td></tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>

    <div class="col-md-6">
        <div class="erp-card">
            <div class="erp-card-header d-flex justify-content-between align-items-center">
                <h5 class="erp-card-title"><i class="fa-solid fa-fingerprint me-2"></i>Tamper-Evident Audit Log (SHA-256)</h5>
                <span class="badge bg-info-subtle text-info">Immutable Chain</span>
            </div>
            <div class="erp-card-body p-0">
                <table class="table table-hover align-middle mb-0" style="font-size: 12.5px;">
                    <thead class="table-light">
                        <tr><th>Action</th><th>Target Table</th><th>User</th><th>Timestamp</th><th>Integrity Hash</th></tr>
                    </thead>
                    <tbody>
                        <tr><td><span class="badge bg-success">INSERT</span></td><td>sales_invoices</td><td>admin@sdk.com</td><td>10:42 AM</td><td><code>e3b0c44298fc...</code></td></tr>
                        <tr><td><span class="badge bg-warning text-dark">APPROVE</span></td><td>purchase_orders</td><td>director@sdk.com</td><td>10:45 AM</td><td><code>8f4b23a19c6e...</code></td></tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</div>
''')

print("All 11 Module Views successfully generated.")
