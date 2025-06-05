using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.ViewModels.AppTask
{
	public class AppTaskViewModel
	{
		public int Id { get; set; }

		public string Title { get; set; } = null!;

		public string Description { get; set; } = null!;

		public string Status { get; set; } = null!;

		public DateTime CreatedAt { get; set; }

		public DateTime? DueDate { get; set; }
	}
}
