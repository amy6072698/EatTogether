using Microsoft.AspNetCore.Mvc;

namespace EatTogether.Controllers
{
	public class AuthController : Controller
	{
		// GET /Auth/Login
		[HttpGet]
		public IActionResult Login() => View();

		// GET /Auth/ResetPassword
		[HttpGet]
		public IActionResult ResetPassword(string token)
		{
			if (string.IsNullOrEmpty(token))
				return RedirectToAction("ResetPasswordInvalid");

			ViewBag.Token = token;
			return View();
		}

		// GET /Auth/ResetPasswordInvalid
		[HttpGet]
		public IActionResult ResetPasswordInvalid() => View();
	}
}
