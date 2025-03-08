using Application.Features.Commands.Users;
using Application.Features.Queries;
using Application.Features.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers;

public sealed class UserController(IMediator mediator) : ApiController(mediator) {

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> RegisterUser(RegisterUserRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> LoginUser(LoginUserRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> AccountVerification(AccountVerificationRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	public async Task<IActionResult> RefreshToken(RefreshTokenRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> ResetPassword(ResetPasswordRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	public async Task<IActionResult> LogoutUser(LogoutUserRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> SendMailVerification(SendMailVerificationRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpDelete]
	public async Task<IActionResult> DeleteUser(DeleteUserRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> GetUsers([FromQuery] ResultUsersQuery request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}
	[HttpGet]
	public async Task<IActionResult> Me([FromQuery] MeRequest request) {
		var response = await Mediator.Send(request);

		if (response.IsSuccessful)
			return Ok(response);

		return StatusCode(response.StatusCode, response);
	}

}