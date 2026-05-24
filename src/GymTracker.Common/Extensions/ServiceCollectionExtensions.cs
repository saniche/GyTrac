using System.Reflection;
using GymTracker.Common.Dispatcher;
using GymTracker.Common.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace GymTracker.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher.Dispatcher>();
        return services;
    }

    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract);

        foreach (var type in types)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (!iface.IsGenericType) continue;

                var def = iface.GetGenericTypeDefinition();

                if (def == typeof(ICommandHandler<>) ||
                    def == typeof(ICommandHandler<,>) ||
                    def == typeof(IQueryHandler<,>))
                {
                    services.AddScoped(iface, type);
                }

                if (def == typeof(IValidator<>))
                {
                    services.AddScoped(iface, type);
                }
            }
        }

        return services;
    }
}
