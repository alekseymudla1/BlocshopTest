using BlocshopTest.Domain.Holds.Models;
using BlocshopTest.Domain.Holds.Services;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;

namespace BlocshopTest.Cache.Cache;

public class HoldsCache : IHoldsCache
{
    private readonly IConnectionMultiplexer _redis;
    private readonly HoldingSettings _settigns;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public HoldsCache(IConnectionMultiplexer redis, IOptions<HoldingSettings> options)
    {
        _redis = redis;
        _settigns = options.Value;
    }

    public async Task<Hold> GetHoldById(Guid holdId)
    {
        var db = _redis.GetDatabase();

        var holdJson = await db.StringGetAsync(HoldKey(holdId));
        if (!holdJson.HasValue)
            return null;

        return JsonSerializer.Deserialize<Hold>(holdJson!, _jsonOptions);
    }

    public async Task<Hold> GetHoldByIdempotencyKey(string idempotencyKey)
    {
        var db = _redis.GetDatabase();
        var idValue = await db.StringGetAsync(IdempotencyIndexKey(idempotencyKey));
        if (!idValue.HasValue)
            return null;

        var holdJson = await db.StringGetAsync(HoldKey(Guid.Parse(idValue!)));
        if (!holdJson.HasValue)
            return null;

        return JsonSerializer.Deserialize<Hold>(holdJson!, _jsonOptions);
    }

    public async Task<IEnumerable<Hold>> GetHoldsByEventId(Guid eventId)
    {
        var db = _redis.GetDatabase();
        var ids = await db.SetMembersAsync(EventIndexKey(eventId));
        if (ids.Length == 0)
            return Enumerable.Empty<Hold>();

        var tasks = ids.Select(id => db.StringGetAsync(HoldKey(Guid.Parse(id!))));
        var results = await Task.WhenAll(tasks);

        var holds = results
            .Where(r => r.HasValue)
            .Select(r => JsonSerializer.Deserialize<Hold>(r!, _jsonOptions)!)
            .ToList();

        return holds;
    }

    public async Task<Hold> SaveHold(Hold model)
    {
        var json = JsonSerializer.Serialize(model, _jsonOptions);

        var ttl = TimeSpan.FromMinutes(_settigns.ExpirationInterval);

        var db = _redis.GetDatabase();
        await db.StringSetAsync(HoldKey(model.Id), json, ttl);
        await db.SetAddAsync(EventIndexKey(model.EventId), model.Id.ToString());
        await db.StringSetAsync(IdempotencyIndexKey(model.IdempotencyKey), model.Id.ToString(), ttl);

        return model;
    }

    public async Task DeleteHold(Guid id)
    {
        var key = HoldKey(id);
        var db = _redis.GetDatabase();

        var value = await db.StringGetAsync(key);
        if (value.HasValue)
        {
            var hold = JsonSerializer.Deserialize<Hold>(value!, _jsonOptions)!;

            await db.SetRemoveAsync(EventIndexKey(hold.EventId), hold.Id.ToString());
            await db.KeyDeleteAsync(IdempotencyIndexKey(hold.IdempotencyKey));
        }

        await db.KeyDeleteAsync(key);
    }

    private static string HoldKey(Guid id) => $"hold:{id}";
    private static string EventIndexKey(Guid eventId) => $"event:{eventId}:holds";
    private static string IdempotencyIndexKey(string idempotencyKey) => $"idemp:{idempotencyKey}:hold";


}
