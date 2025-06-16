using static SimpleTaskManagerApp.Common.EntityValidationConstants;

namespace SimpleTaskManagerApp.ViewModels.Administrator
{
	public class FilterUserViewModelAdmin
	{
		public string? EmailKeyword { get; set; }
		public string? FirstNameKeyword { get; set; }
		public string? LastNameKeyword { get; set; }
		public bool? IsAdmin { get; set; }
		public bool? IsLockedOut { get; set; }
		public int PageNumber { get; set; } = UserIndexPageNumber;
		public int PageSize { get; set; } = UserIndexPageSize;

	}
}
