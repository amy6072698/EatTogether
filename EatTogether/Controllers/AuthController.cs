using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace EatTogether.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
		private readonly JwtHelper _jwtHelper;

		public AuthController(IAuthService authService, JwtHelper jwtHelper)
		{
			_authService = authService;
			_jwtHelper = jwtHelper;
		}

		// GET /Auth/Login
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		// POST /Auth/Login
		[HttpPost]
		public async Task<IActionResult> Login([FromBody] LoginViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				var errors = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.FirstOrDefault();
				return Json(new { success = false, message = errors });
			}

			var result = await _authService.LoginAsync(vm.Account, vm.Password);

			if (!result.IsSuccess)
			{
				return Json(new { success= false, message = result.ErrorMessage });
			}

			var loginDto = result.Value!;

			if (loginDto.MustChangePassword)
			{
				TempData["PendingUserId"] = loginDto.UserId;
				return Json(new { success = true, mustChangePassword = true });
			}

			IssueJwtCookie(loginDto);

			return Json(new { success = true, mustChangePassword = false, redirectUrl = Url.Action("Index", "Home") });
		}

		[HttpPost]
		public async Task<IActionResult> ForceChangePassword([FromBody] ForceChangePasswordViewModel vm)
		{
			// 從 TempData 取出 UserId
			if (TempData["PendingUserId"] is not int userId)
			{
				return Json(new { success = false, message = "操作逾時，請重新登入" });
			}

			// 驗證欄位
			if (!ModelState.IsValid)
			{
				var error = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.FirstOrDefault();
				return Json(new { success = false, message = error });
			}

			// 呼叫 Service
			var result = await _authService.ForceChangePasswordAsync(userId, vm.NewPassword);

			if (!result.IsSuccess)
			{
				return Json(new { success = false, message = result.ErrorMessage });
			}

			// 發行 JWT
			IssueJwtCookie(result.Value!);

			return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
		}

		// POST /Auth/ForgotPassword
		[HttpPost]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				var error = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.FirstOrDefault();
				return Json(new { success = false, message = error });
			}

			await _authService.ForgotPasswordAsync(vm.Email);

			// 不論有無此 Email，一律回傳成功（防帳號枚舉）
			return Json(new { success = true });
		}

		// GET /Auth/ResetPassword?token=xxx
		[HttpGet]
		public async Task<IActionResult> ResetPassword(string token)
		{
			if (string.IsNullOrEmpty(token))
				return RedirectToAction(nameof(ResetPasswordInvalid));

			// 驗證 Token 是否有效
			var isValid = await _authService.ValidateResetTokenAsync(token);
			if (!isValid) return RedirectToAction(nameof(ResetPasswordInvalid));

			ViewBag.Token = token;
			return View();
		}

		// POST /Auth/ResetPassword
		[HttpPost]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordViewModel vm)
		{
			if (!ModelState.IsValid)
			{
				var error = ModelState.Values
					.SelectMany(v => v.Errors)
					.Select(e => e.ErrorMessage)
					.FirstOrDefault();

				return Json(new { success = false, message = error });
			}

			var result = await _authService.ResetPasswordAsync(vm.Token, vm.NewPassword);

			if (!result.IsSuccess)
			{
				return Json(new { success = false, message = result.ErrorMessage });
			}

			return Json(new { success = true });
		}

		// GET /Auth/ResetPasswordInvalid
		[HttpGet]
		public IActionResult ResetPasswordInvalid() => View();

		// POST /Auth/Logout
		[HttpPost]
		public IActionResult Logout()
		{
			// 清除 JWT Cookie
			Response.Cookies.Delete("jwt");

			// Redirect 登入頁（瀏覽器返回按鈕會自動導向登入頁）
			return RedirectToAction("Login");
		}


		private void IssueJwtCookie(LoginDto loginDto)
		{
			var payloadDto = new JwtPayloadDto
			{
				UserId = loginDto.UserId,
				RoleIds = loginDto.RoleIds,
				Name = loginDto.Name,
				RoleNames = loginDto.RoleNames,
				FunctionNames = loginDto.FunctionNames
			};

			var token = _jwtHelper.GenerateToken(payloadDto);

			Response.Cookies.Append("jwt", token, new CookieOptions
			{
				HttpOnly = true,
				Secure = true,
				SameSite = SameSiteMode.Strict,
				Expires = DateTimeOffset.UtcNow.AddHours(8)
			});
		}
	}
}
