using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Infrastructure.Persistence;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        context.Database.Migrate();

        if (context.Products.Any())
        {
        }

        // Можно добавить тестовые продукты для разработки
        // Раскомментируйте если нужно
        /*
        var products = new[]
        {
            new Product
            {
                Name = "Laptop Dell XPS 15",
                Description = "High-performance laptop for developers",
                Price = 1499.99m,
                IsAvailable = true,
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"), // Замените на реальный ID пользователя
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "iPhone 15 Pro",
                Description = "Latest Apple smartphone",
                Price = 999.99m,
                IsAvailable = true,
                UserId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                CreatedAt = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
        */
    }
}