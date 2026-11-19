using RoomHub.Api.Common;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public static class CreateRoomEndpoint
{
    public static IEndpointRouteBuilder MapCreateRoom(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms", async (
                CreateRoomRequest request,
                ICommandHandler<CreateRoomCommand, CreateRoomResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.HandleAsync(
                    new CreateRoomCommand(request.Name, request.Capacity), cancellationToken);

                return result.IsSuccess
                    ? Results.Created($"/api/rooms/{result.Value.Id}", result.Value)
                    : result.Error.ToProblem();
            })
            .WithTags("Rooms");

        return app;
    }
}
