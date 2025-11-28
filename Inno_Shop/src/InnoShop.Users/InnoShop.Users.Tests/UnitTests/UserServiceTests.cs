using FluentAssertions;
using InnoShop.Users.Application.Interfaces.Repositories;
using InnoShop.Users.Application.Services;
using InnoShop.Users.Domain.Entities;
using InnoShop.Users.Domain.Enums;
using InnoShop.Users.Domain.Exceptions;
using Moq;

namespace InnoShop.Users.Tests.UnitTests;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsUser()
    {
        var userId = Guid.NewGuid();
        var expectedUser = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);

        var result = await _userService.GetByIdAsync(userId);

        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedUser);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ThrowsUserNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        Func<Task> act = async () => await _userService.GetByIdAsync(userId);

        await act.Should().ThrowAsync<UserNotFoundException>()
            .WithMessage($"User with ID '{userId}' was not found");
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ThrowsUserAlreadyExistsException()
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

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(existingUser.Email))
            .ReturnsAsync(existingUser);

        var newUser = new User
        {
            Email = "existing@example.com",
            FullName = "New User",
            PasswordHash = "hash",
            Role = UserRole.User
        };

        Func<Task> act = async () => await _userService.CreateAsync(newUser);

        await act.Should().ThrowAsync<UserAlreadyExistsException>()
            .WithMessage($"User with email '{existingUser.Email}' already exists");
    }

    [Fact]
    public async Task CreateAsync_WhenEmailIsNew_CreatesUser()
    {
        var newUser = new User
        {
            Email = "new@example.com",
            FullName = "New User",
            PasswordHash = "hash",
            Role = UserRole.User
        };

        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(newUser.Email))
            .ReturnsAsync((User?)null);

        _userRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var result = await _userService.CreateAsync(newUser);

        result.Should().NotBeNull();
        result.Email.Should().Be(newUser.Email);
        result.IsActive.Should().BeTrue();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

        _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeactivateAsync_WhenUserExists_DeactivatesUser()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "test@example.com",
            PasswordHash = "hash",
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var result = await _userService.DeactivateAsync(userId);

        result.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.UpdatedAt.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsWrong_ThrowsInvalidCredentialsException()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("correctpassword"),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        Func<Task> act = async () => await _userService.ChangePasswordAsync(userId, "wrongpassword", "newpassword");

        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task ChangePasswordAsync_WhenCurrentPasswordIsCorrect_ChangesPassword()
    {
        var userId = Guid.NewGuid();
        var currentPassword = "currentpassword";
        var newPassword = "newpassword";

        var user = new User
        {
            Id = userId,
            FullName = "Test User",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(currentPassword),
            Role = UserRole.User,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

        result.Should().BeTrue();
        BCrypt.Net.BCrypt.Verify(newPassword, user.PasswordHash).Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
        _userRepositoryMock.Verify(x => x.UpdateAsync(user), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}