using SimpleTaskManagerApp.ViewModels.Administrator;
using SimpleTaskManagerApp.ViewModels.AppTask;

namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface IAppTaskService
	{
		Task<IEnumerable<AppTaskListViewModel>> GetAllTasksAsync(Guid userGuid, bool isAdmin);
		Task<AppTaskCreateViewModel> GetCreateViewModelAsync();
		Task CreateAsync(AppTaskCreateViewModel model, string userId);
		Task<EditTaskViewModel?> GetEditViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin);
		Task<bool> PostEditViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin, EditTaskViewModel model);
		Task<bool> PostDeleteViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin);
		Task<DetailsAppTaskViewModel?> GetDetailsViewModelAsync(Guid taskGuid, Guid userGuid, bool isAdmin);

		Task<IEnumerable<AppTaskViewModel>> GetFilteredTasksAsync(string userId, FilterAppTaskViewModelUser filter, bool isAdmin);
	}
}
