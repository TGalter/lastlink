namespace AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;

public sealed class CreateAdvanceRequestCommand
{
    public Guid CreatorId { get; init; }
    public decimal GrossAmount { get; init; }
}