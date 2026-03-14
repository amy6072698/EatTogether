/**
 * EatTogether（義起吃）後台管理系統
 * roles-index.js — 角色與權限管理列表頁 JavaScript
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
let rolesTable = null;  // DataTables instance

/* ============================================================
   DataTables 初始化（無分頁，全部角色一次顯示）
   ============================================================ */
function initDataTable() {
    rolesTable = $('#roles-table').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/2.3.7/i18n/zh-HANT.json'
        },
        paging: 10,
        ordering: true,
        searching: true,
        dom: "<'row'<'col-12'tr>>" +
            "<'row align-items-center mt-2'<'col-auto'i><'col'p>>",
        columnDefs: [
            { targets: '_all', defaultContent: '' },
            { orderable: false, targets: [0, 1, 2, 3] },
            // 加這行，指定不要換行的欄位 index
            { className: 'text-nowrap', targets: [0] }  // 0 = 角色名稱欄
        ]
    });
}

/* ============================================================
   查看總覽 Modal
   ============================================================ */
function initOverviewModal() {
    const btn = document.querySelector('#btn-open-overview');
    if (!btn) return;

    btn.addEventListener('click', function () {
        const modal = document.querySelector('#modal-overview');
        if (modal) bootstrap.Modal.getOrCreateInstance(modal).show();
    });
}

/* ============================================================
   新增角色 Modal
   ============================================================ */
function initCreateModal() {
    const btnOpen = document.querySelector('#btn-open-create');
    if (!btnOpen) return;

    btnOpen.addEventListener('click', function () {
        // 清空表單
        const nameInput = document.querySelector('#create-role-name');
        const descInput = document.querySelector('#create-role-desc');
        if (nameInput) nameInput.value = '';
        if (descInput) descInput.value = '';

        // 取消所有可勾選的權限 checkbox（disabled 的跳過）
        document.querySelectorAll('#create-perm-grid input[type="checkbox"]:not(:disabled)').forEach(cb => {
            cb.checked = false;
        });

        // 取消所有員工勾選
        document.querySelectorAll('#create-employee-list input[type="checkbox"]').forEach(cb => {
            cb.checked = false;
        });

        const modal = document.querySelector('#modal-create-role');
        if (modal) bootstrap.Modal.getOrCreateInstance(modal).show();
    });

    // 新增確認按鈕
    const btnConfirm = document.querySelector('#btn-create-confirm');
    if (!btnConfirm) return;

    btnConfirm.addEventListener('click', async function () {
        const roleName = document.querySelector('#create-role-name')?.value.trim() ?? '';
        if (!roleName) {
            Swal.fire({ icon: 'warning', title: '請填寫角色名稱', confirmButtonColor: '#1A0D08' });
            return;
        }

        const roleDesc = document.querySelector('#create-role-desc')?.value.trim() ?? '';

        const selectedFunctions = [];
        document.querySelectorAll('#create-perm-grid input[name="functions"]:checked').forEach(cb => {
            selectedFunctions.push(cb.value);
        });

        const selectedEmployeeIds = [];
        document.querySelectorAll('#create-employee-list input[type="checkbox"]:checked').forEach(cb => {
            selectedEmployeeIds.push(parseInt(cb.value, 10));
        });

        try {
            const res = await apiFetch('/Role/Create', {
                method: 'POST',
                body: JSON.stringify({
                    roleName,
                    description: roleDesc,
                    functions: selectedFunctions,
                    employeeIds: selectedEmployeeIds
                })
            });
            if (!res) return;

            if (res.ok) {
                bootstrap.Modal.getInstance(document.querySelector('#modal-create-role'))?.hide();
                Swal.fire({ icon: 'success', title: '新增成功', timer: 1500, showConfirmButton: false })
                    .then(() => window.location.reload());
            } else {
                const data = await res.json();
                Swal.fire({ icon: 'error', title: '新增失敗', text: data?.message || '請稍後再試', confirmButtonColor: '#1A0D08' });
            }
        } catch {
            Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
        }
    });
}

/* ============================================================
   編輯角色 Modal
   ============================================================ */
