using Microsoft.EntityFrameworkCore;
using WebApplication3.Models; // Fixed the typo from "Entitis" to "Entities"

namespace WebApplication3.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Employe>employee { get; set; }
    }
}
