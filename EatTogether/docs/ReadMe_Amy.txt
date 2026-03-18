=========
一、資料庫
=========
[V] 建立資料表（T-SQL）
	Users
		Id INT PK IDENTITY(1,1)
		Account VARCHAR(50) Unique
		HashedPassword VARCHAR(70)
		EmployeeNumber VARCHAR(20) Unique
		Name NVARCHAR(50)
		Email VARCHAR(100) Unique
		Phone VARCHAR(10)
		HireDate DATE
		CreatedAt DATETIME2(0) 預設 GETDATE()
		IsActive BIT 預設 1
		IsDeleted BIT 預設 0
		MustChangePassword BIT 預設 0

	PasswordResetTokens
		Id INT PK IDENTITY(1,1)
		UserId INT FK→Users、Index
		Token VARCHAR(32) Unique（Guid 去除符號，32 碼）
		ExpiresAt DATETIME2(0)（建立時間 +60 分鐘）
		IsUsed BIT 預設 0
		CreatedAt DATETIME2(0) 預設 GETDATE()

	Roles
		Id INT PK IDENTITY(1,1)
		RoleName NVARCHAR(20) Unique
		Description NVARCHAR(100) 允許 NULL
		CreatedAt DATETIME2(0) 預設 GETDATE()

	Functions
		Id INT PK IDENTITY(1,1)
		Category NVARCHAR(10)
		FunctionName VARCHAR(50) Unique（程式識別碼，如 Staff_Manage）
		DisplayName NVARCHAR(50)
		Description NVARCHAR(200) 允許 NULL
		IsOwnerOnly BIT 預設 0

	RoleFunctions
		Id INT PK IDENTITY(1,1)
		RoleId INT FK→Roles
		FunctionId INT FK→Functions
		Unique Index(RoleId, FunctionId)

	UserRoles
		Id INT PK IDENTITY(1,1)
		UserId INT FK→Users
		RoleId INT FK→Roles
		Unique Index(UserId, RoleId)

	Members
		Id INT PK IDENTITY(1,1)
		Account VARCHAR(100) Unique
		Name NVARCHAR(50)
		Email VARCHAR(100) Unique
		HashedPassword VARCHAR(70)
		Phone VARCHAR(10) 允許 NULL
		BirthDate DATE 允許 NULL
		IsBlacklisted BIT 預設 0
		CreatedAt DATETIME2(0) 預設 GETDATE()
		IsDeleted BIT 預設 0、Index
		DeletedAt DATETIME2(0) 允許 NULL
		IsConfirmed BIT 預設 0
		AvatarFileName VARCHAR(100) 允許 NULL 預設 NULL
		BlacklistReason NVARCHAR(200) 允許 NULL 預設 NULL

[V] Seed Data — 預設 6 個角色
	店長、副店長、收銀員、外場服務生、內場廚師、工讀生

[V] Seed Data — 13 項功能權限（Functions）
	IsOwnerOnly=1：員工管理（Staff_Manage）、報表管理（Report_Manage）
	IsOwnerOnly=0：員工查看（Staff_View）、會員管理（Member_Manage）、
	               訂單管理（Order_Manage）、更新訂單狀態（Order_UpdateStatus）、
	               菜單管理（Menu_Manage）、訂位管理（Reservation_Manage）、
	               桌位設定（Table_Manage）、優惠券管理（Coupon_Manage）、
	               活動管理（Event_Manage）、文章管理（Article_Manage）、
	               通知管理（Notification_Manage）

[V] Seed Data — RoleFunctions 各角色權限對應（依 requirements.md 第 2 節）

[V] Seed Data — Demo 測試帳號（6 筆）
	密碼統一：Aa000000，MustChangePassword=0，配合登入頁 Demo 按鈕使用
	manager_amy   → 店長（全部 13 項權限）
	vicemgr_lin    → 副店長（11 項權限）
	cashier_wang   → 收銀員 + 外場服務生（多角色聯集示範）
	waiter_liu     → 外場服務生
	chef_zhang     → 內場廚師
	part_cai       → 工讀生（最低權限）


=========
二、專案架構
=========
建立時機說明
	DTO       → 跟著 Repository / Service 同步建立，Repository 一律回傳 DTO，不直接暴露 EfModel
	ViewModel → 跟著 View 同步建立，Controller 將 DTO 轉換為 ViewModel 後傳給 View
	Extension → DTO ↔ ViewModel 轉換擴充方法，跟著 ViewModel 一起建立
	建立順序  → DTO → Extension → Repository → Service → Controller → View

[V] 資料夾結構（已建立）
	Models/DTOs/
	Models/ViewModels/
	Models/Extensions/
	Models/Infra/
	Models/Repositories/
	Models/Services/

=========
共用基礎設施
=========
[V] add HashUtility（Models/Infra/HashUtility.cs）
	安裝 BCrypt.Net-Next（work factor 12）
	static string HashPassword(string password)
	static bool VerifyPassword(string password, string hashedPassword)

[V] add PasswordValidator（Models/Infra/PasswordValidator.cs）
	static bool IsValid(string password)
	// 規則：至少 6 碼，包含英文與數字

[V] add UserNumberGenerator（Models/Infra/UserNumberGenerator.cs）
	// 格式：EMP + 年份(4碼) + 流水號(3碼)，如 EMP2025001
	Task<string> GenerateAsync()
	// 實作細節：
    //   - 年份取 DateTime.Now.Year（伺服器當下時間）
    //   - SQL 使用 WITH (UPDLOCK, HOLDLOCK) 防止併發重複
    //   - 需包在 Transaction 內鎖才會生效
    //   - 結果格式驗證：is string && Length == 10 && TryParse

[V] Result class（Models/Infra/Result.cs）
	bool IsSuccess
	string ErrorMessage
	static Result Success()
	static Result Fail(string errorMessage)

