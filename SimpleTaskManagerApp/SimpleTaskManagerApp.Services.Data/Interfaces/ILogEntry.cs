namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface ILogEntry
	{
		Task LogAsync(string userId, string userEmail, string action, string entityType, string? entityId = null);
	}
}
