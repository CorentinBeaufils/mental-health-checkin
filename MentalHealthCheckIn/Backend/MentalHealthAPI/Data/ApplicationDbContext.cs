using Microsoft.EntityFrameworkCore;
using MentalHealthAPI.Models;

namespace MentalHealthAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        public DbSet<CheckIn> CheckIns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration pour CheckIn
            modelBuilder.Entity<CheckIn>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Mood).IsRequired();
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();
                
                // Index pour améliorer les performances de requête
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Date);
            });
        }
    }
}
