using BlocshopTest.Domain.Events.Services;
using BlocshopTest.Domain.Holds.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlocshopTest.Domain;

public static class DomainServiceCollectionExtension
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add domain services here in the future
        services.AddTransient<IEventsService, EventsService>();
        services.AddTransient<IHoldsService, HoldsService>();
        return services;
    }
}
