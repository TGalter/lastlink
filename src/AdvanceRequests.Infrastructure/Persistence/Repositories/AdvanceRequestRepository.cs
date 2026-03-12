using AdvanceRequests.Application.Abstractions.Persistence;
using AdvanceRequests.Domain.Entities;
using AdvanceRequests.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AdvanceRequests.Infrastructure.Persistence.Repositories;

public sealed class AdvanceRequestRepository : IAdvanceRequestRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AdvanceRequestRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(AdvanceRequest advanceRequest, CancellationToken cancellationToken)
    {
        await _dbContext.AdvanceRequests.AddAsync(advanceRequest, cancellationToken);
    }

    public async Task<AdvanceRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.AdvanceRequests
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> HasPendingRequestForCreatorAsync(Guid creatorId, CancellationToken cancellationToken)
    {
        return await _dbContext.AdvanceRequests
            .AnyAsync(
                x => x.CreatorId == creatorId && x.Status == AdvanceRequestStatus.Pending,
                cancellationToken);
    }

    public async Task<IReadOnlyList<AdvanceRequest>> ListAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.AdvanceRequests
            .OrderByDescending(x => x.RequestedAtUtc)
            .ToListAsync(cancellationToken);
    }
}