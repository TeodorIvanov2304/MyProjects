using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Controllers;
using SimpleTaskManagerApp.Services.Data.Interfaces;

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
            var model  = await _administratorService.GetAllTasksAsync();

			if (!ModelState.IsValid) 
			{
				return NotFound();
			}

			return View(model);
        }
    }
}
