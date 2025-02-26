using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Profile;

public sealed record GetProfileRequest() : IRequest<Result<ProfileDto>>;




internal sealed record GetProfileHandler(
	IRepositoryService<User>    userRepository,
	IRepositoryService<Session> sessionRepository,
	IEncryptionService          encryptionService,
	ITokenService               tokenService
) : IRequestHandler<GetProfileRequest, Result<ProfileDto>>
{
	public async Task<Result<ProfileDto>> Handle(GetProfileRequest request, CancellationToken cancellationToken) {
		User? user = await tokenService.FindUserAsync();

		if (user == null)
			return (404, "Kullanıcı bulunamadı");

		IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => x.UserId == user.Id);

		List<TokenDto> tokenDtos = sessions.Select(x => new TokenDto {
																		 Token = encryptionService.Decrypt(x.Token),
																		 RefreshToken
																			 = encryptionService
																				.Decrypt(x.RefreshToken),
																		 ExpiryTime = x.ExpiryTime,
																		 RefreshTokenExpiryTime
																			 = x.RefreshTokenExpiryTime,
																	 }).ToList();


		ProfileDto profile = new() {
									   Name    = user.Name,
									   Surname = user.Surname,
									   Session = tokenDtos,
								   };

		return profile;
	}
}
