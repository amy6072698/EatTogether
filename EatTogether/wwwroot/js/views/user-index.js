/**
 * EatTogether（義起吃）後台管理系統
 * user-index.js — 員工管理列表頁 JavaScript
 *
 * 規範：
 * - DOM 選取一律使用 querySelector / querySelectorAll
 * - AJAX 一律透過 apiFetch 封裝
 * - DataTables 初始化使用 jQuery API（DataTables 為 jQuery plugin，$() 限此處使用）
 * - 按鈕事件一律使用 document 層級 event delegation，確保 DataTables 重繪後仍有效
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

// 在填入 createdAt 之前，加入此格式化函式
function formatDateTime(isoStr) {
    if (!isoStr) return '';
    const d = new Date(isoStr);
    const pad = n => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())} ${pad(d.getHours())}:${pad(d.getMinutes())}:${pad(d.getSeconds())}`;
}

/* ============================================================
   全域變數
   ============================================================ */
let userTable = null;

/* ============================================================
   DataTables 初始化
   ============================================================ */
function initDataTable() {
    userTable = $('#user-table').DataTable({
        language: {
            url: 'https://cdn.datatables.net/plug-ins/2.3.7/i18n/zh-HANT.json'
        },
        pageLength: 10,
        ordering: true,             // 開啟排序功能（供程式控制）
        order: [[5, 'desc']],       // 預設：到職日期最新
        searching: true,
        dom: "<'row'<'col-12'tr>>" +
            "<'row align-items-center mt-2'<'col-auto'i><'col'p>>",
        columnDefs: [
            // 停用欄位標題點擊排序，但保留第 5（到職日期）、第 6（建立時間）欄可程式控制排序
            { orderable: false, targets: [0, 1, 2, 3, 4, 7, 8, 9] },
            { orderable: true,  targets: [5, 6] }
        ]
    });

    // 在職篩選自訂函式（data[8] = 狀態欄，index 8）
    $.fn.dataTable.ext.search.push(function (settings, data) {
        const chk = document.querySelector('#search-active-only');
        if (!chk || !chk.checked) return true;
        return data[8] && data[8].includes('在職');
    });
}

/* ============================================================
   搜尋列
   ============================================================ */
function initSearch() {
    const btnSearch = document.querySelector('#btn-search');
    const chkActiveOnly = document.querySelector('#search-active-only');

    if (chkActiveOnly) {
        chkActiveOnly.addEventListener('change', function () {
            if (userTable) userTable.draw();
        });
    }

    if (btnSearch) {
        btnSearch.addEventListener('click', function () {
            const empNo   = document.querySelector('#search-emp-no')?.value.trim()   ?? '';
            const name    = document.querySelector('#search-name')?.value.trim()     ?? '';
            const account = document.querySelector('#search-account')?.value.trim()  ?? '';
            const email   = document.querySelector('#search-email')?.value.trim()    ?? '';

            if (userTable) {
                userTable.column(0).search(empNo)
                    .column(1).search(name)
                    .column(2).search(account)
                    .column(3).search(email)
                    .draw();
            }
        });
    }
}

/* ============================================================
   排序下拉（程式控制 DataTables 欄位排序）
   ============================================================ */
function initSortDropdown() {
    const sortSelect = document.querySelector('#sort-select');
    if (!sortSelect) return;

    sortSelect.addEventListener('change', function () {
        if (!userTable) return;
        switch (this.value) {
            case 'HireDateDesc':  userTable.order([5, 'desc']).draw(); break;
            case 'HireDateAsc':   userTable.order([5, 'asc']).draw();  break;
            case 'CreatedAtDesc': userTable.order([6, 'desc']).draw(); break;
        }
    });
}

/* ============================================================
   密碼顯示切換（event delegation — Modal 動態內容安全）
   ============================================================ */
function initPasswordToggles() {
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.toggle-password');
        if (!btn) return;

        const targetId = btn.dataset.target;
        const input = document.querySelector(`#${targetId}`);
        if (!input) return;

        const isPassword = input.type === 'password';
        input.type = isPassword ? 'text' : 'password';
        const icon = btn.querySelector('i');
        if (icon) icon.className = isPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye';
    });
}

