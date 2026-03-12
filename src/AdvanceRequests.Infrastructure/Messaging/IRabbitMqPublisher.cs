namespace AdvanceRequests.Infrastructure.Messaging;

public interface IRabbitMqPublisher
{
    Task PublishAsync(string eventType, string payload, CancellationToken cancellationToken);
}