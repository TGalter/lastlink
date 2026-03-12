using AdvanceRequests.Domain.Enums;

namespace AdvanceRequests.Domain.Entities;

public sealed class AdvanceRequestStatusHistory
{
    private AdvanceRequestStatusHistory()
    {
    }

    public AdvanceRequestStatusHistory(
        Guid id,
        Guid advanceRequestId,
        AdvanceRequestStatus status,
        string changedBy,
        DateTime changedAtUtc)
    {
        Id = id;
        AdvanceRequestId = advanceRequestId;
        Status = status;
        ChangedBy = changedBy;
        ChangedAtUtc = changedAtUtc;
    }

    public Guid Id { get; private set; }
    public Guid AdvanceRequestId { get; private set; }
    public AdvanceRequestStatus Status { get; private set; }
    public string ChangedBy { get; private set; } = string.Empty;
    public DateTime ChangedAtUtc { get; private set; }
}