using Asp.Versioning;

namespace WebApi.Middleware;

public sealed class PreferV2OrHighestVersionSelector(ApiVersioningOptions options) : IApiVersionSelector {
	private readonly ApiVersioningOptions _options = options ?? throw new ArgumentNullException(nameof(options));
	private readonly ApiVersion           _v2      = new(2, 0);

	public ApiVersion SelectVersion(HttpRequest request, ApiVersionModel model) {

		if (!model.SupportedApiVersions.Any())
			return _options.DefaultApiVersion;

		if (model.DeclaredApiVersions.Contains(_v2))
			return _v2;

		if (model.DeclaredApiVersions.Any())
			return model.DeclaredApiVersions.Max();

		return _options.DefaultApiVersion;
		
	}
}