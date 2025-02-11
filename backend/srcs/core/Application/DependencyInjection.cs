using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection {
	public static IServiceCollection AddApplication(this IServiceCollection services) {

		services.AddMediatR(conf => {
			conf.RegisterServicesFromAssemblies(typeof(DependencyInjection).Assembly);
			conf.RegisterServicesFromAssemblies(typeof(AppUser).Assembly);
		});

		return services;
	}
	
}