[V] JWT 設定
	Token 效期：8 小時
	儲存方式：httpOnly Cookie
	- appsettings.json — JWT 金鑰設定
	- Program.cs — 註冊 JWT Authentication
	- Models/DTOs/JwtPayloadDto.cs — 定義 JWT Payload 結構
				public int UserId { get; set; }
				public string Name { get; set; } = "";
				public List<int> RoleIds { get; set; } = new();
				public List<string> RoleNames { get; set; } = new();
	- Models/Infra/JwtHelper.cs — 產生 Token (在 Program.cs 中註冊)
	

[V] 自訂 ActionFilter（Models/Infra/RequirePermissionAttribute.cs）
	RequirePermissionAttribute : Attribute, IAsyncActionFilter
		[RequirePermission("FunctionName")]
		從 JWT 取角色 Id 清單 → 查詢 RoleFunctions 取 FunctionName 聯集 → 驗證是否包含指定 FunctionName
		不符 → 回傳 403 Forbidden
	IRoleFunctionRepository / RoleFunctionRepository（Models/Repositories/FunctionRepository.cs）
		public async Task<IEnumerable<string>> GetFunctionNamesByRoleIdsAsync(List<int> roleIds)
	_Layout.cshtml — 從 JWT Payload 顯示登入者資訊

[V] 全域錯誤頁路由設定
	Program.cs — 設定全域錯誤路由
	建立 Controllers/ErrorController.cs
		[Route("Error/403")]
        public IActionResult Forbidden()
		[Route("Error/404")]
        public IActionResult NotFound()
	建立 Views/Error/Forbidden.cshtml、NotFound.cshtml

[V] 新增 SMTP 設定
	IPasswordResetEmailService / PasswordResetEmailService（Models/Services/PasswordResetEmailService.cs）
		Task SendPasswordResetEmailAsync(string toEmail, string resetLink)
		private static string BuildHtml(string resetLink)
		Program.cs — DI 註冊

[V] DI 註冊（Program.cs）
	builder.Services.AddScoped<IUserRepository, UserRepository>()
	builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>()
	builder.Services.AddScoped<IRoleRepository, RoleRepository>()
	builder.Services.AddScoped<IFunctionRepository, FunctionRepository>()
	builder.Services.AddScoped<IMemberRepository, MemberRepository>()
	builder.Services.AddScoped<IAuthService, AuthService>()
	builder.Services.AddScoped<IUserService, UserService>()
	builder.Services.AddScoped<IRoleService, RoleService>()
	builder.Services.AddScoped<IMemberService, MemberService>()
	builder.Services.AddScoped<IPasswordResetEmailService, PasswordResetEmailService>()

=========
模組一：登入登出
=========
[V] add 登入功能
	url: POST /Auth/Login

	[V] DTO（Models/DTOs/LoginDto.cs）
		LoginDto
			int UserId
			string Account
			string Name
			List<int> RoleIds
			bool MustChangePassword

	[V] IUserRepository / UserRepository（Models/Repositories/UserRepository.cs）
		Task<UserDto?> GetByAccountAsync(string account)
		// UserDto：Id, Account, HashedPassword, IsActive, IsDeleted, MustChangePassword, Name, RoleIds

	[V] IAuthService / AuthService（Models/Services/AuthService.cs）
		ctor(IUserRepository repo)
		Task<Result<LoginDto>> LoginAsync(string account, string password)
			// 驗證 BCrypt 密碼（HashUtility.VerifyPassword）
			// 帳號或密碼錯誤 → 一律回傳「帳號或密碼錯誤」（防帳號枚舉）
			// IsDeleted → 回傳「帳號或密碼錯誤」
			// IsActive → 回傳「此帳號已停用，請聯絡店長」
			// 驗證通過 → 回傳 LoginDto（含 MustChangePassword 旗標）

	[V] ViewModel（Models/ViewModels/LoginViewModel.cs）
		LoginViewModel
			string Account
			string Password

	[V] AuthController（Controllers/AuthController.cs）
		GET /Auth/Login => 顯示登入頁面
		POST /Auth/Login
			驗證通過且 MustChangePassword=0 → 發行 JWT（httpOnly Cookie）→ Redirect 
			驗證通過且 MustChangePassword=1 → 回傳 { mustChangePassword: true }，前端開強制改密碼 Modal
			登入後寫入 httpOnly Cookie

	[V] Login.cshtml（Views/Auth/Login.cshtml）
		深色背景、白色卡片
		帳號 / 密碼欄（含顯示/隱藏 icon）
		「忘記密碼」文字連結
		Demo 帳號選擇區塊（6 角色按鈕，點擊自動填入帳號密碼，密碼統一 Aa000000）
			<!-- TODO: Demo Only，上線前移除 -->
			店長        → manager_amy
			副店長      → vicemgr_lin
			收銀員      → cashier_wang（兼外場服務生，多角色聯集示範）
			外場服務生  → waiter_liu
			內場廚師    → chef_zhang
			工讀生      → part_cai

[V] add 強制改密碼功能
	url: POST /Auth/ForceChangePassword
	觸發條件：MustChangePassword=1（密碼與員工編號相同）

	[V] ViewModel（Models/ViewModels/ForceChangePasswordViewModel.cs）
		ForceChangePasswordViewModel
			string NewPassword
			string ConfirmPassword

	[V] AuthService（modify）
		Task<Result<LoginDto>> ForceChangePasswordAsync(int userId, string newPassword)
			// 確認使用者存在
			// 更新 HashedPassword
			// MustChangePassword → 0

	[V] UserRepository（modify）
		Task<UserDto?> GetByIdAsync(int userId)
		Task UpdatePasswordAsync(int userId, string hashedPassword)
		Task SetMustChangePasswordAsync(int userId, bool value)

	[V] AuthController（modify）
		GET /Auth/Login => 顯示登入頁面(補上)
		POST /Auth/ForceChangePassword

	[V] 強制改密碼 Modal（嵌入 Login.cshtml）
		data-bs-backdrop="static"（不可關閉）
		鎖頭 icon
		新密碼 + 確認密碼（含顯示/隱藏）
		前端密碼一致性驗證
		成功 → SweetAlert2
			標題：「密碼重設完成」
			說明：「即將進入後台系統...」
			按鈕：「進入系統」，5 秒倒數後自動跳轉 Home/Index

