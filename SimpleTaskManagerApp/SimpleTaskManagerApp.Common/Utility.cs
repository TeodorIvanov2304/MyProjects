namespace SimpleTaskManagerApp.Common
{
	public static class Utility
	{	
		//Checks if StatusId is valid
		public static bool IsIdValid(int? id) => id == null || id < 0;

		public static bool IsGuidValid(string? id, ref Guid parsedGuid)
		{

			if (String.IsNullOrWhiteSpace(id))
			{
				return false;
			}

			bool isGuidValid = Guid.TryParse(id, out parsedGuid);

			if (!isGuidValid)
			{
				return false;
			}

			return true;
		}
	}
}
