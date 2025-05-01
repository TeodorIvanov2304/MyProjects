namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class AppTaskListViewModel
	{
		public string Id { get; set; } = null!;

		public string Title { get; set; } = null!;

		public string StatusName { get; set; } = null!;

		public string CreatedAt { get; set; } = null!;

		public string DueDate { get; set; } = null!;
	}
}
