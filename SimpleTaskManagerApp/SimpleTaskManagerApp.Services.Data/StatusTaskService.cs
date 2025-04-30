using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.Status;

namespace SimpleTaskManagerApp.Services.Data
{
	public class StatusTaskService : IStatusService
	{
		private readonly TaskManagerDbContext _dbContext;

        public StatusTaskService(TaskManagerDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task<IEnumerable<StatusViewModel>> GetAllStatusesAsync()
		{
			List<StatusViewModel> statuses = await this._dbContext.Statuses
				.AsNoTracking()
				.Select(s => new StatusViewModel
			{
				Id = s.Id,
				Name = s.Name,
			}).ToListAsync();

			return statuses;
		}

	}
}
