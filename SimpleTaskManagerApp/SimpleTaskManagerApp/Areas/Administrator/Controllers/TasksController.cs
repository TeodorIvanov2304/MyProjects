using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Controllers;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using static SimpleTaskManagerApp.Common.Utility;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class TasksController : BaseController
	{


		private readonly IAdministratorService _administratorService;

		public TasksController(IAdministratorService administratorService)
		{
			this._administratorService = administratorService;
		}
		public async Task<IActionResult> Index()
		{
			var model = await _administratorService.GetAllTasksAsync();

			if (!ModelState.IsValid)
			{
				return NotFound();
			}

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

	}
}
