using AdvanceRequests.Application.Abstractions.Dispatching;

namespace AdvanceRequests.Application.Features.AdvanceRequests.ApproveAdvanceRequest;

public sealed class ApproveAdvanceRequestCommand : ICommand
{
    public Guid AdvanceRequestId { get; init; }
}