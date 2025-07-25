﻿using SimpleTaskManagerApp.ViewModels.Administrator;
using SimpleTaskManagerApp.ViewModels.AppTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
    public interface IAdministratorService
	{	
		//Administrator
		Task<AdminDashboardViewModel> GetDashboardDataAsync();

		//Users
		Task<IEnumerable<AdminUserViewModel>> GetFilteredUsersAsync(FilterUserViewModelAdmin filter);
		Task<bool> PromoteToAdminAsync(string userId, string? currentUserId);
		Task<bool> DemoteFromAdminAsync(string userId, string? currentUserId);
		Task<bool> RemoveUserAsync(string userId);
		Task<bool> LockOnUserAsync(string userId);
		Task<bool> UnlockUserAsync(string userId);

		//Tasks
		Task<IEnumerable<AdminTaskViewModel>> GetAllTasksAsync();
		Task<bool> DeleteTaskPermanentlyAsync(Guid id, string userId);
		Task<bool> SoftDeleteTaskAsync(Guid id, string userId);
		Task<bool> RestoreTaskAsync(Guid id, string userId);
		Task<EditTaskViewModel?> GetEditViewModelAsync(Guid id);
		Task<IEnumerable<AdminTaskViewModel>> GetFilteredTaskAsync(FilterAppTaskViewModelAdmin filter);

		Task<int> GetFilteredTaskCountAsync(FilterAppTaskViewModelAdmin filter);

		Task<int> GetFilteredUsersCountAsync(FilterUserViewModelAdmin filter);
	}
}
