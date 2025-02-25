using System.Net.Mail;
using System.Text.RegularExpressions;
using Application.Services;
using Domain.EmailPatterns;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record RegisterUserRequest(
	string name,
	string surname,
	string email,
	string password) : IRequest<Result<string>>;

internal sealed record RegisterUserHandler(
	IEncryptionService       encryptionService,
	IRepositoryService<User> userRepository,
	ICacheService            cacheService,
	IEmailService            emailService
) : IRequestHandler<RegisterUserRequest, Result<string>> {
	public async Task<Result<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken) {
		User? isEmailExist = await userRepository.FindOneAsync(x => x.Email == request.email);

		if (isEmailExist != null)
			return (409, "Bu e-posta adresi zaten kullanılmaktadır.");

		if (!IsValidEmail(request.email))
			return (404, "Geçersiz e-posta adresi.");

		User user = new() {
			Name     = request.name,
			Surname  = request.surname,
			Email    = request.email,
			CreateAt = DateTimeOffset.UtcNow,
			Password = encryptionService.Encrypt(request.password)
		};

		await userRepository.InsertOneAsync(user);

		string otp = emailService.GenerateOtp(request.email, TimeSpan.FromMinutes(5));

		VerificationMail verificationMail = new(otp);
		await emailService.SendEmailAsync(request.email, verificationMail.Subject, verificationMail.Body);

		return "Kullanıcı başarıyla kaydedildi.";
	}

	private bool IsValidEmail(string email) {
		try {
			var addr = new MailAddress(email);
			return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
		} catch {
			return false;
		}
	}
}