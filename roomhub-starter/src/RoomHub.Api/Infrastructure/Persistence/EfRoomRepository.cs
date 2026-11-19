using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class EfRoomRepository(RoomHubDbContext db) : IRoomRepository
{
    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Rooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public void Add(Room room) => db.Rooms.Add(room);
}
