using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed class CreateRoomHandler(RoomRepository rooms)
{
    public Room Handle(CreateRoomCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            throw new BusinessException("InvalidName", "Room name is required.", 400);

        if (command.Capacity <= 0)
            throw new BusinessException("InvalidCapacity", "Capacity must be positive.", 400);

        var room = new Room
        {
            Id = Guid.NewGuid(),
            Name = command.Name.Trim(),
            Capacity = command.Capacity
        };

        rooms.Add(room);
        return room;
    }
}
