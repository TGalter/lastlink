using AdvanceRequests.Application.Common;

namespace AdvanceRequests.Application.Abstractions.Dispatching;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken);
}