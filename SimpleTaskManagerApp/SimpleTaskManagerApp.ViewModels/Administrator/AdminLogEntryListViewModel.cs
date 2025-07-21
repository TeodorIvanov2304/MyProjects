

using SimpleTaskManagerApp.ViewModels.LogEntry;

namespace SimpleTaskManagerApp.ViewModels.Administrator
{
	public class AdminLogEntryListViewModel
	{
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public IEnumerable<LogEntryViewModel> Logs { get; set; } = new List<LogEntryViewModel>();
	}
}
