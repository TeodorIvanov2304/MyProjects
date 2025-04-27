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
		public const byte AppTaskTitleMaxLength = 100;
		public const int AppTaskDescriptionMaxLength = 800;
	}
}
