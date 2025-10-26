using BlocshopTest.Domain.Events.Models;
using BlocshopTest.Domain.Shared.Services;

namespace BlocshopTest.Domain.Events.Services;

public interface IEventsRepository : IRepositiryBase<Guid, Event>
{
}
