using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed class GetRoomHandler(IRoomRepository rooms)
    : IQueryHandler<GetRoomQuery, GetRoomResponse>
{
    public async Task<Result<GetRoomResponse>> HandleAsync(
        GetRoomQuery query, CancellationToken cancellationToken = default)
    {
        var room = await rooms.GetByIdAsync(query.Id, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        return new GetRoomResponse(room.Id, room.Name, room.Capacity);
    }
}
