using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
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
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
			{
				Title = "Test Task",
				Description = "Test Description",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string userId = Guid.NewGuid().ToString();

			// Act: Attempt to create the task
			await _appTaskService.CreateAsync(model, userId);

			// Assert: Verify task was added
			int taskCount = await _context.AppTasks.CountAsync();
			Assert.Equal(1, taskCount);
		}

		[Fact]
		public async Task CreateAsync_ShouldNotAdd_WhenStatusIdIsInvalid()
		{
			// Arrange: Prepare a model with an invalid status ID
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
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
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
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
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
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
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			List<AppTask> tasks = new List<AppTask>
		{
			new AppTask { Id = Guid.NewGuid(), Title = "Task 1", Description = "Description 1", UserId = userId, Status = new Status { Name = "New" } },
			new AppTask { Id = Guid.NewGuid(), Title = "Task 2", Description = "Description 2", UserId = userId, Status = new Status { Name = "In Progress" } }
		};

			await _context.AddRangeAsync(tasks);
			await _context.SaveChangesAsync();

			// Act: Fetch tasks for that user
			IEnumerable<AppTaskListViewModel> result = await _appTaskService.GetAllTasksAsync(userGuid, isAdmin);

			// Assert: Ensure both tasks are returned
			Assert.Equal(2, result.Count());
			Assert.Contains(result, t => t.Title == "Task 1");
		}

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnAllTasksForAdmin()
		{
			// Arrange: Create tasks for two different users
			string user1Id = Guid.NewGuid().ToString();
			string user2Id = Guid.NewGuid().ToString();

			AppTaskCreateViewModel model1 = new AppTaskCreateViewModel
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

			Guid userGuid = Guid.Parse(user1Id);
			bool isAdmin = true;

			// Act: Admin fetches all tasks
			IEnumerable<AppTaskListViewModel> result = await _appTaskService.GetAllTasksAsync(userGuid, isAdmin);

			// Assert: Ensure both tasks are visible to the admin
			Assert.Equal(2, result.Count());
		}

		[Fact]
		public async Task GetAllTasksAsync_IfUserHasNoTasksShouldReturnEmptyList()
		{
			//Assert: Create user Guid
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			//Act: fetch all user tasks
			IEnumerable<AppTaskListViewModel> result = await _appTaskService.GetAllTasksAsync(userGuid, isAdmin);

			//Assert: Ensure the result is empty, because user has no tasks
			Assert.Empty(result);

		}

		[Fact]
		public async Task GetAllTasksAsync_AdminWithNoTasksInSystem_ShouldReturnEmptyList()
		{
			// Arrange: New user GUID (without tasks in the system)
			string adminUserId = Guid.NewGuid().ToString();
			Guid adminUserGuid = Guid.Parse(adminUserId);
			bool isAdmin = true;

			// Act: fetch all tasks as admin
			IEnumerable<AppTaskListViewModel> result = await _appTaskService.GetAllTasksAsync(adminUserGuid, isAdmin);

			// Assert: Ensure the result is empty
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnEmptyList_WhenUserIsNotAdminAndOwnsNoTasks()
		{
			//Arrange: Create new model and user, and another user
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
			{
				Title = "Task 1",
				Description = "Test description 1",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string ownerUserId = Guid.NewGuid().ToString();
			await _appTaskService.CreateAsync(model, ownerUserId);

			//Create second user, who has no tasks and is not admin
			string anotherUserId = Guid.NewGuid().ToString();
			Guid anotherUserGuid = Guid.Parse(anotherUserId);
			bool isAdmin = false;

			//Act: fetch all tasks of the second user
			IEnumerable<AppTaskListViewModel> result = await _appTaskService.GetAllTasksAsync(anotherUserGuid, isAdmin);

			//Assert: Ensure that task list is empty
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnResults_WhenUserIsNotCreatorButIsAdmin()
		{
			// Arrange: Create two tasks with different non-admin user ID-s
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
			{
				Title = "Task 1",
				Description = "Test description 1",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string modelUserId = Guid.NewGuid().ToString();
			await _appTaskService.CreateAsync(model, modelUserId);

			AppTaskCreateViewModel anotherModel = new AppTaskCreateViewModel
			{
				Title = "Task 2",
				Description = "Test description 2",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string anotherModelUserId = Guid.NewGuid().ToString();
			await _appTaskService.CreateAsync(anotherModel, anotherModelUserId);

			// Arrange: Create an admin user who is not the creator of either task
			string adminUserId = Guid.NewGuid().ToString();
			Guid adminGuid = Guid.Parse(adminUserId);
			bool isAdmin = true;

			// Act: Fetch all tasks as the admin user
			IEnumerable<AppTaskListViewModel> result = await _appTaskService.GetAllTasksAsync(adminGuid, isAdmin);

			// Assert: Ensure both tasks are returned for the admin
			Assert.Equal(2, result.Count());
			Assert.Contains(result, t => t.Title == "Task 1");
			Assert.Contains(result, t => t.Title == "Task 2");
		}

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnEmpty_WhenUserGuidIsEmptyAndNotAdmin()
		{
			// Arrange: Create a valid task with a real user
			string userId = Guid.NewGuid().ToString();

			AppTaskCreateViewModel model = new AppTaskCreateViewModel()
			{
				Title = "Valid Task",
				Description = "Test Description",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			await _appTaskService.CreateAsync(model, userId);

			// Act: Attempt to retrieve tasks with Guid.Empty as user ID and not admin
			var result = await _appTaskService.GetAllTasksAsync(Guid.Empty, isAdmin: false);

			// Assert: No tasks should be returned for invalid user
			Assert.Empty(result);
		}

		[Fact]
		public async Task GetAllTasksAsync_ShouldIncludeStatusNameInResult()
		{
			// Arrange: Create a task with known status
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			AppTaskCreateViewModel model = new AppTaskCreateViewModel()
			{
				Title = "Status Test Task",
				Description = "Checking status name in DTO",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1 //"Pending"
			};

			await _appTaskService.CreateAsync(model, userId);

			// Act: Retrieve tasks for that user
			IEnumerable<AppTaskListViewModel> result = await _appTaskService.GetAllTasksAsync(userGuid, isAdmin);

			// Assert: Ensure status name is included and correct
			AppTaskListViewModel? task = result.FirstOrDefault();
			Assert.NotNull(task);
			Assert.Equal("Pending", task.StatusName);
		}

		// -------------------------
		// GET CREATE VIEW MODEL ASYNC TESTS
		// -------------------------

		[Fact]
		public async Task GetCreateViewModelAsync_ShouldReturnStatusesInViewModel()
		{
			//Act
			var result = await _appTaskService.GetCreateViewModelAsync();

			Assert.NotNull(result);
			Assert.NotNull(result.Statuses);
			Assert.True(result.Statuses.Any());
			Assert.Contains(result.Statuses, s => s.Name == "Pending");
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