using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.ViewModels.UrgencyLevel;

namespace SimpleTaskManagerApp.Services.Data
{
	public class UrgencyLevelService : IUrgencyLevelService
	{
		private readonly TaskManagerDbContext _context;

		public UrgencyLevelService(TaskManagerDbContext context)
		{
			_context = context;
		}

		public async Task<IEnumerable<UrgencyLevelViewModel>> GetAllUrgencyLevelsAsync()
		{
			var levels =  await _context.UrgencyLevels
			   .Select(u => new UrgencyLevelViewModel
			   {
				   Id = u.Id,
				   Name = u.Name,
				   Color = u.Color,
				   Description = u.Description
			   })
			   .ToListAsync();

			return levels;
		}
	}
}
