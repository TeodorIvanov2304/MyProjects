using static SimpleTaskManagerApp.Common.EntityValidationConstants;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SimpleTaskManagerApp.Data.Models
{
	public class ApplicationUser : IdentityUser
	{

		[PersonalData]
		[MaxLength(ApplicationUserFirstNameMaxLength)]
		[Comment("The first name of the user")]
		public string? FirstName { get; set; }

		[PersonalData]
		[MaxLength(ApplicationUserLastNameMaxLength)]
		[Comment("The last name of the user")]
		public string? LastName { get; set; }

	}
}
