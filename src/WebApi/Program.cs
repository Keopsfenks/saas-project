using Scalar.AspNetCore;
using Serilog;
using ServiceDefaults;
using WebApi.Installers;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()  // Konsola log yaz
            .WriteTo.File("logs/webapi-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

builder.Host.UseSerilog();

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();
builder.Services.AddInternalServices(builder.Configuration);
builder.Services.AddExternalServices();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

builder.Configuration
       .SetBasePath(Directory.GetCurrentDirectory())
       .AddJsonFile("appsettings.json",                optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{environment}.json", optional: true,  reloadOnChange: true)
       .AddEnvironmentVariables();

var app = builder.Build();

// Serilog ile loglama
app.Lifetime.ApplicationStarted.Register(() => Log.Information("Uygulama Başlatıldı"));
app.Lifetime.ApplicationStarted.Register(() => Log.Information("Çalışma Ortamı: {Environment}", environment));
app.Lifetime.ApplicationStopped.Register(() => Log.Information("Uygulama Durduruldu"));

// Middlewareler
app.UseSerilogRequestLogging();
app.UseExceptionHandler("/error");

app.MapOpenApi();
app.MapScalarApiReference();
app.MapDefaultEndpoints();
app.AddMiddlewares();

app.MapControllers().RequireRateLimiting("fixed").RequireAuthorization();

app.Run();

Log.CloseAndFlush();