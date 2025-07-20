using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleTaskManagerApp.Controllers;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;
using SimpleTaskManagerApp.ViewModels.Status;
using static SimpleTaskManagerApp.Common.Utility;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
    [Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class TasksController : BaseController
	{


		private readonly IAdministratorService _administratorService;
		private readonly IStatusService _statusService;
		private readonly UserManager<ApplicationUser> _userManager;
		public TasksController(IAdministratorService administratorService, IStatusService statusService, UserManager<ApplicationUser> userManager)
		{
			this._administratorService = administratorService;
			this._statusService = statusService;
			this._userManager = userManager;
		}
		public async Task<IActionResult> Index(FilterAppTaskViewModelAdmin filter)
		{
			CheckPages(filter.PageNumber, filter.PageSize);

			int totalTaskCount = await _administratorService.GetFilteredTaskCountAsync(filter);

			IEnumerable<AdminTaskViewModel> tasks = await _administratorService.GetFilteredTaskAsync(filter);
			IEnumerable<StatusViewModel> statuses = await _statusService.GetAllStatusesAsync();

			IEnumerable<SelectListItem> statusSelectList = statuses.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name
			});

			AdminTasksIndexViewModel model = new AdminTasksIndexViewModel
			{
				Tasks = tasks,
				Filter = filter,
				Statuses = statusSelectList,
				CurrentPage = filter.PageNumber,
				TotalPages = (int)Math.Ceiling(totalTaskCount / (double)filter.PageSize)
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> SoftDelete(string id)
		{
			Guid taskGuid = Guid.Empty;
			bool isValid = IsGuidValid(id, ref taskGuid);
			if (!isValid)
			{
				return NotFound();
			}

			ApplicationUser? user = await _userManager.GetUserAsync(User);

			bool isDeleted = await this._administratorService.SoftDeleteTaskAsync(taskGuid,user!.Id);

			if (!isDeleted)
			{
				return BadRequest("Unable to delete task.");
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeletePermanently(string id)
		{
			Guid taskGuid = Guid.Empty;
			bool isValid = IsGuidValid(id, ref taskGuid);
			if (!isValid)
			{
				return NotFound();
			}

			ApplicationUser? user = await _userManager.GetUserAsync(User);

			bool isDeleted = await this._administratorService.DeleteTaskPermanentlyAsync(taskGuid, user!.Id);

			if (!isDeleted)
			{
				return BadRequest("Unable to delete task.");
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Restore(string id)
		{
			Guid taskGuid = Guid.Empty;
			bool isValid = IsGuidValid(id, ref taskGuid);

			if (!isValid)
			{
				return NotFound();
			}

			bool isRestored = await this._administratorService.RestoreTaskAsync(taskGuid);

			if (!isRestored)
			{
				return BadRequest("Unable to restore task.");
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public async Task<IActionResult> EditPartial(Guid id)
		{
			var model = await _administratorService.GetEditViewModelAsync(id);

			if (model == null)
			{
				return NotFound();
			}

			return PartialView("_EditPartial", model);
		}
	}
}
