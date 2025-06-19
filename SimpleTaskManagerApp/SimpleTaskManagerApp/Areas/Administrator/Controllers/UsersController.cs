using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Controllers;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;
using static SimpleTaskManagerApp.Common.Utility;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class UsersController : BaseController
	{
		private readonly IAdministratorService _administratorService;

		public UsersController(IAdministratorService administratorService)
		{
			this._administratorService = administratorService;
		}

		[HttpGet]
		public async Task<IActionResult> Index(FilterUserViewModelAdmin filter)
		{	
			CheckPages(filter.PageNumber, filter.PageSize);

			var users = await _administratorService.GetFilteredUsersAsync(filter);
			int totalCount = await _administratorService.GetFilteredUsersCountAsync(filter);

			var model = new AdminUsersIndexViewModel
			{
				Users = users,
				Filter = filter,
				CurrentPage = filter.PageNumber,
				TotalPages = (int)Math.Ceiling(totalCount / (double)filter.PageSize)
			};

			return View(users);
		}

		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> MakeAdmin(string id)
		{
			string? currentUserId = GetCurrentUserId();

			bool result = await _administratorService.PromoteToAdminAsync(id, currentUserId);

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

			bool result = await _administratorService.DemoteFromAdminAsync(id, currentUserId);

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

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id)
		{

			Guid userId = Guid.Empty;
			bool isUserValid = IsGuidValid(id, ref userId);

			if (!isUserValid)
			{
				return NotFound();
			}

			bool result = await _administratorService.RemoveUserAsync(id);

			if (!result)
			{
				return NotFound();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LockUser(string id)
		{
			Guid userId = Guid.Empty;
			bool isUserValid = IsGuidValid(id, ref userId);

			if (!isUserValid)
			{
				return NotFound();
			}

			bool result = await _administratorService.LockOnUserAsync(id);

			if (!result)
			{
				return NotFound();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UnlockUser(string id)
		{
			Guid userId = Guid.Empty;
			bool isUserValid = IsGuidValid(id, ref userId);

			if (!isUserValid)
			{
				return NotFound();
			}

			bool result = await _administratorService.UnlockUserAsync(id);

			if (!result)
			{
				return NotFound();
			}

			return RedirectToAction(nameof(Index));
		}
	}
}
