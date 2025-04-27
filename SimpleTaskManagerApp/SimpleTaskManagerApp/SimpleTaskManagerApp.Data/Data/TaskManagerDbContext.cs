using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data.Models.Models;

namespace SimpleTaskManagerApp.Data
{
    public class TaskManagerDbContext : IdentityDbContext<ApplicationUser>
	{
		public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
			: base(options)
		{
		}

		public virtual DbSet<AppTask> AppTasks { get; set; }
		public virtual DbSet<Status> Statuses { get; set; }
	}
}
