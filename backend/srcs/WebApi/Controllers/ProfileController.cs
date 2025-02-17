using Application.Features.Profile;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers;

public sealed class ProfileController(IMediator mediator) : ApiController(mediator) {
	[HttpGet("{id}")]
	public async Task<IActionResult> GetProfile(string id) {
		var request  = new GetProfileRequest(id);
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpPut]
	public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpDelete]
	public async Task<IActionResult> CloseOtherSessions(CloseOtherSessionsRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}
}