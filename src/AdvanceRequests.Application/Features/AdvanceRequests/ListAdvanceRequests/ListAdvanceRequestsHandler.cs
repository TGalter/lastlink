using AdvanceRequests.Application.Abstractions.Persistence;
using AdvanceRequests.Application.Common;
using AdvanceRequests.Application.DTOs;

namespace AdvanceRequests.Application.Features.AdvanceRequests.ListAdvanceRequests;

public sealed class ListAdvanceRequestsHandler
{
    private readonly IAdvanceRequestRepository _repository;

    public ListAdvanceRequestsHandler(IAdvanceRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<AdvanceRequestDto>>> Handle(
        ListAdvanceRequestsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await _repository.ListAsync(cancellationToken);

        var result = items
            .Select(x => new AdvanceRequestDto
            {
                Id = x.Id,
                CreatorId = x.CreatorId,
                GrossAmount = x.GrossAmount,
                FeeAmount = x.FeeAmount,
                NetAmount = x.NetAmount,
                Status = x.Status,
                RequestedAtUtc = x.RequestedAtUtc,
                ProcessedAtUtc = x.ProcessedAtUtc
            })
            .ToList()
            .AsReadOnly();

        return Result<IReadOnlyList<AdvanceRequestDto>>.Success(result);
    }
}