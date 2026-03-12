using AdvanceRequests.Domain.Enums;

namespace AdvanceRequests.Domain.Exceptions;

public sealed class InvalidStatusTransitionException : DomainException
{
    public InvalidStatusTransitionException(AdvanceRequestStatus current, AdvanceRequestStatus target)
        : base($"Invalid status transition from {current} to {target}.")
    {
    }
}