[V] add 忘記密碼 / 重設密碼功能
	url: POST /Auth/ForgotPassword
	url: GET  /Auth/ResetPassword?token=xxx
	url: POST /Auth/ResetPassword

	[V] IPasswordResetTokenRepository / PasswordResetTokenRepository
		（Models/Repositories/PasswordResetTokenRepository.cs）
		Task CreateAsync(PasswordResetToken token)
			// ExpiresAt = 建立時間 +60 分鐘
		Task InvalidatePreviousTokensAsync(int userId)
		Task<PasswordResetToken?> GetValidTokenAsync(string token)
		Task MarkUsedAsync(int tokenId)

	[V] AuthService（modify）
		Task<Result> ForgotPasswordAsync(string email)
			// 查詢 Email 是否存在（不存在仍回傳成功，防枚舉）=> async Task<UserDto?> GetByEmailAsync (新增 UserRepository 方法)
			// 產生 32 碼 Guid Token（去除符號）
			// 寫入 PasswordResetTokens
			// 呼叫 IPasswordResetEmailService 寄送重設連結
		Task<bool> ValidateResetTokenAsync(string token)
			// 驗證：存在 + IsUsed=0 + 未逾 ExpiresAt
		Task<Result> ResetPasswordAsync(string token, string newPassword)
			// 更新 HashedPassword
			// IsUsed → 1（一次性，立即失效）

	[V] ViewModel（Models/ViewModels/ForgotPasswordViewModel.cs）
		ForgotPasswordViewModel
			string Email

	[V] ViewModel（Models/ViewModels/ResetPasswordViewModel.cs）
		ResetPasswordViewModel
			string Token
			string NewPassword
			string ConfirmPassword

	[V] AuthController（modify）
		POST /Auth/ForgotPassword
		GET  /Auth/ResetPassword?token=xxx → 驗證 Token；無效 → Redirect ResetPasswordInvalid
		POST /Auth/ResetPassword
		GET /Auth/ResetPasswordInvalid

	[V] 忘記密碼 Modal（嵌入 Login.cshtml）
		鑰匙 icon
		Email 輸入框
		按鈕：「取消」、「送出」
		成功 → SweetAlert2
			顯示對應 Email、「連結有效時間為 60 分鐘」
			按鈕：「確認」

	[V] ResetPassword.cshtml（Views/Auth/ResetPassword.cshtml）
		url: /Auth/ResetPassword?token=xxx
		鑰匙 icon
		新密碼 + 確認密碼（含顯示/隱藏）
		密碼規則即時勾選提示（長度 ≥ 6 / 含英文 / 含數字）
		按鈕：「確定重設」
		成功 → SweetAlert2
			說明：「請使用新密碼重新登入系統」
			按鈕：「前往登入」

	[V] ResetPasswordInvalid.cshtml（Views/Auth/ResetPasswordInvalid.cshtml）
		警告 icon
		按鈕：「返回登入頁重新申請」（導向 /Auth/Login）

[V] add 登出功能
	url: POST /Auth/Logout

	[V] AuthController（modify）
		POST /Auth/Logout
			清除 JWT Cookie
			Redirect 登入頁（瀏覽器返回自動導向登入頁）

[V] 登入成功落地頁
	url: GET /Home/Index

	[V] HomeController（modify）
		GET /Home/Index

	[V] Home/Index.cshtml（Views/Home/Index.cshtml）
		登入成功落地頁
		顯示：「登入成功，請選擇左側功能選單，開始管理」

[V] 頭像顏色由後端處理
	JwtHelper.GenerateToken() 內根據姓名雜湊計算頭像背景顏色（HashUserIdToColor(int userId)）

=========
模組二：員工管理（Users）
=========
[RequirePermission("Staff_View")] 套用於 GET /User/Index
[RequirePermission("Staff_Manage")] 套用於 Create / Edit / Resign / Reinstate

