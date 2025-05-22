using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;

namespace SimpleTaskManagerApp.Services.Data
{
	public class AdministratorService : IAdministratorService
	{
		private readonly TaskManagerDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
        public AdministratorService(TaskManagerDbContext context, UserManager<ApplicationUser> userManager)
        {
            this._context = context;
			this._userManager = userManager;
        }

		public async Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync()
		{
			List<ApplicationUser> users = await _userManager.Users.ToListAsync();

			HashSet<AdminUserViewModel> result = new();

			foreach (ApplicationUser user in users) 
			{
				IList<string> roles = await _userManager.GetRolesAsync(user);
				AdminUserViewModel userToAdd = new AdminUserViewModel
				{	
					Id = user.Id,
					Email = user.Email!,
					FirstName = user.FirstName!,
					LastName = user.LastName!,
					IsAdmin = roles.Contains("Administrator")
				};

				result.Add(userToAdd);
			}

			return result;
		}

		public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
		{
			int totalUsers = await _context.Users.CountAsync();
			int totalTasks = await _context.AppTasks.CountAsync();
			int completedTasks = await _context.AppTasks.CountAsync(t => t.Status.Name == "Completed");

			AdminDashboardViewModel model = new AdminDashboardViewModel
			{
				TotalUsers = totalUsers,
				TotalTasks = totalTasks,
				CompletedTasks = completedTasks
			};

			return model;
		}
	}
}
