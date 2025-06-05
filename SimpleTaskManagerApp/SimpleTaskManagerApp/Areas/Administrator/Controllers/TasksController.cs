using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleTaskManagerApp.Controllers;
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
		public TasksController(IAdministratorService administratorService, IStatusService statusService)
		{
			this._administratorService = administratorService;
			this._statusService = statusService;
		}
		public async Task<IActionResult> Index(FilterAppTaskViewModelAdmin filter)
		{
			var tasks = await _administratorService.GetFilteredTaskAsync(filter);
			var statuses = await _statusService.GetAllStatusesAsync();

			var statusSelectList = statuses.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name
			});

			var model = new AdminTasksIndexViewModel
			{
				Tasks = tasks,
				Filter = filter,
				Statuses = statusSelectList
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

			bool isDeleted = await this._administratorService.SoftDeleteTaskAsync(taskGuid);

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

			bool isDeleted = await this._administratorService.DeleteTaskPermanentlyAsync(taskGuid);

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
