using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data;

namespace SimpleTaskManagerApp.Services.Tests
{
	public class StatusTaskServiceTests
	{
		private readonly TaskManagerDbContext _context;
		private readonly StatusTaskService _statusService;

        public StatusTaskServiceTests()
        {
			// Set up a fresh in-memory database for each test run
			var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			_context = new TaskManagerDbContext(options);
			_statusService = new StatusTaskService(_context);

			// Seed statuses
			if (!_context.Statuses.Any())
			{
				_context.Statuses.AddRange(
					new Status { Id = 1, Name = "Pending" },
					new Status { Id = 2, Name = "In Progress" },
					new Status { Id = 3, Name = "Completed" }
				);
				_context.SaveChanges();
			}
		}




		// -------------------------
		// Cleanup
		// -------------------------

		public void Dispose()
		{
			// Delete the in-memory database after every test
			_context.Database.EnsureDeleted();

			// Release database resources after every test
			_context.Dispose();
		}
	}
}
