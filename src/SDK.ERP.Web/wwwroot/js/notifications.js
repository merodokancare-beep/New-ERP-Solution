// Enterprise Notification Center & Approval Dispatcher
(function () {
    let allNotifications = [];
    let currentFilterTab = 'ALL';
    let isFetching = false;

    // Get dismissed notification IDs from localStorage
    function getDismissedIds() {
        try {
            const raw = localStorage.getItem('erp_dismissed_notifications');
            return raw ? JSON.parse(raw) : [];
        } catch {
            return [];
        }
    }

    // Save dismissed notification IDs to localStorage
    function addDismissedId(id) {
        try {
            const list = getDismissedIds();
            if (!list.includes(id)) {
                list.push(id);
                // Keep max 200 dismissed IDs to prevent unbounded growth
                if (list.length > 200) list.splice(0, list.length - 200);
                localStorage.setItem('erp_dismissed_notifications', JSON.stringify(list));
            }
        } catch (e) {
            console.warn('Could not persist dismissed notification', e);
        }
    }

    // Load notifications from server
    async function fetchNotifications(isManual = false) {
        if (isFetching) return;
        isFetching = true;

        const refreshIcon = document.getElementById('refreshIcon');
        if (refreshIcon && isManual) {
            refreshIcon.classList.add('fa-spin');
        }

        try {
            const response = await fetch('/Notifications/GetNotifications', {
                method: 'GET',
                headers: { 'Accept': 'application/json' }
            });

            if (!response.ok) throw new Error('Network error: ' + response.status);
            const data = await response.json();

            if (data && data.success) {
                const dismissed = getDismissedIds();
                // Filter out any locally dismissed notification
                allNotifications = (data.items || []).filter(item => !dismissed.includes(item.id));
                updateBadgeAndHeader();
                renderNotificationList();
            }
        } catch (err) {
            console.error('Failed to fetch notifications:', err);
        } finally {
            isFetching = false;
            if (refreshIcon) {
                setTimeout(() => refreshIcon.classList.remove('fa-spin'), 350);
            }
        }
    }

    // Update the topbar bell badge and tab counters
    function updateBadgeAndHeader() {
        const badge = document.getElementById('notificationBadge');
        const bellIcon = document.getElementById('bellIcon');
        const headerSub = document.getElementById('notificationHeaderSub');
        const tabAll = document.getElementById('tabCountAll');
        const tabApprovals = document.getElementById('tabCountApprovals');
        const tabAlerts = document.getElementById('tabCountAlerts');

        const totalCount = allNotifications.length;
        const approvalsCount = allNotifications.filter(n => n.category === 'APPROVAL').length;
        const alertsCount = allNotifications.filter(n => n.category === 'ALERT').length;

        if (tabAll) tabAll.textContent = totalCount;
        if (tabApprovals) tabApprovals.textContent = approvalsCount;
        if (tabAlerts) tabAlerts.textContent = alertsCount;

        if (headerSub) {
            headerSub.textContent = totalCount === 0 
                ? 'All caught up' 
                : `${totalCount} unattended notification${totalCount > 1 ? 's' : ''}`;
        }

        if (badge) {
            if (totalCount > 0) {
                badge.textContent = totalCount > 99 ? '99+' : totalCount;
                badge.classList.remove('d-none');
                badge.classList.add('notif-badge-active');
                if (bellIcon) {
                    bellIcon.className = 'fa-solid fa-bell text-primary';
                }
            } else {
                badge.classList.add('d-none');
                badge.classList.remove('notif-badge-active');
                if (bellIcon) {
                    bellIcon.className = 'fa-regular fa-bell';
                }
            }
        }
    }

    // Render list based on current active tab
    function renderNotificationList() {
        const container = document.getElementById('notificationListContainer');
        if (!container) return;

        let filtered = allNotifications;
        if (currentFilterTab === 'APPROVAL') {
            filtered = allNotifications.filter(n => n.category === 'APPROVAL');
        } else if (currentFilterTab === 'ALERT') {
            filtered = allNotifications.filter(n => n.category === 'ALERT');
        }

        if (filtered.length === 0) {
            container.innerHTML = `
                <div class="notif-empty-state text-center p-4">
                    <div class="notif-empty-icon mx-auto mb-2">
                        <i class="fa-solid fa-circle-check text-success fs-1"></i>
                    </div>
                    <div class="fw-bold text-dark" style="font-size: 13.5px;">All Caught Up!</div>
                    <div class="text-muted mt-1" style="font-size: 12px; line-height: 1.4;">
                        ${currentFilterTab === 'APPROVAL' 
                            ? 'No purchase orders or requests pending your approval.' 
                            : currentFilterTab === 'ALERT'
                            ? 'No overdue invoices or low-stock alerts right now.'
                            : 'You have no unattended notifications or pending approvals.'}
                    </div>
                </div>
            `;
            return;
        }

        let html = '<div class="list-group list-group-flush notif-list-group">';
        filtered.forEach(item => {
            const isApproval = item.category === 'APPROVAL';
            const iconBg = isApproval ? 'bg-purple-subtle text-purple' : getIconColorClass(item.type);

            html += `
                <div class="notif-card-item list-group-item p-3 border-bottom position-relative" id="notif-card-${item.id}">
                    <div class="d-flex align-items-start gap-2.5">
                        <div class="notif-icon-circle rounded-circle d-flex align-items-center justify-content-center flex-shrink-0 ${iconBg}">
                            <i class="${item.icon}"></i>
                        </div>
                        <div class="flex-grow-1 overflow-hidden me-2">
                            <div class="d-flex align-items-center justify-content-between mb-0.5">
                                <span class="fw-bold text-dark text-truncate" style="font-size: 13px;" title="${escapeHtml(item.title)}">
                                    ${escapeHtml(item.title)}
                                </span>
                                <span class="badge ${getBadgeColorClass(item.badgeColor)} rounded-pill flex-shrink-0" style="font-size: 10px; font-weight: 600;">
                                    ${escapeHtml(item.badgeText)}
                                </span>
                            </div>
                            <div class="text-muted text-break mb-2" style="font-size: 11.5px; line-height: 1.35;">
                                ${escapeHtml(item.description)}
                            </div>
                            <div class="d-flex align-items-center justify-content-between">
                                <div class="text-secondary" style="font-size: 10.5px;">
                                    <i class="fa-regular fa-clock me-1"></i>${escapeHtml(item.timeAgo)}
                                </div>
                                <div class="d-flex align-items-center gap-1.5">
                                    ${item.canQuickApprove ? `
                                        <button type="button" 
                                                class="btn btn-sm btn-success py-0.5 px-2 rounded-pill fw-semibold" 
                                                style="font-size: 11px;" 
                                                onclick="window.quickApprovePo(${item.entityId}, '${item.id}', event)">
                                            <i class="fa-solid fa-check me-1"></i>Approve
                                        </button>
                                    ` : ''}
                                    <a href="${item.actionUrl}" class="btn btn-sm btn-primary py-0.5 px-2.5 rounded-pill fw-semibold text-decoration-none" style="font-size: 11px;">
                                        ${escapeHtml(item.actionLabel)} &rarr;
                                    </a>
                                </div>
                            </div>
                        </div>
                        <button type="button" class="btn-close notif-dismiss-btn flex-shrink-0" style="font-size: 9px; opacity: 0.5;" title="Dismiss notification" onclick="window.dismissNotification('${item.id}', event)"></button>
                    </div>
                </div>
            `;
        });
        html += '</div>';

        container.innerHTML = html;
    }

    function getIconColorClass(type) {
        switch (type) {
            case 'PO_APPROVAL': return 'bg-purple-subtle text-purple';
            case 'APPROVAL_REQ': return 'bg-warning-subtle text-warning-emphasis';
            case 'OVERDUE_INVOICE': return 'bg-danger-subtle text-danger';
            case 'LOW_STOCK': return 'bg-warning-subtle text-warning-emphasis';
            case 'UNALLOCATED_RECEIPT': return 'bg-info-subtle text-info';
            case 'OVERDUE_BILL': return 'bg-secondary-subtle text-secondary';
            default: return 'bg-primary-subtle text-primary';
        }
    }

    function getBadgeColorClass(color) {
        switch (color) {
            case 'purple': return 'bg-purple-subtle text-purple border border-purple-subtle';
            case 'danger': return 'bg-danger-subtle text-danger border border-danger-subtle';
            case 'warning': return 'bg-warning-subtle text-warning-emphasis border border-warning-subtle';
            case 'info': return 'bg-info-subtle text-info border border-info-subtle';
            case 'amber': return 'bg-warning-subtle text-warning border';
            default: return 'bg-secondary-subtle text-secondary border';
        }
    }

    function escapeHtml(text) {
        if (!text) return '';
        const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
        return text.toString().replace(/[&<>"']/g, m => map[m]);
    }

    // Global Action: Dismiss a single notification
    window.dismissNotification = function (id, event) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        const el = document.getElementById(`notif-card-${id}`);
        if (el) {
            el.style.transition = 'all 0.25s ease';
            el.style.opacity = '0';
            el.style.transform = 'translateX(20px)';
            setTimeout(() => {
                addDismissedId(id);
                allNotifications = allNotifications.filter(n => n.id !== id);
                updateBadgeAndHeader();
                renderNotificationList();
            }, 250);
        } else {
            addDismissedId(id);
            allNotifications = allNotifications.filter(n => n.id !== id);
            updateBadgeAndHeader();
            renderNotificationList();
        }
    };

    // Global Action: Mark all notifications as read / dismissed
    window.markAllNotificationsAsRead = function () {
        allNotifications.forEach(n => addDismissedId(n.id));
        allNotifications = [];
        updateBadgeAndHeader();
        renderNotificationList();
    };

    // Global Action: Quick Approve PO
    window.quickApprovePo = async function (poId, notifId, event) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        const card = document.getElementById(`notif-card-${notifId}`);
        const btn = event.currentTarget;
        if (btn) {
            btn.disabled = true;
            btn.innerHTML = '<span class="spinner-border spinner-border-sm" role="status"></span>';
        }

        try {
            const token = document.querySelector('#notifCsrfForm input[name="__RequestVerificationToken"]')?.value;
            const formData = new FormData();
            formData.append('id', poId);
            if (token) formData.append('__RequestVerificationToken', token);

            const res = await fetch('/Notifications/QuickApprovePo', {
                method: 'POST',
                body: formData
            });

            const result = await res.json();
            if (result && result.success) {
                if (card) {
                    card.innerHTML = `
                        <div class="p-3 text-center bg-success-subtle rounded text-success fw-semibold" style="font-size: 12.5px;">
                            <i class="fa-solid fa-circle-check me-1"></i> ${result.message}
                        </div>
                    `;
                    setTimeout(() => {
                        window.dismissNotification(notifId);
                    }, 1200);
                }
            } else {
                alert(result?.message || 'Failed to approve PO.');
                if (btn) {
                    btn.disabled = false;
                    btn.innerHTML = '<i class="fa-solid fa-check me-1"></i>Approve';
                }
            }
        } catch (err) {
            console.error(err);
            alert('Error communicating with approval service.');
            if (btn) {
                btn.disabled = false;
                btn.innerHTML = '<i class="fa-solid fa-check me-1"></i>Approve';
            }
        }
    };

    // Global Action: Filter by Tab
    window.filterNotificationTab = function (tab) {
        currentFilterTab = tab;
        document.querySelectorAll('.notif-tab').forEach(btn => {
            if (btn.getAttribute('data-tab') === tab) {
                btn.classList.add('active');
            } else {
                btn.classList.remove('active');
            }
        });
        renderNotificationList();
    };

    // Global Action: Manual Refresh
    window.loadNotifications = function (isManual) {
        fetchNotifications(isManual);
    };

    // Initialize on DOM ready
    document.addEventListener('DOMContentLoaded', function () {
        fetchNotifications(false);

        // Auto-refresh every 60 seconds
        setInterval(() => {
            fetchNotifications(false);
        }, 60000);

        // Fetch freshly when dropdown is opened
        const notifDropdown = document.getElementById('notificationsBtn');
        if (notifDropdown) {
            notifDropdown.addEventListener('show.bs.dropdown', function () {
                fetchNotifications(false);
            });
        }
    });
})();
