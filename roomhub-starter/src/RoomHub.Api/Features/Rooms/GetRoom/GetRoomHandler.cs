using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed class GetRoomHandler(RoomRepository rooms)
{
    public Room Handle(GetRoomQuery query)
    {
        var room = rooms.Get(query.Id);
        if (room is null)
            throw new BusinessException("RoomNotFound", "The room does not exist.", 404);

        return room;
    }
}
