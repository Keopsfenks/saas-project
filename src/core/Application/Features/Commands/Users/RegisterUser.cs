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
	string name,
	string surname,
	string email,
	string password,
	bool   IsAdmin) : IRequest<Result<string>>;

public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.name)
           .NotEmpty().WithMessage("Ad boş olamaz")
           .MaximumLength(50).WithMessage("Ad en fazla 50 karakter uzunluğunda olmalıdır");

        RuleFor(x => x.surname)
           .NotEmpty().WithMessage("Soyad boş olamaz")
           .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter uzunluğunda olmalıdır");

        RuleFor(x => x.email)
           .NotEmpty().WithMessage("E-posta boş olamaz")
           .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

        RuleFor(x => x.password)
           .NotEmpty().WithMessage("Şifre boş olamaz")
           .MinimumLength(6).WithMessage("Şifre en az 6 karakter uzunluğunda olmalıdır")
           .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir")
           .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir")
           .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir")
           .Matches(@"[\W_]").WithMessage("Şifre en az bir özel karakter içermelidir"); // Özel karakterler, örneğin: !, @, #, $

        RuleFor(x => x.IsAdmin)
           .NotNull().WithMessage("Admin durumu belirlenmelidir");
    }
}



internal sealed record RegisterUserHandler(
	IEncryptionService       encryptionService,
	IRepositoryService<User> userRepository,
	ICacheService            cacheService,
	IEmailService            emailService
) : IRequestHandler<RegisterUserRequest, Result<string>> {
	public async Task<Result<string>> Handle(RegisterUserRequest request, CancellationToken cancellationToken) {
		User? isEmailExist = await userRepository.FindOneAsync(x => x.Email == request.email, cancellationToken);

		if (isEmailExist != null)
			return (409, "Bu e-posta adresi zaten kullanılmaktadır.");

		if (!IsValidEmail(request.email))
			return (404, "Geçersiz e-posta adresi.");

		User user = new() {
							  Name     = request.name,
							  Surname  = request.surname,
							  Email    = request.email,
							  Password = encryptionService.Encrypt(request.password),
							  IsAdmin  = request.IsAdmin
						  };

		await userRepository.InsertOneAsync(user, cancellationToken);

		string otp = emailService.GenerateOtp(request.email, TimeSpan.FromMinutes(5));

		VerificationMail verificationMail = new(otp);
		await emailService.SendEmailAsync(request.email, verificationMail.Subject, verificationMail.Body, cancellationToken);

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