using AdvanceRequests.Domain.Entities;
using AdvanceRequests.Domain.Enums;
using AdvanceRequests.Domain.Exceptions;
using FluentAssertions;

namespace AdvanceRequests.UnitTests.Domain.Entities;

public sealed class AdvanceRequestTests
{
    [Fact]
    public void Create_Should_CreatePendingAdvanceRequest_WhenAmountIsValid()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        const decimal grossAmount = 1000m;

        // Act
        var request = AdvanceRequest.Create(creatorId, grossAmount);

        // Assert
        request.CreatorId.Should().Be(creatorId);
        request.GrossAmount.Should().Be(1000m);
        request.FeeAmount.Should().Be(50m);
        request.NetAmount.Should().Be(950m);
        request.Status.Should().Be(AdvanceRequestStatus.Pending);
        request.ProcessedAtUtc.Should().BeNull();
    }

    [Fact]
    public void Create_Should_ThrowException_WhenAmountIsLessThanOrEqualTo100()
    {
        // Arrange
        var creatorId = Guid.NewGuid();

        // Act
        var act = () => AdvanceRequest.Create(creatorId, 100m);

        // Assert
        act.Should().Throw<InvalidAdvanceRequestAmountException>();
    }

    [Fact]
    public void Approve_Should_ChangeStatusToApproved_WhenRequestIsPending()
    {
        // Arrange
        var request = AdvanceRequest.Create(Guid.NewGuid(), 1000m);

        // Act
        request.Approve();

        // Assert
        request.Status.Should().Be(AdvanceRequestStatus.Approved);
        request.ProcessedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Reject_Should_ChangeStatusToRejected_WhenRequestIsPending()
    {
        // Arrange
        var request = AdvanceRequest.Create(Guid.NewGuid(), 1000m);

        // Act
        request.Reject();

        // Assert
        request.Status.Should().Be(AdvanceRequestStatus.Rejected);
        request.ProcessedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Approve_Should_ThrowException_WhenRequestIsNotPending()
    {
        // Arrange
        var request = AdvanceRequest.Create(Guid.NewGuid(), 1000m);
        request.Approve();

        // Act
        var act = () => request.Approve();

        // Assert
        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Fact]
    public void Reject_Should_ThrowException_WhenRequestIsNotPending()
    {
        // Arrange
        var request = AdvanceRequest.Create(Guid.NewGuid(), 1000m);
        request.Reject();

        // Act
        var act = () => request.Reject();

        // Assert
        act.Should().Throw<InvalidStatusTransitionException>();
    }

    [Fact]
    public void Create_Should_AddCreatedDomainEvent()
    {
        // Arrange
        var creatorId = Guid.NewGuid();

        // Act
        var request = AdvanceRequest.Create(creatorId, 1000m);

        // Assert
        request.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void ClearDomainEvents_Should_RemoveAllDomainEvents()
    {
        // Arrange
        var request = AdvanceRequest.Create(Guid.NewGuid(), 1000m);

        // Act
        request.ClearDomainEvents();

        // Assert
        request.DomainEvents.Should().BeEmpty();
    }
}