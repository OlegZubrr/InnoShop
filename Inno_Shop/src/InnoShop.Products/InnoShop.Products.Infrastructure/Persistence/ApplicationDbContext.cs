using InnoShop.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(e => e.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.UserId)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsDeleted);
            entity.HasIndex(e => e.Name);
            entity.HasIndex(e => e.CreatedAt);

            entity.HasQueryFilter(p => !p.IsDeleted);
        });
    }
}