using Microsoft.EntityFrameworkCore;
using WebApplication3.Models; // Fixed the typo from "Entitis" to "Entities"
namespace WebApplication3.Data
{
    public class ApplicationDbContext : DbContext
    {
       
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employe> Employee { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
            modelBuilder.Entity<Employe>()
       .HasOne(e => e.Manager)
       .WithMany(u => u.ManagedEmployees)
       .HasForeignKey(e => e.ManagerId)
       .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
