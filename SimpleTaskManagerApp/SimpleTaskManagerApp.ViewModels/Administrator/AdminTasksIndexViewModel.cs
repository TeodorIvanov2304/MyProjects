using Microsoft.AspNetCore.Mvc.Rendering;

namespace SimpleTaskManagerApp.ViewModels.Administrator
{
    public class AdminTasksIndexViewModel
	{
		public IEnumerable<AdminTaskViewModel> Tasks { get; set; } = new List<AdminTaskViewModel>();
		public FilterAppTaskViewModelAdmin Filter { get; set; } = new FilterAppTaskViewModelAdmin();
		public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
	}
}
