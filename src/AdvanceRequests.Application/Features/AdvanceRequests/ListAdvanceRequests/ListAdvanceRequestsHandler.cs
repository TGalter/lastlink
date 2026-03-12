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

        var filtered = items.AsQueryable();

        if (query.IsAdmin)
        {
            if (query.CreatorId.HasValue)
                filtered = filtered.Where(x => x.CreatorId == query.CreatorId.Value);
        }
        else
        {
            if (!query.RequestingCreatorId.HasValue)
                return Result<IReadOnlyList<AdvanceRequestDto>>.Failure("CreatorId do usuário autenticado não encontrado.");

            filtered = filtered.Where(x => x.CreatorId == query.RequestingCreatorId.Value);
        }

        if (query.Status.HasValue)
            filtered = filtered.Where(x => x.Status == query.Status.Value);

        if (query.FromDate.HasValue)
            filtered = filtered.Where(x => x.RequestedAtUtc >= query.FromDate.Value);

        if (query.ToDate.HasValue)
            filtered = filtered.Where(x => x.RequestedAtUtc <= query.ToDate.Value);

        var result = filtered
            .OrderByDescending(x => x.RequestedAtUtc)
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