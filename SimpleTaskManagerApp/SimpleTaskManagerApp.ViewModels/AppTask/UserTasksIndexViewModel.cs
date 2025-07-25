﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class UserTasksIndexViewModel
	{
		public FilterAppTaskViewModelUser Filter { get; set; } = new();
		public IEnumerable<AppTaskViewModel> Tasks { get; set; } = new List<AppTaskViewModel>();
		public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
		public IEnumerable<SelectListItem> UrgencyLevels { get; set; } = new List<SelectListItem>();

		//For pagination
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
	}
}
