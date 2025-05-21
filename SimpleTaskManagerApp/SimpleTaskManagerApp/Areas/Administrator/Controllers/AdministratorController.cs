using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.ViewModels.Administrator;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class AdministratorController : Controller
	{
		private readonly TaskManagerDbContext _context;

        public AdministratorController(TaskManagerDbContext context)
        {
            this._context = context;
        }

        public async Task<IActionResult> Index()
		{
			var totalUsers = await _context.Users.CountAsync();
			var totalTasks = await _context.AppTasks.CountAsync();
			var completedTasks = await _context.AppTasks.CountAsync(t => t.Status.Name == "Completed");

			var model = new AdminDashboardViewModel
			{
				TotalUsers = totalUsers,
				TotalTasks = totalTasks,
				CompletedTasks = completedTasks
			};

			return View(model);
		}
	}
}
