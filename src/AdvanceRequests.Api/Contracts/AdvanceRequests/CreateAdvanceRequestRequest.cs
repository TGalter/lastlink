namespace AdvanceRequests.Api.Contracts.AdvanceRequests;

public sealed class CreateAdvanceRequestRequest
{
    public Guid CreatorId { get; init; }
    public decimal GrossAmount { get; init; }
}