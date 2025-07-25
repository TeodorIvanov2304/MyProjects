using System.ComponentModel.DataAnnotations;

namespace SimpleTaskManagerApp.ViewModels.UrgencyLevel
{
	public class UrgencyLevelViewModel
	{
		public int Id { get; set; }

		[Display(Name = "Urgency Name")]
		public string Name { get; set; } = null!;

		[Display(Name = "Color")]
		public string? Color { get; set; }

		[Display(Name = "Description")]
		public string? Description { get; set; }
	}
}
