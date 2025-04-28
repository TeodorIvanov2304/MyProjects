using SimpleTaskManagerApp.Data.Models.Models;

namespace SimpleTaskManagerApp.Data.Data.Repositories.Interfaces
{
	public interface ITaskRepository
	{	

		//If include deleted is true, get also the deleted items 
		Task<IEnumerable<AppTask>> GetAllTasksAsync(bool includeDeleted = false);
		Task<AppTask?> GetByIdAsync(Guid id);
		Task AddAsync(AppTask task);
		Task<bool> UpdateAsync(AppTask task);
		Task<bool> DeleteAsync(Guid id);
		Task<int> SaveChangesAsync();
	}
}
