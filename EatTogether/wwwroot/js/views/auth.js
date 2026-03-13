/**
 * EatTogether（義起吃）後台管理系統
 * auth.js — 登入相關頁面共用 JavaScript
 *
 * 規範：
 * - DOM 選取一律使用 querySelector / querySelectorAll
 * - AJAX 一律透過 apiFetch 封裝
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

        // 401 自動導向登入頁
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
   顯示 / 隱藏密碼切換
   ============================================================ */
function initPasswordToggles() {
    document.querySelectorAll('.toggle-password').forEach(btn => {
        btn.addEventListener('click', function () {
            const targetId = this.dataset.target;
            const input = document.querySelector(`#${targetId}`);
            if (!input) return;

            const isPassword = input.type === 'password';
            input.type = isPassword ? 'text' : 'password';

            const icon = this.querySelector('i');
            if (icon) {
                icon.className = isPassword ? 'fa-solid fa-eye-slash' : 'fa-solid fa-eye';
            }
        });
    });
}

/* ============================================================
   密碼強度規則即時檢查
   ============================================================ */
function initPasswordRules(inputId) {
    const input = document.querySelector(`#${inputId}`);
    if (!input) return;

    input.addEventListener('input', function () {
        const val = this.value;

        const ruleLength = document.querySelector('[data-rule="length"]');
        const ruleAlpha  = document.querySelector('[data-rule="alpha"]');
        const ruleNum    = document.querySelector('[data-rule="num"]');

        if (ruleLength) toggleRule(ruleLength, val.length >= 6);
        if (ruleAlpha)  toggleRule(ruleAlpha,  /[a-zA-Z]/.test(val));
        if (ruleNum)    toggleRule(ruleNum,     /[0-9]/.test(val));
    });
}

function toggleRule(el, met) {
    const icon = el.querySelector('i');
    if (met) {
        el.classList.add('met');
        if (icon) icon.className = 'fa-solid fa-check';
    } else {
        el.classList.remove('met');
        if (icon) icon.className = 'fa-regular fa-circle';
    }
}

/* ============================================================
   密碼複雜度驗證
   ============================================================ */
function validatePassword(password) {
    if (password.length < 6) return '密碼至少需要 6 個字元';
    if (!/[a-zA-Z]/.test(password)) return '密碼需包含英文字母';
    if (!/[0-9]/.test(password)) return '密碼需包含數字';
    return null; // 通過
}

/* ============================================================
   顯示 / 隱藏錯誤訊息
   ============================================================ */
function showFieldError(inputId, message) {
    const input = document.querySelector(`#${inputId}`);
    const errorEl = document.querySelector(`#${inputId}-error`);

    if (input) input.classList.add('is-invalid');
    if (errorEl) {
        errorEl.textContent = message;
        errorEl.classList.add('show');
    }
}

function clearFieldError(inputId) {
    const input = document.querySelector(`#${inputId}`);
    const errorEl = document.querySelector(`#${inputId}-error`);

    if (input) input.classList.remove('is-invalid');
    if (errorEl) errorEl.classList.remove('show');
}

function clearAllErrors() {
    document.querySelectorAll('.auth-input').forEach(el => el.classList.remove('is-invalid'));
    document.querySelectorAll('.auth-input-error').forEach(el => el.classList.remove('show'));
}

/* ============================================================
   顯示 / 隱藏警示卡
   ============================================================ */
function showAlert(type, message) {
    const alertEl = document.querySelector('#auth-alert');
    if (!alertEl) return;

    alertEl.className = `auth-alert ${type} show`;
    const msgEl = alertEl.querySelector('.alert-msg');
    if (msgEl) msgEl.textContent = message;
}

function hideAlert() {
    const alertEl = document.querySelector('#auth-alert');
    if (alertEl) alertEl.classList.remove('show');
}

/* ============================================================
   按鈕載入狀態
   ============================================================ */
function setButtonLoading(btnId, loading, originalText) {
    const btn = document.querySelector(`#${btnId}`);
    if (!btn) return;

    if (loading) {
        btn.disabled = true;
        btn.classList.add('btn-loading');
        btn.innerHTML = `<span class="spinner-sm"></span>處理中...`;
    } else {
        btn.disabled = false;
        btn.classList.remove('btn-loading');
        btn.innerHTML = originalText;
    }
}

