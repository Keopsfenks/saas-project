using Application.Dtos;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record LoginUserRequest(
	string Email,
	string Password
	) : IRequest<Result<TokenDto>> {
}

public sealed class LoginUserValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email alanı boş olamaz.")
           .EmailAddress().WithMessage("Girdiğiniz email hatalı.");

        RuleFor(x => x.Password)
           .NotEmpty().WithMessage("Şifre alanı boş olamaz.");
    }
}

internal sealed record LoginUserHandler(
	IEncryptionService            encryptionService,
	IRepositoryService<User>      userRepository,
	IRepositoryService<Session>   sessionsRepository,
	IRepositoryService<Workspace> workspaceRepository,
	IHttpContextAccessor          httpContextAccessor,
	IJwtProvider                  jwtProvider) : IRequestHandler<LoginUserRequest, Result<TokenDto>> {
	public async Task<Result<TokenDto>> Handle(LoginUserRequest request, CancellationToken cancellationToken) {
		User? user = await userRepository.FindOneAsync(x => x.Email == request.Email, cancellationToken);

		if (user == null)
			return (404, "Kullanıcı bulunamadı.");

		if (encryptionService.Decrypt(user.Password) != request.Password)
			return (400, "Şifre hatalı.");

		if (user.EmailConfirmed == false)
			return (400, "Email adresinizi onaylayınız.");

        IEnumerable<Workspace?> workspaces
            = await workspaceRepository.FindAsync(x => x.UserId == user.Id, cancellationToken: cancellationToken);

		string? workspace  = null;
		var     enumerable = workspaces.ToList();

		if (enumerable.Any() && enumerable.First() is not null)
			workspace = enumerable.First()?.Id;

		TokenDto token = await jwtProvider.GenerateJwtToken(user, enumerable, workspace);

		Session session = new() {
									User                   = user,
									UserId                 = user.Id,
									Token                  = encryptionService.Encrypt(token.Token),
									ExpiryTime             = token.ExpiryTime,
									RefreshToken           = encryptionService.Encrypt(token.RefreshToken),
									RefreshTokenExpiryTime = token.RefreshTokenExpiryTime,
								};

		await sessionsRepository.InsertOneAsync(session, cancellationToken);

		return token;
	}
}
