using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using SimpleTaskManagerApp.Data.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTaskManagerApp.Data.Data.Configuration
{
	public class DatabaseSeeder
	{
		public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

			string[] roles = { "Administrator", "User" };

			foreach (var role in roles) 
			{	
				//Create roles, if they don't exist
				if(!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole(role));
				}
			}


			//Create starting admin
			string adminEmail = "admin@taskmanager.com";
			string adminPassword = "Admin123!";

			ApplicationUser? adminUser = await userManager.FindByEmailAsync(adminEmail);

			if (adminUser == null) 
			{
				var newAdmin = new ApplicationUser
				{
					UserName = adminEmail,
					Email = adminEmail,
					EmailConfirmed = true,
					FirstName = "Administrator",
					LastName = "User"
				};

				var result = await userManager.CreateAsync(newAdmin,adminPassword);

				if (result.Succeeded) 
				{
					await userManager.AddToRoleAsync(newAdmin, "Administrator");
				}
			}

			

		}
	}
}