/* ============================================================
   登入頁面邏輯
   ============================================================ */
function initLoginPage() {
    const loginForm = document.querySelector('#login-form');
    if (!loginForm) return;

    initPasswordToggles();

    // Demo 帳號快速填入
    document.querySelectorAll('.demo-role-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const account  = this.dataset.account;
            const password = this.dataset.password;

            const accountInput  = document.querySelector('#account');
            const passwordInput = document.querySelector('#password');

            if (accountInput)  accountInput.value  = account;
            if (passwordInput) passwordInput.value = password;

            // 高亮顯示已選擇的按鈕
            document.querySelectorAll('.demo-role-btn').forEach(b => b.style.background = '');
            this.style.background = 'rgba(232, 114, 42, 0.2)';
            this.style.borderColor = 'rgba(232, 114, 42, 0.5)';

            clearAllErrors();
            hideAlert();
        });
    });

    // 輸入時清除錯誤
    document.querySelectorAll('#account, #password').forEach(input => {
        input.addEventListener('input', function () {
            clearFieldError(this.id);
            hideAlert();
        });
    });

    // 表單送出
    loginForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        const account  = document.querySelector('#account')?.value.trim();
        const password = document.querySelector('#password')?.value;

        // 前端基本驗證
        let hasError = false;
        if (!account) {
            showFieldError('account', '請輸入帳號');
            hasError = true;
        }
        if (!password) {
            showFieldError('password', '請輸入密碼');
            hasError = true;
        }
        if (hasError) return;

        setButtonLoading('login-btn', true, '登入');

        try {
            const response = await apiFetch('/Auth/Login', {
                method: 'POST',
                body: JSON.stringify({ account, password })
            });

            if (!response) return; // 401 已自動導向

            const data = await response.json();

            if (response.ok && data.success) {
                // 強制改密碼 → 開 Modal
                if (data.mustChangePassword) {
                    setButtonLoading('login-btn', false, '登入');
                    const forceModalEl = document.querySelector('#forceChangePasswordModal');
                    const forceModal = bootstrap.Modal.getInstance(forceModalEl)
                                    ?? new bootstrap.Modal(forceModalEl, { backdrop: 'static', keyboard: false });
                    forceModal.show();
                } else {
                    window.location.href = data.redirectUrl || '/Dashboard';
                }
            } else {
                showAlert('danger', data.message || '帳號或密碼錯誤');
                setButtonLoading('login-btn', false, '登入');
            }
        } catch (error) {
            showAlert('danger', '系統發生錯誤，請稍後再試');
            setButtonLoading('login-btn', false, '登入');
        }
    });

}

/* ============================================================
   強制改密碼 Modal（登入頁內）
   ============================================================ */
function initForceChangePasswordModal() {
    const modalEl     = document.querySelector('#forceChangePasswordModal');
    const submitBtn   = document.querySelector('#force-change-btn');
    const newPwdInput = document.querySelector('#force-new-password');
    const confirmInput = document.querySelector('#force-confirm-password');

    if (!modalEl || !submitBtn || !newPwdInput || !confirmInput) return;

    // 初始化密碼顯示切換和規則檢查
    initPasswordToggles();

    // 確認密碼即時比對
    confirmInput.addEventListener('input', function () {
        if (this.value && this.value !== newPwdInput.value) {
            showForceFieldError('force-confirm-password', '兩次密碼不一致');
        } else {
            clearFieldError('force-confirm-password');
        }
    });

    // 送出按鈕
    submitBtn.addEventListener('click', async function () {
        const newPwd     = newPwdInput.value;
        const confirmPwd = confirmInput.value;

        // 清除舊錯誤
        clearFieldError('force-new-password');
        clearFieldError('force-confirm-password');
        const alertEl = document.querySelector('#force-change-alert');
        if (alertEl) alertEl.classList.remove('show');

        let hasError = false;

        const pwdError = validatePassword(newPwd || '');
        if (pwdError) {
            showForceFieldError('force-new-password', pwdError);
            hasError = true;
        }

        if (!confirmPwd) {
            showForceFieldError('force-confirm-password', '請確認密碼');
            hasError = true;
        } else if (newPwd !== confirmPwd) {
            showForceFieldError('force-confirm-password', '兩次密碼不一致');
            hasError = true;
        }

        if (hasError) return;

        setButtonLoading('force-change-btn', true, '確定重設');

        try {
            const response = await apiFetch('/Auth/ForceChangePassword', {
                method: 'POST',
                body: JSON.stringify({ newPassword: newPwd, confirmPassword: confirmPwd })
            });

            if (!response) return;

            const data = await response.json();

            if (response.ok && data.success) {
                // 關閉 Modal，SweetAlert2 顯示成功
                const bsModal = bootstrap.Modal.getInstance(modalEl);
                if (bsModal) bsModal.hide();

                Swal.fire({
                    icon: 'success',
                    title: '密碼重設完成',
                    html: '<p>您的密碼已成功更新，<br>即將進入後台系統...</p>',
                    confirmButtonText: '進入系統',
                    confirmButtonColor: '#d4b84a',
                    allowOutsideClick: false,
                    timer: 5000,
                    timerProgressBar: true
                }).then(() => {
                    window.location.href = data.redirectUrl || '/Dashboard';
                });
            } else {
                showForceModalAlert(data.message || '密碼重設失敗，請再試一次');
                setButtonLoading('force-change-btn', false, '確定重設');
            }
        } catch (error) {
            showForceModalAlert('系統發生錯誤，請稍後再試');
            setButtonLoading('force-change-btn', false, '確定重設');
        }
    });
}

