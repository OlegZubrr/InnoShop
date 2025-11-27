using InnoShop.Users.Domain.Entities;

namespace InnoShop.Users.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string GenerateEmailConfirmationToken();
    string GeneratePasswordResetToken();
}