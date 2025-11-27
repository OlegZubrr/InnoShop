using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Users.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.Migrate();

        if (context.Users.Any()) return;

        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Admin User",
                Email = "admin@innoshop.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = UserRole.Admin,
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "John Doe",
                Email = "john@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = UserRole.User,
                IsActive = true,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Jane Smith",
                Email = "jane@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = UserRole.User,
                IsActive = true,
                IsEmailConfirmed = false,
                EmailConfirmationToken = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                FullName = "Inactive User",
                Email = "inactive@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Role = UserRole.User,
                IsActive = false,
                IsEmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Users.AddRange(users);
        context.SaveChanges();
    }
}