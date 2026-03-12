namespace AdvanceRequests.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}