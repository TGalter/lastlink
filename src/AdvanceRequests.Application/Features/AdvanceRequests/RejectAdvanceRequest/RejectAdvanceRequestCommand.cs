using AdvanceRequests.Application.Abstractions.Dispatching;

namespace AdvanceRequests.Application.Features.AdvanceRequests.RejectAdvanceRequest;

public sealed class RejectAdvanceRequestCommand : ICommand
{
    public Guid AdvanceRequestId { get; init; }
}