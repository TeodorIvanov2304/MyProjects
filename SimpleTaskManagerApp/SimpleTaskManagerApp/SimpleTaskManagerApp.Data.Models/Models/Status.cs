using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SimpleTaskManagerApp.Data.Models.Models
{
	public class Status
	{
		[Key]
		[Required]
		[Comment("Status Id")]
		public int Id { get; set; }

		[Required]
		[MaxLength(50)]
		[Comment("Name of the task status")]
		public string Name { get; set; } = null!;

		public virtual ICollection<AppTask> Tasks { get; set; } = new HashSet<AppTask>();
	}
}