/* 強制改密碼 Modal 專用輔助函式 */
function showForceFieldError(fieldId, message) {
    const input   = document.querySelector(`#${fieldId}`);
    const errorEl = document.querySelector(`#${fieldId}-error`);
    if (input)   input.classList.add('is-invalid');
    if (errorEl) {
        errorEl.querySelector('span').textContent = message;
        errorEl.classList.add('show');
    }
}

function showForceModalAlert(message) {
    const alertEl = document.querySelector('#force-change-alert');
    if (!alertEl) return;
    alertEl.classList.add('show');
    const msgEl = alertEl.querySelector('.alert-msg');
    if (msgEl) msgEl.textContent = message;
}

/* ============================================================
   忘記密碼 Modal（登入頁內）
   ============================================================ */
function initForgotPasswordModal() {
    const submitBtn  = document.querySelector('#forgot-submit-btn');
    const emailInput = document.querySelector('#forgot-email');
    const modalEl    = document.querySelector('#forgotPasswordModal');

    if (!submitBtn || !emailInput || !modalEl) return;

    const bsModal = bootstrap.Modal.getInstance(modalEl)
                 ?? new bootstrap.Modal(modalEl);

    // Modal 開啟時清空上次的狀態
    modalEl.addEventListener('show.bs.modal', function () {
        emailInput.value = '';
        clearFieldError('forgot-email');
        const alertEl = document.querySelector('#forgot-alert');
        if (alertEl) alertEl.classList.remove('show');
    });

    // Email 輸入時清除錯誤
    emailInput.addEventListener('input', function () {
        clearFieldError('forgot-email');
        const alertEl = document.querySelector('#forgot-alert');
        if (alertEl) alertEl.classList.remove('show');
    });

    // 送出按鈕
    submitBtn.addEventListener('click', async function () {
        const email = emailInput.value.trim();

        clearFieldError('forgot-email');

        // 前端驗證
        if (!email) {
            showForgotFieldError('請輸入 Email');
            return;
        }

        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(email)) {
            showForgotFieldError('請輸入有效的 Email 格式');
            return;
        }

        setButtonLoading('forgot-submit-btn', true, '送出');

        try {
            const response = await apiFetch('/Auth/ForgotPassword', {
                method: 'POST',
                body: JSON.stringify({ email })
            });

            if (!response) return;

            const data = await response.json();

            if (response.ok && data.success) {
                // 關閉 Modal，用 SweetAlert2 顯示寄出成功
                bsModal.hide();
                Swal.fire({
                    icon: 'success',
                    title: '重設連結已寄出',
                    html: `
                        <p style="margin-bottom:0.5rem;">已將密碼重設連結送至</p>
                        <p style="font-weight:600;color:#1a0d08;background:#fdf3d0;
                                  display:inline-block;padding:2px 12px;border-radius:6px;
                                  margin-bottom:1rem;">${email}</p>
                        <p style="margin-bottom:0;">
                            連結有效時間為 <strong>60 分鐘</strong>，<br>
                            請盡快至信箱查收並完成重設。
                        </p>`,
                    confirmButtonText: '確認',
                    confirmButtonColor: '#d4b84a',
                    allowOutsideClick: false
                });
            } else {
                showForgotModalAlert(data.message || '送出失敗，請稍後再試');
                setButtonLoading('forgot-submit-btn', false, '送出');
            }
        } catch (error) {
            showForgotModalAlert('系統發生錯誤，請稍後再試');
            setButtonLoading('forgot-submit-btn', false, '送出');
        }
    });

    // Enter 鍵觸發送出
    emailInput.addEventListener('keydown', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            submitBtn.click();
        }
    });
}

