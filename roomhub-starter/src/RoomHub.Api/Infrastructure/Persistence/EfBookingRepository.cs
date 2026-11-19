using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class EfBookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(
        Guid roomId, CancellationToken cancellationToken = default) =>
        await db.Bookings
            .AsNoTracking()
            .Where(b => b.RoomId == roomId && b.Status == BookingStatus.Active)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> ListActiveForPersonOnDayAsync(
        string bookedBy, DateOnly dayUtc, CancellationToken cancellationToken = default)
    {
        var dayStart = dayUtc.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var nextDayStart = dayStart.AddDays(1);

        return await db.Bookings
            .AsNoTracking()
            .Where(b => b.BookedBy == bookedBy
                        && b.Status == BookingStatus.Active
                        && b.StartUtc >= dayStart
                        && b.StartUtc < nextDayStart)
            .ToListAsync(cancellationToken);
    }

    public void Add(Booking booking) => db.Bookings.Add(booking);
}
