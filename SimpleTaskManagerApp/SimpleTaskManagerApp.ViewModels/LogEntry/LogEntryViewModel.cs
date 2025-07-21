namespace SimpleTaskManagerApp.ViewModels.LogEntry
{
	public class LogEntryViewModel
	{
		public string UserEmail { get; set; } = string.Empty;
		public string Action { get; set; } = string.Empty;
		public string EntityType { get; set; } = string.Empty;
		public string? EntityName { get; set; }
		public DateTime TimeStamp { get; set; }
	}
}
