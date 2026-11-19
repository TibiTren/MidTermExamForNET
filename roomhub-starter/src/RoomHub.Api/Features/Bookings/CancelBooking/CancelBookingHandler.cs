using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed class CancelBookingHandler(
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CancelBookingCommand>
{
    public async Task<Result> HandleAsync(
        CancelBookingCommand command, CancellationToken cancellationToken = default)
    {
        var booking = await bookings.GetByIdAsync(command.Id, cancellationToken);
        if (booking is null)
            return BookingErrors.NotFound;

        // R8 + R11 are enforced by the domain.
        var cancelled = booking.Cancel(timeProvider.GetUtcNow().UtcDateTime);
        if (cancelled.IsFailure)
            return cancelled;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
