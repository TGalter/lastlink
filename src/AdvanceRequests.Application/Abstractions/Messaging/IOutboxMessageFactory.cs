using AdvanceRequests.Domain.Common;
using AdvanceRequests.Domain.Entities;

namespace AdvanceRequests.Application.Abstractions.Messaging;

public interface IOutboxMessageFactory
{
    IReadOnlyCollection<OutboxMessage> Create(IEnumerable<IDomainEvent> domainEvents);
}