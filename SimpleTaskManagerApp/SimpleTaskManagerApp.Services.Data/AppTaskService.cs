using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;

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

		public async Task<IEnumerable<AppTaskListViewModel>> GetAllTasksAsync()
		{
			//Split the query in two, because EF cannot
			//translate ToString("yyyy-MM-dd") in SQL.
			var tasksRaw = await this._dbContext.AppTasks
				.Include(t => t.Status)
				.AsNoTracking()
				.Where(t => !t.IsDeleted)
				.ToListAsync();

			var tasks = tasksRaw
				.Select(t => new AppTaskListViewModel
				{
					Id = t.Id.ToString(),
					Title = t.Title,
					StatusName = t.Status.Name,
					DueDate = t.DueDate?.ToString("MM/dd/yyyy HH:mm") ?? "N/A"
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
	}
}
