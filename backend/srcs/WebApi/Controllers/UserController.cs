using Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Abstractions;

namespace WebApi.Controllers;

public sealed class UserController(IMediator mediator) : ApiController(mediator) {

	[HttpPost]
	public async Task<IActionResult> RegisterUser(RegisterUserRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> LoginUser(LoginUserRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> AccountVerification(AccountVerificationRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> RefreshToken(RefreshTokenRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> ResetPassword(ResetPasswordRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpPost]
	public async Task<IActionResult> LogoutUser(LogoutUserRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpDelete]
	public async Task<IActionResult> DeleteUser(DeleteUserRequest request) {
		var response = await mediator.Send(request);

		return Ok(response);
	}

	[HttpGet]
	public async Task<IActionResult> GetUsers() {
		var response = await mediator.Send(new ResultUsersQuery());

		return Ok(response);
	}
}