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
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Issue> Issues { get; set; }

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

            // Configure Announcement entity
            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Content).IsRequired().HasMaxLength(2000);
                entity.Property(a => a.Category).HasMaxLength(100);
                entity.Property(a => a.IsActive).HasDefaultValue(true);
                entity.Property(a => a.Priority).HasDefaultValue(1);
                entity.Property(a => a.CreatedDate).HasDefaultValueSql("datetime('now')");
            });

            // Configure Contact entity
            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
                entity.Property(c => c.Subject).IsRequired().HasMaxLength(500);
                entity.Property(c => c.Message).IsRequired().HasMaxLength(2000);
                entity.Property(c => c.Phone).HasMaxLength(20);
                entity.Property(c => c.Category).HasMaxLength(100);
                entity.Property(c => c.IsRead).HasDefaultValue(false);
                entity.Property(c => c.IsResponded).HasDefaultValue(false);
                entity.Property(c => c.CreatedDate).HasDefaultValueSql("datetime('now')");
            });

            // Configure Issue entity
            modelBuilder.Entity<Issue>(entity =>
            {
                entity.HasKey(i => i.Id);
                entity.Property(i => i.Location).IsRequired().HasMaxLength(200);
                entity.Property(i => i.Category).IsRequired().HasMaxLength(100);
                entity.Property(i => i.Description).IsRequired().HasMaxLength(1000);
                entity.Property(i => i.AttachedFilePath).HasMaxLength(500);
                entity.Property(i => i.Status).HasMaxLength(50).HasDefaultValue("Pending");
                entity.Property(i => i.Upvotes).HasDefaultValue(0);
                entity.Property(i => i.Downvotes).HasDefaultValue(0);
                entity.Property(i => i.UserId).HasMaxLength(100);
                entity.Property(i => i.ReportedDate).HasDefaultValueSql("datetime('now')");
                entity.Property(i => i.AdminResponse).HasMaxLength(50);
                entity.Property(i => i.AdminComments).HasMaxLength(1000);
            });
        }
    }
}