function initEditModal() {
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-edit-role');
        if (!btn) return;

        const row = btn.closest('tr');
        if (!row) return;

        const roleId          = row.dataset.id          ?? '';
        const roleName        = row.dataset.name        ?? '';
        const roleDescription = row.dataset.description ?? '';
        const functions       = (row.dataset.functions    || '').split(',').filter(v => v);
        const employeeIds     = (row.dataset.employeeIds  || '').split(',').filter(v => v);

        // 填入基本資料
        const idInput   = document.querySelector('#edit-role-id');
        const nameInput = document.querySelector('#edit-role-name');
        const descInput = document.querySelector('#edit-role-desc');
        if (idInput)   idInput.value   = roleId;
        if (nameInput) nameInput.value = roleName;
        if (descInput) descInput.value = roleDescription;

        // 勾選/取消 權限（disabled 的跳過）
        document.querySelectorAll('#edit-perm-grid input[type="checkbox"]:not(:disabled)').forEach(cb => {
            cb.checked = functions.includes(cb.value);
        });

        // 勾選/取消 員工指派
        document.querySelectorAll('#edit-employee-list input[type="checkbox"]').forEach(cb => {
            cb.checked = employeeIds.includes(cb.value);
        });

        const modal = document.querySelector('#modal-edit-role');
        if (modal) bootstrap.Modal.getOrCreateInstance(modal).show();
    });

    // 儲存變更確認按鈕
    const btnConfirm = document.querySelector('#btn-edit-confirm');
    if (!btnConfirm) return;

    btnConfirm.addEventListener('click', async function () {
        const roleId   = document.querySelector('#edit-role-id')?.value   ?? '';
        const roleName = document.querySelector('#edit-role-name')?.value.trim() ?? '';
        if (!roleName) {
            Swal.fire({ icon: 'warning', title: '請填寫角色名稱', confirmButtonColor: '#1A0D08' });
            return;
        }

        const roleDesc = document.querySelector('#edit-role-desc')?.value.trim() ?? '';

        const selectedFunctions = [];
        document.querySelectorAll('#edit-perm-grid input[name="edit-functions"]:checked').forEach(cb => {
            selectedFunctions.push(cb.value);
        });

        const selectedEmployeeIds = [];
        document.querySelectorAll('#edit-employee-list input[type="checkbox"]:checked').forEach(cb => {
            selectedEmployeeIds.push(parseInt(cb.value, 10));
        });

        try {
            const res = await apiFetch(`/Role/Edit/${roleId}`, {
                method: 'PUT',
                body: JSON.stringify({
                    roleName,
                    description: roleDesc,
                    functions: selectedFunctions,
                    employeeIds: selectedEmployeeIds
                })
            });
            if (!res) return;

            if (res.ok) {
                bootstrap.Modal.getInstance(document.querySelector('#modal-edit-role'))?.hide();
                Swal.fire({ icon: 'success', title: '儲存成功', timer: 1500, showConfirmButton: false })
                    .then(() => window.location.reload());
            } else {
                const data = await res.json();
                Swal.fire({ icon: 'error', title: '儲存失敗', text: data?.message || '請稍後再試', confirmButtonColor: '#1A0D08' });
            }
        } catch {
            Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
        }
    });
}

/* ============================================================
   刪除角色（事件委派）
   ============================================================ */
function initDeleteAction() {
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-delete-role');
        if (!btn) return;

        const row = btn.closest('tr');
        if (!row) return;

        const roleId    = row.dataset.id        ?? '';
        const roleName  = row.dataset.name      ?? '';
        const isDefault = row.dataset.isDefault === 'true';

        if (isDefault) {
            // 預設角色不可刪除
            Swal.fire({
                icon: 'warning',
                title: `無法刪除「${roleName}」`,
                text: '此角色為系統預設角色，不允許刪除。',
                confirmButtonText: '確認',
                confirmButtonColor: '#1A0D08'
            });
            return;
        }

        // 自訂角色 → 確認後刪除
        Swal.fire({
            icon: 'warning',
            title: `確認刪除角色「${roleName}」？`,
            text: '刪除後，所有擁有此角色的員工將立即失去對應權限。此操作無法復原。',
            showCancelButton: true,
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            reverseButtons: true
        }).then(async result => {
            if (!result.isConfirmed) return;

            try {
                const res = await apiFetch(`/Role/Delete/${roleId}`, { method: 'DELETE' });
                if (!res) return;

                if (res.ok) {
                    Swal.fire({ icon: 'success', title: '已刪除', timer: 1500, showConfirmButton: false })
                        .then(() => window.location.reload());
                } else {
                    const data = await res.json();
                    Swal.fire({ icon: 'error', title: '刪除失敗', text: data?.message || '請稍後再試', confirmButtonColor: '#1A0D08' });
                }
            } catch {
                Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
            }
        });
    });
}

/* ============================================================
   DOMContentLoaded — 初始化入口
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {
    initDataTable();
    initOverviewModal();
    initCreateModal();
    initEditModal();
    initDeleteAction();
});
