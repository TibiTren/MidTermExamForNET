using RoomHub.Api.Common;

namespace RoomHub.Api.Domain;

public static class BookingErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Booking.NotFound", "The booking does not exist.");

    // R2
    public static readonly Error BookedByRequired =
        Error.Validation("Booking.BookedByRequired", "BookedBy is required.");

    // R3
    public static readonly Error EndNotAfterStart =
        Error.Validation("Booking.EndNotAfterStart", "The end must be strictly after the start.");

    // R4
    public static readonly Error AttendeesNotPositive =
        Error.Validation("Booking.AttendeesNotPositive", "Attendees must be strictly positive.");

    public static readonly Error AttendeesExceedCapacity =
        Error.Validation("Booking.AttendeesExceedCapacity", "The room does not have enough seats.");

    // R5
    public static readonly Error StartsInThePast =
        Error.Validation("Booking.StartsInThePast", "A booking cannot start in the past.");

    // R6 / R10
    public static Error DurationTooLong(TimeSpan max) =>
        Error.Validation("Booking.DurationTooLong", $"The booking cannot last more than {max.TotalHours:0} hours in this room.");

    // R7
    public static readonly Error RoomOccupied =
        Error.Conflict("Booking.RoomOccupied", "The room is occupied in this interval.");

    // R8
    public static readonly Error AlreadyCancelled =
        Error.Conflict("Booking.AlreadyCancelled", "The booking is already cancelled.");

    // R9
    public static readonly Error DailyLimitReached =
        Error.Conflict("Booking.DailyLimitReached", "This person already has 2 active bookings on this day.");

    // R11
    public static readonly Error TooLateToCancel =
        Error.Conflict("Booking.TooLateToCancel", "A booking can be cancelled only at least 60 minutes before it starts.");
}
