using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleTaskManagerApp.Data;
using SimpleTaskManagerApp.Data.Data.Repositories;
using SimpleTaskManagerApp.Data.Data.Repositories.Interfaces;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;
using SimpleTaskManagerApp.Services.Data;
using SimpleTaskManagerApp.Data.Data.Configuration;

namespace SimpleTaskManagerApp
{
    public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

			//Add Postgre
			builder.Services.AddDbContext<TaskManagerDbContext>(options =>
				options.UseNpgsql(connectionString));

			//Register ITaskRepository
			builder.Services.AddScoped<ITaskRepository, TaskRepository>();

			//Register AppTaskService
			builder.Services.AddScoped<IAppTaskService, AppTaskService>();

			//Register StatusService
			builder.Services.AddScoped<IStatusService, StatusTaskService>();

			//Register AdministratorService
			builder.Services.AddScoped<IAdministratorService, AdministratorService>();

			//Register LogEntryService
			builder.Services.AddScoped<ILogEntryService, LogEntryService>();

			//Register UrgencyLevelService
			builder.Services.AddScoped<IUrgencyLevelService,UrgencyLevelService>();

			builder.Services.AddDatabaseDeveloperPageExceptionFilter();

			builder.Services.AddDefaultIdentity<ApplicationUser>(options =>

					//Deactivate email confirmation during development
					options.SignIn.RequireConfirmedAccount = false
			)	
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<TaskManagerDbContext>();

			builder.Services.AddControllersWithViews();

			var app = builder.Build();

			

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseMigrationsEndPoint();
			}
			else
			{
				app.UseExceptionHandler("/Error/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseStatusCodePagesWithReExecute("/Error/{0}");
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "areas",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.MapRazorPages();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				await DatabaseSeeder.SeedRolesAsync(services);
			}

			app.MapFallbackToController("NotFoundPage", "Error");

			app.Run();
		}
	}
}
