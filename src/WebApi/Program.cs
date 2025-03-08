using System.IO.Compression;
using System.Threading.RateLimiting;
using APIWeaver;
using Application;
using Asp.Versioning;
using Infrastructure;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;
using WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

builder.Services.AddResponseCompression(options => {
	options.EnableForHttps = true;
	options.Providers.Clear();
	options.Providers.Add<BrotliCompressionProvider>();
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options => {
	options.Level = CompressionLevel.Fastest;
});


// Serilog yapılandırması
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Konsola log yaz
    .WriteTo.File("logs/webapi-.log", rollingInterval: RollingInterval.Day)  // Dosyaya log yaz, günlük dosyalar oluştur
    .CreateLogger();


builder.Services.AddHttpContextAccessor();

builder.Host.UseSerilog();

builder.Configuration
	   .SetBasePath(Directory.GetCurrentDirectory())
	   .AddJsonFile("appsettings.json",                optional: false, reloadOnChange: true)
	   .AddJsonFile($"appsettings.{environment}.json", optional: true,  reloadOnChange: true)
	   .AddEnvironmentVariables();

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiVersioning(options => {
	options.DefaultApiVersion                   = new ApiVersion(1, 0);
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.UnsupportedApiVersionStatusCode     = 404;
	options.ReportApiVersions                   = true;
	options.ApiVersionReader = ApiVersionReader.Combine(
		new UrlSegmentApiVersionReader(),
		new HeaderApiVersionReader("X-Api-Version")
	);
}).AddApiExplorer(options => {
	options.GroupNameFormat       = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});


builder.Services.AddControllers();
builder.Services.AddOpenApi("v1", options => {
	options.AddSecurityScheme("Bearer", scheme => {
		scheme.In   = ParameterLocation.Header;
		scheme.Type = SecuritySchemeType.Http;
		scheme.Scheme = "Bearer";
		scheme.BearerFormat = "JWT";
		scheme.Description = "JWT Authorization header using the Bearer scheme.";
	});
});

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

// Serilog ile loglama
app.Lifetime.ApplicationStarted.Register(() => Log.Information("Uygulama Başlatıldı"));
app.Lifetime.ApplicationStarted.Register(() => Log.Information("Çalışma Ortamı: {Environment}", environment));
app.Lifetime.ApplicationStopped.Register(() => Log.Information("Uygulama Durduruldu"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Rate Limiter Middleware'i ekle
app.UseRateLimiter();

// Serilog middleware'ini kullanarak logları otomatik yakalayın
app.UseSerilogRequestLogging();  // Bu satır ile gelen HTTP istekleri loglanacak

app.UseAuthorization();

app.UseMiddleware<ExceptionHandlingMiddleware>();


app.MapControllers();

app.Run();

Log.CloseAndFlush();
