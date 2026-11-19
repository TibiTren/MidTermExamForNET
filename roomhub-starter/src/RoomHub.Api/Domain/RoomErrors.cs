using RoomHub.Api.Common;

namespace RoomHub.Api.Domain;

public static class RoomErrors
{
    public static readonly Error NotFound =
        Error.NotFound("Room.NotFound", "The room does not exist.");

    public static readonly Error NameRequired =
        Error.Validation("Room.NameRequired", "Room name is required.");

    public static readonly Error CapacityNotPositive =
        Error.Validation("Room.CapacityNotPositive", "Capacity must be strictly positive.");
}
