using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Profile;

public sealed record CloseOtherSessionsRequest(
	string Id,
	string Token) : IRequest<Result<string>>;



internal sealed record CloseOtherSessionsHandler(
	IRepositoryService<Session> sessionRepository,
	IEncryptionService encryptionService) : IRequestHandler<CloseOtherSessionsRequest, Result<string>> {
	public async Task<Result<string>> Handle(CloseOtherSessionsRequest request, CancellationToken cancellationToken) {
		IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => x.UserId == request.Id);

		foreach (Session? session in sessions) {
			if (encryptionService.Decrypt(session!.Token) != request.Token)
				await sessionRepository.DeleteOneAsync(x => x.Id == session.Id);
		}

		return "Diğer oturumlar kapatıldı.";
	}
}