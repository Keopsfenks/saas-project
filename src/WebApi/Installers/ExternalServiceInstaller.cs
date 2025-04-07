using APIWeaver;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using System.IO.Compression;
using System.Threading.RateLimiting;
using WebApi.Abstractions;

namespace WebApi.Installers
{
    public static class ExternalServiceInstaller
    {
        public static IServiceCollection AddExternalServices(this IServiceCollection services)
        {
            services.AddOpenApi("v1", options => {
                options.
                    AddSecurityScheme("Bearer", scheme => {
                    scheme.In           = ParameterLocation.Header;
                    scheme.Type         = SecuritySchemeType.Http;
                    scheme.Scheme       = "Bearer";
                    scheme.BearerFormat = "JWT";
                    scheme.Description  = "JWT Authorization header using the Bearer scheme.";
                });
            });

            services
               .AddControllers()
               .AddOData(opt =>
                    {

                        opt.Select()
                           .Filter()
                           .Count()
                           .Expand()
                           .OrderBy()
                           .SetMaxTop(null);
                    }
                );

            // Rate Limiting Ayarları
            services.AddRateLimiter(options =>
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

            services.AddAuthentication(options =>
                     {
                         options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                         options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
                     })
                    .AddCookie(options =>
                     {
                         options.ExpireTimeSpan    = TimeSpan.FromMinutes(60);
                         options.SlidingExpiration = true;
                         options.LoginPath         = "/user/login";
                     }).AddJwtBearer();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("JwtAuth", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                });
            });

            services.AddResponseCompression(options => {
                options.EnableForHttps = true;
                options.Providers.Clear();
                options.Providers.Add<BrotliCompressionProvider>();
            });

            services.Configure<BrotliCompressionProviderOptions>(options => {
                options.Level = CompressionLevel.Fastest;
            });


            services.AddApiVersioning(options => {
                options.DefaultApiVersion                   = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.UnsupportedApiVersionStatusCode     = 404;
                options.ReportApiVersions                   = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version")
                );
            }).AddApiExplorer(options => {
                options.GroupNameFormat           = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}