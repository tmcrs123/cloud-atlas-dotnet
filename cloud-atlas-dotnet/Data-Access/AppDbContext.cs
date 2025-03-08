using cloud_atlas_dotnet.Models;
using Microsoft.EntityFrameworkCore;

namespace cloud_atlas_dotnet.Data_Access
{
    public class AppDbContext : DbContext
    {
        protected AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Atlas> Atlases { get; set; }


    }
}
