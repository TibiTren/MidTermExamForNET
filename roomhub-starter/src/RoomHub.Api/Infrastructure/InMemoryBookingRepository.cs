using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure;

// In-memory storage: data is lost when the application restarts (replaced by EF Core at Level C).
public sealed class InMemoryBookingRepository : IBookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_bookings.FirstOrDefault(b => b.Id == id));
        }
    }

    public Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            IReadOnlyList<Booking> result = _bookings
                .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
                .ToList();
            return Task.FromResult(result);
        }
    }

    public Task<IReadOnlyList<Booking>> ListActiveForPersonOnDayAsync(
        string bookedBy, DateOnly dayUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = dayUtc.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var nextDayStart = dayStart.AddDays(1);

        lock (_lock)
        {
            IReadOnlyList<Booking> result = _bookings
                .Where(b => b.BookedBy == bookedBy
                            && b.Status == BookingStatus.Active
                            && b.StartUtc >= dayStart
                            && b.StartUtc < nextDayStart)
                .ToList();
            return Task.FromResult(result);
        }
    }

    public void Add(Booking booking)
    {
        lock (_lock)
        {
            _bookings.Add(booking);
        }
    }
}
