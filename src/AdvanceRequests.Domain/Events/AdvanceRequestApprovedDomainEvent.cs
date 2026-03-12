using AdvanceRequests.Domain.Common;

namespace AdvanceRequests.Domain.Events;

public sealed record AdvanceRequestApprovedDomainEvent(
    Guid AdvanceRequestId,
    Guid CreatorId,
    decimal GrossAmount,
    decimal NetAmount,
    decimal FeeAmount) : IDomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}