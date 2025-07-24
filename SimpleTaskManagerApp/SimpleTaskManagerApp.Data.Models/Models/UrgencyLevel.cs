using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static SimpleTaskManagerApp.Common.EntityValidationConstants;

namespace SimpleTaskManagerApp.Data.Models.Models
{
	public class UrgencyLevel
	{
		[Key]
		[Required]
		[Comment("Urgency level Id")]
		public int Id { get; set; }

		[Required]
		[MaxLength(UrgencyLevelNameMaxLength)]
		[Comment("Urgency level name")]
		public string Name { get; set; } = null!;

		[MaxLength(UrgencyLevelColorMaxLength)]
		[Comment("Urgency level color")]
		public string? Color { get; set; }

		[MaxLength(UrgencyLevelDescriptionrMaxLength)]
		[Comment("Urgency level description")]
		public string? Description { get; set; }

		public ICollection<AppTask> Tasks { get; set; } = new List<AppTask>();
	}
}
