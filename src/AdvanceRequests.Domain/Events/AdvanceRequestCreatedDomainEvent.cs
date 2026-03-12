using AdvanceRequests.Domain.Common;

namespace AdvanceRequests.Domain.Events;

public sealed record AdvanceRequestCreatedDomainEvent(
    Guid AdvanceRequestId,
    Guid CreatorId,
    decimal GrossAmount,
    decimal NetAmount,
    decimal FeeAmount) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}