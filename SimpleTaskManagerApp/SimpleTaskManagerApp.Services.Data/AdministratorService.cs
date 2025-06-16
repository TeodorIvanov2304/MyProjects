using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;
using SimpleTaskManagerApp.ViewModels.AppTask;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using static SimpleTaskManagerApp.Common.Utility;

namespace SimpleTaskManagerApp.Services.Data
{
    public class AdministratorService : IAdministratorService
	{
		private readonly TaskManagerDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IStatusService _statusService;
        public AdministratorService(TaskManagerDbContext context, UserManager<ApplicationUser> userManager, IStatusService statusService)
        {
            this._context = context;
			this._userManager = userManager;
			this._statusService = statusService;
        }

		//Administrator
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

		//Users
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
					IsAdmin = roles.Contains("Administrator"),
					IsLockedOut = await _userManager.IsLockedOutAsync(user),
				};

				result.Add(userToAdd);
			}

			return result;
		}


		public async Task<bool> PromoteToAdminAsync(string userId, string? currentUserId)
		{
			Guid userGuid = Guid.Empty;
			bool isUserValid = IsGuidValid(userId, ref userGuid);

			if (!isUserValid || userId == currentUserId) 
			{
				return false;
			}

			ApplicationUser? user = await _userManager.FindByIdAsync(userId);
			bool isUserAdmin = await _userManager.IsInRoleAsync(user!, "Administrator");


			if (user == null || isUserAdmin) 
			{
				return false;
			}

			//Try to promote user in role Administrator
			IdentityResult result = await _userManager.AddToRoleAsync(user, "Administrator");

			return result.Succeeded;
		}

		public async Task<bool> DemoteFromAdminAsync(string userId, string? currentUserId)
		{
			Guid userGuid = Guid.Empty;
			bool isUserValid = IsGuidValid(userId, ref userGuid);

			if (!isUserValid || userId == currentUserId)
			{
				return false;
			}

			ApplicationUser? user = await _userManager.FindByIdAsync(userId);
			bool isUserAdmin = await _userManager.IsInRoleAsync(user!, "Administrator");


			if (user == null || !isUserAdmin)
			{
				return false;
			}

			IdentityResult result = await _userManager.RemoveFromRoleAsync(user, "Administrator");

			return result.Succeeded;
		}

		public async Task<bool> RemoveUserAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) 
			{
				return false;
			}

			await _userManager.DeleteAsync(user);

			return true;
		}

		public async Task<bool> LockOnUserAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if(user == null)
			{
				return false;
			}

			//Block for maximum time
			await _userManager.SetLockoutEndDateAsync(user,DateTimeOffset.MaxValue);

			return true;
		}

		public async Task<bool> UnlockUserAsync(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);

			if (user == null) 
			{
				return false;
			}

			//Unlock the user now
			await _userManager.SetLockoutEndDateAsync(user, null);

			return true;
		}

		//Tasks
		public async Task<IEnumerable<AdminTaskViewModel>> GetAllTasksAsync()
		{
			List<AppTask> tasks = await _context.AppTasks
				.AsNoTracking()
				.Include(u => u.User)
				.Include(s => s.Status)
				.ToListAsync();

			List<AdminTaskViewModel> result = new List<AdminTaskViewModel>();

			foreach (var task in tasks)
			{
				var taskToAdd = new AdminTaskViewModel
				{
					Id = task.Id.ToString(),
					Title = task.Title,
					Description = task.Description,
					CreatedAt = task.CreatedAt,
					DueDate = task.DueDate,
					Status = task.Status.Name,
					IsDeleted = task.IsDeleted,
					CreatedByEmail = task.User.Email!

				};

				result.Add(taskToAdd);
			}
			return result;
		}

		public async Task<bool> DeleteTaskPermanentlyAsync(Guid id)
		{
			AppTask? task = await this._context.AppTasks.FindAsync(id);

			if (task == null || !task.IsDeleted) 
			{
				return false;
			}

			this._context.AppTasks.Remove(task);
			await this._context.SaveChangesAsync();

			//Future logger
			//_logger.LogInformation("Task with ID {TaskId} permanently deleted by admin.", id);
			return true;
		}

		public async Task<bool> SoftDeleteTaskAsync(Guid id)
		{
			var task = await this._context.AppTasks.FindAsync(id);

			if (task == null || task.IsDeleted)
			{
				return false;
			}

			task.IsDeleted = true;
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<bool> RestoreTaskAsync(Guid id)
		{
			var task = await this._context.AppTasks.FindAsync(id);

			if(task == null || !task.IsDeleted)
			{
				return false;
			}

			task.IsDeleted = false;
			await _context.SaveChangesAsync();

			return true;
		}

		public async Task<EditTaskViewModel?> GetEditViewModelAsync(Guid id)
		{
			var task = await _context.AppTasks.FindAsync(id);

			if(task == null)
			{
				return null!;
			}

			var statuses = await this._statusService.GetAllStatusesAsync();

			var model = new EditTaskViewModel
			{
				Id = task.Id,
				Title = task.Title,
				Description = task.Description,
				DueDate = task.DueDate,
				StatusId = task.StatusId,
				Statuses = statuses.Select(s => new AppTaskStatusViewModel
				{
					Id = s.Id,
					Name = s.Name
				})
			};

			return model;
		}

		public async Task<IEnumerable<AdminTaskViewModel>> GetFilteredTaskAsync(FilterAppTaskViewModelAdmin filter)
		{
			IQueryable<AppTask> query = _context.AppTasks
				.AsNoTracking()
				.Include(t => t.Status)
				.Include(t => t.User)
				.AsQueryable();

			if (!String.IsNullOrWhiteSpace(filter.TitleKeyword))
			{
				query = query.Where(t => t.Title.Contains(filter.TitleKeyword));
			}

			if (!String.IsNullOrWhiteSpace(filter.CreatedByEmail))
			{
				query = query.Where(t => t.User.Email!.Contains(filter.CreatedByEmail));
			}

			if (filter.StatusId.HasValue)
			{
				query = query.Where(t => t.StatusId == filter.StatusId);
			}

			if (filter.IsDeleted.HasValue)
			{
				query = query.Where(t => t.IsDeleted == filter.IsDeleted.Value);
			}

			if (filter.CreatedAtFrom.HasValue)
			{
				DateTime fromUtc = DateTime.SpecifyKind(filter.CreatedAtFrom.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.CreatedAt >= fromUtc);
			}

			if (filter.CreatedAtTo.HasValue)
			{
				DateTime toUtc = DateTime.SpecifyKind(filter.CreatedAtTo.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.CreatedAt <= toUtc);
			}

			if (filter.DueDateFrom.HasValue)
			{
				DateTime fromUtc = DateTime.SpecifyKind(filter.DueDateFrom.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.DueDate >= fromUtc);
			}

			if (filter.DueDateTo.HasValue)
			{
				DateTime toUtc = DateTime.SpecifyKind(filter.DueDateTo.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.DueDate <= toUtc);
			}

			List<AdminTaskViewModel> tasks = await query
			   .OrderByDescending(t => t.CreatedAt)
			   .Select(t => new AdminTaskViewModel
			   {
				   Id = t.Id.ToString(),
				   Title = t.Title,
				   Description = t.Description,
				   Status = t.Status.Name,
				   CreatedAt = t.CreatedAt,
				   DueDate = t.DueDate,
				   IsDeleted = t.IsDeleted,
				   CreatedByEmail = t.User.Email!
			   })
			   .ToListAsync();

			return tasks;
		}

		//Task counter
		public async Task<int> GetFilteredTaskCountAsync(FilterAppTaskViewModelAdmin filter)
		{
			IQueryable<AppTask> query = _context.AppTasks.AsNoTracking();

			if (!string.IsNullOrWhiteSpace(filter.TitleKeyword))
			{
				query = query.Where(t => t.Title.Contains(filter.TitleKeyword));
			}

			if (!string.IsNullOrWhiteSpace(filter.CreatedByEmail))
			{
				query = query.Where(t => t.User.Email!.Contains(filter.CreatedByEmail));
			}

			if (filter.StatusId.HasValue)
			{
				query = query.Where(t => t.StatusId == filter.StatusId);
			}

			if (filter.IsDeleted.HasValue)
			{
				query = query.Where(t => t.IsDeleted == filter.IsDeleted.Value);
			}

			if (filter.CreatedAtFrom.HasValue)
			{
				var fromUtc = DateTime.SpecifyKind(filter.CreatedAtFrom.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.CreatedAt >= fromUtc);
			}

			if (filter.CreatedAtTo.HasValue)
			{
				var toUtc = DateTime.SpecifyKind(filter.CreatedAtTo.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.CreatedAt <= toUtc);
			}

			if (filter.DueDateFrom.HasValue)
			{
				var fromUtc = DateTime.SpecifyKind(filter.DueDateFrom.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.DueDate >= fromUtc);
			}

			if (filter.DueDateTo.HasValue)
			{
				var toUtc = DateTime.SpecifyKind(filter.DueDateTo.Value, DateTimeKind.Utc).ToUniversalTime();
				query = query.Where(t => t.DueDate <= toUtc);
			}

			return await query.CountAsync();
		}
	}
}
