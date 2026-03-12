using AdvanceRequests.Application.Abstractions.Dispatching;
using AdvanceRequests.Application.DTOs;

namespace AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;

public sealed class CreateAdvanceRequestCommand : ICommand
{
    public Guid CreatorId { get; init; }
    public decimal GrossAmount { get; init; }
}