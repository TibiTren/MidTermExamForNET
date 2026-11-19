using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Infrastructure.Persistence;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    // SQLite has no date type and loses DateTimeKind. Values are stored as UTC
    // and marked as UTC again when read, so the JSON keeps the trailing "Z".
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter = new(
        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v.ToUniversalTime(), DateTimeKind.Utc),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();
        builder.Property(b => b.BookedBy).IsRequired().HasMaxLength(200);
        builder.Property(b => b.StartUtc).IsRequired().HasConversion(UtcConverter);
        builder.Property(b => b.EndUtc).IsRequired().HasConversion(UtcConverter);
        builder.Property(b => b.Attendees).IsRequired();
        builder.Property(b => b.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasOne<Room>()
            .WithMany()
            .HasForeignKey(b => b.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        // R7 looks up bookings by room; R9 looks up bookings by person and day.
        builder.HasIndex(b => new { b.RoomId, b.Status });
        builder.HasIndex(b => new { b.BookedBy, b.StartUtc });

        // Computed in the domain, not stored.
        builder.Ignore(b => b.IsActive);
        builder.Ignore(b => b.DayUtc);
    }
}
