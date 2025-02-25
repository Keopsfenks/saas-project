using Application.Features.Profile;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers.v1;

[ApiVersion(1)]
public sealed class ProfileController(IMediator mediator) : ApiController(mediator) {
	[HttpGet("{id}")]
	public async Task<IActionResult> GetProfile(string id) {
		var request  = new GetProfileRequest(id);
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPut]
	public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpDelete]
	public async Task<IActionResult> CloseOtherSessions(CloseOtherSessionsRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}
}