[V] add 員工列表功能
	url: GET /User/Index

	[V] DTO（Models/DTOs/UserListDto.cs）
		UserListDto
			int Id
			string EmployeeNumber, Name, Account, Email, Phone
			DateOnly HireDate
			DateTime CreatedAt
			bool IsActive, IsDeleted, CanEdit, CanResign, CanReinstate
			List<int> RoleIds
			List<string> RoleNames

	[V] DTO（Models/DTOs/UserSearchDto.cs）
		UserSearchDto
			string? EmployeeNumber, Name, Account, Email
			bool HideResigned
			string SortBy   // HireDate_Desc / HireDate_Asc / CreatedAt_Desc

	[V] IUserRepository / UserRepository（modify）
		Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto)
		
	[V] IUserService / UserService（Models/Services/UserService.cs）
		ctor(IUserRepository userRepo, IRoleRepository roleRepo)
		Task<IEnumerable<UserListDto>> GetAllAsync(UserSearchDto dto)
		

	[V] ViewModel（Models/ViewModels/UserIndexViewModel.cs）
		UserIndexViewModel
			IEnumerable<UserRowViewModel> Rows
			string? EmployeeNumber, Name, Account, Email
			bool HideResigned
			string SortBy

	[V] ViewModel（Models/ViewModels/UserRowViewModel.cs）
		UserRowViewModel
			// 對應 UserListDto + 前端顯示用欄位
			string StatusText    // 在職 / 請假 / 離職
			string StatusColor   // green / orange / gray
			bool CanEdit, CanResign, CanReinstate

	[V] Extension（Models/Extensions/UserDtoExtension.cs）
		UserRowViewModel ToRowViewModel(this UserListDto dto)

	[V] UserController（Controllers/UserController.cs）
		GET /User/Index

	[V] User/Index.cshtml（Views/User/Index.cshtml）
		DataTables + zh-HANT.json 中文化
		dom 設定（筆數文字靠左、分頁按鈕靠右、同一行）
			"<'row'<'col-12'tr>><'row align-items-center mt-2'<'col-auto'i><'col'p>>"
		搜尋列：label 與輸入框同行水平排列（display: flex; align-items: center）
			員工編號 / 姓名 / 帳號 / Email 輸入框 + 「查詢」按鈕 + 「在職」勾選框
		排序下拉（右上角）：到職最新 / 最舊 / 建立最新
		「+ 新增」按鈕（店長可見）
		表格欄位：員工編號 / 姓名 / 帳號 / Email / 手機 / 到職日期 / 建立時間 / 角色 / 狀態 / 操作
		狀態標籤：在職（綠）/ 請假（橘）/ 離職（灰）
		操作欄：在職/請假 → 🔵編輯 + 🔴離職；離職 → 🔵編輯 + 🟢復職

[working] add 新增員工功能
	url: POST /User/Create

	[V] DTO（Models/DTOs/UserInsertDto.cs）
		UserInsertDto
			string EmployeeNumber, Name, Account, HashedPassword, Email, Phone
			DateOnly HireDate
			bool IsActive, MustChangePassword
			List<int> RoleIds

	[V] DTO（Models/DTOs/UserCreateDto.cs）
		UserCreateDto
			string Name, Account, Password, Email, Phone
			DateOnly HireDate
			bool IsActive
			List<int> RoleIds

	[V] UserNumberGenerator（modify）
		string Generate(string lastEmployeeNumber)
		 只負責編號產生邏輯，不存取資料庫
		 // 員工編號自動產生：EMP + 年份(4碼) + 流水號(3碼)，如 EMP2025001
		 // 取最後一個員工編號 → 流水號 +1

	[V] UserRepository（modify）
		Task<string> GetLastEmployeeNumberByYearAsync(int year); => 找出當年最後一筆員工編號
		Task InsertAsync(UserInsertDto dto);
		Task<bool> IsAccountExistsAsync(string account, int? excludeId = null);
		Task<bool> IsEmailExistsAsync(string email, int? excludeId = null);
		

	[V] UserService（modify）
		Task<string> GetEmployeeNumberPreviewAsync()
        // 預產員工編號（僅供前端開 Modal 顯示用，不在 Transaction 內）
        // 注意：此編號僅供顯示，實際寫入時會在 Transaction 內重新產生

		Task<Result> CreateAsync(UserCreateDto dto)
			// 業務驗證：帳號唯一性（GetByAccountAsync → 不為 null 則 Fail）
			// 業務驗證：Email 唯一性（GetByEmailAsync → 不為 null 則 Fail）
			// 業務驗證：密碼複雜度（PasswordValidator.IsValid）
			// HashUtility.HashPassword → 取得 hashedPassword（明文只在此處使用）
			// BeginTransactionAsync() 開啟交易
			//   → ExecuteInsertAsync(dto, hashedPassword)
			//   → CommitAsync() 提交交易
			// 捕捉 DbUpdateException（UNIQUE / duplicate）
			//   → RollbackAsync() → 進入 RetryCreateAsync(dto, hashedPassword)
			// 其他例外 → RollbackAsync() → Result.Fail("新增員工失敗，請稍後再試")

		Task<Result> RetryCreateAsync(UserCreateDto dto, string hashedPassword)  // private
			// 重新開啟 Transaction，重新呼叫 ExecuteInsertAsync(dto, hashedPassword)，只重試一次
			// 再次失敗 → RollbackAsync() → Result.Fail("新增員工失敗，請重試")

		Task ExecuteInsertAsync(UserCreateDto dto, string hashedPassword)  // private
			// 必須在 Transaction 內呼叫（UPDLOCK/HOLDLOCK 才有效）
			// 1. GetLastEmployeeNumberByYearAsync(DateTime.Now.Year) 取得最後編號
			// 2. UserNumberGenerator.Generate(lastEmpNo) 產生員工編號
			// 3. 比對明文密碼 == 員工編號 → MustChangePassword=true，否則=false
			// 4. 組裝 UserInsertDto
			// 5. InsertAsync(insertDto) 寫入 Users + UserRoles（同一 Transaction）

	[V] ViewModel（Models/ViewModels/UserCreateViewModel.cs）
		UserCreateViewModel  // 前端 Modal 綁定用，ConfirmPassword 僅供前端驗證，不傳後端
			string Name, Account, Password, ConfirmPassword
			string Email, Phone
			DateOnly HireDate
			string IsActive   // 下拉：在職 / 請假
			List<int> RoleIds

	[V] UserController（modify）
		GET  /Users/NextEmployeeNumber → 呼叫 GetEmployeeNumberPreviewAsync()，回傳 { employeeNumber }
		POST /Users/Create
        → ModelState 驗證（格式：必填、長度、Email 格式、角色必選）
        → 呼叫 UserService.CreateAsync(dto)
        → 成功 → Ok()
        → 失敗 → BadRequest({ message })

	[V] 新增員工 Modal（嵌入 User/Index.cshtml）
		員工編號：唯讀，「系統自動產生，不可修改」
		帳號：輸入框 + 「同員工編號」勾選框（勾選後自動帶入）
		密碼：輸入框 + 「同員工編號」勾選框（至少 6 碼含英文與數字）
		啟用狀態：下拉（在職 / 請假）
		Email / 手機（選填）
		到職日期：Flatpickr 日期選擇器（選填）
		角色 Checkbox（多選，至少勾選一個）
		按鈕：「取消」、「新增」
		成功 → SweetAlert2 success，關閉後重新整理列表

