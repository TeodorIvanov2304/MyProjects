using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data;
using SimpleTaskManagerApp.ViewModels.Status;

namespace SimpleTaskManagerApp.Services.Tests
{
	public class StatusTaskServiceTests : IDisposable
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

		}

		[Fact]
		public async Task GetAllStatusesAsync_ShouldReturnAllStatuses()
		{
			// Arrange: Seed test statuses in the in-memory database
			_context.Statuses.AddRange(
					new Status { Id = 1, Name = "Pending" },
					new Status { Id = 2, Name = "In Progress" },
					new Status { Id = 3, Name = "Completed" },
					new Status { Id = 4, Name = "Canceled" }
				);
			await _context.SaveChangesAsync();

			//Act: Fetch all statuses from the service
			List<StatusViewModel> result = (await _statusService.GetAllStatusesAsync()).ToList();

			// Assert: Verify that all expected statuses are returned

			Assert.Equal(4, result.Count);
			Assert.Contains(result, s => s.Name == "Pending");
			Assert.Contains(result, s => s.Name == "In Progress");
			Assert.Contains(result, s => s.Name == "Completed");
			Assert.Contains(result, s => s.Name == "Canceled");
		}

		[Fact]
		public async Task GetAllStatusesAsync_ShouldReturnEmptyList_WhenNoStatusesExist()
		{
			// Arrange: No need to seed statuses – DB is empty

			// Act: Try to fetch statuses
			List<StatusViewModel> result = (await _statusService.GetAllStatusesAsync()).ToList();

			// Assert: Ensure the result is an empty list

			Assert.Empty(result);
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
