using AdvanceRequests.Application.Common;

namespace AdvanceRequests.Application.Abstractions.Dispatching;

public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
{
    Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}