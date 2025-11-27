namespace InnoShop.Users.Application.DTOs;

using InnoShop.Users.Domain.Enums;

public class UserCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
}