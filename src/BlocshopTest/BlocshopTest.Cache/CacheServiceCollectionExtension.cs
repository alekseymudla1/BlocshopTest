using BlocshopTest.Cache.Cache;
using BlocshopTest.Domain.Holds.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlocshopTest.Cache;

public static class CacheServiceCollectionExtension
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add domain services here in the future
        services.AddTransient<IHoldsCache, HoldsCache>();
        return services;
    }
}
