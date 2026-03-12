using AdvanceRequests.Domain.Entities;

namespace AdvanceRequests.Application.Abstractions.Persistence;

public interface IAdvanceRequestRepository
{
    Task AddAsync(AdvanceRequest advanceRequest, CancellationToken cancellationToken);
    Task<AdvanceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> HasPendingRequestForCreatorAsync(Guid creatorId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AdvanceRequest>> ListAsync(CancellationToken cancellationToken);
}