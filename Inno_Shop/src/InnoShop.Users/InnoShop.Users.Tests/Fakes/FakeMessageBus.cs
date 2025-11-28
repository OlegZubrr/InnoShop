using InnoShop.Users.Application.Interfaces.Services;

namespace InnoShop.Users.Tests.Fakes;

public class FakeMessageBus : IMessageBus
{
    public Task PublishAsync<T>(T message, string routingKey) where T : class
    {
        return Task.CompletedTask;
    }
}