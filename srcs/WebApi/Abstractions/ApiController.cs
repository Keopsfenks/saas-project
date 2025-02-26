using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Attributes;

namespace WebApi.Abstractions;

[Route("api/[controller]/[action]")]
[ApiController]
[JwtVerification]
[Authorize]
public abstract class ApiController(IMediator mediator) : ControllerBase {
	protected readonly IMediator Mediator = mediator;
}