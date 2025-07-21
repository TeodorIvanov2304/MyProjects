using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Administrator;
using SimpleTaskManagerApp.ViewModels.LogEntry;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class LogsController : Controller
	{
		private readonly TaskManagerDbContext _context;
		private readonly ILogEntryService _logEntryService;
        public LogsController(TaskManagerDbContext context, ILogEntryService logEntryService)
        {
            this._context = context;
			this._logEntryService = logEntryService;
        }
        public async Task<IActionResult> Index(int page = 1,int pageSize = 100)
		{
			int totalLogs = await _context.LogEntries.CountAsync();
			int totalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);

			IEnumerable<LogEntryViewModel> logs = await _logEntryService.GetLogsAsync(page,pageSize);

			AdminLogEntryListViewModel model = new AdminLogEntryListViewModel
			{
				Logs = logs,
				CurrentPage = page,
				TotalPages = totalPages
			};

			return View(model);
		}
	}
}
