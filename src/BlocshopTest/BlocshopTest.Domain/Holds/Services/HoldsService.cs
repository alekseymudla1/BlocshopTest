using BlocshopTest.Domain.Holds.Models;
using Microsoft.Extensions.Options;

namespace BlocshopTest.Domain.Holds.Services;

public class HoldsService : IHoldsService
{
    private readonly IHoldsCache _holdsCache;
    private readonly HoldingSettings _holdingSettings;
    public HoldsService(IHoldsCache holdsCache,
        IOptions<HoldingSettings> options)
    {
        _holdsCache = holdsCache;
        _holdingSettings = options.Value;
    }

    public async Task<Hold> CreateHold(CreateHold createHold)
    {
        var hold = new Hold
        {
            Id = Guid.NewGuid(),
            IdempotencyKey = createHold.IdempotencyKey,
            EventId = createHold.EventId,
            CustomerId = createHold.CustomerId,
            Seats = createHold.Seats,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(_holdingSettings.ExpirationInterval)
        };
        hold = await _holdsCache.SaveHold(hold);
        return hold;
    }

    public async Task<Hold> GetHoldByIdempotencyKey(CreateHold createHold)
    {
        var hold = await _holdsCache.GetHoldByIdempotencyKey(createHold.IdempotencyKey);
        if(hold is null)
        {
            return null;
        }

        if(hold.EventId == createHold.EventId
            && hold.CustomerId == createHold.CustomerId)
        {
            return hold;
        }

        return null;
    }

    public async Task DeleteHold(Guid holdId)
    {
        await _holdsCache.DeleteHold(holdId);
    }
}
