using Application.Behaviors;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection {
	public static IServiceCollection AddApplication(this IServiceCollection services) {


		services.AddMediatR(conf =>
		{
			conf.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
			conf.AddOpenBehavior(typeof(ValidationBehavior<,>));
		});

		return services;
	}
	
}