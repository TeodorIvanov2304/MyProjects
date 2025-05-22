using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class AdministratorController : Controller
	{
		private readonly IAdministratorService _administratorService;

        public AdministratorController(IAdministratorService administratorService)
        {
            this._administratorService = administratorService;
        }

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			
			var model = await this._administratorService.GetDashboardDataAsync();

			if (!ModelState.IsValid)
			{
				return NotFound();
			}

			return View(model);
		}
	}
}
