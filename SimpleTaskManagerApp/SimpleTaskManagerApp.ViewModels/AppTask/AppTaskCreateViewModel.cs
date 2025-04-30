using System.ComponentModel.DataAnnotations;
using static SimpleTaskManagerApp.Common.EntityValidationConstants;

namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class AppTaskCreateViewModel
	{
		[Required]
		[MinLength(AppTaskTitleMinLength)]
		[MaxLength(AppTaskTitleMaxLength)]
		public string Title { get; set; } = null!;

		[Required]
		[MinLength(AppTaskDescriptionMinLength)]
		[MaxLength(AppTaskDescriptionMaxLength)]
		public string Description { get; set; } = null!;

		[Required]
		[Display(Name ="Due Date")]
		[DataType(DataType.Date)]
		public DateTime DueDate { get; set; }

		[Required]
		[Display(Name = "Status")]
		public Guid StatusId { get; set; }

		public virtual IEnumerable<AppTaskStatusViewModel> Statuses { get; set; } = new List<AppTaskStatusViewModel>();
	}
}
