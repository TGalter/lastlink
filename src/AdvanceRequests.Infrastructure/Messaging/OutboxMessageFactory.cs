using System.Text.Json;
using AdvanceRequests.Application.Abstractions.Messaging;
using AdvanceRequests.Domain.Common;
using AdvanceRequests.Domain.Entities;

namespace AdvanceRequests.Infrastructure.Messaging;

public sealed class OutboxMessageFactory : IOutboxMessageFactory
{
    public IReadOnlyCollection<OutboxMessage> Create(IEnumerable<IDomainEvent> domainEvents)
    {
        return domainEvents
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                domainEvent.GetType().Name,
                JsonSerializer.Serialize(domainEvent),
                domainEvent.OccurredOnUtc))
            .ToList();
    }
}