using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record LogoutUserRequest(
	string Token) : IRequest<Result<string>>;


public sealed class LogoutUserRequestValidator : AbstractValidator<LogoutUserRequest>
{
    public LogoutUserRequestValidator()
    {
        RuleFor(x => x.Token)
           .NotEmpty().WithMessage("Token boş olamaz")
           .NotNull().WithMessage("Token boş olamaz");
    }
}


internal sealed record LogoutUserHandler(
    IRepositoryService<User> userRepository,
    IAuthorizeService AuthorizeService,
    IRepositoryService<Session> sessionRepository) : IRequestHandler<LogoutUserRequest, Result<string>>
{
    public async Task<Result<string>> Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        User? user = await AuthorizeService.FindUserAsync(cancellationToken);

        if (user is null)
            return (404, "Kullanıcı bulunamadı.");

        Session? session
            = await sessionRepository.FindOneAsync(x => x.UserId == user.Id && x.Token == request.Token,
                                                   cancellationToken);

        if (session is null)
            return (404, "Oturum bulunamadı.");

        await sessionRepository.DeleteOneAsync(x => x.UserId == user.Id && x.Token == request.Token, cancellationToken);

        return "Başarıyla çıkış yapıldı.";
    }
}