/* ============================================================
   Flatpickr 日期選擇器初始化
   ============================================================ */
function initDatepickers() {
    const datepickerConfig = {
        dateFormat: 'Y-m-d',
        locale: 'zh_tw',
        allowInput: true,
        disableMobile: false
    };
    if (typeof flatpickr === 'function') {
        const createDate = document.querySelector('#create-hire-date');
        const editDate   = document.querySelector('#edit-hire-date');
        if (createDate) flatpickr(createDate, datepickerConfig);
        if (editDate)   flatpickr(editDate, datepickerConfig);
    }
}

/* ============================================================
   「同員工編號」勾選快速帶入
   prefix: 'create' | 'edit'
   ============================================================ */
function initSameEmpNoCheckboxes(prefix) {
    const empNoField = document.querySelector(`#${prefix}-user-number`);
    const acctCheck  = document.querySelector(`#${prefix}-account-same-empno`);
    const acctField  = document.querySelector(`#${prefix}-account`);
    const pwdCheck   = document.querySelector(`#${prefix}-password-same-empno`);
    const pwdField   = document.querySelector(`#${prefix}-password`);

    if (acctCheck && acctField && empNoField) {
        acctCheck.addEventListener('change', function () {
            if (this.checked) {
                acctField.value = empNoField.value;
                acctField.readOnly = true;
            } else {
                acctField.value = '';
                acctField.readOnly = false;
            }
        });
    }

    if (pwdCheck && pwdField && empNoField) {
        pwdCheck.addEventListener('change', function () {
            if (this.checked) {
                pwdField.value = empNoField.value;
                pwdField.readOnly = true;
            } else {
                pwdField.value = '';
                pwdField.readOnly = false;
            }
        });
    }
}

/* ============================================================
   密碼複雜度驗證
   ============================================================ */
function validatePassword(password) {
    if (!password || password.length < 6) return '密碼至少需要 6 個字元';
    if (!/[a-zA-Z]/.test(password))       return '密碼需包含英文字母（a-z / A-Z）';
    if (!/[0-9]/.test(password))           return '密碼需包含數字（0-9）';
    return null;
}

/* ============================================================
   欄位錯誤提示工具
   ============================================================ */
function showFieldError(inputEl, message) {
    if (!inputEl) return;
    inputEl.classList.add('is-invalid');

    // 優先找外層 col div 預先放好的 invalid-feedback（避免 flex 排版問題）
    const colEl = inputEl.closest('.col-md-6, .col-md-4, .col-md-3, .col-md-2, .col-12');
    let errorEl = colEl?.querySelector('.invalid-feedback');

    // fallback：找 parentElement 內的 invalid-feedback
    if (!errorEl) {
        errorEl = inputEl.parentElement.querySelector('.invalid-feedback');
    }

    // 最後手段：動態建立（無預先放置時）
    if (!errorEl) {
        errorEl = document.createElement('div');
        errorEl.className = 'invalid-feedback';
        inputEl.parentElement.appendChild(errorEl);
    }

    errorEl.textContent = message;
    errorEl.style.display = 'block';

    //let errorEl = inputEl.closest('.input-with-check, .position-relative, div')?.querySelector('.invalid-feedback');
    //if (!errorEl || errorEl.closest('.input-with-check')) {
    //    errorEl = inputEl.parentElement.querySelector('.invalid-feedback');
    //}
    //if (!errorEl) {
    //    errorEl = document.createElement('div');
    //    errorEl.className = 'invalid-feedback';
    //    inputEl.parentElement.appendChild(errorEl);
    //}
    //errorEl.textContent = message;
}

function clearAllFieldErrors(formEl) {
    if (!formEl) return;
    formEl.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
    formEl.querySelectorAll('.invalid-feedback').forEach(el => {
        el.textContent = '';
        el.style.display = '';
    });
    const roleErr = formEl.querySelector('#role-error, #edit-role-error');
    if (roleErr) roleErr.textContent = '';

    //if (!formEl) return;
    //formEl.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
    //formEl.querySelectorAll('.invalid-feedback').forEach(el => el.textContent = '');
    //const roleErr = formEl.querySelector('#role-error, #edit-role-error');
    //if (roleErr) roleErr.textContent = '';
}

