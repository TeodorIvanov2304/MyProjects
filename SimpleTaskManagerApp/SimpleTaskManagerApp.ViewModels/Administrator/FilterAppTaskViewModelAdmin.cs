using static SimpleTaskManagerApp.Common.EntityValidationConstants;
namespace SimpleTaskManagerApp.ViewModels.Administrator
{
    public class FilterAppTaskViewModelAdmin
    {
        public string? TitleKeyword { get; set; }
        public string? CreatedByEmail { get; set; }
        public int? StatusId { get; set; }
		public int? UrgencyLevelId { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;

		//For pagination
		public int PageNumber { get; set; } = AppTaskIndexPageNumber;
		public int PageSize { get; set; } = AppTaskIndexPageSize;
	}
}
