using AdvanceRequests.Domain.Enums;

namespace AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;

public sealed class ListAdvanceRequestsQuery
{
    public Guid? CreatorId { get; init; }
    public AdvanceRequestStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public bool IsAdmin { get; init; }
    public Guid? RequestingCreatorId { get; init; }
}