/* ============================================================
   新增員工 Modal
   ============================================================ */
function initCreateModal() {
    const modal      = document.querySelector('#modal-create-user');
    const form       = document.querySelector('#form-create-user');
    const submitBtn  = document.querySelector('#btn-create-user');
    const empNoField = document.querySelector('#create-user-number');

    if (!modal) return;

    modal.addEventListener('show.bs.modal', async function () {
        clearAllFieldErrors(form);
        if (form) form.reset();

        // 重設同員工編號勾選框與 readOnly
        ['#create-account-same-empno', '#create-password-same-empno'].forEach(sel => {
            const cb = document.querySelector(sel);
            if (cb) cb.checked = false;
        });
        const acctField = document.querySelector('#create-account');
        const pwdField  = document.querySelector('#create-password');
        if (acctField) acctField.readOnly = false;
        if (pwdField)  pwdField.readOnly  = false;

        

        // 呼叫後端取得預覽編號
        try {
            const res = await apiFetch('/Users/NextEmployeeNumber');
            if (res && res.ok) {
                const data = await res.json();
                if (empNoField) empNoField.value = data.employeeNumber;
            }
        } catch {
            if (empNoField) empNoField.value = '（取得失敗，請關閉重試）';
        }

    });

    initSameEmpNoCheckboxes('create');

    if (!submitBtn || !form) return;

    submitBtn.addEventListener('click', async function () {
        clearAllFieldErrors(form);

        const name     = document.querySelector('#create-name');
        const status   = document.querySelector('#create-status');
        const account  = document.querySelector('#create-account');
        const pwd      = document.querySelector('#create-password');
        const email    = document.querySelector('#create-email');
        const phone    = document.querySelector('#create-phone');
        const hireDate = document.querySelector('#create-hire-date');
        const checkedRoles = form.querySelectorAll('.role-checkbox:checked');

        let hasError = false;

        if (!name?.value.trim())    { showFieldError(name, '請輸入姓名');   hasError = true; }
        if (!account?.value.trim()) { showFieldError(account, '請輸入帳號'); hasError = true; }

        const pwdError = validatePassword(pwd?.value);
        if (pwdError) { showFieldError(pwd, pwdError); hasError = true; }

        if (checkedRoles.length === 0) {
            let errEl = form.querySelector('#role-error');
            if (!errEl) {
                errEl = document.createElement('div');
                errEl.id = 'role-error';
                errEl.className = 'text-danger mt-1';
                errEl.style.fontSize = '0.82rem';
                form.querySelector('.role-checkbox-grid')?.parentElement.appendChild(errEl);
            }
            errEl.textContent = '請至少選擇一個角色';
            hasError = true;
        }

        if (hasError) return;

        const payload = {
            name:     name.value.trim(),
            isActive: status?.value === '1',
            account:  account.value.trim(),
            password: pwd.value,
            email: email?.value.trim() ?? '',
            phone: phone?.value.trim() ?? '',
            hireDate: hireDate?.value ?? '',
            roleIds:  Array.from(checkedRoles).map(cb => parseInt(cb.value))
        };

        submitBtn.disabled = true;
        submitBtn.textContent = '新增中...';

        try {
            const res = await apiFetch('/Users/Create', { method: 'POST', body: JSON.stringify(payload) });
            if (!res) return;

            if (res.ok) {
                bootstrap.Modal.getInstance(modal)?.hide();
                Swal.fire({
                    icon: 'success', title: '新增成功',
                    text: '員工帳號已建立，請口頭通知員工初始密碼。',
                    confirmButtonText: '確認', confirmButtonColor: '#1A0D08'
                }).then(() => window.location.reload());
            } else {
                const data = await res.json();
                Swal.fire({ icon: 'error', title: '新增失敗', text: data?.message || '請稍後再試', confirmButtonColor: '#1A0D08' });
            }
        } catch {
            Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
        } finally {
            submitBtn.disabled = false;
            submitBtn.textContent = '新增';
        }
    });
}

