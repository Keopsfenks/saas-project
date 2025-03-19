using Application.Dtos;
using Application.Services;
using Domain.EmailPatterns;
using Domain.Entities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Profile;

public sealed record UpdateProfileRequest(
	bool    SessionClose,
	string? Name     = null,
	string? Surname  = null,
	string? Email    = null,
	string? Password = null) : IRequest<Result<UserDto>>;


public sealed class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Name)
           .MaximumLength(50).WithMessage("Ad en fazla 50 karakter uzunluğunda olmalıdır")
           .When(x => !string.IsNullOrEmpty(x.Name));

        RuleFor(x => x.Surname)
           .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter uzunluğunda olmalıdır")
           .When(x => !string.IsNullOrEmpty(x.Surname));

        RuleFor(x => x.Email)
           .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz")
           .When(x => !string.IsNullOrEmpty(x.Email));

        RuleFor(x => x.Password)
           .MinimumLength(6).WithMessage("Şifre en az 6 karakter uzunluğunda olmalıdır")
           .When(x => !string.IsNullOrEmpty(x.Password));

        RuleFor(x => x.SessionClose)
           .NotNull().WithMessage("Oturum kapama durumu belirtilmelidir");
    }
}

internal sealed record UpdateProfileHandler(
	IRepositoryService<User>    userRepository,
	IRepositoryService<Session> sessionRepository,
	IEncryptionService          encryptionService,
	IAuthorizeService				AuthorizeService,
	IEmailService               emailService,
	ICacheService               cacheService,
	IMediator mediator): IRequestHandler<UpdateProfileRequest, Result<UserDto>> {

	public async Task<Result<UserDto>> Handle(UpdateProfileRequest request, CancellationToken cancellationToken) {

		User? user = await AuthorizeService.FindUserAsync(cancellationToken);

		if (user is null)
			return (404, "Kullanıcı bulunamadı.");

		if (request.Name is not null)
			user.Name = request.Name;

		if (request.Surname is not null)
			user.Surname = request.Surname;

		if (request.Email is not null) {
			user.Email          = request.Email;
			user.EmailConfirmed = false;


			string           otp  = emailService.GenerateOtp(user.Email, TimeSpan.FromMinutes(5));
			VerificationMail mail = new VerificationMail(otp);

			await emailService.SendEmailAsync(user.Email, mail.Subject, mail.Body, cancellationToken);

		}

		if (request.Password is not null)
			user.Password = encryptionService.Encrypt(request.Password);

		if (request.SessionClose)
        {
            IEnumerable<Session?> sessions
                = await sessionRepository.FindAsync(x => x.UserId == user.Id, cancellationToken: cancellationToken);

			foreach (Session? session in sessions)
				await sessionRepository.DeleteOneAsync(x => x.Id == session!.Id, cancellationToken);

			cacheService.Remove(user.Email);
		}

		await userRepository.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken);

        return new UserDto(user);
    }

}