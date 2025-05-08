using SimpleTaskManagerApp.ViewModels.AppTask;

namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface IAppTaskService
	{
		Task<IEnumerable<AppTaskListViewModel>> GetAllTasksAsync(string userId, bool isAdmin);
		Task<AppTaskCreateViewModel> GetCreateViewModelAsync();
		Task CreateAsync(AppTaskCreateViewModel model, string userId);
	}
}
