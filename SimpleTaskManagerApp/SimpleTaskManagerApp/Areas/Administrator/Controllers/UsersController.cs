using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Controllers;
using SimpleTaskManagerApp.Services.Data.Interfaces;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles ="Administrator")]
	public class UsersController : BaseController
	{
		private readonly IAdministratorService _administratorService;

        public UsersController(IAdministratorService administratorService)
        {
            this._administratorService = administratorService;
        }

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var users = await _administratorService.GetAllUsersAsync();
			return View(users);
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> MakeAdmin(string id)
		{
			string? currentUserId = GetCurrentUserId();

			var result = await _administratorService.PromoteToAdminAsync(id, currentUserId);

			if (result)
			{
				TempData["Success"] = "User promoted to Administrator.";
			}
			else
			{
				TempData["Error"] = "Could not promote user.";
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> RemoveAdmin(string id)
		{
			string? currentUserId = GetCurrentUserId();

			var result = await _administratorService.DemoteFromAdminAsync(id, currentUserId);

			if (result)
			{
				TempData["Success"] = "User demoted from Administrator.";
			}
			else
			{
				TempData["Error"] = "Could not demote user.";
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
