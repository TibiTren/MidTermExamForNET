using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure;

// In-memory storage: data is lost when the application restarts (replaced by EF Core at Level C).
public sealed class InMemoryRoomRepository : IRoomRepository
{
    private readonly List<Room> _rooms = new();
    private readonly object _lock = new();

    public Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            return Task.FromResult(_rooms.FirstOrDefault(r => r.Id == id));
        }
    }

    public void Add(Room room)
    {
        lock (_lock)
        {
            _rooms.Add(room);
        }
    }
}
