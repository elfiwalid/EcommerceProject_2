using EcommerceProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace EcommerceProject.Data
{
      public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Produit> Produits { get; set; }
    public DbSet<DeliveryViewModel> Deliveries { get; set; }

        public DbSet<Category> Categories { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Produit>()
            .HasKey(p => p.Id);

        modelBuilder.Entity<Produit>()
            .Property(p => p.Prix)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<DeliveryViewModel>()
            .HasKey(d => d.Id);
    }
}
}