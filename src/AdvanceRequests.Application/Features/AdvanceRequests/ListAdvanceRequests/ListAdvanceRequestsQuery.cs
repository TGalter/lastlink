using AdvanceRequests.Application.Abstractions.Dispatching;
using AdvanceRequests.Application.DTOs;
using AdvanceRequests.Domain.Enums;

namespace AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;

public sealed class ListAdvanceRequestsQuery : IQuery<IReadOnlyList<AdvanceRequestDto>>
{
    public Guid? CreatorId { get; init; }
    public AdvanceRequestStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public bool IsAdmin { get; init; }
    public Guid? RequestingCreatorId { get; init; }
}