using RoomHub.Api.Common;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public static IEndpointRouteBuilder MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings", async (
                Guid roomId,
                CreateBookingRequest request,
                ICommandHandler<CreateBookingCommand, CreateBookingResponse> handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.HandleAsync(
                    new CreateBookingCommand(
                        roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees),
                    cancellationToken);

                return result.IsSuccess
                    ? Results.Created($"/api/bookings/{result.Value.Id}", result.Value)
                    : result.Error.ToProblem();
            })
            .WithTags("Bookings");

        return app;
    }
}
