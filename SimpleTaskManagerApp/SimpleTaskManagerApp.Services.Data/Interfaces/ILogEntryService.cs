using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.ViewModels.LogEntry;

namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface ILogEntryService
	{
		Task LogAsync(string userId, string userEmail, string action, string entityType, string? entityName);
		Task<IEnumerable<LogEntryViewModel>> GetLogsAsync(int page,  int pageSize);
	}
}
