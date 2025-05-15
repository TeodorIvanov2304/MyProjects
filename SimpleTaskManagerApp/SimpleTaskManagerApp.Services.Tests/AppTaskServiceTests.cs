using Microsoft.EntityFrameworkCore;
using Moq;
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
	}
}