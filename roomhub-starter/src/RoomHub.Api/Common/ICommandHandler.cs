namespace RoomHub.Api.Common;

// Command that changes state and returns a value (e.g. the created resource).
public interface ICommandHandler<in TCommand, TResponse>
{
    Task<Result<TResponse>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

// Command that changes state and returns nothing on success.
public interface ICommandHandler<in TCommand>
{
    Task<Result> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}
