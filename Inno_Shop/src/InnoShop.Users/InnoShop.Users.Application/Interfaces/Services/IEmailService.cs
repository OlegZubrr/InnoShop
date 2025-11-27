namespace InnoShop.Users.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailConfirmationAsync(string toEmail, string token, string userName);
    Task SendPasswordResetAsync(string toEmail, string token, string userName);
    Task SendWelcomeEmailAsync(string toEmail, string userName);
}