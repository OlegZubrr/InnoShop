namespace InnoShop.Users.Infrastructure.Configuration;

public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string ExchangeName { get; set; } = "innoshop.users";
    public string QueueName { get; set; } = "user.events";
}