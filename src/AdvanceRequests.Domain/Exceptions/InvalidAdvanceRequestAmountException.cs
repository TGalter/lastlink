namespace AdvanceRequests.Domain.Exceptions;

public sealed class InvalidAdvanceRequestAmountException : DomainException
{
    public InvalidAdvanceRequestAmountException(decimal amount)
        : base($"O valor solicitdo deve ser maior que 100. Valor fornecido: {amount}.")
    {
    }
}