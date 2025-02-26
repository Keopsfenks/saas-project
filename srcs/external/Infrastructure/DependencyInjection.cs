using System.Security.Claims;
using Application.Services;
using Infrastructure.Services;
using Infrastructure.Settings.DatabaseSetting;
using Infrastructure.Settings.EmailSettings;
using Infrastructure.Settings.SecuritySettings;
using Infrastructure.Variables;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection {
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {

		services.AddScoped<IJwtProvider, JwtProvider>();
		services.AddScoped<IEmailService, EmailService>();
		services.AddScoped<ITokenService, TokenService>();
		services.AddSingleton<IEncryptionService, EncryptionService>();

		services.AddHostedService<SessionsManagementService>();

		services.ConfigureOptions<JwtTokenOptionsSetup>();
		services.AddAuthentication(options => {
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
		}).AddJwtBearer();

		services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
		services.Configure<SecuritySettings>(configuration.GetSection("SecuritySettings"));
		services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

		services.AddSingleton<IEmailSettings>(x => x.GetRequiredService<IOptions<EmailSettings>>().Value);
		services.AddSingleton<ISecuritySettings>(x => x.GetRequiredService<IOptions<SecuritySettings>>().Value);
		services.AddSingleton<IDatabaseSettings>(x => x.GetRequiredService<IOptions<DatabaseSettings>>().Value);


		services.AddSingleton<IMongoClient>(serviceProvider => {
			var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
			return new MongoClient(databaseSettings.ConnectionString);
		});

		services.AddScoped<IMongoDatabase>(serviceProvider => {
			var client           = serviceProvider.GetRequiredService<IMongoClient>();
			var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
			return client.GetDatabase(databaseSettings.DatabaseName);
		});

		services.AddHttpContextAccessor();

		services.AddScoped(typeof(IRepositoryService<>), typeof(RepositoryService<>));

		IEmailSettings? settings = configuration.GetSection("EmailSettings").Get<EmailSettings>();

		services.AddFluentEmail(settings?.Email)
				.AddSmtpSender(settings!.MailHost, settings.MailPort);

		services.AddSingleton<IConnectionMultiplexer>(provider => {
			var securitySettings = provider.GetRequiredService<ISecuritySettings>();
			return ConnectionMultiplexer.Connect(securitySettings.RedisConnectionString);
		});

		services.AddMemoryCache();
		services.AddScoped<ICacheService, MemoryCacheService>();
		services.AddScoped<ICacheService, RedisCacheService>();

		return services;
	}
}