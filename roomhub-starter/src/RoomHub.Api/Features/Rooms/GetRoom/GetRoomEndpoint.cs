namespace RoomHub.Api.Features.Rooms.GetRoom;

public static class GetRoomEndpoint
{
    public static IEndpointRouteBuilder MapGetRoom(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/rooms/{id:guid}", (Guid id, GetRoomHandler handler) =>
            {
                var room = handler.Handle(new GetRoomQuery(id));
                return Results.Ok(new GetRoomResponse(room.Id, room.Name, room.Capacity));
            })
            .WithTags("Rooms");

        return app;
    }
}
