namespace AdvanceRequests.Domain.Exceptions;

public sealed class PendingAdvanceRequestAlreadyExistsException : DomainException
{
    public PendingAdvanceRequestAlreadyExistsException(Guid creatorId)
        : base($"Creator '{creatorId}' already has a pending advance request.")
    {
    }
}