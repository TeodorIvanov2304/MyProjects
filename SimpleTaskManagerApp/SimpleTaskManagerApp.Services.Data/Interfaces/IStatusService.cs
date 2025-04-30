using SimpleTaskManagerApp.ViewModels.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.Services.Data.Interfaces
{
	public interface IStatusService
	{
		Task<IEnumerable<StatusViewModel>> GetAllStatusesAsync();
	}
}
