using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using System.Diagnostics;

namespace SimpleTaskManagerApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;

		public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager)
		{
			_logger = logger;
			_userManager = userManager;
		}

		public async Task<IActionResult> Index()
		{
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				var user = await _userManager.GetUserAsync(User);
				if (await _userManager.IsInRoleAsync(user!, "Administrator"))
				{
					return RedirectToAction("Index", "Administrator", new { area = "Administrator" });
				}
			}

			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
