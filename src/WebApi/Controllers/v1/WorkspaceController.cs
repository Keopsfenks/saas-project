using Application.Features.Commands.Workspaces.v1;
using Application.Features.Queries;
using Application.Features.Queries.Workspaces;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers.v1;

[ApiVersion(1)]
public sealed class WorkspaceController(IMediator mediator) : ApiController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> CreateWorkspace([FromBody] CreateWorkspaceRequest request)
    {
        var response = await Mediator.Send(request);

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeWorkspace([FromBody] ChangeWorkspaceRequest request)
    {
        var response = await Mediator.Send(request);

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateWorkspace([FromBody] UpdateWorkSpaceRequest request)
    {
        var response = await Mediator.Send(request);

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteWorkspace([FromBody] DeleteWorkspaceRequest request)
    {
        var response = await Mediator.Send(request);

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }
    [HttpGet]
    public async Task<IActionResult> GetWorkspaces()
    {
        var response = await Mediator.Send(new ResultWorkspaceQuery());

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }
}