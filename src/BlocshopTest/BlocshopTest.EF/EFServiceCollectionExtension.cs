using BlocshopTest.Domain.Events.Services;
using BlocshopTest.EF.Repositories.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BlocshopTest.EF;

public static class EFServiceCollectionExtension
{
    public static IServiceCollection AddEFServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IEventsRepository, EventsRepository>();
        return services;
    }
}