[] add 編輯員工功能
	url: GET  /User/Edit/{id}
	url: PUT  /User/Edit/{id}

	[] DTO（Models/DTOs/UserEditDto.cs）
		UserEditDto
			int Id
			string EmployeeNumber（唯讀）
			DateTime CreatedAt（唯讀）
			string Name, Account
			string Password
			string Email, Phone
			DateOnly HireDate
			bool IsActive
			List<int> RoleIds

	[] Extension（Models/Extensions/UserDtoExtension.cs）（modify）
		UserEditViewModel ToEditViewModel(this UserEditDto dto)

	[] UserRepository（modify）
		Task<UserEditDto?> GetForEditAsync(int id)
		Task UpdateAsync(UserEditDto dto)
		Task UpdateUserRolesAsync(int userId, List<int> roleIds)
			// 先刪後插（BatchUpdate UserRoles）

	[] UserService（modify）
		Task<UserEditDto?> GetForEditAsync(int id)
		Task<Result> UpdateAsync(int id, UserEditDto dto)
			// 密碼留空 → 維持原值
			// 密碼有填入 → PasswordValidator.IsValid → 比對明文密碼 == 員工編號 → 更新 HashedPassword + MustChangePassword
			// BatchUpdate UserRoles

	[] ViewModel（Models/ViewModels/UserEditViewModel.cs）
		UserEditViewModel
			// 同 UserCreateViewModel + Id, EmployeeNumber, CreatedAt（唯讀欄位）
			// Password 說明文字：「留空表示不修改」

	[] UserController（modify）
		GET /User/Edit/{id} → 回傳 UserEditViewModel（預填現有資料 + 全部角色清單）
		PUT /User/Edit/{id}

	[V] 編輯員工 Modal（嵌入 User/Index.cshtml）
		員工編號：唯讀
		帳號：可編輯 + 「同員工編號」勾選框
		密碼：輸入框 + 「同員工編號」勾選框
			說明文字：「留空表示不修改，密碼同員工編號登入時將被強制重設密碼」
		建立時間：唯讀
		其餘欄位同新增 Modal
		按鈕：「取消」、「儲存變更」
		成功 → SweetAlert2 success，關閉後重新整理列表

[] add 離職處理
	url: PATCH /User/Resign/{id}

	[] UserRepository（modify）
		Task ResignAsync(int id)
		// IsDeleted → 1

	[] UserService（modify）
		Task<Result> ResignAsync(int id, int operatorId)
		// 禁止對自身帳號執行（operatorId == id → 回傳錯誤）

	[] UserController（modify）
		PATCH /User/Resign/{id}

	[V] 離職確認 SweetAlert2（嵌入 User/Index.cshtml）
		圖示：⚠️ 警告
		標題：「確認離職處理？」
		說明：「{姓名}（{員工編號}）的帳號將立即無法登入後台系統，請確認後再執行。」
		按鈕：「取消」、「確認」

[] add 復職處理
	url: PATCH /User/Reinstate/{id}

	[] UserRepository（modify）
		Task ReinstateAsync(int id)
		// IsDeleted → 0、IsActive → 1

	[] UserService（modify）
		Task<Result> ReinstateAsync(int id)

	[] UserController（modify）
		PATCH /User/Reinstate/{id}

	[V] 復職確認 SweetAlert2（嵌入 User/Index.cshtml）
		圖示：⚠️ 警告
		標題：「確認復職處理？」
		說明：「{姓名}（{員工編號}）的帳號將立即恢復登入權限。」
		按鈕：「取消」、「確認」

=========
模組三：角色與權限管理
=========
[RequirePermission("Staff_Manage")] 套用於所有 Role Actions

[] add 角色列表功能
	url: GET /Role/Index

	[] DTO（Models/DTOs/RoleListDto.cs）
		RoleListDto
			int Id
			string RoleName, Description
			List<int> FunctionIds
			List<string> FunctionDisplayNames
			int UserCount

	[] IFunctionRepository / FunctionRepository（Models/Repositories/FunctionRepository.cs）
		Task<IEnumerable<Function>> GetAllAsync()

	[] IRoleRepository / RoleRepository（Models/Repositories/RoleRepository.cs）
		Task<IEnumerable<RoleListDto>> GetAllAsync()

	[] IRoleService / RoleService（Models/Services/RoleService.cs）
		ctor(IRoleRepository repo, IFunctionRepository funcRepo)
		Task<IEnumerable<RoleListDto>> GetAllAsync()

	[] ViewModel（Models/ViewModels/RoleIndexViewModel.cs）
		RoleIndexViewModel
			IEnumerable<RoleRowViewModel> Rows

	[] ViewModel（Models/ViewModels/RoleRowViewModel.cs）
		RoleRowViewModel
			// 對應 RoleListDto
			bool CanDelete   // 預設 6 個角色 = false

	[] Extension（Models/Extensions/RoleDtoExtension.cs）
		RoleRowViewModel ToRowViewModel(this RoleListDto dto)

	[] RoleController（Controllers/RoleController.cs）
		GET /Role/Index

	[V] Role/Index.cshtml（Views/Role/Index.cshtml）
		DataTables + zh-HANT.json 中文化 + 支援分頁
		dom 設定（筆數文字靠左、分頁按鈕靠右、同一行）
			"<'row'<'col-12'tr>><'row align-items-center mt-2'<'col-auto'i><'col'p>>"
		columnDefs: { className: 'text-nowrap', targets: '_all' }（防止欄位折行）
		「查看總覽」按鈕（開權限總覽 Modal）
		「+ 新增角色」按鈕
		表格欄位：角色名稱 / 角色描述 / 已擁有的權限（綠色標籤）/ 員工數 / 操作

	[] roles-index.css（CSS 修正）
		DataTables 分頁樣式移至 .roles-index { } 命名空間外
		改用 #roles-table_wrapper 選取器
			#roles-table_wrapper .dataTables_info { font-size: 1rem; color: #6c757d; }
			#roles-table_wrapper .dataTables_paginate { text-align: right; }
			#roles-table_wrapper .dataTables_paginate .pagination { justify-content: flex-end; margin-bottom: 0; font-size: 1rem; }

