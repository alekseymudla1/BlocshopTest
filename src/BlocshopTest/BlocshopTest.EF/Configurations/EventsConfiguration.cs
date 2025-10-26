using BlocshopTest.Domain.Events.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlocshopTest.EF.Configurations;

public class EventsConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.TotalSeats).IsRequired();
        builder.Property(e => e.Date).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Ignore(e => e.AvailableSeats);
    }
}
