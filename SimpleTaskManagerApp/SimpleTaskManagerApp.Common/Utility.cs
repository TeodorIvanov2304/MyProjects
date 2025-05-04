namespace SimpleTaskManagerApp.Common
{
	public static class Utility
	{	
		//Checks if StatusId is valid
		public static bool IsIdValid(int? id) => id == null || id < 0;

	}
}
