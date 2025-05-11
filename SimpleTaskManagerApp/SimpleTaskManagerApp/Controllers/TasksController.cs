using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;
using SimpleTaskManagerApp.ViewModels.Status;
using System.Security.Claims;
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
        public async Task<IActionResult> Index()
		{
			string? userId = GetCurrentUserId();

			Guid userGuid = Guid.Empty;

			bool isValid = IsGuidValid(userId,ref userGuid);

			if (!isValid)
			{
				//Soon
				//return RedirectToAction("Custom404","Error");
				return NotFound();
			}

			bool isAdmin = User.IsInRole("Administrator");

			IEnumerable<AppTaskListViewModel> models = await _appTaskService.GetAllTasksAsync(userId, isAdmin);

			return View(models);
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
	}
}
