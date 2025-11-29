using InnoShop.Products.Domain.Entities;

namespace InnoShop.Products.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, bool includeDeleted = false);
    Task<IEnumerable<Product>> GetAllAsync(bool includeDeleted = false);
    Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId, bool includeDeleted = false);

    Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId,
        int pageNumber,
        int pageSize,
        bool includeDeleted = false);

    Task<Product> CreateAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task SoftDeleteByUserIdAsync(Guid userId);
    Task RestoreByUserIdAsync(Guid userId);
}