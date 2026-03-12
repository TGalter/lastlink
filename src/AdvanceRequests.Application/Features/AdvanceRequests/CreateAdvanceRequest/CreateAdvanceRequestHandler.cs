using AdvanceRequests.Application.Abstractions.Dispatching;
using AdvanceRequests.Application.Abstractions.Messaging;
using AdvanceRequests.Application.Abstractions.Persistence;
using AdvanceRequests.Application.Common;
using AdvanceRequests.Application.DTOs;
using AdvanceRequests.Domain.Entities;
using AdvanceRequests.Domain.Exceptions;

namespace AdvanceRequests.Application.Features.AdvanceRequests.CreateAdvanceRequest;

public sealed class CreateAdvanceRequestHandler : ICommandHandler<CreateAdvanceRequestCommand, AdvanceRequestDto>
{
    private readonly IAdvanceRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxMessageFactory _outboxMessageFactory;

    public CreateAdvanceRequestHandler(
        IAdvanceRequestRepository repository,
        IUnitOfWork unitOfWork,
        IOutboxMessageFactory outboxMessageFactory)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outboxMessageFactory = outboxMessageFactory;
    }

    public async Task<Result<AdvanceRequestDto>> Handle(
        CreateAdvanceRequestCommand command,
        CancellationToken cancellationToken)
    {
        var hasPending = await _repository.HasPendingRequestForCreatorAsync(command.CreatorId, cancellationToken);

        if (hasPending)
            throw new PendingAdvanceRequestAlreadyExistsException(command.CreatorId);

        var advanceRequest = AdvanceRequest.Create(command.CreatorId, command.GrossAmount);

        await _repository.AddAsync(advanceRequest, cancellationToken);

        var outboxMessages = _outboxMessageFactory.Create(advanceRequest.DomainEvents);
        await _unitOfWork.AddOutboxMessagesAsync(outboxMessages, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        advanceRequest.ClearDomainEvents();

        return Result<AdvanceRequestDto>.Success(new AdvanceRequestDto
        {
            Id = advanceRequest.Id,
            CreatorId = advanceRequest.CreatorId,
            GrossAmount = advanceRequest.GrossAmount,
            FeeAmount = advanceRequest.FeeAmount,
            NetAmount = advanceRequest.NetAmount,
            Status = advanceRequest.Status,
            RequestedAtUtc = advanceRequest.RequestedAtUtc,
            ProcessedAtUtc = advanceRequest.ProcessedAtUtc
        });
    }
}