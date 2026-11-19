namespace RoomHub.Api.Features.Bookings.GetBooking;

public static class GetBookingEndpoint
{
    public static IEndpointRouteBuilder MapGetBooking(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/bookings/{id:guid}", (Guid id, GetBookingHandler handler) =>
            {
                var b = handler.Handle(new GetBookingQuery(id));
                return Results.Ok(new GetBookingResponse(
                    b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString()));
            })
            .WithTags("Bookings");

        return app;
    }
}
