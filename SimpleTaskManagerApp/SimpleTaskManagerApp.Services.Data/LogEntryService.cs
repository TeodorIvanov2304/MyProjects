using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.LogEntry;
using System.ComponentModel.DataAnnotations;

namespace SimpleTaskManagerApp.Services.Data
{
	public class LogEntryService : ILogEntryService
	{
		private readonly TaskManagerDbContext _context;

        public LogEntryService(TaskManagerDbContext context)
        {
            this._context = context;
        }

		public async Task<IEnumerable<LogEntryViewModel>> GetLogsAsync(int page, int pageSize)
		{
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

			return logs;
		}

		public async Task LogAsync(string userId, string userEmail, string action, string entityType, string? entityName)
		{
			LogEntry log = new LogEntry
			{
				UserId = userId,
				UserEmail = userEmail,
				Action = action,
				EntityType = entityType,
				EntityName = entityName
			};

			await _context.LogEntries.AddAsync(log);
			await _context.SaveChangesAsync();
		}
	}
}
