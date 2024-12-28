using EcommerceProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcommerceProject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // DbSet for Products
        public DbSet<Produit> Produits { get; set; }

        // DbSet for CartItem is not required because it's not a database entity.
        // DbSet for other models like LoginViewModel, PaymentViewModel, RegisterViewModel 
        // are also not included because these are view models, not database entities.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure precision for decimal fields
            modelBuilder.Entity<Produit>()
                .Property(p => p.Prix)
                .HasColumnType("decimal(18,2)");
        }
    }
}
