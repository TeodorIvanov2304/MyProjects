using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleTaskManagerApp.Services.Data.Interfaces;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles ="Administrator")]
	public class UsersController : Controller
	{
		private readonly IAdministratorService _administratorService;

        public UsersController(IAdministratorService administratorService)
        {
            this._administratorService = administratorService;
        }

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var users = await _administratorService.GetAllUsersAsync();
			return View(users);
		}
	}
}