/* ============================================================
   編輯員工 Modal（event delegation — 確保 DataTables 重繪後有效）
   ============================================================ */
function initEditModal() {
    const modal = document.querySelector('#modal-edit-user');
    if (!modal) return;

    document.addEventListener('click', async function (e) {
        const btn = e.target.closest('.btn-edit-user');
        if (!btn) return;

        const userId = btn.dataset.id;

        clearAllFieldErrors(document.querySelector('#form-edit-user'));

        // 呼叫後端取得預填資料 GET /Users/Edit/${userId}
        try {
            const res = await apiFetch(`/Users/Edit/${userId}`);

            if (!res) return;

            if (!res.ok) {
                Swal.fire({ icon: 'error', title: '載入失敗', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
                return;
            }

            const data = await res.json();

            document.querySelector('#edit-user-id').value = data.id;
            document.querySelector('#edit-user-number').value = data.employeeNumber;
            document.querySelector('#edit-name').value = data.name;
            document.querySelector('#edit-status').value = data.isActive ? '1' : '0';
            document.querySelector('#edit-account').value = data.account;
            document.querySelector('#edit-password').value = '';
            document.querySelector('#edit-email').value = data.email;
            document.querySelector('#edit-phone').value = data.phone;
            document.querySelector('#edit-hire-date').value = data.hireDate;
            document.querySelector('#edit-created-at').value = formatDateTime(data.createdAt);

            // 角色預填
            document.querySelectorAll('#modal-edit-user .role-checkbox').forEach(cb => {
                cb.checked = data.roleIds.includes(parseInt(cb.value));
            });

            // 重設同員工編號勾選框與 readOnly
            ['#edit-account-same-empno', '#edit-password-same-empno'].forEach(sel => {
                const cb = document.querySelector(sel);
                if (cb) cb.checked = false;
            });
            const acctField = document.querySelector('#edit-account');
            const pwdField = document.querySelector('#edit-password');
            if (acctField) acctField.readOnly = false;
            if (pwdField) pwdField.readOnly = false;

            bootstrap.Modal.getOrCreateInstance(modal).show();
        }
        catch {
            Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
        }

        
    });

    initSameEmpNoCheckboxes('edit');

    const submitBtn = document.querySelector('#btn-save-user');
    const form      = document.querySelector('#form-edit-user');
    if (!submitBtn || !form) return;

    submitBtn.addEventListener('click', async function () {
        clearAllFieldErrors(form);

        const userId   = document.querySelector('#edit-user-id')?.value;
        const name     = document.querySelector('#edit-name');
        const status = document.querySelector('#edit-status');
        const account = document.querySelector('#edit-account');
        const pwd      = document.querySelector('#edit-password');
        const email    = document.querySelector('#edit-email');
        const phone    = document.querySelector('#edit-phone');
        const hireDate = document.querySelector('#edit-hire-date');
        const checkedRoles = form.querySelectorAll('.role-checkbox:checked');

        let hasError = false;

        if (!name?.value.trim()) { showFieldError(name, '請輸入姓名'); hasError = true; }
        if (!account?.value.trim()) { showFieldError(account, '請輸入帳號'); hasError = true; }
        if (!email?.value.trim()) { showFieldError(email, '請輸入 Email'); hasError = true; }
        if (!phone?.value.trim()) { showFieldError(phone, '請輸入手機號碼'); hasError = true; }
        if (!hireDate?.value.trim()) { showFieldError(hireDate, '請選擇到職日期'); hasError = true; }

        // 密碼有填才驗證複雜度
        if (pwd?.value) {
            const pwdError = validatePassword(pwd.value);
            if (pwdError) { showFieldError(pwd, pwdError); hasError = true; }
        }

        if (checkedRoles.length === 0) {
            let errEl = form.querySelector('#edit-role-error');
            if (!errEl) {
                errEl = document.createElement('div');
                errEl.id = 'edit-role-error';
                errEl.className = 'text-danger mt-1';
                errEl.style.fontSize = '0.82rem';
                form.querySelector('.role-checkbox-grid')?.parentElement.appendChild(errEl);
            }
            errEl.textContent = '請至少選擇一個角色';
            hasError = true;
        }

        if (hasError) return;

        const payload = {
            name:     name.value.trim(),
            isActive: status?.value === '1',
            account: account.value.trim(),
            password: pwd?.value || null,
            email:    email?.value.trim() || null,
            phone:    phone?.value.trim() || null,
            hireDate: hireDate?.value || null,
            roleIds:  Array.from(checkedRoles).map(cb => parseInt(cb.value))
        };

        submitBtn.disabled = true;
        submitBtn.textContent = '儲存中...';

        try {
            const res = await apiFetch(`/Users/Edit/${userId}`, {
                method: 'PUT',
                body: JSON.stringify(payload)
            });

            if (!res) return;

            if (res.ok) {
                bootstrap.Modal.getInstance(modal)?.hide();
                Swal.fire({
                    icon: 'success', title: '儲存成功', text: '員工資料已更新。',
                    confirmButtonText: '確認', confirmButtonColor: '#1A0D08'
                }).then(() => window.location.reload());
            } else {
                const data = await res.json();
                Swal.fire({ icon: 'error', title: '儲存失敗', text: data?.message || '請稍後再試', confirmButtonColor: '#1A0D08' });
            }
        } catch {
            Swal.fire({ icon: 'error', title: '系統錯誤', text: '請稍後再試', confirmButtonColor: '#1A0D08' });
        } finally {
            submitBtn.disabled = false;
            submitBtn.textContent = '儲存變更';
        }
    });
}

/* ============================================================
   離職處理（event delegation）
   ============================================================ */
function initResignAction() {
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-resign-user');
        if (!btn) return;

        const userId  = btn.dataset.id;
        const empName = btn.dataset.name;
        const empNo   = btn.dataset.empNo;

        Swal.fire({
            icon: 'warning',
            title: '確認離職處理？',
            html: `<p class="mb-0"><strong>${empName}（${empNo}）</strong>的帳號將立即無法登入後台系統，請確認後再執行。</p>`,
            showCancelButton: true,
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d',
            reverseButtons: true
        }).then(async result => {
            if (!result.isConfirmed) return;
            try {
                const res = await apiFetch(`/Users/Resign/${userId}`, { method: 'PATCH' });
                if (!res) return;
                if (res.ok) {
                    Swal.fire({ icon: 'success', title: '已完成離職處理', timer: 1500, showConfirmButton: false })
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
   復職處理（event delegation）
   ============================================================ */
function initReinstateAction() {
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('.btn-reinstate-user');
        if (!btn) return;

        const userId  = btn.dataset.id;
        const empName = btn.dataset.name;
        const empNo   = btn.dataset.empNo;

        Swal.fire({
            icon: 'warning',
            title: '確認復職處理？',
            html: `<p class="mb-0"><strong>${empName}（${empNo}）</strong>的帳號將立即恢復登入權限。</p>`,
            showCancelButton: true,
            confirmButtonText: '確認',
            cancelButtonText: '取消',
            confirmButtonColor: '#198754',
            cancelButtonColor: '#6c757d',
            reverseButtons: true
        }).then(async result => {
            if (!result.isConfirmed) return;
            try {
                const res = await apiFetch(`/Users/Reinstate/${userId}`, { method: 'PATCH' });
                if (!res) return;
                if (res.ok) {
                    Swal.fire({ icon: 'success', title: '已完成復職處理', timer: 1500, showConfirmButton: false })
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
   新增按鈕開啟 Modal
   ============================================================ */
function initOpenCreateModal() {
    const btn   = document.querySelector('#btn-open-create');
    const modal = document.querySelector('#modal-create-user');
    if (btn && modal) {
        btn.addEventListener('click', function () {
            bootstrap.Modal.getOrCreateInstance(modal).show();
        });
    }
}

/* ============================================================
   DOMContentLoaded — 初始化入口
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {
    initDataTable();
    initSearch();
    initSortDropdown();
    initPasswordToggles();
    initDatepickers();
    initOpenCreateModal();
    initCreateModal();
    initEditModal();
    initResignAction();
    initReinstateAction();
});
