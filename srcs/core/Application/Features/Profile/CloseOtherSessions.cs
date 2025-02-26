using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Profile;

public sealed record CloseOtherSessionsRequest(
	string Token) : IRequest<Result<string>>;



internal sealed record CloseOtherSessionsHandler(
	IRepositoryService<Session> sessionRepository,
	ITokenService               tokenService,
	IEncryptionService          encryptionService) : IRequestHandler<CloseOtherSessionsRequest, Result<string>> {
	public async Task<Result<string>> Handle(CloseOtherSessionsRequest request, CancellationToken cancellationToken) {

		User? user = await tokenService.FindUserAsync();

		if (user is null)
			return (404, "Kullanıcı bulunamadı.");

		if (string.IsNullOrWhiteSpace(request.Token))
			return (400, "Token boş olamaz.");

		IEnumerable<Session?> sessions
			= await sessionRepository.FindAsync(x => x.UserId == user.Id &&
													 x.Token != encryptionService.Encrypt(request.Token));

		foreach (Session? session in sessions)
			await sessionRepository.DeleteOneAsync(x => x.Id == session!.Id);

		return "Diğer oturumlar kapatıldı.";
	}
}