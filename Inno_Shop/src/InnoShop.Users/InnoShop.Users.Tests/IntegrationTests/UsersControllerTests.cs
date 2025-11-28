using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using InnoShop.Users.Application.DTOs;
using InnoShop.Users.Domain.Enums;
using InnoShop.Users.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InnoShop.Users.Tests.IntegrationTests;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginUserAsync(string email, string password,
        string fullName = "Integration Test User", bool isAdmin = false)
    {
        var registerDto = new UserCreateDto
        {
            FullName = fullName,
            Email = email,
            Password = password
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        if (registerResponse.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await registerResponse.Content.ReadAsStringAsync();
            throw new Exception($"Registration failed: {registerResponse.StatusCode}, Content: {errorContent}");
        }

        registerResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                user.IsEmailConfirmed = true;
                user.EmailConfirmationToken = null;

                if (isAdmin) user.Role = UserRole.Admin;

                await dbContext.SaveChangesAsync();

                dbContext.Entry(user).State = EntityState.Detached;
            }
        }

        var loginDto = new LoginDto
        {
            Email = email,
            Password = password
        };

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", loginDto);

        if (loginResponse.StatusCode != HttpStatusCode.OK)
        {
            var errorContent = await loginResponse.Content.ReadAsStringAsync();
            throw new Exception($"Login failed: {loginResponse.StatusCode}, Content: {errorContent}");
        }

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        loginResult.Should().NotBeNull();
        loginResult!.Token.Should().NotBeNullOrEmpty();

        return loginResult.Token;
    }


    [Fact]
    public async Task Register_WithValidData_ReturnsSuccess()
    {
        var email = "integration@test.com";
        var password = "password123";

        var registerDto = new UserCreateDto
        {
            FullName = "Integration Test User",
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var content = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(responseContent,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        content.Should().NotBeNull();
        content!.Should().ContainKey("user");

        var userJson = content["user"].GetRawText();
        var user = JsonSerializer.Deserialize<UserResponseDto>(userJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        user.Should().NotBeNull();
        user!.Email.Should().Be(email);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var email = "login@test.com";
        var password = "password123";

        var token = await RegisterAndLoginUserAsync(email, password);

        token.Should().NotBeNullOrEmpty();
    }


    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsConflict()
    {
        var email = $"duplicate_{Guid.NewGuid()}@test.com";
        var registerDto = new UserCreateDto
        {
            FullName = "Test User",
            Email = email,
            Password = "password123"
        };

        var firstResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondResponse = await _client.PostAsJsonAsync("/api/auth/register", registerDto);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
    {
        var loginDto = new LoginDto
        {
            Email = "nonexistent@test.com",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var email = $"wrongpass_{Guid.NewGuid()}@test.com";
        var correctPassword = "password123";

        var registerDto = new UserCreateDto
        {
            FullName = "Test User",
            Email = email,
            Password = correctPassword
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.IsEmailConfirmed = true;
                user.EmailConfirmationToken = null;
                await dbContext.SaveChangesAsync();
            }
        }

        var loginDto = new LoginDto
        {
            Email = email,
            Password = "wrongpassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithUnconfirmedEmail_ReturnsForbidden()
    {
        var email = $"unconfirmed_{Guid.NewGuid()}@test.com";
        var password = "password123";

        var registerDto = new UserCreateDto
        {
            FullName = "Test User",
            Email = email,
            Password = password
        };
        await _client.PostAsJsonAsync("/api/auth/register", registerDto);

        var loginDto = new LoginDto
        {
            Email = email,
            Password = password
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }


    [Fact]
    public async Task GetAllUsers_WithoutAuth_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAllUsers_WithUserRole_ReturnsForbidden()
    {
        var email = $"user_{Guid.NewGuid()}@test.com";
        var password = "password123";

        var token = await RegisterAndLoginUserAsync(email, password);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAllUsers_WithAdminRole_ReturnsSuccess()
    {
        var email = $"admin_{Guid.NewGuid()}@test.com";
        var password = "admin123";

        var token = await RegisterAndLoginUserAsync(email, password, "Admin User", true);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("/api/users");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>(
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        users.Should().NotBeNull();
        users!.Should().NotBeEmpty();
    }
}