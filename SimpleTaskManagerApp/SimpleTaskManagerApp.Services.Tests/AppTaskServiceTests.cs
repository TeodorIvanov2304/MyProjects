using Microsoft.EntityFrameworkCore;
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
		// GET DETAILS VIEW MODEL ASYNC TESTS
		// ----

		[Fact]
		public async Task GetDetailsViewModelAsync_ShouldReturnDetails_WhenUserIsOwner()
		{
			// Arrange: Create a user and a task owned by that user
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			DateTime createdAt = DateTime.UtcNow;
			bool isAdmin = false;

			// Add a user so we can test username generation
			AppTask task = new AppTask()
			{
				Id = Guid.NewGuid(),
				Title = "Task Title",
				Description = "Task Description",
				CreatedAt = createdAt,
				DueDate = createdAt.AddDays(1),
				UserId = userId,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Petersen"
				}
			};

			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Call the service to get the task details for the user (who is not admin but is the owner)
			var result = await _appTaskService.GetDetailsViewModelAsync(task.Id, userGuid, isAdmin);

			// Assert: Ensure the result contains the correct data from the task
			Assert.NotNull(result);
			Assert.IsType<DetailsAppTaskViewModel>(result);
			Assert.Equal("Peter Petersen", result.Username);
			Assert.Equal("Task Title", result.Title);
			Assert.Equal("Task Description", result.Description);
			Assert.Equal(createdAt, result.CreatedAt);
			Assert.Equal(createdAt.AddDays(1), result.DueDate);
			Assert.Equal("Pending", result.StatusName);
		}

		[Fact]
		public async Task GetDetailsViewModel_ShouldReturnNull_WhenUserIsNotOwnerAndNotAdmin()
		{
			// Arrange: Create a task that belongs to a different user
			string userId = Guid.NewGuid().ToString();     
			
			//Craete another user
			Guid anotherUserGuid = Guid.NewGuid();
			DateTime createdAt = DateTime.UtcNow;
			bool isAdmin = false;

			// Create a task with the specified owner
			AppTask task = new AppTask()
			{
				Id = Guid.NewGuid(),
				Title = "Private Task",
				Description = "Private Task Description",
				CreatedAt = createdAt,
				DueDate = createdAt.AddDays(1),
				UserId = userId,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Petersen"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Try to get task details as a user who is not the owner and not an admin
			var result = await _appTaskService.GetDetailsViewModelAsync(task.Id, anotherUserGuid, isAdmin);

			// Assert: The method should return null because the user is unauthorized to view this task
			Assert.Null(result);
		}

		[Fact]
		public async Task GetDetailsViewModelAsync_ShouldReturnDetails_WhenUserIsAdmin()
		{
			// Arrange: Create a regular user and an admin user
			string userId = Guid.NewGuid().ToString();
			Guid adminGuid = Guid.NewGuid();
			DateTime createdAt = DateTime.UtcNow;
			bool isAdmin = true;

			// Create a task assigned to the regular user
			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Admin Task",
				Description = "Admin Task Description",
				CreatedAt = createdAt,
				DueDate = createdAt.AddDays(1),
				UserId = userId,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Petersen"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Fetch task details as admin
			var result = await _appTaskService.GetDetailsViewModelAsync(task.Id, adminGuid, isAdmin);

			// Assert: Ensure the result is correct and accessible by admin
			Assert.NotNull(result);
			Assert.Equal("Admin Task", result.Title);
			Assert.Equal("Admin Task Description", result.Description);
			Assert.Equal("Pending", result.StatusName);
			Assert.Equal("Peter Petersen", result.Username);
		}

		[Fact]
		public async Task GetDetailsViewModelAsync_ShouldReturnNull_WhenTaskIsDeleted()
		{
			// Arrange: Create a user and a soft-deleted task assigned to them
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			DateTime createdAt = DateTime.UtcNow;
			bool isAdmin = false;

			var task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Deleted Task",
				Description = "Deleted Task Description",
				CreatedAt = createdAt,
				DueDate = createdAt.AddDays(1),
				UserId = userId,
				IsDeleted = true, // Soft delete
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Petersen"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Try to fetch the details as a regular user
			var result = await _appTaskService.GetDetailsViewModelAsync(task.Id, userGuid, isAdmin);

			// Assert: The task should not be returned because it is marked as deleted
			Assert.Null(result);

		}


		// -------------------------
		// POST DELETE VIEW MODEL ASYNC TESTS
		// ----


		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnTrue_WhenUserIsOwner()
		{
			// Arrange: Create a user and a task assigned to them
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Task to delete",
				Description = "To be deleted",
				UserId = userId,
				IsDeleted = false,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Petersen"
				}
			};

			// Save task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Attempt to delete the task as the owner
			bool result = await _appTaskService.PostDeleteViewModelAsync(task.Id, userGuid, isAdmin);

			// Assert: The deletion should be successful
			Assert.True(result);

			// Assert: Task should be marked as deleted in the database (soft delete)
			AppTask taskToDelete = await _context.AppTasks.FirstAsync(t => t.Id == task.Id);
			Assert.True(taskToDelete.IsDeleted);
		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnTrue_WhenUserIsAdmin()
		{
			// Arrange: Create a user and a task assigned to him, create Admin user
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);

			Guid adminGuid = Guid.NewGuid();
			bool isAdmin = true;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Admin Task delete",
				Description = "To be deleted by Admin",
				UserId = userId,
				IsDeleted = false,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Petersen"
				}
			};

			// Save task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Attempt to delete the task as Admin
			bool result = await _appTaskService.PostDeleteViewModelAsync(task.Id, adminGuid, isAdmin);

			// Assert: The deletion should be successful
			Assert.True(result);

			// Assert: Task should be marked as deleted in the database (soft delete)
			AppTask taskToDelete = await _context.AppTasks.FirstAsync(t => t.Id == task.Id);
			Assert.True(taskToDelete.IsDeleted);
		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnFalse_WhenTaskIsNotExisting()
		{
			// Arrange: Create a valid user and a task assigned to them
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Task to not delete",
				Description = "Not to be deleted",
				UserId = userId,
				IsDeleted = false,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Petersen"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Use a different (non-existent) task ID
			Guid nonExistingTaskId = Guid.NewGuid();

			// Act: Attempt to delete a task that doesn't exist
			bool result = await _appTaskService.PostDeleteViewModelAsync(nonExistingTaskId, userGuid, isAdmin);

			// Assert: Method should return false since task doesn't exist
			Assert.False(result);

			// Assert: The existing task should not be marked as deleted
			AppTask existingTask = await _context.AppTasks.FirstAsync(t => t.Id == task.Id);
			Assert.False(existingTask.IsDeleted);
		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnFalse_WhenTaskDoesNotExist_AndUserIsAdmin()
		{
			// Arrange: Create a valid admin user
			string adminUserId = Guid.NewGuid().ToString();
			Guid adminGuid = Guid.Parse(adminUserId);
			bool isAdmin = true;

			// Create and save a valid task with a different user
			string ownerUserId = Guid.NewGuid().ToString();
			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Existing Task",
				Description = "Should not be affected",
				UserId = adminUserId,
				IsDeleted = false,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = adminUserId,
					FirstName = "John",
					LastName = "Doe"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Generate a non-existing task ID
			Guid nonExistingTaskId = Guid.NewGuid();

			// Act: Try to delete a task that doesn't exist
			bool result = await _appTaskService.PostDeleteViewModelAsync(nonExistingTaskId, adminGuid, isAdmin);

			// Assert: Should return false, since task doesn't exist
			Assert.False(result);

			// Assert: The existing task should remain unaffected
			AppTask existingTask = await _context.AppTasks.FirstAsync(t => t.Id == task.Id);
			Assert.False(existingTask.IsDeleted);
		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnFalse_WhenTaskIsDeleted()
		{
			// Arrange: Create a user and a task that is already marked as deleted
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Already deleted Task",
				Description = "Should not be affected",
				UserId = userId,
				IsDeleted = true, // <- already deleted
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Pederson"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Attempt to delete an already deleted task
			bool result = await _appTaskService.PostDeleteViewModelAsync(task.Id, userGuid, isAdmin);

			// Assert: Should return false
			Assert.False(result);

			// Assert: Ensure the task is still marked as deleted
			AppTask? taskFromDb = await _context.AppTasks.FindAsync(task.Id);
			Assert.True(taskFromDb?.IsDeleted);
		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnFalse_WhenAdminTriesToDeleteAlreadyDeletedTask()
		{
			// Arrange: Create a user and a task that is already marked as deleted
			string adminId = Guid.NewGuid().ToString();
			Guid adminGuid = Guid.Parse(adminId);
			bool isAdmin = true;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Already deleted Task",
				Description = "Should not be affected",
				UserId = adminId,
				IsDeleted = true,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = adminId,
					FirstName = "Peter",
					LastName = "Pederson"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Attempt to delete an already deleted task with admin
			bool result = await _appTaskService.PostDeleteViewModelAsync(task.Id, adminGuid, isAdmin);

			// Assert: Should return false
			Assert.False(result);

			// Assert: Ensure the task is still marked as deleted
			AppTask? taskFromDb = await _context.AppTasks.FindAsync(task.Id);
			Assert.True(taskFromDb?.IsDeleted);
		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnFalse_WhenUserIsNotOwnerOrAdmin()
		{
			// Arrange: Create a task owned by a different user
			string ownerUserId = Guid.NewGuid().ToString();
			Guid nonOwnerGuid = Guid.NewGuid(); // another user
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Restricted Task",
				Description = "Should not be deleted by other users",
				UserId = ownerUserId,
				IsDeleted = false, // Task is not deleted yet
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = ownerUserId,
					FirstName = "Peter",
					LastName = "Pederson"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: Try to delete the task with a user who is neither the owner nor an admin
			bool result = await _appTaskService.PostDeleteViewModelAsync(task.Id, nonOwnerGuid, isAdmin);

			// Assert: Should return false
			Assert.False(result);

			// Assert: Task should still not be marked as deleted
			AppTask? taskFromDb = await _context.AppTasks.FindAsync(task.Id);
			Assert.False(taskFromDb?.IsDeleted);

		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnFalse_WhenTaskIdIsEmpty()
		{
			// Arrange: create new user
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = true;

			// Act: Try deleting task with empty task.Id
			bool result = await _appTaskService.PostDeleteViewModelAsync(Guid.Empty, userGuid, isAdmin);

			// Assert: Ensure that result is false
			Assert.False(result);
		}

		[Fact]
		public async Task PostDeleteViewModelAsync_ShouldReturnFalse_WhenUserIdIsEmpty()
		{
			// Arrange: Create a task owned by a user
			string userId = Guid.NewGuid().ToString();
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Restricted Task",
				Description = "Should not be deleted by other users",
				UserId = userId,
				IsDeleted = false,
				StatusId = 1,
				Status = await _context.Statuses.FirstAsync(s => s.Id == 1),
				User = new ApplicationUser
				{
					Id = userId,
					FirstName = "Peter",
					LastName = "Pederson"
				}
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Act: try to delete the task with empty user Guid
			bool result = await _appTaskService.PostDeleteViewModelAsync(task.Id, Guid.Empty, isAdmin);

			//Assert: Ensure that the service returns false result
			Assert.False(result);
		}

		// -------------------------
		// POST EDIT VIEW MODEL ASYNC TESTS
		// ----


		[Fact]
		public async Task PostEditViewModelAsync_ShouldEditTask_WhenUserIsOwner()
		{
			// Arrange: Setup user, task and model with new data
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			AppTask originalTask = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Original Title",
				Description = "Original Description",
				DueDate = DateTime.UtcNow.AddDays(2),
				StatusId = 1,
				UserId = userId,
				IsDeleted = false
			};

			await _context.AppTasks.AddAsync(originalTask);
			await _context.SaveChangesAsync();

			EditTaskViewModel updatedModel = new EditTaskViewModel
			{
				Id = originalTask.Id,
				Title = "Updated Title",
				Description = "Updated Description",
				DueDate = DateTime.UtcNow.AddDays(5),
				StatusId = 2
			};

			// Act: Try to edit the model
			bool  result = await _appTaskService.PostEditViewModelAsync(originalTask.Id, userGuid, isAdmin, updatedModel);

			// Assert: Ensure that update is successful
			Assert.True(result);

			AppTask? updatedTask = await _taskRepository.GetByIdAsync(originalTask.Id);
			Assert.Equal("Updated Title", updatedTask?.Title);
			Assert.Equal("Updated Description", updatedTask?.Description);
			Assert.Equal(updatedModel.DueDate, updatedTask?.DueDate);
			Assert.Equal(2, updatedTask?.StatusId);
		}

		[Fact]
		public async Task PostEditViewModelAsync_ShouldEditTask_WhenUserIsAdmin()
		{
			// Arrange: Setup user, task and model with new data
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = true;

			//Create admin ID
			Guid adminGuid = Guid.NewGuid();

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "User Title",
				Description = "User Description",
				DueDate = DateTime.UtcNow.AddDays(2),
				StatusId = 1,
				UserId = userId,
				IsDeleted = false
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			//Edit the view model
			EditTaskViewModel updatedModel = new EditTaskViewModel
			{
				Id = task.Id,
				Title = "Admin Title",
				Description = "Admin Description",
				DueDate = DateTime.UtcNow.AddDays(5),
				StatusId = 2
			};

			//Act: Try to edit the model with admin ID
			bool result = await _appTaskService.PostEditViewModelAsync(task.Id, adminGuid, isAdmin, updatedModel);

			//Assert: Ensure that updates are successful
			Assert.True(result);

			//Asser: Ensure that values are correct
			AppTask? updatedTask = await _taskRepository.GetByIdAsync(task.Id);
			Assert.Equal("Admin Title", updatedTask?.Title);
			Assert.Equal("Admin Description", updatedTask?.Description);
			Assert.Equal(updatedTask?.DueDate, task.DueDate);
			Assert.Equal(2, updatedTask?.StatusId);
		}

		[Fact]
		public async Task PostEditViewModelAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
		{
			// Arrange: Create a user and a valid task
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "User Title",
				Description = "User Description",
				DueDate = DateTime.UtcNow.AddDays(2),
				StatusId = 1,
				UserId = userId,
				IsDeleted = false
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			//Prepare an edited view model 
			EditTaskViewModel updatedModel = new EditTaskViewModel
			{
				Id = task.Id,
				Title = "Edited Title",
				Description = "Edited Description",
				DueDate = DateTime.UtcNow.AddDays(5),
				StatusId = 2
			};

			//Use a non-existent task ID 
			Guid nonExistentTaskId = Guid.Empty;

			//Act: Attempt to edit a task that does not exist
			bool result = await _appTaskService.PostEditViewModelAsync(nonExistentTaskId, userGuid, isAdmin, updatedModel);

			//Assert: The edit should fail since the task with the provided ID doesn't exist
			Assert.False(result);
		}

		[Fact]
		public async Task PostEditViewModelAsync_ShouldReturnFalse_WhenTaskIsDeleted()
		{
			// Arrange: Create a user and a task that is already soft-deleted
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Some Title",
				Description = "Some Description",
				DueDate = DateTime.UtcNow.AddDays(2),
				StatusId = 1,
				UserId = userId,
				IsDeleted = true
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Prepare an updated view model with new values
			EditTaskViewModel updatedModel = new EditTaskViewModel
			{
				Id = task.Id,
				Title = "Edited Title",
				Description = "Edited Description",
				DueDate = DateTime.UtcNow.AddDays(5),
				StatusId = 2
			};

			// Act: Attempt to edit a task that is already marked as deleted
			bool result = await _appTaskService.PostEditViewModelAsync(task.Id, userGuid, isAdmin, updatedModel);

			// Assert: Editing should fail because the task is deleted
			Assert.False(result);

			// Assert: The task should still be marked as deleted in the database
			AppTask? taskFromDb = await _context.AppTasks.FindAsync(task.Id);
			Assert.True(taskFromDb?.IsDeleted);

			// Assert: Ensure that task details were not changed
			Assert.Equal("Some Title", taskFromDb?.Title);
			Assert.Equal("Some Description", taskFromDb?.Description);
			Assert.Equal(1, taskFromDb?.StatusId);
		}

		[Fact]
		public async Task PostEditViewModelAsync_ShouldReturnFalse_WhenUserIsNotOwnerAndIsNotAdmin()
		{
			// Arrange: Create a valid task for a specific user, and another user who is neither the owner nor an admin
			string userId = Guid.NewGuid().ToString();
			Guid userGuid = Guid.Parse(userId);
			Guid anotherUserGuid = Guid.NewGuid();
			bool isAdmin = false;

			AppTask task = new AppTask
			{
				Id = Guid.NewGuid(),
				Title = "Valid User Title",
				Description = "Valid User Description",
				DueDate = DateTime.UtcNow.AddDays(2),
				StatusId = 1,
				UserId = userId,
				IsDeleted = false
			};

			// Save the task to the in-memory database
			await _context.AppTasks.AddAsync(task);
			await _context.SaveChangesAsync();

			// Prepare an updated view model with new values
			EditTaskViewModel updatedModel = new EditTaskViewModel
			{
				Id = task.Id,
				Title = "Invalid User Title",
				Description = "Invalid User Description",
				DueDate = DateTime.UtcNow.AddDays(5),
				StatusId = 2
			};

			// Act: Attempt to update the task with a user who is neither the owner nor an admin
			bool result = await _appTaskService.PostEditViewModelAsync(task.Id, anotherUserGuid, isAdmin, updatedModel);

			// Assert: Editing should fail because the user is unauthorized
			Assert.False(result);

			// Assert: The task should remain unchanged in the database
			AppTask? updatedTask = await _taskRepository.GetByIdAsync(task.Id);
			Assert.Equal("Valid User Title", updatedTask?.Title);
			Assert.Equal("Valid User Description", updatedTask?.Description);
			Assert.Equal(1, updatedTask?.StatusId);
			Assert.Equal(task.DueDate, updatedTask?.DueDate);
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