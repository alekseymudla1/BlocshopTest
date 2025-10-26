using BlocshopTest.Domain.Events.Models;

namespace BlocshopTest.EF.DataSeed;

public static class EventsDataSeed
{
    public static async Task Seed(ApplicationDbContext context)
    {
        if(context == null) throw new ArgumentNullException(nameof(context));

        if (!context.Set<Event>().Any())
        {
            var events = new List<Event>
            {
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Scorpions ft Symphonic Orchestra",
                    Date = DateTimeOffset.UtcNow.AddHours(1).AddDays(5)
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Nirvana Tribute",
                    Date = DateTimeOffset.UtcNow.AddHours(2).AddDays(5)
                },
                new Event
                {
                    Id = Guid.NewGuid(),
                    Name = "Vivaldi. Four Seasons",
                    Date = DateTimeOffset.UtcNow.AddHours(2).AddDays(5)
                }
            };
            await context.Set<Event>().AddRangeAsync(events);
            await context.SaveChangesAsync();
        }
    }
}
