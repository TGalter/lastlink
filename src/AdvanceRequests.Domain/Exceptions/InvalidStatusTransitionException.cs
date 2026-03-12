using AdvanceRequests.Domain.Enums;

namespace AdvanceRequests.Domain.Exceptions;

public sealed class InvalidStatusTransitionException : DomainException
{
    public InvalidStatusTransitionException(AdvanceRequestStatus current, AdvanceRequestStatus target)
        : base($"Transição de status inválida de {current} para {target}.")
    {
    }
}