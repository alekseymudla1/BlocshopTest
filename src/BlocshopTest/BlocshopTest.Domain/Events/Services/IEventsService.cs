using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Shared.Models;

namespace BlocshopTest.Domain.Events.Services;

public interface IEventsService
{
    Task<Page<Event>> GetEventsPage(string search, int? page, int? pageSize);
    Task<Event> CreateEvent(Event eventEntity);
    Task<Event> GetEventById(Guid id);
    Task<Confirmation> ConfirmRegistration(Guid eventId, Guid holdId);
}
