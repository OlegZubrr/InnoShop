using InnoShop.Products.Application.Interfaces.Repositories;
using InnoShop.Products.Domain.Entities;
using InnoShop.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InnoShop.Products.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, bool includeDeleted = false)
    {
        IQueryable<Product> query = _context.Products;

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync(bool includeDeleted = false)
    {
        IQueryable<Product> query = _context.Products;

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await query.OrderByDescending(p => p.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId, bool includeDeleted = false)
    {
        IQueryable<Product> query = _context.Products;

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        return await query
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteByUserIdAsync(Guid userId)
    {
        var products = await _context.Products
            .Where(p => p.UserId == userId)
            .ToListAsync();

        foreach (var product in products)
        {
            product.IsDeleted = true;
            product.DeletedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
    }

    public async Task RestoreByUserIdAsync(Guid userId)
    {
        var products = await _context.Products
            .IgnoreQueryFilters()
            .Where(p => p.UserId == userId && p.IsDeleted)
            .ToListAsync();

        foreach (var product in products)
        {
            product.IsDeleted = false;
            product.DeletedAt = null;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId,
        int pageNumber,
        int pageSize,
        bool includeDeleted = false)
    {
        IQueryable<Product> query = _context.Products;

        if (includeDeleted)
            query = query.IgnoreQueryFilters();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(lowerSearchTerm) ||
                p.Description.ToLower().Contains(lowerSearchTerm));
        }

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        if (isAvailable.HasValue)
            query = query.Where(p => p.IsAvailable == isAvailable.Value);

        if (userId.HasValue)
            query = query.Where(p => p.UserId == userId.Value);

        var totalCount = await query.CountAsync();

        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }
}