[] add 權限總覽功能
	url: GET /Role/Overview

	[] DTO（Models/DTOs/RoleOverviewDto.cs）
		RoleOverviewDto
			List<string> RoleNames
			List<string> FunctionDisplayNames
			bool[,] Matrix   // Matrix[functionIndex, roleIndex]

	[] RoleRepository（modify）
		Task<RoleOverviewDto> GetOverviewAsync()

	[] RoleService（modify）
		Task<RoleOverviewDto> GetOverviewAsync()

	[] RoleController（modify）
		GET /Role/Overview（JSON，供 Modal AJAX 呼叫）

	[V] 權限總覽 Modal（嵌入 Role/Index.cshtml）
		標題：「權限總覽」
		13 項 × 所有角色矩陣（✓ / —），唯讀

[] add 新增角色功能
	url: POST /Role/Create

	[] DTO（Models/DTOs/RoleCreateDto.cs）
		RoleCreateDto
			string RoleName
			string? Description
			List<int> FunctionIds   // 排除 IsOwnerOnly=1 的項目
			List<int> UserIds

	[] RoleRepository（modify）
		Task CreateAsync(RoleCreateDto dto)
		Task SetRoleFunctionsAsync(int roleId, List<int> functionIds)
		Task SetUsersByRoleIdAsync(int roleId, List<int> userIds)

	[] RoleService（modify）
		Task<Result> CreateAsync(RoleCreateDto dto)
			// BatchInsert RoleFunctions（排除 IsOwnerOnly=1）
			// BatchInsert UserRoles

	[] ViewModel（Models/ViewModels/RoleCreateViewModel.cs）
		RoleCreateViewModel
			string RoleName
			string? Description
			List<int> SelectedFunctionIds
			List<int> SelectedUserIds
			List<Function> AllFunctions    // 供 Checkbox 卡片渲染
			List<UserListDto> AllUsers     // 供員工指派清單渲染

	[] Extension（Models/Extensions/RoleDtoExtension.cs）（modify）
		RoleCreateViewModel ToCreateViewModel(IEnumerable<Function> allFunctions, IEnumerable<UserListDto> allUsers)

	[] RoleController（modify）
		GET  /Role/Create → 回傳 RoleCreateViewModel（含全部 Functions + 全部在職員工）
		POST /Role/Create

	[V] 新增角色 Modal（嵌入 Role/Index.cshtml）
		角色名稱（必填）/ 角色描述（選填）
		權限 Checkbox 卡片（兩欄，右側可捲動）
			灰底不可勾選：「員工管理」、「報表管理」（IsOwnerOnly=1）
			其餘 11 項可自由勾選
		員工指派 Checkbox 清單
			每列顯示：姓名（.emp-name，粗體）、員工編號（.emp-no）、帳號（.emp-account）、狀態標籤
		按鈕：「取消」、「新增」
		成功 → SweetAlert2 success，關閉後重新整理列表

[] add 編輯角色功能
	url: GET /Role/Edit/{id}
	url: PUT /Role/Edit/{id}

	[] DTO（Models/DTOs/RoleEditDto.cs）
		RoleEditDto
			int Id
			string RoleName
			string? Description
			List<int> FunctionIds
			List<int> UserIds

	[] Extension（Models/Extensions/RoleDtoExtension.cs）（modify）
		RoleEditViewModel ToEditViewModel(this RoleEditDto dto, IEnumerable<Function> allFunctions, IEnumerable<UserListDto> allUsers)

	[] RoleRepository（modify）
		Task<RoleEditDto?> GetForEditAsync(int id)
		Task UpdateAsync(RoleEditDto dto)
			// 更新 RoleName、Description
			// 同步更新 RoleFunctions（先刪後插）
			// 同步更新 UserRoles（先刪後插）

	[] RoleService（modify）
		Task<RoleEditDto?> GetForEditAsync(int id)
		Task<Result> UpdateAsync(int id, RoleEditDto dto)

	[] ViewModel（Models/ViewModels/RoleEditViewModel.cs）
		RoleEditViewModel
			// 同 RoleCreateViewModel + Id

	[] RoleController（modify）
		GET /Role/Edit/{id} → 回傳 RoleEditViewModel（預填現有資料）
		PUT /Role/Edit/{id}

	[V] 編輯角色 Modal（嵌入 Role/Index.cshtml）
		預填現有資料
		已指派員工預先勾選
		員工清單顯示格式同新增 Modal
		按鈕：「取消」、「儲存變更」
		成功 → SweetAlert2 success，關閉後重新整理列表

[] add 刪除角色功能
	url: DELETE /Role/Delete/{id}

	[] RoleRepository（modify）
		Task DeleteAsync(int id)
			// Delete RoleFunctions → Delete UserRoles → Delete Role

	[] RoleService（modify）
		Task<Result> DeleteAsync(int id)
			// 預設 6 個角色不可刪除 → 回傳錯誤訊息

	[] RoleController（modify）
		DELETE /Role/Delete/{id}

	[V] 刪除角色（Role/Index.cshtml JS）
		預設 6 個角色 → SweetAlert2 warning「無法刪除『{角色名稱}』」
		自訂角色 → SweetAlert2 確認
			說明：「刪除後，所有擁有此角色的員工將立即失去對應權限。此操作無法復原。」
			按鈕：「取消」、「確認」

