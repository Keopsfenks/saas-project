using Asp.Versioning;

namespace WebApi.Middleware;

public sealed class PreferV2OrHighestVersionSelector(ApiVersioningOptions options) : IApiVersionSelector
{
    private readonly ApiVersioningOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    public ApiVersion SelectVersion(HttpRequest request, ApiVersionModel model)
    {


        return new ApiVersion(2, 0);
    }
}