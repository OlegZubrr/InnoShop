using InnoShop.Products.Domain.Entities;

namespace InnoShop.Products.Application.Interfaces.Services;

public interface IProductService
{
    Task<Product?> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId);

    Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId,
        int pageNumber,
        int pageSize);

    Task<Product> CreateAsync(Product product, Guid userId);
    Task<Product> UpdateAsync(Guid id, Product updatedProduct, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}