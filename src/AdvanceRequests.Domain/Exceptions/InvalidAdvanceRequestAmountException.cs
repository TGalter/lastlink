namespace AdvanceRequests.Domain.Exceptions;

public sealed class InvalidAdvanceRequestAmountException : DomainException
{
    public InvalidAdvanceRequestAmountException(decimal amount)
        : base($"The requested amount must be greater than 100. Provided amount: {amount}.")
    {
    }
}