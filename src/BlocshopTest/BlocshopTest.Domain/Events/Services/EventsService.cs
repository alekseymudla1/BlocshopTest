using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Holds.Services;
using BlocshopTest.Domain.Reservations.Models;
using BlocshopTest.Domain.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace BlocshopTest.Domain.Events.Services;

public class EventsService : IEventsService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IHoldsCache _holdsCache;
    private readonly PageSettings _pageSettings;
    public EventsService(IEventsRepository eventsRepository,
        IHoldsCache holdsCache,
        IOptions<PageSettings> pageSettingsOptions) 
    {
        _eventsRepository = eventsRepository;
        _holdsCache = holdsCache;
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
        var holds = await _holdsCache.GetHoldsByEventId(eventEntity.Id);
        eventEntity.Holds = holds.ToList();
        return eventEntity;
    }

    public async Task<Event> CreateEvent(Event eventEntity)
    {
        await _eventsRepository.AddAsync(eventEntity);
        return eventEntity;
    }

    public async Task<Confirmation> ConfirmRegistration(Guid eventId, Guid holdId)
    {
        var eventEntity = await _eventsRepository.GetByIdAsync(eventId);
        if (eventEntity == null)
        {
            return new Confirmation { Result = ConfirmationResult.EventNotFound };
        }
        var hold = await _holdsCache.GetHoldById(holdId);
        if (hold == null)
        {
            return new Confirmation { Result = ConfirmationResult.HoldNotFound };
        }
        var reservation = new Reservation
        {
            EventId = eventId,
            HoldId = holdId,
            Seats = hold.Seats,
        };
        eventEntity.Reservations.Add(reservation);
        await _eventsRepository.UpdateAsync(eventEntity);
        await _holdsCache.DeleteHold(holdId);

        return new Confirmation { Result = ConfirmationResult.Success };
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
