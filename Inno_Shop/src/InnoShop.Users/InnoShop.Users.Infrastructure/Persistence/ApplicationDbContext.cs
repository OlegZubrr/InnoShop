using InnoShop.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Users.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(u => u.Role)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(u => u.IsEmailConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(u => u.EmailConfirmationToken)
                .HasMaxLength(500);

            entity.Property(u => u.PasswordResetToken)
                .HasMaxLength(500);

            entity.Property(u => u.PasswordResetTokenExpiry);

            entity.Property(u => u.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(u => u.UpdatedAt);
        });
    }
}