using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record LoginUserRequest(
	string email,
	string password
	) : IRequest<Result<TokenDto>> {
}



internal sealed record LoginUserHandler(
	IEncryptionService          encryptionService,
	IRepositoryService<User>    userRepository,
	IRepositoryService<Session> sessionsRepository,
	IRepositoryService<Workspace> workspaceRepository,
	IJwtProvider                jwtProvider) : IRequestHandler<LoginUserRequest, Result<TokenDto>> {
	public async Task<Result<TokenDto>> Handle(LoginUserRequest request, CancellationToken cancellationToken) {
		User? user = await userRepository.FindOneAsync(x => x.Email == request.email);

		if (user == null)
			return (500, "Kullanıcı bulunamadı.");

		if (encryptionService.Decrypt(user.Password) != request.password) {
			return (500, "Şifre hatalı.");
		}

		IEnumerable<Workspace?> workspaces = await workspaceRepository.FindAsync(x => x.UserId == user.Id);

		string? workspace  = null;
		var     enumerable = workspaces.ToList();

		if (enumerable.Any() && enumerable.First() is not null)
			workspace = enumerable.First()?.Id;

		TokenDto token = await jwtProvider.GenerateJwtToken(user, enumerable, workspace);

		Session session = new() {
														Token                  = encryptionService.Encrypt(token.Token),
														ExpiryTime             = token.ExpiryTime,
														RefreshToken           = encryptionService.Encrypt(token.RefreshToken),
															RefreshTokenExpiryTime = token.RefreshTokenExpiryTime,
														User                   = user,
														UserId                 = user.Id,
													};

		await sessionsRepository.InsertOneAsync(session);


		return token;

	}
}
