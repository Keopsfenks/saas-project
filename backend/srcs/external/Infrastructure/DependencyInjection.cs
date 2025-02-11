using Application.Services;
using Infrastructure.Services;
using Infrastructure.Settings.DatabaseSetting;
using Infrastructure.Settings.SecuritySettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection {
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
	#region ServiceRegistration
		services.AddScoped<IUserService, UserService>();
		services.AddSingleton<IEncryptionService, EncryptionService>();
	#endregion

		services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
		services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));

		services.AddSingleton<ISecuritySettings>(x => x.GetRequiredService<IOptions<SecuritySettings>>().Value);
		services.AddSingleton<IDatabaseSettings>(x => x.GetRequiredService<IOptions<DatabaseSettings>>().Value);

		return services;
	}
}