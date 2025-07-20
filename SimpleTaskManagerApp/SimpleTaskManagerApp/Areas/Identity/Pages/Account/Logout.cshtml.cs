// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Services.Data.Interfaces;

namespace SimpleTaskManagerApp.Areas.Identity.Pages.Account
{
	public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ILogEntryService _logService;
		public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, UserManager<ApplicationUser> userManager, ILogEntryService logService)
        {
            _signInManager = signInManager;
            _logger = logger;
			_userManager = userManager;
			_logService = logService;
        }

		public async Task<IActionResult> OnPost(string returnUrl = null)
		{

			var user = await _userManager.GetUserAsync(User);

			if (user != null)
			{	
				if(await _userManager.IsInRoleAsync(user, "Administrator"))
				{
					await _logService.LogAsync(user.Id, user.Email ?? "Unknown", "Administrator logged out", "Authentication", user.UserName);
				}
				else
				{
					await _logService.LogAsync(user.Id, user.Email ?? "Unknown", "User logged out", "Authentication", user.UserName);

				}
			}

			await _signInManager.SignOutAsync();
			_logger.LogInformation("User logged out.");

			if (returnUrl != null)
			{
				return LocalRedirect(returnUrl);
			}
			else
			{
				// This needs to be a redirect so that the browser performs a new
				// request and the identity for the user gets updated.
				return RedirectToPage();
			}
		}
	}
}
