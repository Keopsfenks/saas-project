using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record LogoutUserRequest(
	string UserId,
	string Token) : IRequest<Result<string>>;



internal sealed record LogoutUserHandler(
	IRepositoryService<User> userRepository,
	IRepositoryService<Session> sessionRepository) : IRequestHandler<LogoutUserRequest, Result<string>> {
	public async Task<Result<string>> Handle(LogoutUserRequest request, CancellationToken cancellationToken) {
		User? user = await userRepository.FindOneAsync(x => x.Id == request.UserId);

		if (user is null)
			return (500, "Kullanıcı bulunamadı.");

		Session? session = await sessionRepository.FindOneAsync(x => x.UserId == request.UserId && x.Token == request.Token);

		if (session is null)
			return (500, "Oturum bulunamadı.");

		await sessionRepository.DeleteOneAsync(x => x.UserId == request.UserId && x.Token == request.Token);

		return "Başarıyla çıkış yapıldı.";
	}
}