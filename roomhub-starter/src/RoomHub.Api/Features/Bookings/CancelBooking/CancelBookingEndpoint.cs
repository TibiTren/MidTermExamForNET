namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static IEndpointRouteBuilder MapCancelBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{id:guid}/cancel", (Guid id, CancelBookingHandler handler) =>
            {
                handler.Handle(new CancelBookingCommand(id));
                return Results.NoContent();
            })
            .WithTags("Bookings");

        return app;
    }
}
