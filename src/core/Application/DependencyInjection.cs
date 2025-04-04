using Application.Behaviors;
using Application.Factories.Abstractions;
using Application.Factories.Interfaces;
using Application.Factories.Parameters;
using Application.Factories.Parameters.Requests;
using Application.Factories.Providers;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehaviour<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        services.AddHttpClient();

        services
           .AddScoped<AProvider<MNGRequest.Provider>,
                MNGProvider<MNGRequest.Provider>>();

        services
           .AddScoped<AProvider<YURTICIRequest.Provider>,
                YURTICIProvider<YURTICIRequest.Provider>>();

        return services;
    }

}