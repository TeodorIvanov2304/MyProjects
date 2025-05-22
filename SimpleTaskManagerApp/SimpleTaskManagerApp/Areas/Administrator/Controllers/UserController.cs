using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles ="Administrator")]
	public class UserController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
