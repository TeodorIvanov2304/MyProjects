using SimpleTaskManagerApp.ViewModels.UrgencyLevel;

namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface IUrgencyLevelService
	{
		Task<IEnumerable<UrgencyLevelViewModel>> GetAllAsync();
	}
}
