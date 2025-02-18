using Application.Services;
using Domain.EmailPatterns;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record ForgotPasswordRequest(
	string Email) : IRequest<Result<string>>;




internal sealed record ForgotPasswordHandler(
	IEmailService emailService,
	ICacheService cacheService,
	IRepositoryService<User> userRepository) : IRequestHandler<ForgotPasswordRequest, Result<string>> {
	public async Task<Result<string>> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken) {
		User? user = await userRepository.FindOneAsync(x => x.Email == request.Email);

		if (user is null)
			return (500, "Girdiğiniz mail adresi ile kayıtlı bir kullanıcı bulunamadı.");

		string otp = emailService.GenerateOtp();

		cacheService.Set(request.Email, otp, TimeSpan.FromMinutes(5));

		ForgotPasswordMail mail = new(otp);

		await emailService.SendEmailAsync(request.Email, mail.Subject, mail.Body);

		return "Şifre sıfırlama maili gönderildi.";

	}
}