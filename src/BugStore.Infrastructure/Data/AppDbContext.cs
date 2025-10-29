using BugStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BugStore.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderLine> OrderLines { get; set; } = null!;
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var envConnection = Environment.GetEnvironmentVariable("ConnectionStrings__Default");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            if (!string.IsNullOrEmpty(envConnection))
            {
                optionsBuilder.UseNpgsql(envConnection);
            }
            else
            {
                optionsBuilder.UseSqlite("Data Source=bugstore-app.db");
            }

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
