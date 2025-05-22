using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;

namespace SimpleTaskManagerApp.Services.Data
{
	public class AdministratorService : IAdministratorService
	{
		private readonly TaskManagerDbContext _context;

        public AdministratorService(TaskManagerDbContext context)
        {
            this._context = context;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
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

			return model;
		}
	}
}
