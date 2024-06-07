using Microsoft.Extensions.DependencyInjection;

namespace BuberDinner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // register services here
        return services;
    }
}