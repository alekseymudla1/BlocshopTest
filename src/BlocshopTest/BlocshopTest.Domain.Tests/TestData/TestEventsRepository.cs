using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Events.Services;
using Castle.DynamicProxy.Generators;
using System.Linq;
using System.Linq.Expressions;

namespace BlocshopTest.Domain.Tests.TestData;

internal class TestEventsRepository : IEventsRepository
{
    private readonly List<Event> _events = [
        new Event() {
            Id = new Guid("11111111-1111-1111-1111-111111111111"),
            Name = "test event 1",
            Date = new DateTimeOffset(2026, 1, 1, 20, 0, 0, new TimeSpan(0)),
            TotalSeats = 2000,
            CreatedAt = new DateTimeOffset(2022, 10, 1, 20, 0, 0, new TimeSpan(0)),
        },
        new Event() {
            Id = new Guid("22222222-2222-2222-2222-222222222222"),
            Name = "test event 2",
            Date = new DateTimeOffset(2026, 1, 3, 20, 0, 0, new TimeSpan(0)),
            TotalSeats = 1000,
            CreatedAt = new DateTimeOffset(2022, 10, 1, 20, 0, 0, new TimeSpan(0)),
        },
        new Event() {
            Id = new Guid("33333333-3333-3333-3333-333333333333"),
            Name = "test event 3",
            Date = new DateTimeOffset(2026, 1, 2, 20, 0, 0, new TimeSpan(0)),
            TotalSeats = 1500,
            CreatedAt = new DateTimeOffset(2022, 10, 1, 20, 0, 0, new TimeSpan(0)),
        },
        new Event() {
            Id = new Guid("44444444-4444-4444-4444-444444444444"),
            Name = "other event 1",
            Date = new DateTimeOffset(2026, 1, 2, 20, 0, 0, new TimeSpan(0)),
            TotalSeats = 1700,
            CreatedAt = new DateTimeOffset(2022, 10, 1, 20, 0, 0, new TimeSpan(0)),
        },
        new Event() {
            Id = new Guid("55555555-5555-5555-5555-555555555555"),
            Name = "other event 2",
            Date = new DateTimeOffset(2026, 1, 3, 20, 0, 0, new TimeSpan(0)),
            TotalSeats = 1800,
            CreatedAt = new DateTimeOffset(2022, 10, 1, 20, 0, 0, new TimeSpan(0)),
        },
        new Event() {
            Id = new Guid("66666666-6666-6666-6666-666666666666"),
            Name = "other event 3",
            Date = new DateTimeOffset(2026, 1, 1, 20, 0, 0, new TimeSpan(0)),
            TotalSeats = 900,
            CreatedAt = new DateTimeOffset(2022, 10, 1, 20, 0, 0, new TimeSpan(0)),
        }
    ];

    public Task AddAsync(Event entity)
    {
        return Task.Run(() => _events.Add(entity));
    }

    public Task DeleteAsync(Event entity)
    {
        return Task.Run(() => _events.Remove(entity));
    }

    public Task<IEnumerable<Event>> GetAllAsync()
    {
        return Task.FromResult(_events as IEnumerable<Event>);
    }

    public Task<Event> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_events.FirstOrDefault(e => e.Id == id));
    }

    public Task<IEnumerable<Event>> GetFilteredPageAsync(Expression<Func<Event, bool>> filterExpression, int page, int pageSize)
    {
        var filter = filterExpression.Compile();

        return Task.FromResult(_events.Where(filter).Skip((page - 1) * pageSize).Take(pageSize));
    }

    public Task UpdateAsync(Event entity)
    {
        return Task.Run(() => Update(entity));
    }

    private void Update(Event entity)
    {
        var existingEvent = _events.FirstOrDefault(e => e.Id == entity.Id);
        if(existingEvent == null)
        {
            return;
        }
        existingEvent.Name = entity.Name;
        existingEvent.TotalSeats = entity.TotalSeats;
        existingEvent.UpdatedAt = DateTimeOffset.UtcNow;
    }
}
