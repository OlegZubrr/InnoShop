using System.Text;
using System.Text.Json;
using InnoShop.Users.Application.Interfaces.Services;
using InnoShop.Users.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace InnoShop.Users.Infrastructure.Services;

public class RabbitMQMessageBus : IMessageBus, IDisposable
{
    private readonly ILogger<RabbitMQMessageBus> _logger;
    private readonly RabbitMQSettings _settings;
    private IModel? _channel;
    private IConnection? _connection;

    public RabbitMQMessageBus(
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQMessageBus> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        InitializeRabbitMQ();
    }

    public void Dispose()
    {
        try
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ connection closed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while disposing RabbitMQ connection");
        }
    }

    public async Task PublishAsync<T>(T message, string routingKey) where T : class
    {
        if (_channel == null || !_channel.IsOpen)
        {
            _logger.LogWarning("RabbitMQ channel is not open. Attempting to reconnect...");
            InitializeRabbitMQ();
        }

        try
        {
            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = _channel!.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.DeliveryMode = 2;

            _channel.BasicPublish(
                _settings.ExchangeName,
                routingKey,
                properties,
                body
            );

            _logger.LogInformation(
                "Message published to RabbitMQ. Exchange: {Exchange}, RoutingKey: {RoutingKey}, Message: {Message}",
                _settings.ExchangeName,
                routingKey,
                json
            );

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to RabbitMQ. RoutingKey: {RoutingKey}", routingKey);
            throw;
        }
    }

    private void InitializeRabbitMQ()
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
                true,
                false
            );

            _logger.LogInformation("RabbitMQ connection established successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RabbitMQ connection");
        }
    }
}