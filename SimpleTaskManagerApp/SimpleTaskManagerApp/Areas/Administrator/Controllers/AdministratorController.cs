using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Controllers;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class AdministratorController : BaseController
	{
		private readonly IAdministratorService _administratorService;
		private readonly UserManager<ApplicationUser> _userManager;

        public AdministratorController(IAdministratorService administratorService, UserManager<ApplicationUser> userManager)
        {
            this._administratorService = administratorService;
			this._userManager = userManager;
        }

		[HttpGet]
		public async Task<IActionResult> Index()
		{

			AdminDashboardViewModel model = await this._administratorService.GetDashboardDataAsync();

			if (!ModelState.IsValid)
			{
				return NotFound();
			}

			ViewBag.WelcomeMessage = TempData["WelcomeAdminName"];
			return View(model);
		}

		
	}
}
