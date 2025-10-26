using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Events.Services;

namespace BlocshopTest.EF.Repositories.Events;

public class EventsRepository : RepositoryBase<ApplicationDbContext, Guid, Event>
    , IEventsRepository
{
    public EventsRepository(ApplicationDbContext context) : base(context)
    {
    }
}
