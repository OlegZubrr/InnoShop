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
    }
}