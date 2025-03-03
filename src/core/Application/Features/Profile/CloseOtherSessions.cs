using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Profile;

public sealed record CloseOtherSessionsRequest : IRequest<Result<string>>;



internal sealed record CloseOtherSessionsHandler(
	IRepositoryService<Session> sessionRepository,
	IAuthorizeService               AuthorizeService,
	IEncryptionService          encryptionService) : IRequestHandler<CloseOtherSessionsRequest, Result<string>> {
	public async Task<Result<string>> Handle(CloseOtherSessionsRequest request, CancellationToken cancellationToken) {
		User?   user  = await AuthorizeService.FindUserAsync(cancellationToken);
		string token = await AuthorizeService.GetTokenAsync(cancellationToken);


		if (user is null)
			return (404, "Kullanıcı bulunamadı.");

		if (string.IsNullOrWhiteSpace(token))
			return (400, "Token boş olamaz.");

		IEnumerable<Session?> sessions
			= await sessionRepository.FindAsync(x => x.UserId == user.Id &&
													 x.Token != encryptionService.Encrypt(token), cancellationToken: cancellationToken);

		foreach (Session? session in sessions)
			await sessionRepository.DeleteOneAsync(x => x.Id == session!.Id, cancellationToken);

		return "Diğer oturumlar kapatıldı.";
	}
}