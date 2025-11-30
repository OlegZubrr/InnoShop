using InnoShop.Products.Application.Interfaces.Repositories;
using InnoShop.Products.Application.Services;
using InnoShop.Products.Domain.Entities;
using InnoShop.Products.Domain.Exceptions;
using Moq;
using Xunit;

namespace InnoShop.Products.Tests.UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsProduct()
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "Test Product",
            Description = "Test Description",
            Price = 100,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, false))
            .ReturnsAsync(product);

        var result = await _productService.GetByIdAsync(productId);

        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProduct_ThrowsException()
    {
        var productId = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(productId, false))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<ProductNotFoundException>(() =>
            _productService.GetByIdAsync(productId));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "Product 1", Price = 100, CreatedAt = DateTime.UtcNow },
            new() { Id = Guid.NewGuid(), Name = "Product 2", Price = 200, CreatedAt = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync(false))
            .ReturnsAsync(products);

        var result = await _productService.GetAllAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateAsync_ValidProduct_ReturnsCreatedProduct()
    {
        var userId = Guid.NewGuid();
        var product = new Product
        {
            Name = "New Product",
            Description = "New Description",
            Price = 200
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                p.Id = Guid.NewGuid();
                return p;
            });

        var result = await _productService.CreateAsync(product, userId);

        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(userId, result.UserId);
        Assert.NotEqual(Guid.Empty, result.Id);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ValidUpdate_ReturnsUpdatedProduct()
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 100,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var updatedProduct = new Product
        {
            Name = "Updated Name",
            Description = "Old Description",
            Price = 150,
            IsAvailable = true
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, false))
            .ReturnsAsync(existingProduct);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var result = await _productService.UpdateAsync(productId, updatedProduct, userId);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(150, result.Price);
        Assert.NotNull(result.UpdatedAt);
    }

    [Fact]
    public async Task UpdateAsync_WrongUser_ThrowsUnauthorizedException()
    {
        var productId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();

        var existingProduct = new Product
        {
            Id = productId,
            Name = "Product",
            Description = "Description",
            Price = 100,
            UserId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        var updatedProduct = new Product { Name = "Updated" };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, false))
            .ReturnsAsync(existingProduct);

        await Assert.ThrowsAsync<UnauthorizedProductAccessException>(() =>
            _productService.UpdateAsync(productId, updatedProduct, wrongUserId));
    }

    [Fact]
    public async Task DeleteAsync_ValidRequest_DeletesProduct()
    {
        var productId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            UserId = userId,
            Name = "Test",
            Description = "Test",
            Price = 100,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, false))
            .ReturnsAsync(product);
        _mockRepository.Setup(r => r.DeleteAsync(productId))
            .Returns(Task.CompletedTask);

        var result = await _productService.DeleteAsync(productId, userId);

        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WrongUser_ThrowsUnauthorizedException()
    {
        var productId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var wrongUserId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            UserId = ownerId,
            Name = "Test",
            Description = "Test",
            Price = 100,
            CreatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(productId, false))
            .ReturnsAsync(product);

        await Assert.ThrowsAsync<UnauthorizedProductAccessException>(() =>
            _productService.DeleteAsync(productId, wrongUserId));
    }

    [Fact]
    public async Task SearchAsync_WithFilters_ReturnsFilteredProducts()
    {
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "Product 1", Price = 100, Description = "Desc", CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Product 2", Price = 200, Description = "Desc", CreatedAt = DateTime.UtcNow
            }
        };

        _mockRepository.Setup(r => r.SearchAsync(
                It.IsAny<string?>(),
                It.IsAny<decimal?>(),
                It.IsAny<decimal?>(),
                It.IsAny<bool?>(),
                It.IsAny<Guid?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                false))
            .ReturnsAsync((products, 2));

        var (result, totalCount) = await _productService.SearchAsync(
            null,
            50,
            150,
            null,
            null,
            1,
            10);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal(2, totalCount);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserProducts()
    {
        var userId = Guid.NewGuid();
        var products = new List<Product>
        {
            new()
            {
                Id = Guid.NewGuid(), Name = "Product 1", UserId = userId, Price = 100, Description = "Desc",
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(), Name = "Product 2", UserId = userId, Price = 200, Description = "Desc",
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockRepository.Setup(r => r.GetByUserIdAsync(userId, false))
            .ReturnsAsync(products);

        var result = await _productService.GetByUserIdAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, p => Assert.Equal(userId, p.UserId));
    }
}