using AdvanceRequests.Domain.Enums;

namespace AdvanceRequests.Application.DTOs;

public sealed class AdvanceRequestDto
{
    public Guid Id { get; init; }
    public Guid CreatorId { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal FeeAmount { get; init; }
    public decimal NetAmount { get; init; }
    public AdvanceRequestStatus Status { get; init; }
    public DateTime RequestedAtUtc { get; init; }
    public DateTime? ProcessedAtUtc { get; init; }
}