using SimpleTaskManagerApp.ViewModels.Administrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface IAdministratorService
	{
		Task<AdminDashboardViewModel> GetDashboardDataAsync();
		Task<IEnumerable<AdminUserViewModel>> GetAllUsersAsync();
		Task<bool> PromoteToAdminAsync(string userId, string? currentUserId);
	}
}
