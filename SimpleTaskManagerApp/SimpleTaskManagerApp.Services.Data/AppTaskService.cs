using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;
using SimpleTaskManagerApp.Data.Models.Models;

namespace SimpleTaskManagerApp.Services.Data
{
	public class AppTaskService : IAppTaskService
	{
		private readonly ITaskRepository _taskRepository;

        public AppTaskService(ITaskRepository taskRepository)
        {
            this._taskRepository = taskRepository;
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
	}
}
