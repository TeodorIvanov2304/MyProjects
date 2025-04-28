using SimpleTaskManagerApp.Data.Models.Models;

namespace SimpleTaskManagerApp.Data.Data.Repositories.Interfaces
{
	public interface ITaskRepository
	{
		Task<IEnumerable<AppTask>> GetAllTasksAsync();
		Task<AppTask?> GetByIdAsync(Guid id);
		Task AddAsync(AppTask task);
		Task UpdateAsync(AppTask task);
		Task DeleteAsync(Guid id);
		Task SaveChangesAsync();
	}
}
