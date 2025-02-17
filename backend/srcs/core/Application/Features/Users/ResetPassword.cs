using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record ResetPasswordRequest(
	string Email,
	string Otp,
	string Password) : IRequest<Result<string>>;



internal sealed record ResetPasswordHandler(
	IRepositoryService<User> userRepository,
	ICacheService cacheService,
	IEncryptionService encryptionService) : IRequestHandler<ResetPasswordRequest, Result<string>> {
	public async Task<Result<string>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken) {
		User? user = await userRepository.FindOneAsync(x => x.Email == request.Email);

		if (user is null)
			return (500, "Kullanıcı Bulunamadı");

		string? otp = cacheService.Get<string>(request.Email);

		if (otp == null)
			return (500, "Doğrulama kodu bulunamadı.");

		if (otp != request.Otp)
			return (500, "Girdiğiniz doğrulama kodu hatalı.");

		user.Password = encryptionService.Encrypt(request.Password);

		await userRepository.ReplaceOneAsync(x => x.Email == request.Email, user);

		cacheService.Remove(request.Email);

		return "Şifreniz başarıyla değiştirildi.";
	}
}