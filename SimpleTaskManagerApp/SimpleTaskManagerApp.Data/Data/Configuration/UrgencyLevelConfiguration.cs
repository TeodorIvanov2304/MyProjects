using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleTaskManagerApp.Data.Models.Models;

namespace SimpleTaskManagerApp.Data.Data.Configuration
{
		public class UrgencyLevelConfiguration : IEntityTypeConfiguration<UrgencyLevel>
		{
			public void Configure(EntityTypeBuilder<UrgencyLevel> builder)
			{
				builder.HasData(SeedUrgencyLevels());
			}

			private IEnumerable<UrgencyLevel> SeedUrgencyLevels()
			{
				IEnumerable<UrgencyLevel> levels = new HashSet<UrgencyLevel>()
				{
					new UrgencyLevel()
					{
						Id = 1,
						Name = "Low",
						Color = "#28a745", //Green
						Description = "Not urgent"
					},

					new UrgencyLevel()
					{
						Id = 2,
						Name = "Medium",
						Color = "#ffc107", //Yellow
						Description = "Moderately urgent"
					},

					new UrgencyLevel()
					{
						Id = 3,
						Name = "High",
						Color = "#dc3545", //Red
						Description = "Requires immediate attention"
					},
				};

				return levels;
			}
		}
}
