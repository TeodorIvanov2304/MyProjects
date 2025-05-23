 using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SimpleTaskManagerApp.Controllers
{
	[Authorize]
	public class BaseController : Controller
	{
		protected string GetCurrentUserId()
		{
			var user = User?.FindFirstValue(ClaimTypes.NameIdentifier);

			if (user == null)
			{
				return string.Empty;
			}

			return user;
		}
	}
}
