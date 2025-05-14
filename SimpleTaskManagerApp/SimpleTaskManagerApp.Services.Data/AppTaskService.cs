using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;
using static SimpleTaskManagerApp.Common.EntityValidationConstants;

namespace SimpleTaskManagerApp.Services.Data
{
	public class AppTaskService : IAppTaskService
	{
		private readonly ITaskRepository _taskRepository;
		private readonly IStatusService _statusService;
		private readonly TaskManagerDbContext _dbContext;

		public AppTaskService(ITaskRepository taskRepository, IStatusService statusService, TaskManagerDbContext dbContext)
		{
			this._taskRepository = taskRepository;
			this._statusService = statusService;
			this._dbContext = dbContext;
		}

		public async Task CreateAsync(AppTaskCreateViewModel model, string userId)
		{
			var task = new AppTask
			{
				Title = model.Title,
				Description = model.Description,
				DueDate = model.DueDate.ToUniversalTime(),
				StatusId = model.StatusId,
				UserId = userId
			};

			await _taskRepository.AddAsync(task);
			await _taskRepository.SaveChangesAsync();
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
			var statuses = await this._statusService.GetAllStatusesAsync();

			return new AppTaskCreateViewModel
			{
				Statuses = statuses.Select(s => new AppTaskStatusViewModel
				{
					Id = s.Id,
					Name = s.Name
				})
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

			var detailsTask = new DetailsAppTaskViewModel ()
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
			var task = await this._taskRepository.GetByIdAsync(taskGuid);

			if (task == null || task.IsDeleted)
			{
				return null;
			}

			
			if (!isAdmin && task.UserId != userGuid.ToString())
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

		public async Task<bool> PostDeleteViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin)
		{
			var task = await this._taskRepository.GetByIdAsync(taskGuid);

			if (task == null || task.IsDeleted)
			{
				return false;
			}

			if (task.UserId != userGuid.ToString() && !isAdmin) 
			{
				return false;
			}

			task.IsDeleted = true;

			await this._dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> PostEditViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin, EditTaskViewModel model)
		{
			var task = await this._taskRepository.GetByIdAsync(taskGuid);

			if (task == null || task.IsDeleted)
			{
				return false;
			}


			if (!isAdmin && task.UserId != userGuid.ToString())
			{
				return false!;
			}

			var statuses = await this._statusService.GetAllStatusesAsync();

			task.Id = model.Id;
			task.Title = model.Title;
			task.Description = model.Description;
			task.DueDate = model.DueDate;
			task.StatusId = model.StatusId;

			await this._taskRepository.SaveChangesAsync();

			return true;
		}
	}
}
