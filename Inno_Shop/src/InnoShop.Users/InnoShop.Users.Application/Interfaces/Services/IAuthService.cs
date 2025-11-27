using InnoShop.Users.Application.DTOs;

namespace InnoShop.Users.Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserResponseDto> RegisterAsync(UserCreateDto createDto);
    Task<bool> ConfirmEmailAsync(string token);
    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}