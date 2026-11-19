using RoomHub.Api.Common;

namespace RoomHub.Api.Domain;

public sealed class Room
{
    public const int LargeRoomCapacityThreshold = 10;

    // R6: default maximum booking duration.
    public static readonly TimeSpan DefaultMaxBookingDuration = TimeSpan.FromHours(4);

    // R10: rooms with capacity > 10 can be booked for at most 2 hours.
    public static readonly TimeSpan LargeRoomMaxBookingDuration = TimeSpan.FromHours(2);

    private Room()
    {
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    public bool IsLarge => Capacity > LargeRoomCapacityThreshold;

    public TimeSpan MaxBookingDuration => IsLarge ? LargeRoomMaxBookingDuration : DefaultMaxBookingDuration;

    // R1: non-empty name and strictly positive capacity.
    public static Result<Room> Create(string? name, int capacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoomErrors.NameRequired;

        if (capacity <= 0)
            return RoomErrors.CapacityNotPositive;

        return new Room
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Capacity = capacity
        };
    }
}
