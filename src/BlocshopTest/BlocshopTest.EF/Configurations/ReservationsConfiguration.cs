using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Reservations.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlocshopTest.EF.Configurations;

public class ReservationsConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.EventId).IsRequired();
        builder.Property(x => x.HoldId).IsRequired();
        builder.Property(x => x.Seats).IsRequired();

        builder
            .HasOne<Event>()
            .WithMany(x => x.Reservations)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
