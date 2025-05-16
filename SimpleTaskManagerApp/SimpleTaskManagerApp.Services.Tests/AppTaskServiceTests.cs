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
	public class AppTaskServiceTests
	{
		private readonly TaskManagerDbContext _context;
		private readonly ITaskRepository _taskRepository;
		private readonly IStatusService _statusService;
		private readonly AppTaskService _appTaskService;

		public AppTaskServiceTests()
		{	
			//Create InMemory DbContext, not the real Db
			var options = new DbContextOptionsBuilder<TaskManagerDbContext>()
				.UseInMemoryDatabase("TestDb_NoMoq")
				.Options;

			_context = new TaskManagerDbContext(options);

			_taskRepository = new TaskRepository(_context);
			_statusService = new StatusTaskService(_context);

			_appTaskService = new AppTaskService(
				_taskRepository,
				_statusService,
				_context
			);
		}


		//CREATE ASYNC

		[Fact]
		public async Task CreateAsync_ShouldAddTaskToDatabase_WhenValidInput()
		{
			// Arrange
			var model = new AppTaskCreateViewModel
			{
				Title = "Test Task",
				Description = "Test Description",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			var userId = Guid.NewGuid().ToString();

			// Act
			await _appTaskService.CreateAsync(model, userId);

			// Assert
			var taskCount = await _context.AppTasks.CountAsync();
			Assert.Equal(1, taskCount);
		}

		[Fact]
		public async Task CreateAsync_ShouldNotAdd_WhenStatusIdIsInvalid()
		{	
			//Arrange
			var model = new AppTaskCreateViewModel
			{
				Title = "Bad Status Task",
				Description = "Invalid Status",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 999
			};

			string userId = Guid.NewGuid().ToString();

			//Act + Assert
			await Assert.ThrowsAsync<InvalidOperationException>(() => _appTaskService.CreateAsync(model, userId));
		}

		[Fact]
		public async Task CreateAsync_ShouldNotAdd_WhenTitleIsNullOrEmpty()
		{
			//Arrange
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
			{
				Title = "",
				Description = "Empty or null title",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string userId = Guid.NewGuid().ToString();

			await Assert.ThrowsAsync<ArgumentNullException>(() => _appTaskService.CreateAsync(model, userId));
		}

		[Fact]
		public async Task CreateAsync_ShouldNotAdd_WhenDescriptionIsNullOrEmpty()
		{
			//Arrange
			AppTaskCreateViewModel model = new AppTaskCreateViewModel
			{
				Title = "Bad description",
				Description = "",
				DueDate = DateTime.Now.AddDays(1),
				StatusId = 1
			};

			string userId = Guid.NewGuid().ToString();

			//Act + Assert
			await Assert.ThrowsAsync<ArgumentNullException>(() => _appTaskService.CreateAsync(model, userId));
		}


		//GET ALL ASYNC

		[Fact]
		public async Task GetAllTasksAsync_ShouldReturnTasksForUser()
		{
			//Arrange 

			var userId = Guid.NewGuid().ToString();
			var userGuid = Guid.Parse(userId);
			var isAdmin = false;

			var tasks = new List<AppTask>() 
			{
				new AppTask { Id = Guid.NewGuid(), Title = "Task 1", Description = "Description 1",UserId = userId, Status = new Status { Name = "New" } },
				new AppTask { Id = Guid.NewGuid(), Title = "Task 2", Description = "Description 2",UserId = userId, Status = new Status { Name = "In Progress" } }
			};

			await _context.AddRangeAsync(tasks);
			await _context.SaveChangesAsync();

			//Act
			var result = await _appTaskService.GetAllTasksAsync(userGuid, isAdmin);


			//Assert
			Assert.Equal(2, result.Count());
			Assert.Contains(result, t => t.Title == "Task 1");
		}
	}
}