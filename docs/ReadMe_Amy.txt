[V]連結資料庫
	設定EfModels
	設定連線字串
	註冊 DbContext 到 DI
=========
開發會員機制
[]add 會員註冊功能
	url: /Auth/Register/
	[V] 雜湊密碼的公用程式
		安裝 BCrypt.Net-Next 套件
		/Models/Infra/HashUtility.cs
			static string HashPassword(string password)
			static bool VerifyPassword(string password, string hashedPassword)

		------ web chat 提示詞 ------
		asp.net core mvc 我想用 BCrypt.Net-Next 套件寫出雜湊密碼功能，請提供範例
			HashUtility.cs
						static string HashPassword(string password)
						static bool VerifyPassword(string password, string hashedPassword)

	[V] ViewModel, Dto, RegisterViewModel 轉 RegisterDto 的擴充方法
		RegisterViewModel class
			Account, Password, ConfirmPassword, Name, Email, Mobile properties

			------ Copilot 提示詞 ------
			建立 RegisterViewModel class 屬性，請參考 Models/EfModels/Member.cs，並參考 EStoreContext class 為各屬性加入 Data Annotations

		RegisterDto class
			Account, Password, Name, Email, Mobile, HashedPassword, NewMemberConfirmCode, IsConfirmed properties

		RegisterViewModelExtension class
			RegisterDto ToDto(this RegisterViewModel vm)

	[V] 建立Service/Repository並註冊到DI
		IMemberRepository interface
			void Register(RegisterDto dto)
			bool IsExists(string account)
		
		MemberRepository class 實作 IMemberRepository

		Result class
			IsSuccess, ErrorMessage
			static Result Success();
			static Result Fail(string errorMessage)

		AuthService class
			ctor(IMemberRepository repo)
			Result Register(RegisterDto dto)
			// 未來會再加Login, Logout,..

		------ Copilot 提示詞 ------
		註冊 IMemberRepository, AuthService