=========
模組四：會員管理
=========
[RequirePermission("Member_Manage")] 套用於所有 Member Actions

[] add 會員列表功能
	url: GET /Member/Index

	[] DTO（Models/DTOs/MemberListDto.cs）
		MemberListDto
			int Id
			string Name, Account, Email
			string? Phone
			DateTime CreatedAt
			bool IsConfirmed, IsBlacklisted, IsDeleted
			string? BlacklistReason

	[] DTO（Models/DTOs/MemberSearchDto.cs）
		MemberSearchDto
			string? Name, Account, Email, Phone
			string Status   // All / Normal / Unconfirmed / Blacklisted / Deleted
			string SortBy   // CreatedAt_Desc / CreatedAt_Asc

	[] IMemberRepository / MemberRepository（Models/Repositories/MemberRepository.cs）
		Task<IEnumerable<MemberListDto>> GetAllAsync(MemberSearchDto dto)
			// 狀態優先順序：IsDeleted=1 → 已刪除；IsBlacklisted=1 → 黑名單；IsConfirmed=0 → 未驗證；其餘 → 正常

	[] IMemberService / MemberService（Models/Services/MemberService.cs）
		ctor(IMemberRepository repo)
		Task<IEnumerable<MemberListDto>> GetAllAsync(MemberSearchDto dto)

	[] ViewModel（Models/ViewModels/MemberIndexViewModel.cs）
		MemberIndexViewModel
			IEnumerable<MemberRowViewModel> Rows
			string? Name, Account, Email, Phone
			string Status, SortBy

	[] ViewModel（Models/ViewModels/MemberRowViewModel.cs）
		MemberRowViewModel
			// 對應 MemberListDto + 前端顯示用欄位
			string StatusText    // 啟用中 / 未驗證 / 黑名單 / 已刪除
			string StatusColor   // green / yellow / red / gray
			string ButtonType    // blacklist / unblacklist / disabled / none

	[] Extension（Models/Extensions/MemberDtoExtension.cs）
		MemberRowViewModel ToRowViewModel(this MemberListDto dto)

	[] MemberController（Controllers/MemberController.cs）
		GET /Member/Index

	[V] Members/Index.cshtml（Views/Members/Index.cshtml）
		DataTables + zh-HANT.json 中文化
		dom 設定（筆數文字靠左、分頁按鈕靠右、同一行）
			"<'row'<'col-12'tr>><'row align-items-center mt-2'<'col-auto'i><'col'p>>"
		搜尋列：label 與輸入框同行水平排列（display: flex; align-items: center）
			姓名 / 帳號 / Email / 手機 輸入框 + 狀態下拉 + 「查詢」按鈕
			狀態下拉改變後立即篩選（不需按查詢按鈕）
		排序下拉（右上角）：註冊日期最新 / 最舊（選擇後立即生效）
		狀態標籤：啟用中（綠）/ 未驗證（黃）/ 黑名單（紅）/ 已刪除（灰）
		操作欄：每列同一時間只顯示一個按鈕
			啟用中 / 未驗證 → 加入黑名單（紅，可點）
			黑名單 → 解除黑名單（綠，可點）
			已刪除 → 加入黑名單（灰，Disabled）；解除黑名單不顯示

	[] members-index.css（CSS 修正）
		DataTables 分頁樣式移至 .members-index { } 命名空間外
		改用 #members-table_wrapper 選取器
			#members-table_wrapper .dataTables_info { font-size: 1rem; color: #6c757d; }
			#members-table_wrapper .dataTables_paginate { text-align: right; }
			#members-table_wrapper .dataTables_paginate .pagination { justify-content: flex-end; margin-bottom: 0; font-size: 1rem; }

[] add 會員詳情功能
	url: GET /Member/Detail/{id}

	[] DTO（Models/DTOs/MemberDetailDto.cs）
		MemberDetailDto
			// 同 MemberListDto + BirthDate, AvatarFileName, DeletedAt

	[] Extension（Models/Extensions/MemberDtoExtension.cs）（modify）
		MemberDetailViewModel ToDetailViewModel(this MemberDetailDto dto)

	[] MemberRepository（modify）
		Task<MemberDetailDto?> GetByIdAsync(int id)

	[] MemberService（modify）
		Task<MemberDetailDto?> GetDetailAsync(int id)

	[] ViewModel（Models/ViewModels/MemberDetailViewModel.cs）
		MemberDetailViewModel
			// 對應 MemberDetailDto
			// 黑名單原因：黑名單狀態顯示；未填寫顯示「（未填寫）」

	[] MemberController（modify）
		GET /Member/Detail/{id}（JSON，供 Modal AJAX 呼叫）

	[V] 會員詳情 Modal（嵌入 Members/Index.cshtml）
		唯讀
		欄位：姓名 / 帳號 / Email / 手機 / 生日 / 註冊時間 / 刪除時間 / 狀態 / 黑名單原因
		黑名單原因：僅「黑名單」狀態顯示；未填寫顯示「（未填寫）」
		底部按鈕：「關閉」

