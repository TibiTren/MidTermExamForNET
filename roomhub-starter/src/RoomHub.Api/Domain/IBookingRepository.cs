namespace RoomHub.Api.Domain;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Active bookings of a room (R7).
    Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default);

    // Active bookings of a person whose start falls on the given UTC day, in any room (R9).
    Task<IReadOnlyList<Booking>> ListActiveForPersonOnDayAsync(
        string bookedBy, DateOnly dayUtc, CancellationToken cancellationToken = default);

    void Add(Booking booking);
}
