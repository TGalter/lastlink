namespace AdvanceRequests.Domain.Exceptions;

public sealed class PendingAdvanceRequestAlreadyExistsException : DomainException
{
    public PendingAdvanceRequestAlreadyExistsException(Guid creatorId)
        : base($"O criador '{creatorId}' já possui uma solicitação de adiantamento pendente.")
    {
    }
}