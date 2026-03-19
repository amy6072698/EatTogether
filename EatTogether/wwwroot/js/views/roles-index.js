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
    const config = {
        credentials: 'include',
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...(options.headers || {})
        }
    };
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
        paging: true,
        ordering: false,
        searching: true,
        dom: "<'row'<'col-12'tr>>" +
            "<'row align-items-center mt-2'<'col-auto'i><'col'p>>",
        columnDefs: [
            { targets: '_all', defaultContent: '' },
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

    btn.addEventListener('click',async function () {
        try {
            // 向後端請求總覽資料
            const res = await apiFetch('/Roles/Overview', { method: 'GET' });
            if (!res || !res.ok) {
                Swal.fire({ icon: 'error', title: '無法載入總覽資料', confirmButtonColor: '#1A0D08' });
                return;
            }

            // TODO: 渲染總覽矩陣（如需動態渲染）
            const modal = document.querySelector('#modal-overview');
            if (modal) bootstrap.Modal.getOrCreateInstance(modal).show();
        } catch (error) {
            console.error('載入總覽資料失敗:', error);
            Swal.fire({ icon: 'error', title: '系統錯誤', confirmButtonColor: '#1A0D08' });
        }
    });
}

/* ============================================================
   新增角色 Modal
   ============================================================ */
