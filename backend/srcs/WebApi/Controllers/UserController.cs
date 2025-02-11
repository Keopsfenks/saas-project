using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers;

public sealed class UserController(IMediator mediator) : ApiController(mediator) {

	[HttpPost]
	public async Task<IActionResult> RegisterUser(RegisterUser request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}
}