using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Data.Models.Models.Enums;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;
using SimpleTaskManagerApp.ViewModels.Status;
using SimpleTaskManagerApp.ViewModels.UrgencyLevel;
using static SimpleTaskManagerApp.Common.EntityValidationConstants;
using static SimpleTaskManagerApp.Common.Utility;

namespace SimpleTaskManagerApp.Services.Data
{
	public class AppTaskService : IAppTaskService
	{
		private readonly ITaskRepository _taskRepository;
		private readonly IStatusService _statusService;
		private readonly TaskManagerDbContext _dbContext;
		private readonly ILogEntryService _logEntryService;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUrgencyLevelService _urgencyLevelService;

		public AppTaskService(ITaskRepository taskRepository, IStatusService statusService, TaskManagerDbContext dbContext, ILogEntryService logEntryService, UserManager<ApplicationUser> userManager, IUrgencyLevelService urgencyLevelService)
		{
			this._taskRepository = taskRepository;
			this._statusService = statusService;
			this._dbContext = dbContext;
			this._logEntryService = logEntryService;
			this._userManager = userManager;
			this._urgencyLevelService = urgencyLevelService;
		}

		public async Task CreateAsync(AppTaskCreateViewModel model, string userId)
		{

			//Check for valid status
			if (!Enum.IsDefined(typeof(TaskStatusEnum), model.StatusId))
			{
				throw new InvalidOperationException($"Invalid status ID: {model.StatusId}");
			}

			
			//Check for empty title
			if (string.IsNullOrWhiteSpace(model.Title))
			{
				throw new ArgumentNullException(nameof(model.Title), "Title cannot be null or empty.");
			}

			//Check for empty description
			if (string.IsNullOrWhiteSpace(model.Description))
			{
				throw new ArgumentNullException(nameof(model.Description), "Description cannot be null or empty.");
			}

			var task = new AppTask
			{
				Title = model.Title,
				Description = model.Description,
				DueDate = model.DueDate.ToUniversalTime(),
				StatusId = model.StatusId,
				UserId = userId,
				UrgencyLevelId = model.UrgencyLevelId
			};

			await _taskRepository.AddAsync(task);
			await _taskRepository.SaveChangesAsync();

			var user = await _userManager.FindByIdAsync(userId);

			await _logEntryService.LogAsync(userId, user!.Email ?? "Unknown", "Created task", "Task", model.Title);
		}

		public async Task<IEnumerable<AppTaskListViewModel>> GetAllTasksAsync(Guid userGuid, bool isAdmin)
		{
			//Split the query in two, because EF cannot
			//translate ToString("Date-format") in SQL.
			var query = _dbContext.AppTasks
			.Include(t => t.Status)
			.AsNoTracking()
			.Where(t => !t.IsDeleted);

			//Check for userId/Admin
			if (!isAdmin)
			{
				query = query.Where(t => t.UserId == userGuid.ToString());
			}

			var tasksRaw = await query.ToListAsync();

			var tasks = tasksRaw
				.Select(t => new AppTaskListViewModel
				{
					Id = t.Id.ToString(),
					Title = t.Title,
					StatusName = t.Status.Name,
					CreatedAt = t.CreatedAt.ToString(AllDateFormat),
					DueDate = t.DueDate?.ToString(AllDateFormat) ?? "N/A"
				})
				.ToList();

			return tasks;
		}
		public async Task<AppTaskCreateViewModel> GetCreateViewModelAsync()
		{
			IEnumerable<StatusViewModel> statuses = await _statusService.GetAllStatusesAsync();
			IEnumerable<UrgencyLevelViewModel> urgency = await _urgencyLevelService.GetAllUrgencyLevelsAsync();

			return new AppTaskCreateViewModel
			{
				Statuses = statuses.Select(s => new AppTaskStatusViewModel
				{
					Id = s.Id,
					Name = s.Name
				}),
				UrgencyLevels = urgency 
			};
		}

		public async Task<DetailsAppTaskViewModel?> GetDetailsViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin)
		{
			var task = await this._dbContext.AppTasks
				.Include(t => t.User)
				.Include(t => t.Status)
				.FirstOrDefaultAsync(t => t.Id == taskGuid);

			if (task == null || task.IsDeleted)
			{
				return null;
			}


			if (!isAdmin && task.UserId != userGuid.ToString())
			{
				return null!;
			}

			var detailsTask = new DetailsAppTaskViewModel()
			{
				Username = task.User.FirstName + " " + task.User.LastName,
				Title = task.Title,
				Description = task.Description,
				CreatedAt = task.CreatedAt,
				DueDate = task.DueDate,
				StatusName = task.Status.Name
			};

			return detailsTask;
		}