function initCreateModal() {
    const btnOpen = document.querySelector('#btn-open-create');
    if (!btnOpen) return;

    btnOpen.addEventListener('click', async function () {
        try {
            // 向後端請求表單資料
            const res = await apiFetch('/Roles/Create', { method: 'GET' });
            if (!res || !res.ok) {
                Swal.fire({ icon: 'error', title: '無法載入表單資料', confirmButtonColor: '#1A0D08' });
                return;
            }

            const data = await res.json();

            // 清空輸入欄位
            document.querySelector('#create-role-name').value = '';
            document.querySelector('#create-role-desc').value = '';

            // 渲染權限卡片與員工清單
            renderPermissionCards('#create-perm-grid', data.allFunctions || [], []);
            renderEmployeeList('#create-employee-list', data.allUsers || [], []);

            // 顯示 Modal
            const modal = document.querySelector('#modal-create-role');
            if (modal) bootstrap.Modal.getOrCreateInstance(modal).show();
        }
        catch (error) {
            console.error('載入表單資料失敗:', error);
            Swal.fire({ icon: 'error', title: '系統錯誤', confirmButtonColor: '#1A0D08' });
        }
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
            const res = await apiFetch('/Roles/Create', {
                method: 'POST',
                body: JSON.stringify({
                    roleName,
                    description: roleDesc,
                    functionIds: selectedFunctions.map(v => parseInt(v, 10)),
                    userIds: selectedEmployeeIds
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
    document.addEventListener('click', async function (e) {
        const btn = e.target.closest('.btn-edit-role');
        if (!btn) return;

        const row = btn.closest('tr');
        if (!row) return;

        const roleId = row.dataset.id ?? '';
        if (!roleId) return;

        try {
            const res = await apiFetch(`/Roles/Edit/${roleId}`, { method: 'GET' });
            if (!res || !res.ok) {
                Swal.fire({ icon: 'error', title: '無法載入角色資料', confirmButtonColor: '#1A0D08' });
                return;
            }

            const data = await res.json();

            // 填入基本資料
            const idInput = document.querySelector('#edit-role-id');
            const nameInput = document.querySelector('#edit-role-name');
            const descInput = document.querySelector('#edit-role-desc');
            if (idInput) idInput.value = data.id;
            if (nameInput) nameInput.value = data.roleName || '';
            if (descInput) descInput.value = data.description || '';

            // 重新渲染權限卡片與員工清單
            renderPermissionCards('#edit-perm-grid', data.allFunctions || [], data.selectedFunctionIds || []);
            renderEmployeeList('#edit-employee-list', data.allUsers || [], data.selectedUserIds || []);

            // 顯示 Modal
            const modal = document.querySelector('#modal-edit-role');
            if (modal) bootstrap.Modal.getOrCreateInstance(modal).show();
        }
        catch (error) {
            console.error('載入角色資料失敗:', error);
            Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
        }
        
        
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
            const res = await apiFetch(`/Roles/Edit/${roleId}`, {
                method: 'PUT',
                body: JSON.stringify({
                    id: parseInt(roleId, 10),
                    roleName,
                    description: roleDesc,
                    functionIds: selectedFunctions.map(v => parseInt(v, 10)),
                    userIds: selectedEmployeeIds
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
   動態渲染權限卡片（根據後端回傳的 AllFunctions）
   ============================================================ */
function renderPermissionCards(containerId, allFunctions, selectedIds) {
    const container = document.querySelector(containerId);
    if (!container) return;

    container.innerHTML = '';

    // 🔥 建立 Bootstrap row 容器
    const row = document.createElement('div');
    row.className = 'row g-2';

    allFunctions.forEach((func, index) => {
        const col = document.createElement('div');
        col.className = 'col-md-6';  // 每個卡片佔 50% 寬度

        const isOwnerOnly = func.isOwnerOnly;
        const isChecked = selectedIds.includes(func.id);

        // 根據不同 containerId 使用不同的 name 和 id 前綴
        const prefix = containerId.includes('create') ? 'create' : 'edit';
        const inputName = containerId.includes('create') ? 'functions' : 'edit-functions';

        col.innerHTML = `
            <div class="perm-card ${isOwnerOnly ? 'owner-only' : ''}">
                <div class="form-check">
                    <input type="checkbox" 
                           class="form-check-input" 
                           id="${prefix}-perm-${index}"
                           name="${inputName}" 
                           value="${func.id}"
                           ${isChecked ? 'checked' : ''}
                           ${isOwnerOnly ? 'disabled' : ''} />
                    <label class="form-check-label" for="${prefix}-perm-${index}">
                        <div class="d-flex align-items-center">
                            <span class="perm-card-name">${func.displayName}</span>
                            ${isOwnerOnly ? '<span class="owner-only-tag ms-2">僅限店長</span>' : ''}
                        </div>
                        <span class="perm-card-desc">${func.description || ''}</span>
                    </label>
                </div>
            </div>
        `;

        row.appendChild(col);
    });

    container.appendChild(row);
}

/* ============================================================
   動態渲染員工清單（根據後端回傳的 AllUsers）
   ============================================================ */
function renderEmployeeList(containerId, allUsers, selectedIds) {
    const container = document.querySelector(containerId);
    if (!container) return;

    container.innerHTML = '';

    allUsers.forEach((user, index) => {
        const isChecked = selectedIds.includes(user.id);
        const statusBadge = user.isActive
            ? '<span class="badge bg-success ms-1">在職</span>'
            : '<span class="badge bg-warning ms-1">請假</span>';

        // 根據不同 containerId 使用不同的 id 前綴
        const prefix = containerId.includes('create') ? 'create' : 'edit';

        const item = document.createElement('div');
        item.className = 'employee-assign-item';
        item.innerHTML = `
            <div class="form-check">
                <input type="checkbox" 
                       class="form-check-input me-2" 
                       id="${prefix}-emp-${index}" 
                       value="${user.id}"
                       ${isChecked ? 'checked' : ''} />
                <label class="form-check-label" for="${prefix}-emp-${index}">
                    <span class="emp-name">${user.name}</span>
                    <span class="emp-no">${user.employeeNumber}</span>
                    <span class="emp-account">· ${user.account}</span>
                    ${statusBadge}
                </label>
            </div>
        `;

        container.appendChild(item);
    });
}

/* ============================================================
   刪除角色（事件委派）
   ============================================================ */
function initDeleteAction() {
    document.addEventListener('click',async function (e) {
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
                const res = await apiFetch(`/Roles/Delete/${roleId}`, { method: 'DELETE' });
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
