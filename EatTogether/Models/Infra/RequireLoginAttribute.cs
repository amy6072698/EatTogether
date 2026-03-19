using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EatTogether.Models.Infra
{
	public class RequireLoginAttribute : Attribute, IAsyncAuthorizationFilter
	{
		public Task OnAuthorizationAsync(AuthorizationFilterContext context)
		{
			var user = context.HttpContext.User;

			if (user?.Identity?.IsAuthenticated != true)
			{
				context.Result = new RedirectToActionResult("Login", "Auth", null);
			}

			return Task.CompletedTask;
		}
	}
}
