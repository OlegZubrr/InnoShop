using InnoShop.Products.Application.Interfaces.Repositories;
using InnoShop.Products.Application.Interfaces.Services;
using InnoShop.Products.Domain.Entities;
using InnoShop.Products.Domain.Exceptions;

namespace InnoShop.Products.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ProductNotFoundException(id);

        return product;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId)
    {
        return await _productRepository.GetByUserIdAsync(userId);
    }

    public async Task<(IEnumerable<Product> Products, int TotalCount)> SearchAsync(
        string? searchTerm,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isAvailable,
        Guid? userId,
        int pageNumber,
        int pageSize)
    {
        var (products, totalCount) = await _productRepository.SearchAsync(
            searchTerm,
            minPrice,
            maxPrice,
            isAvailable,
            userId,
            pageNumber,
            pageSize);

        return (products, totalCount);
    }

    public async Task<Product> CreateAsync(Product product, Guid userId)
    {
        product.UserId = userId;
        product.CreatedAt = DateTime.UtcNow;

        return await _productRepository.CreateAsync(product);
    }

    public async Task<Product> UpdateAsync(Guid id, Product updatedProduct, Guid userId)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ProductNotFoundException(id);

        if (product.UserId != userId)
            throw new UnauthorizedProductAccessException();

        if (!string.IsNullOrEmpty(updatedProduct.Name))
            product.Name = updatedProduct.Name;

        if (!string.IsNullOrEmpty(updatedProduct.Description))
            product.Description = updatedProduct.Description;

        if (updatedProduct.Price > 0)
            product.Price = updatedProduct.Price;

        product.IsAvailable = updatedProduct.IsAvailable;
        product.UpdatedAt = DateTime.UtcNow;

        return await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ProductNotFoundException(id);

        if (product.UserId != userId)
            throw new UnauthorizedProductAccessException();

        await _productRepository.DeleteAsync(id);
        return true;
    }
}