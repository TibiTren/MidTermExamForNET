using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed class GetBookingHandler(BookingRepository bookings)
{
    public Booking Handle(GetBookingQuery query)
    {
        var booking = bookings.Get(query.Id);
        if (booking is null)
            throw new BusinessException("BookingNotFound", "The booking does not exist.", 404);

        return booking;
    }
}
