namespace SimpleTaskManagerApp.Common
{
	public static class EntityValidationConstants
	{
		//Application user
		public const byte ApplicationUserFirstNameMinLength = 2;
		public const byte ApplicationUserFirstNameMaxLength = 250;
		public const byte ApplicationUserLastNameMinLength = 2;
		public const byte ApplicationUserLastNameMaxLength = 250;

		//AppTask
		public const byte AppTaskTitleMinLength = 3;
		public const byte AppTaskTitleMaxLength = 100;
		public const int AppTaskDescriptionMinLength = 10;
		public const int AppTaskDescriptionMaxLength = 800;

		//Date format
		public const string AllDateFormat = "MM/dd/yyyy HH:mm";

		//Pagination
		public const byte AppTaskIndexPageNumber = 1;
		public const byte AppTaskIndexPageSize = 10;
	}
}
