using Microsoft.AspNetCore.Mvc.Rendering;
using SimpleTaskManagerApp.ViewModels.AppTask;

namespace SimpleTaskManagerApp.ViewModels.Administrator
{
	public class AdminTasksIndexViewModel
	{
		public IEnumerable<AdminTaskViewModel> Tasks { get; set; } = new List<AdminTaskViewModel>();
		public FilterAppTaskViewModel Filter { get; set; } = new FilterAppTaskViewModel();
		public IEnumerable<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
	}
}
