using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTaskManagerApp.Data.Models.Models;
using SimpleTaskManagerApp.Data.Models.Models.Enums;

namespace SimpleTaskManagerApp.Data.Configuration
{
	public class StatusConfiguration : IEntityTypeConfiguration<Status>
	{
		public void Configure(EntityTypeBuilder<Status> builder)
		{
			builder.HasData(this.SeedStatusTypes());
		}

		//Seed status types
		private IEnumerable<Status> SeedStatusTypes()
		{
			IEnumerable<Status> types = new HashSet<Status>() 
			{
				new Status()  
				{
					Id = (int)TaskStatusEnum.Pending, 
					Name = "Pending"
				},

				new Status()
				{
					Id = (int)TaskStatusEnum.InProgress,
					Name = "In progress"
				},

				new Status()
				{
					Id = (int)TaskStatusEnum.Completed,
					Name = "Completed"
				},

				new Status()
				{
					Id = (int)TaskStatusEnum.Canceled,
					Name = "Canceled",
				}
			};

			return types;

		}
	}
}
