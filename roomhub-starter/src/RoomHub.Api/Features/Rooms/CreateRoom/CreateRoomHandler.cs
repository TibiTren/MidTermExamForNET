using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed class CreateRoomHandler(IRoomRepository rooms)
    : ICommandHandler<CreateRoomCommand, CreateRoomResponse>
{
    public Task<Result<CreateRoomResponse>> HandleAsync(
        CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var created = Room.Create(command.Name, command.Capacity);
        if (created.IsFailure)
            return Task.FromResult(Result<CreateRoomResponse>.Failure(created.Error));

        var room = created.Value;
        rooms.Add(room);

        return Task.FromResult(Result<CreateRoomResponse>.Success(
            new CreateRoomResponse(room.Id, room.Name, room.Capacity)));
    }
}
