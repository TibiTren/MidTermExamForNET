namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public static IEndpointRouteBuilder MapCreateRoom(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms", (CreateRoomRequest request, CreateRoomHandler handler) =>
            {
                var room = handler.Handle(new CreateRoomCommand(request.Name, request.Capacity));
                return Results.Created(
                    $"/api/rooms/{room.Id}",
                    new CreateRoomResponse(room.Id, room.Name, room.Capacity));
            })
            .WithTags("Rooms");

        return app;
    }
}
