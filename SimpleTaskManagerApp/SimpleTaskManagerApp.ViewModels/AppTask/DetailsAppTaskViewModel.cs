namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class DetailsAppTaskViewModel
	{
		public string Username { get; set; } = null!;
		public string Title { get; set; } = null!;
		public string Description { get; set; } = null!;
		public DateTime CreatedAt { get; set; }
		public DateTime? DueDate { get; set; }
		public string StatusName { get; set; } = null!;
	}
}
