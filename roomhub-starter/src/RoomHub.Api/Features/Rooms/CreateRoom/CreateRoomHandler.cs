using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed class CreateRoomHandler(IRoomRepository rooms, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoomCommand, CreateRoomResponse>
{
    public async Task<Result<CreateRoomResponse>> HandleAsync(
        CreateRoomCommand command, CancellationToken cancellationToken = default)
    {
        var created = Room.Create(command.Name, command.Capacity);
        if (created.IsFailure)
            return created.Error;

        var room = created.Value;
        rooms.Add(room);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateRoomResponse(room.Id, room.Name, room.Capacity);
    }
}
