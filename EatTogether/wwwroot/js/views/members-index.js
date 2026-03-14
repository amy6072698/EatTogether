/**
 * EatTogether（義起吃）後台管理系統
 * members-index.js — 會員管理列表頁 JavaScript
 *
 * 規範：
 * - DOM 選取一律使用 querySelector / querySelectorAll
 * - AJAX 一律透過 apiFetch 封裝
 * - DataTables 初始化使用 jQuery API（DataTables 為 jQuery plugin，$() 限此處使用）
 */

'use strict';

/* ============================================================
   apiFetch — 統一 API 呼叫封裝
   ============================================================ */
async function apiFetch(url, options = {}) {
    const defaults = {
        credentials: 'include',
        headers: {
            'Content-Type': 'application/json',
            ...(options.headers || {})
        }
    };
    const config = { ...options, ...defaults };
    try {
        const response = await fetch(url, config);
        if (response.status === 401) {
            window.location.href = '/Auth/Login';
            return null;
        }
        return response;
    } catch (error) {
        console.error('apiFetch error:', error);
        throw error;
    }
}

/* ============================================================
   全域變數
   ============================================================ */
let membersTable = null;        // DataTables instance
let currentStatusFilter = '';   // '' | 'active' | 'unverified' | 'blacklisted' | 'deleted'

// 狀態值對應顯示文字（與 View 的 status-badge 文字一致）
const STATUS_TEXT = {
    active:      '啟用中',
    unverified:  '未驗證',
    blacklisted: '黑名單',
    deleted:     '已刪除'
};

/* ============================================================
   DataTables 初始化
   ============================================================ */
function initDataTable() {
    membersTable = $('#members-table').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/2.3.7/i18n/zh-HANT.json'
        },
        pageLength: 10,
        ordering: true,
        order: [[5, 'desc']],       // 預設：註冊時間最新
        searching: true,
        //dom: 'tip',
        dom: "<'row'<'col-12'tr>>" +
            "<'row align-items-center mt-2'<'col-auto'i><'col'p>>",
        columnDefs: [
            { targets: '_all', defaultContent: '' },
            // 停用欄位標題點擊排序，僅保留第 5（註冊時間）欄供程式控制
            { orderable: false, targets: [0, 1, 2, 3, 4, 6, 7, 8] },
            { orderable: true,  targets: [5] }
        ]
    });

    // 狀態篩選自訂函式（data[7] = 狀態欄，index 7）
    $.fn.dataTable.ext.search.push(function (settings, data) {
        if (!currentStatusFilter) return true;
        const statusText = STATUS_TEXT[currentStatusFilter] ?? '';
        return data[7] && data[7].includes(statusText);
    });
}

/* ============================================================
   搜尋列
   ============================================================ */
function initSearch() {
    const btnSearch   = document.querySelector('#btn-search');
    const statusSelect = document.querySelector('#search-status');

    // 狀態下拉：選單改變時立即篩選
    if (statusSelect) {
        statusSelect.addEventListener('change', function () {
            currentStatusFilter = this.value;
            if (membersTable) membersTable.draw();
        });
    }

    if (!btnSearch) return;

    btnSearch.addEventListener('click', function () {
        const name      = document.querySelector('#search-name')?.value.trim()    ?? '';
        const account   = document.querySelector('#search-account')?.value.trim() ?? '';
        const email     = document.querySelector('#search-email')?.value.trim()   ?? '';
        const phone     = document.querySelector('#search-phone')?.value.trim()   ?? '';
        const statusVal = document.querySelector('#search-status')?.value         ?? '';

        currentStatusFilter = statusVal;

        // 多欄位串接全文搜尋（後端連線後改為 apiFetch 送參數）
        const combined = [name, account, email, phone].filter(v => v).join(' ');
        if (membersTable) {
            membersTable.search(combined).draw();
        }
    });
}

/* ============================================================
   排序下拉
   ============================================================ */
function initSortDropdown() {
    const sortSelect = document.querySelector('#sort-select');
    if (!sortSelect) return;

    sortSelect.addEventListener('change', function () {
        if (!membersTable) return;
        switch (this.value) {
            case 'CreatedAtDesc': membersTable.order([5, 'desc']).draw(); break;
            case 'CreatedAtAsc':  membersTable.order([5, 'asc']).draw();  break;
        }
    });
}

/* ============================================================
   加入黑名單（含選填原因）
   ============================================================ */
