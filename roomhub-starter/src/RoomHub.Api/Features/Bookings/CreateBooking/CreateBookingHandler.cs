using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Infrastructure;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

public sealed class CreateBookingHandler(RoomRepository rooms, BookingRepository bookings)
{
    public Booking Handle(CreateBookingCommand command)
    {
        var room = rooms.Get(command.RoomId);
        if (room is null)
            throw new BusinessException("RoomNotFound", "The room does not exist.", 404);

        if (string.IsNullOrWhiteSpace(command.BookedBy))
            throw new BusinessException("InvalidBookedBy", "BookedBy is required.", 400);

        if (command.EndUtc <= command.StartUtc)
            throw new BusinessException("InvalidRange", "The end must be after the start.", 400);

        if (command.Attendees <= 0)
            throw new BusinessException("InvalidAttendees", "Attendees must be positive.", 400);

        if (command.Attendees > room.Capacity)
            throw new BusinessException("AttendeesExceedCapacity", "The room does not have enough seats.", 400);

        foreach (var existing in bookings.ListForRoom(command.RoomId))
        {
            if (existing.StartUtc <= command.EndUtc && command.StartUtc <= existing.EndUtc)
                throw new BusinessException("RoomOccupied", "The room is occupied in this interval.", 409);
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = command.RoomId,
            BookedBy = command.BookedBy.Trim(),
            StartUtc = command.StartUtc,
            EndUtc = command.EndUtc,
            Attendees = command.Attendees,
            Status = BookingStatus.Active
        };

        bookings.Add(booking);
        return booking;
    }
}
