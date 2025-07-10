using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;

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
        public async Task<IActionResult> Index()
		{
			var logs = await _context.LogEntries
				.OrderByDescending(l => l.TimeStamp)
				.Take(100)
				.ToListAsync();

			return View(logs);
		}
	}
}
