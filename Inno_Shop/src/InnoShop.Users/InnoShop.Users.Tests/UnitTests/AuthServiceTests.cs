using AutoMapper;
using FluentAssertions;
using InnoShop.Users.Application.DTOs;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Application.Mapping;
using InnoShop.Users.Application.Services;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using InnoShop.Users.Domain.Exceptions;
using InnoShop.Users.Infrastructure.Services;
using Moq;

namespace InnoShop.Users.Tests.UnitTests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _emailServiceMock = new Mock<IEmailService>();

        var mapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<UserProfile>(); });
        _mapper = mapperConfig.CreateMapper();

        _passwordHasher = new PasswordHasher();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _tokenServiceMock.Object,
            _emailServiceMock.Object,
            _passwordHasher,
            _mapper
        );
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        var password = "password123";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.User,
            IsActive = true,
            IsEmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = password
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(x => x.GenerateAccessToken(user))
            .Returns("access-token");

        _tokenServiceMock
            .Setup(x => x.GenerateRefreshToken())
            .Returns("refresh-token");

        var result = await _authService.LoginAsync(loginDto);

        result.Should().NotBeNull();
        result.Token.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        result.User.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsInvalidCredentialsException()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            IsActive = true
        };

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = "wrongpassword"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task LoginAsync_WithDeactivatedUser_ThrowsUserDeactivatedException()
    {
        var password = "password123";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            IsActive = false
        };

        var loginDto = new LoginDto
        {
            Email = user.Email,
            Password = password
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        Func<Task> act = async () => await _authService.LoginAsync(loginDto);

        await act.Should().ThrowAsync<UserDeactivatedException>();
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_CreatesUser()
    {
        var createDto = new UserCreateDto
        {
            FullName = "New User",
            Email = "new@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(createDto.Email))
            .ReturnsAsync((User?)null);

        _tokenServiceMock
            .Setup(x => x.GenerateEmailConfirmationToken())
            .Returns("confirmation-token");

        _emailServiceMock
            .Setup(x => x.SendEmailConfirmationAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(createDto);

        result.Should().NotBeNull();
        result.Email.Should().Be(createDto.Email);
        result.FullName.Should().Be(createDto.FullName);
        result.Role.Should().Be(UserRole.User);
        result.IsActive.Should().BeTrue();

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _emailServiceMock.Verify(x => x.SendEmailConfirmationAsync(
            createDto.Email,
            "confirmation-token",
            createDto.FullName), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsUserAlreadyExistsException()
    {
        var existingUser = new User
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            FullName = "Existing User",
            PasswordHash = "hash",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createDto = new UserCreateDto
        {
            FullName = "New User",
            Email = "existing@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(createDto.Email))
            .ReturnsAsync(existingUser);

        Func<Task> act = async () => await _authService.RegisterAsync(createDto);

        await act.Should().ThrowAsync<UserAlreadyExistsException>()
            .WithMessage($"User with email '{createDto.Email}' already exists");
    }

    [Fact]
    public async Task ConfirmEmailAsync_WithValidToken_ConfirmsEmail()
    {
        var token = "valid-token";
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            EmailConfirmationToken = token,
            IsEmailConfirmed = false,
            PasswordHash = "hash",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailConfirmationTokenAsync(token))
            .ReturnsAsync(user);

        _emailServiceMock
            .Setup(x => x.SendWelcomeEmailAsync(user.Email, user.FullName))
            .Returns(Task.CompletedTask);

        var result = await _authService.ConfirmEmailAsync(token);

        result.Should().BeTrue();
        user.IsEmailConfirmed.Should().BeTrue();
        user.EmailConfirmationToken.Should().BeNull();
        user.UpdatedAt.Should().NotBeNull();

        _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _emailServiceMock.Verify(x => x.SendWelcomeEmailAsync(user.Email, user.FullName), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmailAsync_WithInvalidToken_ThrowsInvalidTokenException()
    {
        var token = "invalid-token";

        _userRepositoryMock
            .Setup(x => x.GetByEmailConfirmationTokenAsync(token))
            .ReturnsAsync((User?)null);

        Func<Task> act = async () => await _authService.ConfirmEmailAsync(token);

        await act.Should().ThrowAsync<InvalidTokenException>()
            .WithMessage("Invalid or expired email confirmation token");
    }
}