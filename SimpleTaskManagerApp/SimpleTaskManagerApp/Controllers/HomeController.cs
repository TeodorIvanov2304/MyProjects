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
		private readonly ILogEntryService _logService;
		public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ILogEntryService logService)
		{
			_logger = logger;
			_userManager = userManager;
			_logService = logService;
		}

		//Redirect after login
		[Route("home/redirect")]
		public async Task<IActionResult> RedirectAfterLogin()
		{
			if (User.Identity == null || !User.Identity.IsAuthenticated)
			{
				//User not logged in
				return RedirectToAction("Index", "Home");
			}

			ApplicationUser? user = await _userManager.GetUserAsync(User);

			if (user == null)
			{
				return RedirectToAction("Index", "Home");
			}

			bool isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
			string message = isAdmin ? "Administrator logged in" : "User logged in";
			TempData["WelcomeBack"] = $"Welcome back, {user.UserName}!";

			await _logService.LogAsync(user.Id, user.Email ?? "", message, "Authentication", user.UserName);

			if (isAdmin)
			{
				return RedirectToAction("Index", "Administrator", new { area = "Administrator" });
			}
			else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		//Regular user Index
		public IActionResult Index()
		{

			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				ViewBag.WelcomeBackMessage = TempData["WelcomeBack"];
			}
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Terms()
		{
			return View();
		}

		public IActionResult Contacts()
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
