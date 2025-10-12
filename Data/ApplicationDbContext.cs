using Microsoft.EntityFrameworkCore;
using Programming_7312_Part_1.Models;

namespace Programming_7312_Part_1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Event entity
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ImagePath).HasMaxLength(500);
                entity.Property(e => e.Upvotes).HasDefaultValue(0);
                entity.Property(e => e.Downvotes).HasDefaultValue(0);
                entity.Property(e => e.ViewCount).HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("datetime('now')");

                // Configure Tags as JSON
                entity.Property(e => e.Tags)
                    .HasConversion(
                        v => string.Join(",", v),
                        v => string.IsNullOrEmpty(v) ? new List<string>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    );
            });
        }
    }
}