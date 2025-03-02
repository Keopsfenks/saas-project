using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace WebApi;

internal sealed class BearerSecuritySchemeTransformer(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
	public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
	{
		var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
		if (authenticationSchemes.Any(authScheme => authScheme.Name == "Bearer"))
		{
			var requirements = new Dictionary<string, OpenApiSecurityScheme>
							   {
								   ["Bearer"] = new OpenApiSecurityScheme
												{
													BearerFormat = "JWT",
													Name         = "JWT Authentication",
													In           = ParameterLocation.Header,
													Type         = SecuritySchemeType.Http,
													Scheme       = JwtBearerDefaults.AuthenticationScheme,
													Description  = "Put **_ONLY_** yourt JWT Bearer token on textbox below!",

													Reference = new OpenApiReference {
																	Id   = JwtBearerDefaults.AuthenticationScheme,
																	Type = ReferenceType.SecurityScheme
																}
												}
							   };
			document.Components                 ??= new OpenApiComponents();
			document.Components.SecuritySchemes =   requirements;

			foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
			{
				operation.Value.Security.Add(new OpenApiSecurityRequirement
											 {
												 [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }] = Array.Empty<string>()
											 });
			}
		}
	}
}