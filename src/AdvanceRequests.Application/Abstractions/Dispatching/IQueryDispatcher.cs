using AdvanceRequests.Application.Common;

namespace AdvanceRequests.Application.Abstractions.Dispatching;

public interface IQueryDispatcher
{
    Task<Result<TResult>> DispatchAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult>;
}