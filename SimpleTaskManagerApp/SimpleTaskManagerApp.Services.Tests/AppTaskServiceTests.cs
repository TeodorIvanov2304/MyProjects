using Microsoft.EntityFrameworkCore;
using Moq;
using NuGet.DependencyResolver;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Data.Repositories;
using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.AppTask;

namespace SimpleTaskManagerApp.Services.Tests
{
	public class AppTaskServiceTests : IDisposable
	{
		private readonly TaskManagerDbContext _context;
		private readonly ITaskRepository _taskRepository;
		private readonly IStatusService _statusService;
		private readonly AppTaskService _appTaskService;

		public AppTaskServiceTests()
		{
			// Create an in-memory DbContext with a unique database name (using Guid)
			var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			// Initialize the database context
			_context = new TaskManagerDbContext(options);

			// Initialize the task repository
			_taskRepository = new TaskRepository(_context);

			// Initialize the status service
			_statusService = new StatusTaskService(_context);

			// Initialize the AppTaskService
			_appTaskService = new AppTaskService(
				_taskRepository,
				_statusService,
				_context
			);

			// Seed default statuses if not already present
			if (!_context.Statuses.Any())
			{
				_context.Statuses.AddRange(
					new Status { Id = 1, Name = "Pending" },
					new Status { Id = 2, Name = "In Progress" },
					new Status { Id = 3, Name = "Completed" },
					new Status { Id = 4, Name = "Canceled" }
				);

				_context.SaveChanges();
			}
		}

		// -------------------------
		// CREATE ASYNC TESTS
		// -------------------------

		[Fact]
		public async Task CreateAsync_ShouldAddTaskToDatabase_WhenValidInput()
		{
			// Arrange: Prepare a valid task creation model
			var model = new AppTaskCreateViewModel
			{
				Title = "Test Task",
				Description = "Test Description",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			var userId = Guid.NewGuid().ToString();

			// Act: Attempt to create the task
			await _appTaskService.CreateAsync(model, userId);

			// Assert: Verify task was added
			var taskCount = await _context.AppTasks.CountAsync();
			Assert.Equal(1, taskCount);
		}

		[Fact]
		public async Task CreateAsync_ShouldNotAdd_WhenStatusIdIsInvalid()
		{
			// Arrange: Prepare a model with an invalid status ID
			var model = new AppTaskCreateViewModel
			{
				Title = "Bad Status Task",
				Description = "Invalid Status",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 999
			};

			string userId = Guid.NewGuid().ToString();

			// Act & Assert: Expect an exception due to invalid status ID
			await Assert.ThrowsAsync<InvalidOperationException>(() => _appTaskService.CreateAsync(model, userId));
		}

		[Fact]
		public async Task CreateAsync_ShouldNotAdd_WhenTitleIsNullOrEmpty()
		{
			// Arrange: Prepare a model with an empty title
			var model = new AppTaskCreateViewModel
			{
				Title = "",
				Description = "Empty or null title",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string userId = Guid.NewGuid().ToString();

			// Act & Assert: Expect an exception due to null or empty title
			await Assert.ThrowsAsync<ArgumentNullException>(() => _appTaskService.CreateAsync(model, userId));
		}

		[Fact]
		public async Task CreateAsync_ShouldNotAdd_WhenDescriptionIsNullOrEmpty()
		{
			// Arrange: Prepare a model with an empty description
			var model = new AppTaskCreateViewModel
			{
				Title = "Bad description",
				Description = "",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string userId = Guid.NewGuid().ToString();

			// Act & Assert: Expect an exception due to null or empty description
			await Assert.ThrowsAsync<ArgumentNullException>(() => _appTaskService.CreateAsync(model, userId));
		}

		// -------------------------
		// GET ALL ASYNC TESTS
		// -------------------------

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnTasksForUser()
		{
			// Arrange: Add tasks for a specific user
			var userId = Guid.NewGuid().ToString();
			var userGuid = Guid.Parse(userId);
			var isAdmin = false;

			var tasks = new List<AppTask>
		{
			new AppTask { Id = Guid.NewGuid(), Title = "Task 1", Description = "Description 1", UserId = userId, Status = new Status { Name = "New" } },
			new AppTask { Id = Guid.NewGuid(), Title = "Task 2", Description = "Description 2", UserId = userId, Status = new Status { Name = "In Progress" } }
		};

			await _context.AddRangeAsync(tasks);
			await _context.SaveChangesAsync();

			// Act: Fetch tasks for that user
			var result = await _appTaskService.GetAllTasksAsync(userGuid, isAdmin);

			// Assert: Ensure both tasks are returned
			Assert.Equal(2, result.Count());
			Assert.Contains(result, t => t.Title == "Task 1");
		}

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnAllTasksForAdmin()
		{
			// Arrange: Create tasks for two different users
			var user1Id = Guid.NewGuid().ToString();
			var user2Id = Guid.NewGuid().ToString();

			var model1 = new AppTaskCreateViewModel
			{
				Title = "Task 1",
				Description = "Test description 1",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			await _appTaskService.CreateAsync(model1, user1Id);

			var model2 = new AppTaskCreateViewModel
			{
				Title = "Task 2",
				Description = "Test description 2",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			await _appTaskService.CreateAsync(model2, user2Id);
			await _context.SaveChangesAsync();

			var userGuid = Guid.Parse(user1Id);
			var isAdmin = true;

			// Act: Admin fetches all tasks
			var result = await _appTaskService.GetAllTasksAsync(userGuid, isAdmin);

			// Assert: Ensure both tasks are visible to the admin
			Assert.Equal(2, result.Count());
		}

		// -------------------------
		// Cleanup
		// -------------------------

		public void Dispose()
		{
			// Delete the in-memory database
			_context.Database.EnsureDeleted();

			// Release database resources
			_context.Dispose();
		}
	}
}