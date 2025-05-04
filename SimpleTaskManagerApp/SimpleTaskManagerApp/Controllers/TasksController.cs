using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;
using System.Security.Claims;

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
			var model = await _appTaskService.GetAllTasksAsync();

			return View(model);
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Create()
		{
			var model = await this._appTaskService.GetCreateViewModelAsync();
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

			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
			await _appTaskService.CreateAsync(model, userId);

			return RedirectToAction(nameof(Index));
		}


		//CreatePartial
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> CreatePartial()
		{
			var model = new AppTaskCreateViewModel
			{
				Statuses = (await _appTaskService.GetCreateViewModelAsync()).Statuses
			};


			return PartialView("_CreatePartial", model);
		}
	}
}
