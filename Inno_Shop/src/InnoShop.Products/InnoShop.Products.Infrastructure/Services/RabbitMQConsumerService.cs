using System.Text;
using System.Text.Json;
using InnoShop.Products.Application.Events;
using InnoShop.Products.Application.Interfaces.Repositories;
using InnoShop.Products.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using IModel = RabbitMQ.Client.IModel;

namespace InnoShop.Products.Infrastructure.Services;

public class RabbitMQConsumerService : BackgroundService
{
    private readonly ILogger<RabbitMQConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly RabbitMQSettings _settings;
    private IModel? _channel;
    private IConnection? _connection;

    public RabbitMQConsumerService(
        IServiceProvider serviceProvider,
        ILogger<RabbitMQConsumerService> logger,
        IOptions<RabbitMQSettings> settings)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _settings = settings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _settings.HostName,
                Port = _settings.Port,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(
                _settings.ExchangeName,
                ExchangeType.Topic,
                true);

            var queueDeclareResult = _channel.QueueDeclare(
                string.Empty);

            var queueName = queueDeclareResult.QueueName;

            _channel.QueueBind(
                queueName,
                _settings.ExchangeName,
                "user.deactivated");

            _channel.QueueBind(
                queueName,
                _settings.ExchangeName,
                "user.activated");

            _channel.QueueBind(
                queueName,
                _settings.ExchangeName,
                "user.deleted");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                _logger.LogInformation("Received message: {RoutingKey}", routingKey);

                try
                {
                    ProcessMessageAsync(routingKey, message).GetAwaiter().GetResult();
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queueName,
                false,
                consumer);

            _logger.LogInformation("RabbitMQ Consumer started");

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting RabbitMQ Consumer");
        }
    }

    private async Task ProcessMessageAsync(string routingKey, string message)
    {
        using var scope = _serviceProvider.CreateScope();
        var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

        switch (routingKey)
        {
            case "user.deactivated":
                var deactivatedEvent = JsonSerializer.Deserialize<UserDeactivatedEvent>(message);
                if (deactivatedEvent != null)
                {
                    _logger.LogInformation("Hiding products for user {UserId}", deactivatedEvent.UserId);
                    await productRepository.SoftDeleteByUserIdAsync(deactivatedEvent.UserId);
                }

                break;

            case "user.activated":
                var activatedEvent = JsonSerializer.Deserialize<UserActivatedEvent>(message);
                if (activatedEvent != null)
                {
                    _logger.LogInformation("Restoring products for user {UserId}", activatedEvent.UserId);
                    await productRepository.RestoreByUserIdAsync(activatedEvent.UserId);
                }

                break;

            case "user.deleted":
                var deletedEvent = JsonSerializer.Deserialize<UserDeletedEvent>(message);
                if (deletedEvent != null)
                    _logger.LogInformation("Processing deletion of user {UserId}", deletedEvent.UserId);
                await productRepository.DeleteAsync(deletedEvent.UserId);
                break;
        }
    }

    public override void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
        base.Dispose();
    }
}