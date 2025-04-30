using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;

namespace SimpleTaskManagerApp.Services.Data
{
	public class AppTaskService : IAppTaskService
	{
		private readonly ITaskRepository _taskRepository;
		private readonly IStatusService _statusService;
		

        public AppTaskService(ITaskRepository taskRepository, IStatusService statusService)
        {
            this._taskRepository = taskRepository;
			this._statusService = statusService;
        }

		public Task CreateAsync(AppTaskCreateViewModel model, string userId)
		{
			throw new NotImplementedException();
		}

		public async Task<IEnumerable<AppTaskListViewModel>> GetAllTasksAsync()
		{
			var tasks = await _taskRepository.GetAllTasksAsync();

			return tasks
				.Where(t => !t.IsDeleted)
				.Select(t => new AppTaskListViewModel
				{
					Id = t.Id.ToString(),
					Title = t.Title,
					StatusName = t.Status.Name,
					DueDate = t.DueDate.ToString()!
				})
				.ToList();
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
