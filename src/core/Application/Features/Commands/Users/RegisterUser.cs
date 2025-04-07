using System.Net.Mail;
using System.Text.RegularExpressions;
using Application.Services;
using Domain.EmailPatterns;
using Domain.Entities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record RegisterUserRequest(
	string Name,
	string Surname,
	string Email,
	string Password,
	bool   IsAdmin) : IRequest<Result<string>>;


public sealed class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
           .MaximumLength(50).WithMessage("İsim en fazla 50 karakter olabilir.");

        RuleFor(x => x.Surname)
           .MaximumLength(50).WithMessage("Soyisim en fazla 50 karakter olabilir.");

        RuleFor(x => x.Email)
           .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.Password)
           .NotEmpty().WithMessage("Şifre boş olamaz.")
           .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
           .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir.")
           .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
           .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
           .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
           .Matches(@"[\W_]").WithMessage("Şifre en az bir özel karakter içermelidir.");
    }
}
internal sealed record RegisterUserHandler(
	IEncryptionService       encryptionService,
	IRepositoryService<User> userRepository,
	ICacheService            cacheService,
	IEmailService            emailService
) : IRequestHandler<RegisterUserRequest, Result<string>> {
	public async Task<Result<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken) {
		User? isEmailExist = await userRepository.FindOneAsync(x => x.Email == request.Email, cancellationToken);

		if (isEmailExist != null)
			return (409, "Bu e-posta adresi zaten kullanılmaktadır.");

		if (!IsValidEmail(request.Email))
			return (404, "Geçersiz e-posta adresi.");

		User user = new() {
							  Name     = request.Name,
							  Surname  = request.Surname,
							  Email    = request.Email,
							  Password = encryptionService.Encrypt(request.Password),
							  IsAdmin  = request.IsAdmin
						  };

		await userRepository.InsertOneAsync(user, cancellationToken);

		string otp = emailService.GenerateOtp(request.Email, TimeSpan.FromMinutes(5));

		VerificationMail verificationMail = new(otp);
		await emailService.SendEmailAsync(request.Email, verificationMail.Subject, verificationMail.Body, cancellationToken);

		return "Kullanıcı başarıyla kaydedildi.";
	}

	private bool IsValidEmail(string email) {
		try {
			var addr = new MailAddress(email);
			return Regex.IsMatch(addr.Address, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
		} catch {
			return false;
		}
	}
}