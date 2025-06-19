using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;
using SimpleTaskManagerApp.ViewModels.Status;
using static SimpleTaskManagerApp.Common.Utility;

namespace SimpleTaskManagerApp.Controllers
{
	public class TasksController : BaseController
	{
		private readonly IAppTaskService _appTaskService;
		private readonly IStatusService _statusService;
		public TasksController(IAppTaskService appTaskService, IStatusService statusService)
        {
            this._appTaskService = appTaskService;
			this._statusService = statusService;
        }

		[HttpGet]
		public async Task<IActionResult> Index(FilterAppTaskViewModelUser filter)
		{
			string? userId = GetCurrentUserId();

			if (!Guid.TryParse(userId, out Guid userGuid))
			{
				return NotFound();
			}

			CheckPages(filter.PageSize, filter.PageNumber);

			bool isAdmin = User.IsInRole("Administrator");

			int totalTaskCount = await _appTaskService.GetFilteredTasksCountAsync(userId, filter, isAdmin);

			IEnumerable<AppTaskViewModel> tasks = await _appTaskService.GetFilteredTasksAsync(userId, filter, isAdmin);

			IEnumerable<StatusViewModel> statuses = await _statusService.GetAllStatusesAsync();

			IEnumerable<SelectListItem> statusSelectList = statuses.Select(s => new SelectListItem
			{
				Value = s.Id.ToString(),
				Text = s.Name
			});


			UserTasksIndexViewModel model = new UserTasksIndexViewModel
			{
				Filter = filter,
				Statuses = statusSelectList,
				Tasks = tasks,
				CurrentPage = filter.PageNumber,
				TotalPages = (int)Math.Ceiling(totalTaskCount / (double)filter.PageSize)
			};

			return View(model);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Create()
		{
			AppTaskCreateViewModel model = await this._appTaskService.GetCreateViewModelAsync();
			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AppTaskCreateViewModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Statuses = (await _appTaskService.GetCreateViewModelAsync()).Statuses;

				return View(model);
			}

			string? userId = GetCurrentUserId();
			await _appTaskService.CreateAsync(model, userId);

			return RedirectToAction(nameof(Index));
		}


		//GET CreatePartial
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> CreatePartial()
		{
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
			{
				Statuses = (await _appTaskService.GetCreateViewModelAsync()).Statuses
			};


			return PartialView("_CreatePartial", model);
		}

		//POST CreatePartial
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CreatePartial(AppTaskCreateViewModel model)
		{
			if (!ModelState.IsValid)
			{

				model.Statuses = (IEnumerable<AppTaskStatusViewModel>)await this._statusService.GetAllStatusesAsync();

				return PartialView("_CreatePartial", model);
			}

			await _appTaskService.CreateAsync(model, GetCurrentUserId());
			return Ok(); 
		}

		//GET EditPartial
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> EditPartial(string? id)
		{

			Guid taskGuid = Guid.Empty;
			bool isTaskGuidValid = IsGuidValid(id,ref taskGuid);

			if (!isTaskGuidValid)
			{

				//return RedirectToAction("Custom404","Error");
				return NotFound();
			}

			string? userId = GetCurrentUserId();
			Guid userGuid = Guid.Empty;
			bool isUserValid = IsGuidValid(userId,ref userGuid);

			if (!isUserValid)
			{
				
				//return RedirectToAction("Custom404","Error");
				return NotFound();
			}

			bool isAdmin = User.IsInRole("Administrator");

			EditTaskViewModel? model = await this._appTaskService.GetEditViewModelAsync(taskGuid, userGuid, isAdmin);

			if (model == null) 
			{
				return NotFound();
			}
			
			return PartialView("_EditPartial",model);
		}

		//POST EditPartial
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPartial(EditTaskViewModel model)
		{
			if (!ModelState.IsValid) 
			{
				model.Statuses = (IEnumerable<AppTaskStatusViewModel>)await _statusService.GetAllStatusesAsync();
				return PartialView("_EditPartial", model);
			}

			Guid taskGuid = Guid.Empty;
			bool isTaskValid = IsGuidValid(model.Id.ToString(), ref taskGuid);

			if (!isTaskValid)
			{

				//return RedirectToAction("Custom404","Error");
				return NotFound();
			}

			string? userId = GetCurrentUserId();
			Guid userGuid = Guid.Empty;
			bool isUserValid = IsGuidValid(userId, ref userGuid);

			if (!isUserValid)
			{

				//return RedirectToAction("Custom404","Error");
				return NotFound();
			}

			bool isAdmin = User.IsInRole("Administrator");

			var result = await this._appTaskService.PostEditViewModelAsync(taskGuid, userGuid, isAdmin, model);

			if (!result)
			{
				return BadRequest();
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost("Tasks/Delete/{taskId}")]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(Guid taskId)
		{
			string? userId = GetCurrentUserId();
			Guid userGuid = Guid.Empty;
			bool isUserValid = IsGuidValid(userId, ref userGuid);

			if (!isUserValid) 
			{
				return NotFound();
			}

			bool isAdmin = User.IsInRole("Administrator");

			bool isDeleted = await this._appTaskService.PostDeleteViewModelAsync(taskId, userGuid, isAdmin);

			if (!isDeleted)
			{
				return BadRequest("Unable to delete task.");
			}

			return Ok();
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> DetailsPartial(string? id)
		{
			string? userId = GetCurrentUserId();
			Guid userGuid = Guid.Empty;
			bool isUserValid = IsGuidValid(userId, ref userGuid);

			if (!isUserValid)
			{
				return NotFound();
			}

			Guid taskGuid = Guid.Empty;
			bool isTaskValid = IsGuidValid(id, ref taskGuid);

			if (!isTaskValid)
			{

				//return RedirectToAction("Custom404","Error");
				return NotFound();
			}

			bool isAdmin = User.IsInRole("Administrator");

			var model = await this._appTaskService.GetDetailsViewModelAsync(taskGuid, userGuid, isAdmin);

			if (model == null)
			{
				return NotFound();
			}

			return PartialView("_DetailsPartial", model);
		}
	}
}
