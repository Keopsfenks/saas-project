using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;
using WebUI.Filters;
using WebUI.Helpers;
using WebUI.Resources;
using WebUI.Services;

namespace WebUI
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddWebUIServices(this IServiceCollection services, string environment)
        {

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                  .AddCookie(options =>
                  {
                      options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                      options.SlidingExpiration = true;
                      options.LoginPath = "/user/login";
                  });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CookieAuth", policy =>
                {
                    policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddLocalization(o => o.ResourcesPath = "");
            // services.AddHttpContextAccessor();

            var mvcBuilder = services.AddControllersWithViews(o =>
            {
                o.Filters.Add<ActionFilter>();
                o.ModelMetadataDetailsProviders.Add(new DescriptionProvider(services.BuildServiceProvider().GetService<IStringLocalizer<SharedResource>>()));
            })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("SharedResource", assemblyName.Name);
                    };
                });

            if (environment == "Development")
            {
                mvcBuilder.AddRazorRuntimeCompilation();
            }

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("tr-TR"),
                };
                options.DefaultRequestCulture = new RequestCulture("tr-TR");

                // You must explicitly state which cultures your application supports.
                // These are the cultures the app supports for formatting 
                // numbers, dates, etc.

                options.SupportedCultures = supportedCultures;

                // These are the cultures the app supports for UI strings, 
                // i.e. we have localized resources for.

                options.SupportedUICultures = supportedCultures;
                options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());

            });

            // Customise default API behaviour
            services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
