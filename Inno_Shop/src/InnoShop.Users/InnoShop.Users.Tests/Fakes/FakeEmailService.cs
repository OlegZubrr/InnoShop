using InnoShop.Users.Application.Interfaces.Services;

namespace InnoShop.Users.Tests.Fakes;

public class FakeEmailService : IEmailService
{
    public Task SendEmailConfirmationAsync(string email, string token, string fullName)
    {
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string email, string fullName)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(string email, string token, string fullName)
    {
        return Task.CompletedTask;
    }
}