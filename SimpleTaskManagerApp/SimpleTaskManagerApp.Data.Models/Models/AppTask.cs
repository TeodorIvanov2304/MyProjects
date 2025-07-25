using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SimpleTaskManagerApp.Common.EntityValidationConstants;

namespace SimpleTaskManagerApp.Data.Models.Models
{
	public class AppTask
	{

		[Key]
		[Required]
		//The DB will generate new GUID, to avoid conflict of responsibilities with EF
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Comment("Unique task identifier")]
		public Guid Id { get; set; }

		[Required]
		[MaxLength(AppTaskTitleMaxLength)]
		[Comment("Task title")]
		public string Title { get; set; } = null!;

		[Required]
		[MaxLength(AppTaskDescriptionMaxLength)]
		[Comment("Task description")]
		public string Description { get; set; } = null!;

		[Comment("Start date of the task")]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		[Comment("Task due date")]
		public DateTime? DueDate { get; set; }

		[Required]
		[Comment("Whether the task is soft-deleted")]
		public bool IsDeleted { get; set; } = false;

		[Required]
		[Comment("User identifier")]
		public string UserId { get; set; } = null!;

		[Required]
		[ForeignKey(nameof(UserId))]
		public ApplicationUser User { get; set; } = null!;

		[Required]
		[Comment("Status identifier")]
		public int StatusId { get; set; }

		[Required]
		[ForeignKey(nameof(StatusId))]
		public Status Status { get; set; } = null!;

		[Comment("Urgency level identifier")]
		public int? UrgencyLevelId { get; set; }

		[ForeignKey(nameof(UrgencyLevelId))]
		public UrgencyLevel UrgencyLevel { get; set; } = null!;

	}
}