function initBlacklistAction() {
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-blacklist-member');
        if (!btn) return;

        const memberId   = btn.dataset.id;
        const memberName = btn.dataset.name;

        Swal.fire({
            title: '確認加入黑名單？',
            html: `<p class="mb-3">將 <strong>${memberName}</strong> 加入黑名單後，該帳號將立即無法登入前台網站。</p>`,
            input: 'text',
            inputPlaceholder: '請輸入原因，例如：多次惡意取消訂位',
            inputAttributes: { maxlength: 200 },
            showCancelButton: true,
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            reverseButtons: true,
            inputValidator: () => null  // 原因為選填，不強制
        }).then(async result => {
            if (!result.isConfirmed) return;

            const reason = result.value?.trim() || null;

            try {
                const res = await apiFetch(`/Members/Blacklist/${memberId}`, {
                    method: 'PATCH',
                    body: JSON.stringify({ reason })
                });
                if (!res) return;

                if (res.ok) {
                    Swal.fire({ icon: 'success', title: '已加入黑名單', timer: 1500, showConfirmButton: false })
                        .then(() => window.location.reload());
                } else {
                    const data = await res.json();
                    Swal.fire({ icon: 'error', title: '操作失敗', text: data?.message || '請稍後再試', confirmButtonColor: '#1A0D08' });
                }
            } catch {
                Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
            }
        });
    });
}

/* ============================================================
   解除黑名單
   ============================================================ */
function initUnblacklistAction() {
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-unblacklist-member');
        if (!btn) return;

        const memberId   = btn.dataset.id;
        const memberName = btn.dataset.name;

        Swal.fire({
            title: '確認解除黑名單？',
            html: `<p class="mb-0">解除 <strong>${memberName}</strong> 的黑名單限制後，該會員帳號將可正常登入前台網站。</p>`,
            showCancelButton: true,
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            confirmButtonColor: '#198754',
            cancelButtonColor: '#6c757d',
            reverseButtons: true
        }).then(async result => {
            if (!result.isConfirmed) return;

            try {
                const res = await apiFetch(`/Members/Unblacklist/${memberId}`, { method: 'PATCH' });
                if (!res) return;

                if (res.ok) {
                    Swal.fire({ icon: 'success', title: '已解除黑名單', timer: 1500, showConfirmButton: false })
                        .then(() => window.location.reload());
                } else {
                    const data = await res.json();
                    Swal.fire({ icon: 'error', title: '操作失敗', text: data?.message || '請稍後再試', confirmButtonColor: '#1A0D08' });
                }
            } catch {
                Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
            }
        });
    });
}

/* ============================================================
   查看詳情 Modal
   ============================================================ */
function initDetailModal() {
    const modal = document.querySelector('#modal-member-detail');
    if (!modal) return;

    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-detail-member');
        if (!btn) return;

        const row = btn.closest('tr');

            const status          = row?.dataset.status          ?? '';
            const blacklistReason = row?.dataset.blacklistReason ?? '';
            const deletedAt       = row?.dataset.deletedAt       ?? '';

            // 填入唯讀欄位
            const set = (id, val) => {
                const el = document.querySelector(id);
                if (el) el.textContent = val || '—';
            };

            set('#detail-name',       row?.dataset.name);
            set('#detail-account',    row?.dataset.account);
            set('#detail-email',      row?.dataset.email);
            set('#detail-phone',      row?.dataset.phone);
            set('#detail-birthdate',  row?.dataset.birthdate);
            set('#detail-created-at', row?.dataset.createdAt);
            set('#detail-deleted-at', deletedAt || '');

            // 狀態標籤
            const statusEl = document.querySelector('#detail-status');
            if (statusEl) {
                const statusMap = {
                    active:      { text: '啟用中', cls: 'success' },
                    unverified:  { text: '未驗證', cls: 'warning' },
                    blacklisted: { text: '黑名單', cls: 'danger' },
                    deleted:     { text: '已刪除', cls: 'secondary' }
                };
                const s = statusMap[status] ?? { text: '—', cls: 'secondary' };
                statusEl.innerHTML = `<span class="badge bg-${s.cls}">${s.text}</span>`;
            }

            // 黑名單原因（僅黑名單狀態顯示）
            const reasonRow = document.querySelector('#detail-reason-row');
            const reasonEl  = document.querySelector('#detail-blacklist-reason');
            if (reasonRow && reasonEl) {
                if (status === 'blacklisted') {
                    reasonRow.style.display = '';
                    reasonEl.textContent = blacklistReason || '（未填寫）';
                    reasonEl.className   = blacklistReason ? 'detail-value blacklist-reason' : 'detail-value text-muted fst-italic';
                } else {
                    reasonRow.style.display = 'none';
                }
            }

            // 刪除時間（非已刪除時顯示空白）
            const deletedAtEl = document.querySelector('#detail-deleted-at');
            if (deletedAtEl) {
                deletedAtEl.textContent = status === 'deleted' && deletedAt ? deletedAt : '—';
            }

            bootstrap.Modal.getOrCreateInstance(modal).show();
    });
}

/* ============================================================
   DOMContentLoaded — 初始化入口
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {
    initDataTable();
    initSearch();
    initSortDropdown();
    initBlacklistAction();
    initUnblacklistAction();
    initDetailModal();
});
