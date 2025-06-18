namespace SimpleTaskManagerApp.ViewModels.Administrator
{
	public class AdminUserViewModel
	{
		public string Id { get; set; } = null!;
		public string Email { get; set; } = null!;
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public bool IsAdmin { get; set; }
		public bool IsLockedOut {  get; set; }
	}
}
