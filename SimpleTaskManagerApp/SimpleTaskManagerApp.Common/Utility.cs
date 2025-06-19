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

		//Ensure DateTime is UTC
		public static DateTime? EnsureUtc(DateTime? dt)
		{
			if (!dt.HasValue)
			{
				return null;
			}

			return DateTime.SpecifyKind(dt.Value, DateTimeKind.Utc);
		}
		
		public static void CheckPages(int pageNumber, int pageSize)
		{
			if(pageNumber <= 0)
			{
				pageNumber = 1;
			}

			if (pageSize <= 0 || pageSize > 100) 
			{
				pageSize = 10;
			}
		}
	}
}