		public async Task<EditTaskViewModel?> GetEditViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin)
		{
			AppTask? task = await _taskRepository.GetByIdAsync(taskGuid);

			if (task == null || task.IsDeleted)
			{
				return null;
			}


			if (!isAdmin && task.UserId != userGuid.ToString())
			{
				return null!;
			}

			IEnumerable<StatusViewModel> statuses = await _statusService.GetAllStatusesAsync();

			IEnumerable<UrgencyLevelViewModel> urgencyLevels = await _urgencyLevelService.GetAllUrgencyLevelsAsync();

			EditTaskViewModel model = new EditTaskViewModel
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
				}),
				UrgencyLevels = urgencyLevels.Select(u => new UrgencyLevelViewModel
				{
					Id = u.Id,
					Name = u.Name,
					Color = u.Color
				})
			};

			return model;
		}

		public async Task<bool> PostDeleteViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin)
		{
			AppTask? task = await this._taskRepository.GetByIdAsync(taskGuid);

			if (task == null || task.IsDeleted)
			{
				return false;
			}

			if (task.UserId != userGuid.ToString() && !isAdmin)
			{
				return false;
			}

			task.IsDeleted = true;

			ApplicationUser? user = await this._userManager.FindByIdAsync(userGuid.ToString());
			await this._logEntryService.LogAsync(userGuid.ToString(), user!.Email!, "Deleted task", "Task", task.Title);
			
			await this._dbContext.SaveChangesAsync();

			return true;
		}

		public async Task<bool> PostEditViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin, EditTaskViewModel model)
		{
			AppTask? task = await this._taskRepository.GetByIdAsync(taskGuid);

			if (task == null || task.IsDeleted)
			{
				return false;
			}


			if (!isAdmin && task.UserId != userGuid.ToString())
			{
				return false;
			}

			IEnumerable<StatusViewModel> statuses = await this._statusService.GetAllStatusesAsync();

			task.Title = model.Title;
			task.Description = model.Description;
			task.DueDate = EnsureUtc(model.DueDate);
			task.StatusId = model.StatusId;

			try
			{
				await this._taskRepository.SaveChangesAsync();

				ApplicationUser? user = await _userManager.FindByIdAsync(userGuid.ToString());
				await _logEntryService.LogAsync(user!.Id, user.Email!, "Edited task", "Task", task.Title);

			}
			catch (Exception ex)
			{
				Console.WriteLine("SaveChangesAsync ERROR: " + ex.Message);
				throw;
			}

			return true;
		}

		public async Task<IEnumerable<AppTaskViewModel>> GetFilteredTasksAsync(string userId, FilterAppTaskViewModelUser filter, bool isAdmin)
		{
			var query = _dbContext.AppTasks
				.AsNoTracking()
				.Where(t => !t.IsDeleted);

			if (!isAdmin)
			{
				query = query.Where(t => t.User.Id == userId);
			}

			if (!string.IsNullOrWhiteSpace(filter.TitleKeyword))
			{
				query = query.Where(t => t.Title.ToLower().Contains(filter.TitleKeyword.ToLower()));
			}

			if (filter.StatusId.HasValue)
			{
				query = query.Where(t => t.StatusId == filter.StatusId.Value);
			}

			if (filter.CreatedAtFrom.HasValue)
			{
				var fromUtc = DateTime.SpecifyKind(filter.CreatedAtFrom.Value, DateTimeKind.Utc);
				query = query.Where(t => t.CreatedAt >= fromUtc);
			}

			if (filter.CreatedAtTo.HasValue)
			{
				var toUtc = DateTime.SpecifyKind(filter.CreatedAtTo.Value, DateTimeKind.Utc);
				query = query.Where(t => t.CreatedAt <= toUtc);
			}

			if (filter.DueDateFrom.HasValue)
			{
				var fromUtc = DateTime.SpecifyKind(filter.DueDateFrom.Value, DateTimeKind.Utc);
				query = query.Where(t => t.DueDate >= fromUtc);
			}

			if (filter.DueDateTo.HasValue)
			{
				var toUtc = DateTime.SpecifyKind(filter.DueDateTo.Value, DateTimeKind.Utc);
				query = query.Where(t => t.DueDate <= toUtc);
			}

			if (filter.UrgencyLevelId.HasValue)
			{
				query = query.Where(u => u.UrgencyLevelId == filter.UrgencyLevelId);
			}

			return await query
				.OrderByDescending(t => t.CreatedAt)
				.Skip((filter.PageNumber - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.Select(t => new AppTaskViewModel
				{
					Id = t.Id.ToString(),
					Title = t.Title,
					Description = t.Description,
					Status = t.Status.Name,
					CreatedAt = t.CreatedAt,
					DueDate = t.DueDate,
					UrgencyLevelName = t.UrgencyLevel.Name,
					UrgencyLevelColor = t.UrgencyLevel.Color!
				})
				.ToListAsync();
		}

		//Tasks counter
		public async Task<int> GetFilteredTasksCountAsync(string userId, FilterAppTaskViewModelUser filter, bool isAdmin)
		{
			var query = _dbContext.AppTasks
				.AsNoTracking()
				.Where(t => !t.IsDeleted);

			if (!isAdmin)
			{
				query = query.Where(t => t.User.Id == userId);
			}

			if (!string.IsNullOrWhiteSpace(filter.TitleKeyword))
			{
				query = query.Where(t => t.Title.ToLower().Contains(filter.TitleKeyword.ToLower()));
			}

			if (filter.StatusId.HasValue)
			{
				query = query.Where(t => t.StatusId == filter.StatusId.Value);
			}

			if (filter.CreatedAtFrom.HasValue)
			{
				var fromUtc = DateTime.SpecifyKind(filter.CreatedAtFrom.Value, DateTimeKind.Utc);
				query = query.Where(t => t.CreatedAt >= fromUtc);
			}

			if (filter.CreatedAtTo.HasValue)
			{
				var toUtc = DateTime.SpecifyKind(filter.CreatedAtTo.Value, DateTimeKind.Utc);
				query = query.Where(t => t.CreatedAt <= toUtc);
			}

			if (filter.DueDateFrom.HasValue)
			{
				var fromUtc = DateTime.SpecifyKind(filter.DueDateFrom.Value, DateTimeKind.Utc);
				query = query.Where(t => t.DueDate >= fromUtc);
			}

			if (filter.DueDateTo.HasValue)
			{
				var toUtc = DateTime.SpecifyKind(filter.DueDateTo.Value, DateTimeKind.Utc);
				query = query.Where(t => t.DueDate <= toUtc);
			}

			return await query.CountAsync();
		}


	}

}

