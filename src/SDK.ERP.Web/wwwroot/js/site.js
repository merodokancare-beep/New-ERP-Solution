/**
 * Enterprise Commercial ERP - Core Client Interactive Logic
 * Handles:
 * 1. Sidebar toggle (collapse / expand & mobile drawer)
 * 2. Quick Add (+) dropdown menu & smart modal triggers
 * 3. Keyboard shortcuts (Alt+N for Quick Add, Ctrl+B for Sidebar, / for Search)
 * 4. URL query parameter auto-modal launchers (?action=create)
 * 5. Global search navigation & dark mode toggle
 * 6. Global URL hash tab switching
 */

document.addEventListener('DOMContentLoaded', function () {
    const appContainer = document.getElementById('appContainer');
    const sidebarToggleBtn = document.getElementById('sidebarToggleBtn');
    const mobileSidebarToggleBtn = document.getElementById('mobileSidebarToggleBtn');
    const sidebarBackdrop = document.getElementById('sidebarBackdrop');
    const quickAddBtn = document.getElementById('quickAddBtn');
    const globalSearchInput = document.getElementById('globalSearchInput');
    const themeToggleBtn = document.getElementById('themeToggleBtn');

    // =========================================================================
    // 1. SIDEBAR COLLAPSE & EXPAND LOGIC
    // =========================================================================
    
    // Restore saved sidebar collapsed state on desktop
    if (localStorage.getItem('erp_sidebar_collapsed') === 'true' && window.innerWidth > 991) {
        if (appContainer) {
            appContainer.classList.add('sidebar-collapsed');
        }
    }

    // Toggle function
    function toggleSidebar() {
        if (!appContainer) return;

        if (window.innerWidth <= 991) {
            // Mobile: toggle mobile drawer
            appContainer.classList.toggle('sidebar-mobile-open');
        } else {
            // Desktop: toggle compact/expanded view
            appContainer.classList.toggle('sidebar-collapsed');
            const isCollapsed = appContainer.classList.contains('sidebar-collapsed');
            localStorage.setItem('erp_sidebar_collapsed', isCollapsed ? 'true' : 'false');
        }
    }

    if (sidebarToggleBtn) {
        sidebarToggleBtn.addEventListener('click', function (e) {
            e.preventDefault();
            toggleSidebar();
        });
    }

    if (mobileSidebarToggleBtn) {
        mobileSidebarToggleBtn.addEventListener('click', function (e) {
            e.preventDefault();
            toggleSidebar();
        });
    }

    // Close mobile drawer when clicking backdrop
    if (sidebarBackdrop) {
        sidebarBackdrop.addEventListener('click', function () {
            if (appContainer) {
                appContainer.classList.remove('sidebar-mobile-open');
            }
        });
    }

    // Close mobile drawer when clicking a nav item on mobile
    const navLinks = document.querySelectorAll('.app-sidebar .nav-item-link');
    navLinks.forEach(function (link) {
        link.addEventListener('click', function () {
            if (window.innerWidth <= 991 && appContainer) {
                appContainer.classList.remove('sidebar-mobile-open');
            }
        });
    });

    // Initialize Bootstrap Tooltips for nav items (visible when collapsed)
    if (typeof bootstrap !== 'undefined' && bootstrap.Tooltip) {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.forEach(function (tooltipTriggerEl) {
            new bootstrap.Tooltip(tooltipTriggerEl, {
                boundary: document.body,
                trigger: 'hover'
            });
        });
    }

    // =========================================================================
    // 2. QUICK ADD (+) MENU & MODAL LAUNCHERS
    // =========================================================================

    // Listen for clicks on Quick Action cards inside the Quick Add dropdown
    const quickActionCards = document.querySelectorAll('.quick-action-card');
    quickActionCards.forEach(function (card) {
        card.addEventListener('click', function (e) {
            const shortcut = card.getAttribute('data-shortcut');
            let handledDirectly = false;

            // Helper to close dropdown menu cleanly
            function closeQuickAddDropdown() {
                if (quickAddBtn && typeof bootstrap !== 'undefined') {
                    const dd = bootstrap.Dropdown.getInstance(quickAddBtn);
                    if (dd) dd.hide();
                }
            }

            // Check if current page already has the requested modal in DOM
            if (shortcut === 'invoice') {
                const invoiceModal = document.getElementById('createInvoiceModal');
                if (invoiceModal && typeof bootstrap !== 'undefined') {
                    e.preventDefault();
                    closeQuickAddDropdown();
                    bootstrap.Modal.getOrCreateInstance(invoiceModal).show();
                    handledDirectly = true;
                }
            } else if (shortcut === 'project') {
                const projectModal = document.getElementById('createProjectModal');
                if (projectModal && typeof bootstrap !== 'undefined') {
                    e.preventDefault();
                    closeQuickAddDropdown();
                    bootstrap.Modal.getOrCreateInstance(projectModal).show();
                    handledDirectly = true;
                }
            } else if (shortcut === 'po') {
                const poModal = document.getElementById('createPoModal');
                if (poModal && typeof bootstrap !== 'undefined') {
                    e.preventDefault();
                    closeQuickAddDropdown();
                    bootstrap.Modal.getOrCreateInstance(poModal).show();
                    handledDirectly = true;
                }
            } else if (shortcut === 'client') {
                const clientModal = document.getElementById('createClientModal');
                if (clientModal && typeof bootstrap !== 'undefined') {
                    e.preventDefault();
                    closeQuickAddDropdown();
                    const clientsTab = document.getElementById('clients-tab');
                    if (clientsTab) bootstrap.Tab.getOrCreateInstance(clientsTab).show();
                    bootstrap.Modal.getOrCreateInstance(clientModal).show();
                    handledDirectly = true;
                }
            } else if (shortcut === 'vendor') {
                const vendorModal = document.getElementById('createVendorModal');
                if (vendorModal && typeof bootstrap !== 'undefined') {
                    e.preventDefault();
                    closeQuickAddDropdown();
                    const vendorsTab = document.getElementById('vendors-tab');
                    if (vendorsTab) bootstrap.Tab.getOrCreateInstance(vendorsTab).show();
                    bootstrap.Modal.getOrCreateInstance(vendorModal).show();
                    handledDirectly = true;
                }
            } else if (shortcut === 'item') {
                const itemModal = document.getElementById('createItemModal');
                if (itemModal && typeof bootstrap !== 'undefined') {
                    e.preventDefault();
                    closeQuickAddDropdown();
                    const itemsTab = document.getElementById('items-tab');
                    if (itemsTab) bootstrap.Tab.getOrCreateInstance(itemsTab).show();
                    bootstrap.Modal.getOrCreateInstance(itemModal).show();
                    handledDirectly = true;
                }
            }

            // If not handled on current page, allow default navigation to link with ?action=create
        });
    });

    // Auto-launch modal if navigated with query string ?action=create
    try {
        const urlParams = new URLSearchParams(window.location.search);
        if (urlParams.get('action') === 'create' && typeof bootstrap !== 'undefined') {
            const tab = urlParams.get('tab');
            if (tab === 'vendors') {
                const tabEl = document.getElementById('vendors-tab');
                if (tabEl) bootstrap.Tab.getOrCreateInstance(tabEl).show();
                const modalEl = document.getElementById('createVendorModal');
                if (modalEl) setTimeout(() => bootstrap.Modal.getOrCreateInstance(modalEl).show(), 250);
            } else if (tab === 'items') {
                const tabEl = document.getElementById('items-tab');
                if (tabEl) bootstrap.Tab.getOrCreateInstance(tabEl).show();
                const modalEl = document.getElementById('createItemModal');
                if (modalEl) setTimeout(() => bootstrap.Modal.getOrCreateInstance(modalEl).show(), 250);
            } else if (tab === 'clients') {
                const tabEl = document.getElementById('clients-tab');
                if (tabEl) bootstrap.Tab.getOrCreateInstance(tabEl).show();
                const modalEl = document.getElementById('createClientModal');
                if (modalEl) setTimeout(() => bootstrap.Modal.getOrCreateInstance(modalEl).show(), 250);
            } else if (document.getElementById('createInvoiceModal')) {
                setTimeout(() => bootstrap.Modal.getOrCreateInstance(document.getElementById('createInvoiceModal')).show(), 250);
            } else if (document.getElementById('createProjectModal')) {
                setTimeout(() => bootstrap.Modal.getOrCreateInstance(document.getElementById('createProjectModal')).show(), 250);
            } else if (document.getElementById('createPoModal')) {
                setTimeout(() => bootstrap.Modal.getOrCreateInstance(document.getElementById('createPoModal')).show(), 250);
            }
        }
    } catch (e) {
        console.warn('URL parameter auto-modal handler:', e);
    }

    // =========================================================================
    // 3. KEYBOARD SHORTCUTS
    // =========================================================================
    document.addEventListener('keydown', function (e) {
        const activeTag = document.activeElement ? document.activeElement.tagName.toLowerCase() : '';
        const isTyping = activeTag === 'input' || activeTag === 'textarea' || activeTag === 'select';

        // Alt + N: Toggle Quick Add Dropdown
        if (e.altKey && (e.key === 'n' || e.key === 'N')) {
            e.preventDefault();
            if (quickAddBtn && typeof bootstrap !== 'undefined') {
                const dd = bootstrap.Dropdown.getOrCreateInstance(quickAddBtn);
                dd.toggle();
            }
        }

        // Ctrl + B: Toggle Sidebar
        if (e.ctrlKey && (e.key === 'b' || e.key === 'B')) {
            e.preventDefault();
            toggleSidebar();
        }

        // / : Focus global search (when not already typing)
        if (e.key === '/' && !isTyping) {
            e.preventDefault();
            if (globalSearchInput) {
                globalSearchInput.focus();
                globalSearchInput.select();
            }
        }

        // Escape: Close mobile sidebar or search focus
        if (e.key === 'Escape') {
            if (appContainer && appContainer.classList.contains('sidebar-mobile-open')) {
                appContainer.classList.remove('sidebar-mobile-open');
            }
            if (document.activeElement === globalSearchInput) {
                globalSearchInput.blur();
            }
        }
    });

    // =========================================================================
    // 4. GLOBAL SEARCH NAVIGATION
    // =========================================================================
    if (globalSearchInput) {
        const navigationMap = [
            { terms: ['dash', 'home', 'overview', 'kpi'], url: '/' },
            { terms: ['master', 'catalog', 'client', 'vendor', 'item', 'unit', 'tax'], url: '/Masters' },
            { terms: ['project', 'contract', 'margin', 'delivery'], url: '/Projects' },
            { terms: ['sale', 'tax invoice', 'invoice', 'billing', 'ar'], url: '/Sales' },
            { terms: ['receipt', 'allocate', 'payment', 'collection'], url: '/Sales/AllocatePayment' },
            { terms: ['procure', 'purchase', 'po', 'vendor order'], url: '/Procurement' },
            { terms: ['inventory', 'stock', 'warehouse', 'store'], url: '/Procurement/Inventory' },
            { terms: ['account', 'ledger', 'journal', 'coa', 'trial balance'], url: '/Accounts' },
            { terms: ['gst', 'tax compliance', 'gstr', 'eway'], url: '/Gst' },
            { terms: ['bank', 'cheque', 'brs', 'statement', 'cash'], url: '/Banking' },
            { terms: ['payroll', 'salary', 'employee', 'payslip', 'hr'], url: '/Payroll' },
            { terms: ['asset', 'fixed asset', 'ticket', 'device'], url: '/Assets' },
            { terms: ['report', 'analytics', 'mis', 'audit report'], url: '/Reports' },
            { terms: ['admin', 'audit', 'rbac', 'role', 'user management'], url: '/Admin' }
        ];

        globalSearchInput.addEventListener('keydown', function (e) {
            if (e.key === 'Enter') {
                const query = globalSearchInput.value.trim().toLowerCase();
                if (!query) return;

                const match = navigationMap.find(item => item.terms.some(t => t.includes(query) || query.includes(t)));
                if (match) {
                    window.location.href = match.url;
                } else {
                    // Default to search or masters
                    window.location.href = '/Masters';
                }
            }
        });
    }

    // =========================================================================
    // 5. THEME TOGGLE (DARK / LIGHT MODE)
    // =========================================================================
    const savedTheme = localStorage.getItem('erp_theme') || 'light';
    if (savedTheme === 'dark') {
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        if (themeToggleBtn) {
            themeToggleBtn.innerHTML = '<i class="fa-solid fa-sun"></i>';
        }
    }

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener('click', function () {
            const currentTheme = document.documentElement.getAttribute('data-bs-theme') || 'light';
            const nextTheme = currentTheme === 'dark' ? 'light' : 'dark';

            document.documentElement.setAttribute('data-bs-theme', nextTheme);
            localStorage.setItem('erp_theme', nextTheme);

            themeToggleBtn.innerHTML = nextTheme === 'dark'
                ? '<i class="fa-solid fa-sun text-warning"></i>'
                : '<i class="fa-solid fa-moon"></i>';
        });
    }

    // =========================================================================
    // 6. GLOBAL URL HASH TAB ACTIVATION
    // =========================================================================
    function activateTabFromHash() {
        if (window.location.hash) {
            const hash = window.location.hash.replace('#', '');
            const targetTab = document.getElementById(hash + '-tab') || 
                              document.querySelector(`[data-bs-target="#tab-${hash}"]`) ||
                              document.querySelector(`[href="#tab-${hash}"]`);
            if (targetTab && typeof bootstrap !== 'undefined' && bootstrap.Tab) {
                bootstrap.Tab.getOrCreateInstance(targetTab).show();
            }
        }
    }
    activateTabFromHash();
    window.addEventListener('hashchange', activateTabFromHash);
});
