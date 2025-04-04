    using Microsoft.AspNetCore.Diagnostics;
    using Serilog;

    namespace WebApi.Installers
    {
        public static class MiddlewareInstaller
        {
            public static void AddMiddlewares(this WebApplication app)
            {
                app.UseHttpsRedirection();

                app.UseCors(x => x
                                .AllowAnyHeader()
                                .AllowCredentials()
                                .AllowAnyMethod()
                                .SetIsOriginAllowed(_ => true));

                app.UseAuthentication();
                app.UseAuthorization();

                app.UseResponseCompression();
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(async context =>
                    {
                        context.Response.StatusCode  = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";

                        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                        var errorMessage = exceptionHandlerPathFeature?.Error.Message ?? "Beklenmeyen bir hata oluştu.";

                        Log.Error(exceptionHandlerPathFeature?.Error, "Hata oluştu: {ErrorMessage}", errorMessage);

                        var errorResponse = new { Error = errorMessage };
                        await context.Response.WriteAsJsonAsync(errorResponse);
                    });
                });

                app.UseRateLimiter();
            }
        }
    }