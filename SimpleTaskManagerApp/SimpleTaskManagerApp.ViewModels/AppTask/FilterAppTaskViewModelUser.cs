using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class FilterAppTaskViewModelUser
	{
		public string? TitleKeyword { get; set; }
		public DateTime? CreatedAtFrom { get; set; }
		public DateTime? CreatedAtTo { get; set; }
		public DateTime? DueDateFrom { get; set; }
		public DateTime? DueDateTo { get; set; }
		public int? StatusId { get; set; }

		//For pagination
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
