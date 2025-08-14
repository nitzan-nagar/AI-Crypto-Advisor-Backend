using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace AI.CryptoAdvisor.Api.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // SQLite - קובץ בדיסק
            optionsBuilder.UseSqlite("Data Source=CryptoAdvisor.db");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
