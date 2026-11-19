using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed class GetBookingHandler(IBookingRepository bookings)
    : IQueryHandler<GetBookingQuery, GetBookingResponse>
{
    public async Task<Result<GetBookingResponse>> HandleAsync(
        GetBookingQuery query, CancellationToken cancellationToken = default)
    {
        var b = await bookings.GetByIdAsync(query.Id, cancellationToken);
        if (b is null)
            return BookingErrors.NotFound;

        return new GetBookingResponse(
            b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString());
    }
}
