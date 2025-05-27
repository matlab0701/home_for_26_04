using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext(options)
{
    public virtual List<Order> orders { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>()
        .HasOne(o => o.User)
        .WithMany()
        .HasForeignKey(o => o.UserId);

        builder.Entity<Order>()
        .HasOne(o => o.Product)
        .WithMany(p => p.Order)
        .HasForeignKey(o => o.ProductId);

        builder.Entity<Category>()
        .HasMany(c => c.Products)
        .WithOne(p => p.Category)
        .HasForeignKey(p => p.CategoryId);



    }

}
