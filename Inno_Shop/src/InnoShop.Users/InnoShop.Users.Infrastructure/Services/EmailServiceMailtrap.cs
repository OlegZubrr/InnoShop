using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Infrastructure.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace InnoShop.Users.Infrastructure.Services;

public class EmailServiceMailtrap : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailServiceMailtrap> _logger;

    public EmailServiceMailtrap(
        IOptions<EmailSettings> emailSettings,
        ILogger<EmailServiceMailtrap> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailConfirmationAsync(string toEmail, string token, string userName)
    {
        var subject = "Confirm Your Email - InnoShop";
        var confirmationLink = $"http://localhost:5000/api/auth/confirm-email?token={token}";

        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Hello {userName}!</h2>
                    <p>Thank you for registering at InnoShop.</p>
                    <p>Please confirm your email address by clicking the button below:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' 
                           style='display: inline-block; padding: 12px 30px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Confirm Email
                        </a>
                    </div>
                    <p style='color: #666; font-size: 14px;'>Or copy and paste this link into your browser:</p>
                    <p style='color: #0066cc; word-break: break-all;'>{confirmationLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>This link will expire in 24 hours.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='color: #999; font-size: 12px;'>Best regards,<br>InnoShop Team</p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPasswordResetAsync(string toEmail, string token, string userName)
    {
        var subject = "Reset Your Password - InnoShop";
        var resetLink = $"http://localhost:5000/api/auth/reset-password?token={token}";

        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Hello {userName}!</h2>
                    <p>We received a request to reset your password for your InnoShop account.</p>
                    <p>Click the button below to reset your password:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='display: inline-block; padding: 12px 30px; background-color: #f44336; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Reset Password
                        </a>
                    </div>
                    <p style='color: #666; font-size: 14px;'>Or copy and paste this link into your browser:</p>
                    <p style='color: #0066cc; word-break: break-all;'>{resetLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>This link will expire in 1 hour.</p>
                    <p style='color: #999; font-size: 12px;'>If you didn't request this, please ignore this email and your password will remain unchanged.</p>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='color: #999; font-size: 12px;'>Best regards,<br>InnoShop Team</p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "Welcome to InnoShop!";

        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #4CAF50;'>Welcome {userName}!</h2>
                    <p>Your email has been successfully confirmed.</p>
                    <p>You can now enjoy all features of InnoShop:</p>
                    <ul style='color: #333;'>
                        <li>Browse products</li>
                        <li>Create your own listings</li>
                        <li>Manage your account</li>
                    </ul>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='http://localhost:5000' 
                           style='display: inline-block; padding: 12px 30px; background-color: #2196F3; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Start Shopping
                        </a>
                    </div>
                    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                    <p style='color: #999; font-size: 12px;'>Happy shopping!<br>InnoShop Team</p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            // Подключаемся к Mailtrap
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

            // Аутентификация
            await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

            // Отправляем письмо
            await client.SendAsync(message);

            // Отключаемся
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}: {Message}", toEmail, ex.Message);
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }
}