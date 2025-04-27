using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleTaskManagerApp.Data.Models.Models
{
    public class AppTask
    {
        [Key]
        [Required]
        [Comment("Unique task identifier")]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Comment("Task title")]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(800)]
        [Comment("Task description")]
        public string Description { get; set; } = null!;

        [Comment("Start date of the task")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Comment("Task due date")]
        public DateTime? DueDate { get; set; }


        [Required]
        [Comment("User identifier")]
        public string UserId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

    }
}
