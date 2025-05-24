using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.ViewModels.Administrator
{
	public class AdminTaskViewModel
	{
		public string Id { get; set; } = null!;
		public string Title { get; set; } = null!;
		public string Description { get; set; } = null!;
		public DateTime CreatedAt { get; set; }
		public DateTime? DueDate { get; set; }
		public string Status = null!;
		public string CreatedByEmail { get; set; } = null!;
		public bool IsDeleted { get; set; }
	}
}
