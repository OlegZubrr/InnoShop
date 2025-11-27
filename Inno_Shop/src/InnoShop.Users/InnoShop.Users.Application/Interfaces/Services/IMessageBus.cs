namespace InnoShop.Users.Application.Interfaces.Services;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string routingKey) where T : class;
}