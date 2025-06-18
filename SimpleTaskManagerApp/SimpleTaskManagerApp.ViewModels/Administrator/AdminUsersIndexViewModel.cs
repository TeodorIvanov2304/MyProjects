namespace SimpleTaskManagerApp.ViewModels.Administrator
{
	public class AdminUsersIndexViewModel
	{
		public IEnumerable<AdminUserViewModel> Users { get; set; } = new List<AdminUserViewModel>();
		public FilterUserViewModelAdmin Filter { get; set; } = new();
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
	}
}
