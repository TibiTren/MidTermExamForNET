namespace RoomHub.Api.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public static IEndpointRouteBuilder MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings",
                (Guid roomId, CreateBookingRequest request, CreateBookingHandler handler) =>
                {
                    var booking = handler.Handle(new CreateBookingCommand(
                        roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees));

                    return Results.Created(
                        $"/api/bookings/{booking.Id}",
                        new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString()));
                })
            .WithTags("Bookings");

        return app;
    }
}
