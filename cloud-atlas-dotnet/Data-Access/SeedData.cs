using cloud_atlas_dotnet.Models;
using Microsoft.EntityFrameworkCore;

namespace cloud_atlas_dotnet.Data_Access
{
    public static class SeedData
    {
        public static void SeedAndMigrate(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            context.Database.Migrate();

            if (!context.Atlases.Any())
            {
                var atlases = new List<Atlas>
                {
                    new Atlas {Title = "Atlas-1", Id = new Guid(), MarkersCount = 0, Owner = new Guid()},
                    new Atlas {Title = "Atlas-2", Id = new Guid(), MarkersCount = 0, Owner = new Guid()},
                    new Atlas {Title = "Atlas-3", Id = new Guid(), MarkersCount = 0, Owner = new Guid()},
                    new Atlas {Title = "Atlas-4", Id = new Guid(), MarkersCount = 0, Owner = new Guid()},
                };
                context.Atlases.AddRange(atlases);
                context.SaveChanges();

            }

        }
    }
}
