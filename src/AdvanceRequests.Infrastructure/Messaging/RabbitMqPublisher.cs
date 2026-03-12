using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace AdvanceRequests.Infrastructure.Messaging;

public sealed class RabbitMqPublisher : IRabbitMqPublisher, IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;
    private readonly IConnection _connection;
    private readonly IChannel _channel;

    public RabbitMqPublisher(
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqPublisher> logger)
    {
        _options = options.Value;
        _logger = logger;

        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password
        };

        _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

        _channel.ExchangeDeclareAsync(
            exchange: _options.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null).GetAwaiter().GetResult();
    }

    public async Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken)
    {
        var routingKey = ToRoutingKey(eventType);
        var body = Encoding.UTF8.GetBytes(payload);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await _channel.BasicPublishAsync(
            exchange: _options.ExchangeName,
            routingKey: routingKey,
            mandatory: false,
            basicProperties: properties,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Published event {EventType} to exchange {Exchange} with routing key {RoutingKey}",
            eventType, _options.ExchangeName, routingKey);
    }

    private static string ToRoutingKey(string eventType)
    {
        return eventType switch
        {
            "AdvanceRequestCreatedDomainEvent" => "advance-request.created",
            "AdvanceRequestApprovedDomainEvent" => "advance-request.approved",
            "AdvanceRequestRejectedDomainEvent" => "advance-request.rejected",
            _ => "advance-request.unknown"
        };
    }

    public void Dispose()
    {
        _channel.Dispose();
        _connection.Dispose();
    }
}