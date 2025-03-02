using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record LogoutUserRequest(string Token) : IRequest<Result<string>>;



internal sealed record LogoutUserHandler(
	IRepositoryService<User>    userRepository,
	IAuthorizeService               AuthorizeService,
	IRepositoryService<Session> sessionRepository) : IRequestHandler<LogoutUserRequest, Result<string>> {
	public async Task<Result<string>> Handle(LogoutUserRequest request, CancellationToken cancellationToken) {
		User? user = await AuthorizeService.FindUserAsync();

		if (user is null)
			return (404, "Kullanıcı bulunamadı.");

		Session? session = await sessionRepository.FindOneAsync(x => x.UserId == user.Id && x.Token == request.Token);

		if (session is null)
			return (404, "Oturum bulunamadı.");

		await sessionRepository.DeleteOneAsync(x => x.UserId == user.Id && x.Token == request.Token);

		return "Başarıyla çıkış yapıldı.";
	}
}