namespace InnoShop.Users.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public UserResponseDto User { get; set; }
}