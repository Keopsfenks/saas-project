using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record MeReqguest : IRequest<Result<UserDto>>;



internal sealed class MeHandler(
	IAuthorizeService authorizeService) : IRequestHandler<MeRequest, Result<UserDto>> {
	public async Task<Result<UserDto>> Handle(MeRequest request, CancellationToken cancellationToken) {
		User? user = await authorizeService.FindUserAsync(cancellationToken);

		if (user is null)
			return (404, "Kullanıcı bulunamadı");


		UserDto userDto = new() {
									Id             = user.Id,
									Email          = user.Email,
									Name           = user.Name,
									Surname        = user.Surname,
									EmailConfirmed = user.EmailConfirmed,
								};

		return userDto;

	}
}