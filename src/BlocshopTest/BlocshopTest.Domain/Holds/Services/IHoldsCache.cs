using BlocshopTest.Domain.Holds.Models;

namespace BlocshopTest.Domain.Holds.Services;

public interface IHoldsCache
{
    Task<Hold> GetHoldById(Guid holdId);
    Task<IEnumerable<Hold>> GetHoldsByEventId(Guid eventId);
    Task<Hold> GetHoldByIdempotencyKey(string idempotencyKey);
    Task<Hold> SaveHold(Hold model);
    Task DeleteHold(Guid id);
}
