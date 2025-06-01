namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class FilterAppTaskViewModel
	{
		public string? TitleKeyword { get; set; }

		public string? CreatedByEmail { get; set; }

		public int? StatusId { get; set; }

		public bool? IsDeleted { get; set; }

		public DateTime? DueDateFrom { get; set; }
		public DateTime? DueDateTo { get; set; }

		public DateTime? CreatedAtFrom { get; set; }
		public DateTime? CreatedAtTo { get; set; }

		public string? SortBy { get; set; } 
		public bool SortDescending { get; set; } = false;
	}
}
