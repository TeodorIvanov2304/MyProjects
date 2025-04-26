using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SimpleTaskManagerApp.Data
{
	public class TaskManagerDbContext : IdentityDbContext
	{
		public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options)
			: base(options)
		{
		}
	}
}
