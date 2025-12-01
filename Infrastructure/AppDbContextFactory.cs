using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AdopcionOnline.Infrastructure
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(
        @"Server=CARLOS;Database=AdopcionOnlineDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
    );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}