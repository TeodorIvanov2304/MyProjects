using SimpleTaskManagerApp.ViewModels.UrgencyLevel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SimpleTaskManagerApp.Common.EntityValidationConstants;

namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class EditTaskViewModel
	{
		[Key]
		[Required]
		public Guid Id { get; set; }

		[Required]
		[MinLength(AppTaskTitleMinLength)]
		[MaxLength(AppTaskTitleMaxLength)]
		public string Title { get; set; } = null!;

		[Required]
		[MinLength(AppTaskDescriptionMinLength)]
		[MaxLength(AppTaskDescriptionMaxLength)]
		public string Description { get; set; } = null!;

		[Required]
		[Display(Name = "Due Date")]
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = AllDateFormat, ApplyFormatInEditMode = true)]
		public DateTime? DueDate { get; set; }

		[Required]
		[Display(Name = "Status")]
		public int StatusId { get; set; }

		[Required]
		[Display(Name ="Urgency Level")]
		public int UrgencyLevelId { get; set; }

		public virtual IEnumerable<AppTaskStatusViewModel> Statuses { get; set; } = new List<AppTaskStatusViewModel>();
		public IEnumerable<UrgencyLevelViewModel> UrgencyLevels { get; set; } = new List<UrgencyLevelViewModel>();
	}
}
