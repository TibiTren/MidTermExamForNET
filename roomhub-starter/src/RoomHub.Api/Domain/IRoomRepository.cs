namespace RoomHub.Api.Domain;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    void Add(Room room);
}
