using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Data.Models.Models;

namespace SimpleTaskManagerApp.Data.Data.Repositories
{
	public class TaskRepository : ITaskRepository
	{
		private readonly TaskManagerDbContext _dbContext;


        public TaskRepository(TaskManagerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

		public async Task<IEnumerable<AppTask>> GetAllTasksAsync(bool includeDeleted = false)
		{
			IQueryable<AppTask> query = this._dbContext.AppTasks.AsNoTracking();

			if (!includeDeleted)
			{
				query = query.Where(t => !t.IsDeleted);
			}

			return await query.ToListAsync();
		}

		public async Task AddAsync(AppTask task)
		{
			await this._dbContext.AppTasks.AddAsync(task);
			await this._dbContext.SaveChangesAsync();
		}

		public async Task<bool> DeleteAsync(Guid id)
		{
			AppTask? entity = await this.GetByIdAsync(id);

			if (entity is null)
			{
				return false;
			}

			entity.IsDeleted = true;
			await UpdateAsync(entity);

			return true;
		}

		public async Task<bool> DeletePermanently(Guid id)
		{
			AppTask? entity = await this.GetByIdAsync(id);

			if(entity == null)
			{
				return false;
			}

			_dbContext.AppTasks.Remove(entity);
			await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<AppTask?> GetByIdAsync(Guid id)
		{
			var entity = await this._dbContext.AppTasks.FirstOrDefaultAsync(t => t.Id == id);

			return entity;
		}

		public async Task<int> SaveChangesAsync()
		{
			return await this._dbContext.SaveChangesAsync();
		}

		public async Task<bool> UpdateAsync(AppTask task)
		{
			this._dbContext.AppTasks.Update(task);
			return await SaveChangesAsync() > 0;
		}
	}
}
