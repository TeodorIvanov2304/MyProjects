using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class UserTasksIndexViewModel
	{
		public FilterAppTaskViewModelUser Filter { get; set; } = new();
		public IEnumerable<AppTaskListViewModel> Tasks { get; set; } = new List<AppTaskListViewModel>();
	}
}
