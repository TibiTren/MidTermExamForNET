using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
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

        var startUtc = ToUtc(command.StartUtc);
        var endUtc = ToUtc(command.EndUtc);

        var roomBookings = await bookings.ListActiveForRoomAsync(room.Id, cancellationToken);

        var person = command.BookedBy?.Trim() ?? string.Empty;
        var sameDayBookingsOfPerson = person.Length == 0
            ? Array.Empty<Booking>()
            : await bookings.ListActiveForPersonOnDayAsync(
                person, DateOnly.FromDateTime(startUtc), cancellationToken);

        var created = Booking.Create(
            room,
            command.BookedBy,
            startUtc,
            endUtc,
            command.Attendees,
            timeProvider.GetUtcNow().UtcDateTime,
            roomBookings,
            sameDayBookingsOfPerson);

        if (created.IsFailure)
            return created.Error;

        var booking = created.Value;
        bookings.Add(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString());
    }

    // "...Z" arrives as UTC; an offset like "+02:00" arrives as local time; no offset is treated as UTC.
    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
