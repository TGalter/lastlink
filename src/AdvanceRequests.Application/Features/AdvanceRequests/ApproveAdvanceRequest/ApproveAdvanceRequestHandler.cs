using AdvanceRequests.Application.Abstractions.Messaging;
using AdvanceRequests.Application.Abstractions.Persistence;
using AdvanceRequests.Application.Common;
using AdvanceRequests.Application.Abstractions.Dispatching;

namespace AdvanceRequests.Application.Features.AdvanceRequests.ApproveAdvanceRequest;

public sealed class ApproveAdvanceRequestHandler : ICommandHandler<ApproveAdvanceRequestCommand>
{
    private readonly IAdvanceRequestRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxMessageFactory _outboxMessageFactory;

    public ApproveAdvanceRequestHandler(
        IAdvanceRequestRepository repository,
        IUnitOfWork unitOfWork,
        IOutboxMessageFactory outboxMessageFactory)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _outboxMessageFactory = outboxMessageFactory;
    }

    public async Task<Result> Handle(
        ApproveAdvanceRequestCommand command,
        CancellationToken cancellationToken)
    {
        var advanceRequest = await _repository.GetByIdAsync(command.AdvanceRequestId, cancellationToken);

        if (advanceRequest is null)
            return Result.Failure("Advance request not found.");

        advanceRequest.Approve();

        var outboxMessages = _outboxMessageFactory.Create(advanceRequest.DomainEvents);
        await _unitOfWork.AddOutboxMessagesAsync(outboxMessages, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        advanceRequest.ClearDomainEvents();

        return Result.Success();
    }
}