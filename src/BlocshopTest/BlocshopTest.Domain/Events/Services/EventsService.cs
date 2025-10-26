using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace BlocshopTest.Domain.Events.Services;

public class EventsService : IEventsService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly PageSettings _pageSettings;
    public EventsService(IEventsRepository eventsRepository,
        IOptions<PageSettings> pageSettingsOptions) 
    {
        _eventsRepository = eventsRepository;
        _pageSettings = pageSettingsOptions.Value;
    }

    public async Task<IEnumerable<Event>> GetEventsPage(string search, int? page, int? pageSize)
    {
        var filter = BuildSearchExpression(search);
        return await _eventsRepository.GetFilteredPageAsync(filter, page ?? _pageSettings.DefaultPage, pageSize ?? _pageSettings.DefaultPageSize);
    }

    public async Task<Event> GetEventById(Guid id)
    {
        var eventEntity = await _eventsRepository.GetByIdAsync(id);
        return eventEntity;
    }

    public async Task<Event> CreateEvent(Event eventEntity)
    {
        await _eventsRepository.AddAsync(eventEntity);
        return eventEntity;
    }

    private Expression<Func<Event, bool>> BuildSearchExpression(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
        {
            return e => true;
        }
        return e => EF.Functions.Like(e.Name, $"%{search}%");
    }
}
