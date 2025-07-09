namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface ILogEntryService
	{
		Task LogAsync(string userId, string userEmail, string action, string entityType, string? entityId = null);
	}
}
