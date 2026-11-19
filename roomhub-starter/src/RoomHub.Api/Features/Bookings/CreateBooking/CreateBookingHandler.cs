using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, CreateBookingResponse>
{
    public async Task<Result<CreateBookingResponse>> HandleAsync(
        CreateBookingCommand command, CancellationToken cancellationToken = default)
    {
        // 404 before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return RoomErrors.NotFound;

        var roomBookings = await bookings.ListActiveForRoomAsync(room.Id, cancellationToken);

        var person = command.BookedBy?.Trim() ?? string.Empty;
        var sameDayBookingsOfPerson = person.Length == 0
            ? Array.Empty<Booking>()
            : await bookings.ListActiveForPersonOnDayAsync(
                person, DateOnly.FromDateTime(command.StartUtc), cancellationToken);

        var created = Booking.Create(
            room,
            command.BookedBy,
            command.StartUtc,
            command.EndUtc,
            command.Attendees,
            timeProvider.GetUtcNow().UtcDateTime,
            roomBookings,
            sameDayBookingsOfPerson);

        if (created.IsFailure)
            return created.Error;

        var booking = created.Value;
        bookings.Add(booking);

        return new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString());
    }
}
