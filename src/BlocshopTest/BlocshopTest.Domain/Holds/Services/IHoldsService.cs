using BlocshopTest.Domain.Holds.Models;

namespace BlocshopTest.Domain.Holds.Services;

public interface IHoldsService
{
    Task<Hold> CreateHold(CreateHold createHold);
    Task<Hold> GetHoldByIdempotencyKey(CreateHold createHold);
    Task DeleteHold(Guid holdId);
}
