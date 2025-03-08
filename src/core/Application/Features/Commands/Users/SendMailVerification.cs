using Application.Services;
using Domain.EmailPatterns;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record SendMailVerificationRequest(
	string Email) : IRequest<Result<string>>;


internal sealed record SendMailVerificationHandler(
	IEmailService            emailService,
	ICacheService            cacheService,
	IRepositoryService<User> userRepository) : IRequestHandler<SendMailVerificationRequest, Result<string>> {
	public async Task<Result<string>> Handle(SendMailVerificationRequest request, CancellationToken cancellationToken) {
		User? user = await userRepository.FindOneAsync(x => x.Email == request.Email, cancellationToken);

		if (user is null)
			return (404, "Girdiğiniz mail adresi ile kayıtlı bir kullanıcı bulunamadı.");

		string otp = emailService.GenerateOtp(request.Email, TimeSpan.FromMinutes(5));

		VerificationMail mail = new(otp);

		await emailService.SendEmailAsync(request.Email, mail.Subject, mail.Body, cancellationToken);

		return "Doğrulama maili gönderildi.";
	}
}