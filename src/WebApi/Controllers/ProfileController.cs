using Application.Features.Profile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers;

public sealed class ProfileController(IMediator mediator) : ApiController(mediator)
{

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var response = await Mediator.Send(new GetProfileRequest());

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var response = await Mediator.Send(request);

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }

    [HttpDelete]
    public async Task<IActionResult> CloseOtherSessions(CloseOtherSessionsRequest request)
    {
        var response = await Mediator.Send(request);

        if (response.IsSuccessful)
            return Ok(response);

        return StatusCode(response.StatusCode, response);
    }
}