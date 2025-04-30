using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Services.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;

namespace SimpleTaskManagerApp.Controllers
{
	public class TasksController : Controller
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
	}
}
