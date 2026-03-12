using AdvanceRequests.Domain.Common;
using AdvanceRequests.Domain.Enums;
using AdvanceRequests.Domain.Events;
using AdvanceRequests.Domain.Exceptions;

namespace AdvanceRequests.Domain.Entities;

public sealed class AdvanceRequest
{
    private const decimal FeePercentage = 0.05m;
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<AdvanceRequestStatusHistory> _statusHistory = new();
    public IReadOnlyCollection<AdvanceRequestStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    private AdvanceRequest()
    {
        _statusHistory.Add(new AdvanceRequestStatusHistory(
    Guid.NewGuid(),
    Id,
    AdvanceRequestStatus.Pending,
    "system",
    RequestedAtUtc));

    }

    private AdvanceRequest(Guid id, Guid creatorId, decimal grossAmount)
    {
        if (grossAmount <= 100)
            throw new InvalidAdvanceRequestAmountException(grossAmount);

        Id = id;
        CreatorId = creatorId;
        GrossAmount = grossAmount;
        FeePercentageApplied = FeePercentage;
        FeeAmount = Math.Round(grossAmount * FeePercentageApplied, 2, MidpointRounding.ToEven);
        NetAmount = grossAmount - FeeAmount;
        Status = AdvanceRequestStatus.Pending;
        RequestedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new AdvanceRequestCreatedDomainEvent(
            Id,
            CreatorId,
            GrossAmount,
            NetAmount,
            FeeAmount));
    }

    public Guid Id { get; private set; }
    public Guid CreatorId { get; private set; }
    public decimal GrossAmount { get; private set; }
    public decimal FeePercentageApplied { get; private set; }
    public decimal FeeAmount { get; private set; }
    public decimal NetAmount { get; private set; }
    public AdvanceRequestStatus Status { get; private set; }
    public DateTime RequestedAtUtc { get; private set; }
    public DateTime? ProcessedAtUtc { get; private set; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static AdvanceRequest Create(Guid creatorId, decimal grossAmount)
    {
        return new AdvanceRequest(Guid.NewGuid(), creatorId, grossAmount);
    }

    public void Approve()
    {
        if (Status != AdvanceRequestStatus.Pending)
            throw new InvalidStatusTransitionException(Status, AdvanceRequestStatus.Approved);

        Status = AdvanceRequestStatus.Approved;
        ProcessedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new AdvanceRequestApprovedDomainEvent(
            Id,
            CreatorId,
            GrossAmount,
            NetAmount,
            FeeAmount));

            _statusHistory.Add(new AdvanceRequestStatusHistory(
    Guid.NewGuid(),
    Id,
    AdvanceRequestStatus.Approved,
    "admin",
    ProcessedAtUtc.Value));

    }

    public void Reject()
    {
        if (Status != AdvanceRequestStatus.Pending)
            throw new InvalidStatusTransitionException(Status, AdvanceRequestStatus.Rejected);

        Status = AdvanceRequestStatus.Rejected;
        ProcessedAtUtc = DateTime.UtcNow;

        AddDomainEvent(new AdvanceRequestRejectedDomainEvent(
            Id,
            CreatorId,
            GrossAmount,
            NetAmount,
            FeeAmount));

            _statusHistory.Add(new AdvanceRequestStatusHistory(
    Guid.NewGuid(),
    Id,
    AdvanceRequestStatus.Rejected,
    "admin",
    ProcessedAtUtc.Value));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}