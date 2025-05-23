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
		Task<bool> DemoteFromAdminAsync(string userId, string? currentUserId);
		Task<bool> RemoveUserAsync(string userId);
		Task<bool> LockOnUserAsync(string userId);
		Task<bool> UnlockUserAsync(string userId);
	}
}
