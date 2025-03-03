using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Profile;

public sealed record GetProfileRequest : IRequest<Result<ProfileDto>>;

internal sealed record GetProfileHandler(
	IRepositoryService<User>    userRepository,
	IRepositoryService<Session> sessionRepository,
	IEncryptionService          encryptionService,
	IAuthorizeService               AuthorizeService
) : IRequestHandler<GetProfileRequest, Result<ProfileDto>>
{
	public async Task<Result<ProfileDto>> Handle(GetProfileRequest request, CancellationToken cancellationToken) {

		User? user = await AuthorizeService.FindUserAsync();

		if (user == null)
			return (404, "Kullanıcı bulunamadı");

		IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => x.UserId == user.Id);

		List<TokenDto> tokenDtos = sessions
								  .OrderBy(cr => cr!.ExpiryTime)
								  .Select(x => new TokenDto {
																		 Token = encryptionService.Decrypt(x!.Token),
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
