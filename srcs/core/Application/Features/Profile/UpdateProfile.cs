using Application.Features.Users;
using Application.Services;
using Domain.Dtos;
using Domain.EmailPatterns;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Profile;

public sealed record UpdateProfileRequest(
	string? Name,
	string? Surname,
	string? Email,
	string? Password,
	bool    SessionClose) : IRequest<Result<string>>;


internal sealed record UpdateProfileHandler(
	IRepositoryService<User>    userRepository,
	IRepositoryService<Session> sessionRepository,
	IEncryptionService          encryptionService,
	ITokenService				TokenService,
	IEmailService               emailService,
	ICacheService               cacheService,
	IMediator mediator): IRequestHandler<UpdateProfileRequest, Result<string>> {

	public async Task<Result<string>> Handle(UpdateProfileRequest request, CancellationToken cancellationToken) {

		User? user = await TokenService.FindUserAsync();

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

			await emailService.SendEmailAsync(user.Email, mail.Subject, mail.Body);

		}

		if (request.Password is not null)
			user.Password = encryptionService.Encrypt(request.Password);

		if (request.SessionClose) {
			IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => x.UserId == user.Id);

			foreach (Session? session in sessions)
				await sessionRepository.DeleteOneAsync(x => x.Id == session!.Id);

			cacheService.Remove(user.Email);
		}

		await userRepository.ReplaceOneAsync(x => x.Id == user.Id, user);

		return "Kullanıcı başarıyla güncellendi.";
	}

}