using AdvanceRequests.Domain.Entities;

namespace AdvanceRequests.Application.Abstractions.Persistence;

public interface IUnitOfWork
{
    Task AddOutboxMessagesAsync(IEnumerable<OutboxMessage> messages, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}