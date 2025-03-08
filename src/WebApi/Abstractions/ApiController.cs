using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Abstractions;

[Route("api/[controller]/[action]")]
[ApiController]
[Authorize(Policy = "JwtAuth")]
public abstract class ApiController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator Mediator = mediator;
}