/* 忘記密碼 Modal 專用的錯誤顯示輔助函式 */
function showForgotFieldError(message) {
    const input   = document.querySelector('#forgot-email');
    const errorEl = document.querySelector('#forgot-email-error');
    if (input)   input.classList.add('is-invalid');
    if (errorEl) {
        errorEl.querySelector('span').textContent = message;
        errorEl.classList.add('show');
    }
}

function showForgotModalAlert(message) {
    const alertEl = document.querySelector('#forgot-alert');
    if (!alertEl) return;
    alertEl.classList.add('show');
    const msgEl = alertEl.querySelector('.alert-msg');
    if (msgEl) msgEl.textContent = message;
}

/* ============================================================
   重設密碼頁面（從 Email 連結進入）
   ============================================================ */
function initResetPasswordPage() {
    const form = document.querySelector('#reset-password-form');
    if (!form) return;

    initPasswordToggles();
    initPasswordRules('new-password');

    const newPwdInput    = document.querySelector('#new-password');
    const confirmPwdInput = document.querySelector('#confirm-password');

    if (confirmPwdInput) {
        confirmPwdInput.addEventListener('input', function () {
            if (newPwdInput && this.value && this.value !== newPwdInput.value) {
                showFieldError('confirm-password', '兩次密碼不一致');
            } else {
                clearFieldError('confirm-password');
            }
        });
    }

    form.addEventListener('submit', async function (e) {
        e.preventDefault();

        const token      = document.querySelector('#reset-token')?.value;
        const newPwd     = newPwdInput?.value;
        const confirmPwd = confirmPwdInput?.value;

        clearAllErrors();
        hideAlert();

        let hasError = false;

        const pwdError = validatePassword(newPwd || '');
        if (pwdError) {
            showFieldError('new-password', pwdError);
            hasError = true;
        }

        if (!confirmPwd) {
            showFieldError('confirm-password', '請確認密碼');
            hasError = true;
        } else if (newPwd !== confirmPwd) {
            showFieldError('confirm-password', '兩次密碼不一致');
            hasError = true;
        }

        if (hasError) return;

        setButtonLoading('reset-submit-btn', true, '確定重設');

        try {
            const response = await apiFetch('/Auth/ResetPassword', {
                method: 'POST',
                body: JSON.stringify({ token, newPassword: newPwd, confirmPassword: confirmPwd })
            });

            if (!response) return;

            const data = await response.json();

            if (response.ok && data.success) {
                Swal.fire({
                    icon: 'success',
                    title: '密碼重設完成',
                    html: '<p>您的密碼已成功更新，<br>請使用新密碼重新登入系統</p>',
                    confirmButtonText: '前往登入',
                    confirmButtonColor: '#d4b84a',
                    allowOutsideClick: false
                }).then(() => {
                    window.location.href = '/Auth/Login';
                });
            } else {
                showAlert('danger', data.message || '密碼重設失敗，請稍後再試');
                setButtonLoading('reset-submit-btn', false, '確定重設');
            }
        } catch (error) {
            showAlert('danger', '系統發生錯誤，請稍後再試');
            setButtonLoading('reset-submit-btn', false, '確定重設');
        }
    });
}

/* ============================================================
   頁面初始化路由
   ============================================================ */
document.addEventListener('DOMContentLoaded', function () {
    const container = document.querySelector('[data-auth-page]');
    if (!container) return;

    const page = container.dataset.authPage;

    switch (page) {
        case 'login':
            initLoginPage();
            initForgotPasswordModal();
            initForceChangePasswordModal();
            break;
        case 'reset-password':
            initResetPasswordPage();
            break;
    }
});
