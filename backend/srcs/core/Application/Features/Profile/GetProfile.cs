using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Profile;

public sealed record GetProfileRequest(
	string Id) : IRequest<Result<List<ProfileDto>>>;




internal sealed record GetProfileHandler(
	IRepositoryService<User>    userRepository,
	IRepositoryService<Session> sessionRepository,
	IEncryptionService          encryptionService
) : IRequestHandler<GetProfileRequest, Result<List<ProfileDto>>>
{
	public async Task<Result<List<ProfileDto>>> Handle(GetProfileRequest request, CancellationToken cancellationToken) {
		IEnumerable<User?> users = await userRepository.FindAsync(x => x.Id == request.Id);

		IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => x.UserId == request.Id);

		List<TokenDto> tokenDtos = sessions.Select(x => new TokenDto {
																		 Token = encryptionService.Decrypt(x.Token),
																		 RefreshToken
																			 = encryptionService
																				.Decrypt(x.RefreshToken),
																		 ExpiryTime = x.ExpiryTime,
																		 RefreshTokenExpiryTime
																			 = x.RefreshTokenExpiryTime,
																	 }).ToList();


		List<ProfileDto> profiles = users.Select(x => new ProfileDto {
																	Name    = x.Name,
																	Surname = x.Surname,
																	Token   = encryptionService.Decrypt(x.Password),
																	Session = tokenDtos,
																}).ToList();

		return profiles;
	}
}
