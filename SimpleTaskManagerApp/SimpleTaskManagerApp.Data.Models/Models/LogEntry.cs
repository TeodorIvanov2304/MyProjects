using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleTaskManagerApp.Data.Models.Models
{
	public class LogEntry
	{
		[Key]
		[Required]
		//The DB will generate new GUID, to avoid conflict of responsibilities with EF
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Comment("Unique log identifier")]
		public Guid Id { get; set; }

		[Required]
		[Comment("User identifier")]
		public string UserId { get; set; } = null!;

		[Required]
		[EmailAddress]
		[Comment("User email address")]
		public string UserEmail { get; set; } = null!;

		[Required]
		[Comment("Action performed by the user")]
		public string Action { get; set; } = null!;

		[Required]
		[Comment("The type of object that was changed or used.")]
		public string EntityType { get; set; } = null!;

		[Comment("The ID of the object")]
		public string? EntityId { get; set; }

		[Comment("Timestamp of the current log")]
		public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

	}
}
