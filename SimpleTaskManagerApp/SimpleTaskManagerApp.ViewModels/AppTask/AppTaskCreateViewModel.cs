﻿using SimpleTaskManagerApp.ViewModels.UrgencyLevel;
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
		[DataType(DataType.DateTime)]
		[DisplayFormat(DataFormatString = AllDateFormat, ApplyFormatInEditMode = true)]
		public DateTime DueDate { get; set; }

		[Required]
		[Display(Name = "Status")]
		public int StatusId { get; set; }

		[Required]
		[Display(Name = "Urgency level")]
		public int UrgencyLevelId { get; set; }

		public virtual IEnumerable<AppTaskStatusViewModel> Statuses { get; set; } = new List<AppTaskStatusViewModel>();
		public virtual IEnumerable<UrgencyLevelViewModel> UrgencyLevels { get; set; } = new List<UrgencyLevelViewModel>();
	}
}
