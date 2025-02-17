using System.Threading.RateLimiting;
using Application;
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

builder.Configuration
	   .SetBasePath(Directory.GetCurrentDirectory())
	   .AddJsonFile("appsettings.json",                optional: false, reloadOnChange: true)
	   .AddJsonFile($"appsettings.{environment}.json", optional: true,  reloadOnChange: true)
	   .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Rate Limiting Ayarları
builder.Services.AddRateLimiter(options =>
{
	options.AddFixedWindowLimiter("fixed", config =>
	{
		config.Window               = TimeSpan.FromSeconds(1);          // 1 saniyelik pencere
		config.PermitLimit          = 100;                              // 100 istek izin ver
		config.QueueLimit           = 50;                               // 50 isteğe kadar kuyruğa al
		config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst; // İlk gelen önce işlenir
	});
	options.RejectionStatusCode = 429;
});

var app = builder.Build();

Console.WriteLine($"Çalışma Ortamı: {environment}");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Rate Limiter Middleware'i ekle
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();