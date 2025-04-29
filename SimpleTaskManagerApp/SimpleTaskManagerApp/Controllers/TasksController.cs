using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Services.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;

namespace SimpleTaskManagerApp.Controllers
{
	public class TasksController : Controller
	{
		private readonly IAppTaskService _appTaskService;

        public TasksController(IAppTaskService appTaskService)
        {
            this._appTaskService = appTaskService;
        }

		[HttpGet]
        public async Task<IActionResult> Index()
		{	
			var model = await _appTaskService.GetAllTasksAsync();

			return View(model);
		}
	}
}
