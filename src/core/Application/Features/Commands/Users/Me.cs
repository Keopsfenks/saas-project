using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record MeRequest : IRequest<Result<UserDto>>;

internal sealed class MeHandler(
	IAuthorizeService authorizeService) : IRequestHandler<MeRequest, Result<UserDto>> {
	public async Task<Result<UserDto>> Handle(MeRequest request, CancellationToken cancellationToken) {
		User? user = await authorizeService.FindUserAsync(cancellationToken);

		if (user is null)
			return (404, "Kullanıcı bulunamadı");


		UserDto userDto = new(user);

		return userDto;

	}
}