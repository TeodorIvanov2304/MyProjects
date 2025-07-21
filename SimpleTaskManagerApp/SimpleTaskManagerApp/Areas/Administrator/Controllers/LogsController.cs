using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.ViewModels.Administrator;
using SimpleTaskManagerApp.ViewModels.LogEntry;

namespace SimpleTaskManagerApp.Areas.Administrator.Controllers
{
	[Area("Administrator")]
	[Authorize(Roles = "Administrator")]
	public class LogsController : Controller
	{
		private readonly TaskManagerDbContext _context;

        public LogsController(TaskManagerDbContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index(int page = 1,int pageSize = 100)
		{
			int totalLogs = await _context.LogEntries.CountAsync();
			int totalPages = (int)Math.Ceiling(totalLogs / (double)pageSize);

			List<LogEntryViewModel> logs = await _context.LogEntries
				.OrderByDescending(l => l.TimeStamp)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Select(l => new LogEntryViewModel
				{
					UserEmail = l.UserEmail,
					Action = l.Action,
					EntityType = l.EntityType,
					EntityName = l.EntityName,
					TimeStamp = l.TimeStamp
				})
				.ToListAsync();

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
