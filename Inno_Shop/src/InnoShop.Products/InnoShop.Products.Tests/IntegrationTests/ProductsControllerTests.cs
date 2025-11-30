using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using InnoShop.Products.Application.DTOs;
using InnoShop.Products.Domain.Entities;
using InnoShop.Products.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace InnoShop.Products.Tests.IntegrationTests;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private void SeedDatabase(Action<ApplicationDbContext> seedAction)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        db.Products.RemoveRange(db.Products);
        db.SaveChanges();

        seedAction(db);
    }

    private void SetAuthToken(Guid userId, string role = "User")
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.TestAuthHeaderName);
        _client.DefaultRequestHeaders.Add(TestAuthHandler.TestAuthHeaderName, $"{userId}:{role}");
    }

    private void ClearAuthToken()
    {
        _client.DefaultRequestHeaders.Remove(TestAuthHandler.TestAuthHeaderName);
    }

    [Fact]
    public async Task GetAll_ReturnsAllProducts()
    {
        SeedDatabase(db =>
        {
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Laptop",
                    Description = "Gaming laptop",
                    Price = 1500,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Mouse",
                    Description = "Wireless mouse",
                    Price = 50,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            );
            db.SaveChanges();
        });

        var response = await _client.GetAsync("/api/products");

        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>();
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
    }

    [Fact]
    public async Task GetById_ExistingProduct_ReturnsProduct()
    {
        var productId = Guid.NewGuid();
        SeedDatabase(db =>
        {
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                Price = 100,
                UserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        });

        var response = await _client.GetAsync($"/api/products/{productId}");

        response.EnsureSuccessStatusCode();
        var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
        Assert.NotNull(product);
        Assert.Equal("Test Product", product.Name);
        Assert.Equal(100, product.Price);
    }

    [Fact]
    public async Task GetById_NonExistingProduct_ReturnsNotFound()
    {
        var nonExistingId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/products/{nonExistingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Search_WithFilters_ReturnsFilteredProducts()
    {
        SeedDatabase(db =>
        {
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Cheap Laptop",
                    Description = "Budget laptop",
                    Price = 500,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Expensive Laptop",
                    Description = "Gaming laptop",
                    Price = 2000,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Mouse",
                    Description = "Wireless mouse",
                    Price = 50,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            );
            db.SaveChanges();
        });

        var response = await _client.GetAsync(
            "/api/products/search?searchTerm=laptop&minPrice=400&maxPrice=1000");

        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var productsElement = result.GetProperty("products");
        var products = JsonSerializer.Deserialize<List<ProductResponseDto>>(
            productsElement.GetRawText(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Equal("Cheap Laptop", products[0].Name);
    }

    [Fact]
    public async Task Search_WithPagination_ReturnsPagedResults()
    {
        SeedDatabase(db =>
        {
            for (var i = 1; i <= 15; i++)
                db.Products.Add(new Product
                {
                    Id = Guid.NewGuid(),
                    Name = $"Product {i}",
                    Description = $"Description {i}",
                    Price = i * 100,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                });
            db.SaveChanges();
        });

        var response = await _client.GetAsync(
            "/api/products/search?pageNumber=2&pageSize=5");

        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(jsonString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var productsElement = result.GetProperty("products");
        var products = JsonSerializer.Deserialize<List<ProductResponseDto>>(
            productsElement.GetRawText(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var totalCount = result.GetProperty("totalCount").GetInt32();

        Assert.NotNull(products);
        Assert.Equal(5, products.Count);
        Assert.Equal(15, totalCount);
    }

    [Fact]
    public async Task Create_WithValidData_ReturnsCreatedProduct()
    {
        var userId = Guid.NewGuid();
        SetAuthToken(userId);

        var createDto = new ProductCreateDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 299.99m,
            IsAvailable = true
        };

        var response = await _client.PostAsJsonAsync("/api/products", createDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(product);
        Assert.Equal("New Product", product.Name);
        Assert.Equal(299.99m, product.Price);
        Assert.Equal(userId, product.UserId);
    }

    [Fact]
    public async Task Create_WithoutAuthentication_ReturnsUnauthorized()
    {
        ClearAuthToken();

        var createDto = new ProductCreateDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 299.99m
        };

        var response = await _client.PostAsJsonAsync("/api/products", createDto);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithInvalidData_ReturnsBadRequest()
    {
        var userId = Guid.NewGuid();
        SetAuthToken(userId);

        var createDto = new ProductCreateDto
        {
            Name = "", 
            Description = "Description",
            Price = -10 
        };

        var response = await _client.PostAsJsonAsync("/api/products", createDto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_OwnProduct_ReturnsUpdatedProduct()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        SeedDatabase(db =>
        {
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Original Name",
                Description = "Original Description",
                Price = 100,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        });

        SetAuthToken(userId);

        var updateDto = new ProductUpdateDto
        {
            Name = "Updated Name",
            Price = 150
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updateDto);

        response.EnsureSuccessStatusCode();
        var product = await response.Content.ReadFromJsonAsync<ProductResponseDto>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(product);
        Assert.Equal("Updated Name", product.Name);
        Assert.Equal(150, product.Price);
    }

    [Fact]
    public async Task Update_OtherUsersProduct_ReturnsForbidden()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        SeedDatabase(db =>
        {
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Original Name",
                Description = "Description",
                Price = 100,
                UserId = ownerId,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        });

        SetAuthToken(otherUserId);

        var updateDto = new ProductUpdateDto
        {
            Name = "Updated Name"
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{productId}", updateDto);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Delete_OwnProduct_ReturnsNoContent()
    {
        var userId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        SeedDatabase(db =>
        {
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Product to Delete",
                Description = "Description",
                Price = 100,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        });

        SetAuthToken(userId);

        var response = await _client.DeleteAsync($"/api/products/{productId}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/products/{productId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_OtherUsersProduct_ReturnsForbidden()
    {
        var ownerId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        SeedDatabase(db =>
        {
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Product",
                Description = "Description",
                Price = 100,
                UserId = ownerId,
                CreatedAt = DateTime.UtcNow
            });
            db.SaveChanges();
        });

        SetAuthToken(otherUserId);

        var response = await _client.DeleteAsync($"/api/products/{productId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetByUserId_ReturnsOnlyUserProducts()
    {
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();

        SeedDatabase(db =>
        {
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "User Product 1",
                    Description = "Description",
                    Price = 100,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "User Product 2",
                    Description = "Description",
                    Price = 200,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Other Product",
                    Description = "Description",
                    Price = 300,
                    UserId = otherUserId,
                    CreatedAt = DateTime.UtcNow
                }
            );
            db.SaveChanges();
        });

        var response = await _client.GetAsync($"/api/products/user/{userId}");

        response.EnsureSuccessStatusCode();
        var products = await response.Content.ReadFromJsonAsync<List<ProductResponseDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        Assert.NotNull(products);
        Assert.Equal(2, products.Count);
        Assert.All(products, p => Assert.Equal(userId, p.UserId));
    }
}