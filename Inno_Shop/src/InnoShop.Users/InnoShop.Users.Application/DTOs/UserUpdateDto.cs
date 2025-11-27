using InnoShop.Users.Domain.Enums;

namespace InnoShop.Users.Application.DTOs;

public class UserUpdateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
}