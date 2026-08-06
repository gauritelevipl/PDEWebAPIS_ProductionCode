using Microsoft.EntityFrameworkCore;

namespace YourNamespace.Data
{
    public class HangfireDbContext : DbContext
    {
        public HangfireDbContext(DbContextOptions<HangfireDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Hangfire"); // Optional: Use a separate schema
        }
    }
}