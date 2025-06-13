using static SimpleTaskManagerApp.Common.EntityValidationConstants;

namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class FilterAppTaskViewModelUser
	{
		public string? TitleKeyword { get; set; }
		public DateTime? CreatedAtFrom { get; set; }
		public DateTime? CreatedAtTo { get; set; }
		public DateTime? DueDateFrom { get; set; }
		public DateTime? DueDateTo { get; set; }
		public int? StatusId { get; set; }

		//For pagination
		public int PageNumber { get; set; } = AppTaskIndexPageNumber;
		public int PageSize { get; set; } = AppTaskIndexPageSize;
	}
}