[] add 黑名單管理功能
	url: PATCH /Member/Blacklist/{id}
	url: PATCH /Member/Unblacklist/{id}

	[] MemberRepository（modify）
		Task UpdateBlacklistAsync(int id, bool isBlacklisted, string? reason)

	[] MemberService（modify）
		Task<Result> BlacklistAsync(int id, string? reason)
			// IsBlacklisted → 1，儲存 BlacklistReason（允許 NULL）
		Task<Result> UnblacklistAsync(int id)
			// IsBlacklisted → 0，BlacklistReason → NULL

	[] MemberController（modify）
		PATCH /Member/Blacklist/{id}
		PATCH /Member/Unblacklist/{id}

	[V] 加入黑名單 SweetAlert2（嵌入 Members/Index.cshtml）
		標題：「確認加入黑名單？」
		說明：「將 {姓名} 加入黑名單後，該帳號將立即無法登入前台網站。」
		輸入欄位：黑名單原因（選填）
			placeholder：「請輸入原因，例如：多次惡意取消訂位」
		按鈕：「取消」、「確認」

	[V] 解除黑名單 SweetAlert2（嵌入 Members/Index.cshtml）
		標題：「確認解除黑名單？」
		說明：「解除 {姓名} 的黑名單限制後，該會員帳號將可正常登入前台網站。」
		按鈕：「取消」、「確認」

=========
共用版面（_Layout.cshtml）
=========
[V] 已完成
	<head> 加入 favicon（favicon.svg 主要，favicon-32x32.png 備用）
	Navbar Logo 改用 <img src="/images/logo-full.svg" alt="義起吃" />
	全站 CSS 品牌色票變數（9 色）
		--eat-brand-dark    #1A0D08（Navbar / Sidebar / 主要按鈕）
		--eat-brand-gold    #F5D87A（Sidebar active 文字）
		--eat-brand-copper  #C9A96E（次要強調、分隔線）
		--eat-warm-white    #F5EDD8（Sidebar 一般文字）
		--eat-bg            #F2EDE4（主內容區外側背景）
		--eat-card-white    #FFFFFF（Modal / SweetAlert / 卡片）
		--eat-sidebar-act   #3B1F12（Sidebar 父項目 active 底色）
		--eat-sidebar-sub   #4E2A17（Sidebar 子項目 active 底色）
		--eat-sidebar-hover #2C1610（Sidebar 項目 hover 底色）

[] 待完成
	Navbar 右側：從 JWT Payload 動態顯示「登入者姓名 + 角色」與「登出」按鈕
	Sidebar：從 JWT Payload 角色聯集動態顯示/隱藏選單項目（含分組標題）
	頁面內操作按鈕依權限動態隱藏，無權限者不渲染至 DOM
	403 頁面：顯示角色名稱，保留完整 Layout，無額外按鈕
	404 頁面：超大「404」字樣，保留完整 Layout，無額外按鈕

[] 建立 wwwroot/uploads/avatars/ 目錄
	對應 Members.AvatarFileName

=========
UI 元件規格（全站）
=========
Users / Members / Roles 三個 Index 頁面均適用

[] 字體最小尺寸
	所有可見文字不得小於 1rem（16px）
	涵蓋：表格內文 / 搜尋 label / 分頁文字 / badge 文字

[] Badge 尺寸（.status-badge / .role-badge / .perm-badge）
	font-size: 0.9rem
	padding 水平不小於 0.7em

[] 搜尋列 label + input 同行
	.search-field { display: flex; align-items: center; gap: 0.5rem; }
	label 與輸入框水平排列，不上下堆疊

[] DataTables 分頁列靠左靠右
	CSS 選取器一律用 #table-id_wrapper（不可寫在命名空間 class 內）
	#xxx-table_wrapper .dataTables_info → 靠左
	#xxx-table_wrapper .dataTables_paginate → 靠右
	#xxx-table_wrapper .dataTables_paginate .pagination → justify-content: flex-end

[] 角色列表頁欄位 nowrap
	columnDefs: { className: 'text-nowrap', targets: '_all' }

=========
測試與 Demo 收尾
=========
[] 功能驗證
	登入完整流程：一般登入 / 員工編號為密碼觸發強制改密碼 / 忘記密碼 Email 流程
	JWT 效期 8 小時到期自動登出並導向登入頁
	JWT httpOnly Cookie（DevTools Application 確認 JS 無法讀取）
	密碼重設 Token 一次性（使用後再次存取 → 連結無效頁）
	密碼重設 Token 60 分鐘逾時驗證
	直接輸入無權限 URL → 403 頁面（各角色逐一測試）
	未登入直接輸入後台 URL → 導向登入頁
	員工管理 CRUD + 離職/復職流程 + 自身帳號離職保護
	角色新增/編輯/刪除（預設角色保護；刪除後員工即時失去對應權限）
	會員黑名單加入/解除（含原因填寫 / 不填寫）
	密碼複雜度驗證（不足 6 碼 / 無數字 / 無英文均應拒絕）

[] 程式碼慣例檢查
	全專案非同步方法加 Async 後綴並使用 async / await
	全專案前端 AJAX 均透過 apiFetch，無裸 fetch()
	全專案 JS DOM 選取無 getElement* 系列，一律使用 querySelector
	全專案資料庫刪除一律採軟刪除（IsDeleted=1）

[] UI 元件規格驗收
	三個 Index 頁面所有文字字體 ≥ 1rem（16px）
	badge 字體 0.9rem，padding 水平 ≥ 0.7em
	搜尋列 label 與輸入框同行（.search-field flex 排列）
	DataTables 分頁列靠左靠右（#table-id_wrapper 選取器）
	角色列表頁欄位 text-nowrap

[] Demo 前準備
	Demo 帳號選擇區塊正常運作（6 個角色一鍵填入，各角色登入成功）
	各角色登入後 Sidebar 顯示正確（對照 requirements.md 第 2 節權限一覽）
	DataTables 套用 zh-HANT.json 中文化
	所有 AJAX 操作有 Loading 狀態與成功/失敗提示
	Logo 與 Favicon 於各頁面正常顯示
	資料庫重建腳本可正常執行（Seed Data 完整，可重跑）