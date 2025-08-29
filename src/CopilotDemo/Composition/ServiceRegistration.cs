using CopilotDemo.Commands;
using CopilotDemo.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CopilotDemo.Composition;

public static class ServiceRegistration
{
    public static IServiceCollection AddGameServices(this IServiceCollection services)
    {
        return services
            .AddTransient<INumberGuessingService, NumberGuessingService>()
            .AddTransient<NumberGuessingCommand>();
    }
}