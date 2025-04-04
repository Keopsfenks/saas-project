using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using TS.Result;

namespace WebApi.Middleware;

public sealed class ExceptionHandlingMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        Result<string> err;

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode  = 500;

        if (exception.GetType() == typeof(ValidationException))
        {
            httpContext.Response.StatusCode = 403;

            err = Result<string>.Failure(
                403, ((ValidationException)exception).Errors.Select(s => s.PropertyName).ToList());

            await httpContext.Response.WriteAsJsonAsync(err, cancellationToken: cancellationToken);

            return true;
        }

        err = Result<string>.Failure(exception.Message);

        await httpContext.Response.WriteAsJsonAsync(err, cancellationToken: cancellationToken);

        return true;
    }
}