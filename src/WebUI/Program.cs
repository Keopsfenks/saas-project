using Application;
using Infrastructure;
using Microsoft.Extensions.Options;
using Serilog;

namespace WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            // Serilog yapılandırması
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()  // Konsola log yaz
                .WriteTo.File("logs/webapi-.log", rollingInterval: RollingInterval.Day)  // Dosyaya log yaz, günlük dosyalar oluştur
                .CreateLogger();

            builder.Services.AddHttpContextAccessor();

            builder.Host.UseSerilog();

            builder.Configuration
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                   .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                   .AddEnvironmentVariables();

            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddWebUIServices(environment);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
