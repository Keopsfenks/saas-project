using Application.Services;
using Domain.Dtos;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record RefreshTokenRequest(
	string UserId,
	string RefreshToken) : IRequest<Result<string>>;




internal sealed record RefreshTokenHandler(
	IRepositoryService<User> userRepository,
	IRepositoryService<Session> sessionRepository,
	IEncryptionService encryptionService,
	IRepositoryService<Workspace> workspaceRepository,
	IJwtProvider jwtProvider) : IRequestHandler<RefreshTokenRequest, Result<string>> {
	public async Task<Result<string>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken) {
		User? user = await userRepository.FindOneAsync(x => x.Id == request.UserId);

		if (user == null)
			return (404, "Kullanıcı bulunamadı.");

		IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => x.UserId == user.Id);

		Session? session = sessions.FirstOrDefault(x => x?.RefreshToken is not null && encryptionService.Decrypt(x.RefreshToken) == request.RefreshToken);

		IEnumerable<Workspace?> workspaces = await workspaceRepository.FindAsync(x => x.UserId == user.Id);




		if (session == null)
			return (404, "Oturum bulunamadı.");

		if (session.RefreshTokenExpiryTime < DateTime.UtcNow)
			return (404, "Oturumun süresi dolmuş.");

		var      enumerable = workspaces.ToList();
		TokenDto token      = await jwtProvider.GenerateJwtToken(user, enumerable, enumerable.First()!.Id);;

		session.RefreshTokenExpiryTime = token.RefreshTokenExpiryTime;
		session.ExpiryTime             = token.ExpiryTime;
		session.RefreshToken           = encryptionService.Encrypt(token.RefreshToken);
		session.Token                  = encryptionService.Encrypt(token.Token);

		await sessionRepository.ReplaceOneAsync(x => x.Id == session.Id, session);

		return "Başarılı bir şekilde oturum yenilendi.";
	}
}