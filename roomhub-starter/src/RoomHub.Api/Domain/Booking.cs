using RoomHub.Api.Common;

namespace RoomHub.Api.Domain;

public sealed class Booking
{
    // R9: a participant can have at most 2 active bookings per UTC day.
    public const int MaxActiveBookingsPerPersonPerDay = 2;

    // R11: a booking can be cancelled only if it starts at least 60 minutes from now.
    public static readonly TimeSpan MinNoticeForCancellation = TimeSpan.FromMinutes(60);

    private Booking()
    {
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public bool IsActive => Status == BookingStatus.Active;

    // The "day" of a booking is the UTC date of its start (R9).
    public DateOnly DayUtc => DateOnly.FromDateTime(StartUtc);

    // R7: half-open intervals [start, end). Touching intervals do not overlap.
    public bool OverlapsWith(DateTime startUtc, DateTime endUtc) =>
        StartUtc < endUtc && startUtc < EndUtc;

    /// <summary>
    /// Creates a new active booking after checking every booking rule.
    /// </summary>
    /// <param name="room">The room being booked (must exist; 404 is checked by the caller).</param>
    /// <param name="nowUtc">Current time, taken from TimeProvider by the caller.</param>
    /// <param name="roomBookings">Existing bookings of the same room (for R7).</param>
    /// <param name="sameDayBookingsOfPerson">Existing bookings of the same person on the same UTC day (for R9).</param>
    public static Result<Booking> Create(
        Room room,
        string? bookedBy,
        DateTime startUtc,
        DateTime endUtc,
        int attendees,
        DateTime nowUtc,
        IEnumerable<Booking> roomBookings,
        IEnumerable<Booking> sameDayBookingsOfPerson)
    {
        // R2
        if (string.IsNullOrWhiteSpace(bookedBy))
            return BookingErrors.BookedByRequired;

        // R3
        if (endUtc <= startUtc)
            return BookingErrors.EndNotAfterStart;

        // R4
        if (attendees <= 0)
            return BookingErrors.AttendeesNotPositive;

        if (attendees > room.Capacity)
            return BookingErrors.AttendeesExceedCapacity;

        // R5
        if (startUtc < nowUtc)
            return BookingErrors.StartsInThePast;

        // R6 + R10: the room decides its maximum duration.
        if (endUtc - startUtc > room.MaxBookingDuration)
            return BookingErrors.DurationTooLong(room.MaxBookingDuration);

        // R7: only active bookings of this room block the interval.
        if (roomBookings.Any(b => b.RoomId == room.Id && b.IsActive && b.OverlapsWith(startUtc, endUtc)))
            return BookingErrors.RoomOccupied;

        // R9: count active bookings of the same person on the same UTC day, across all rooms.
        var person = bookedBy.Trim();
        var day = DateOnly.FromDateTime(startUtc);
        var activeThatDay = sameDayBookingsOfPerson.Count(b =>
            b.IsActive && b.BookedBy == person && b.DayUtc == day);

        if (activeThatDay >= MaxActiveBookingsPerPersonPerDay)
            return BookingErrors.DailyLimitReached;

        return new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = room.Id,
            BookedBy = person,
            StartUtc = startUtc,
            EndUtc = endUtc,
            Attendees = attendees,
            Status = BookingStatus.Active
        };
    }

    // R8 + R11
    public Result Cancel(DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            return BookingErrors.AlreadyCancelled;

        if (StartUtc - nowUtc < MinNoticeForCancellation)
            return BookingErrors.TooLateToCancel;

        Status = BookingStatus.Cancelled;
        return Result.Success();
    }
}
