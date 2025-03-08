using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record RefreshTokenRequest(
    string RefreshToken) : IRequest<Result<string>>;




internal sealed record RefreshTokenHandler(
    IRepositoryService<User> userRepository,
    IRepositoryService<Session> sessionRepository,
    IEncryptionService encryptionService,
    IRepositoryService<Workspace> workspaceRepository,
    IAuthorizeService AuthorizeService,
    IJwtProvider jwtProvider) : IRequestHandler<RefreshTokenRequest, Result<string>>
{
    public async Task<Result<string>> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        User? user = await AuthorizeService.FindUserAsync(cancellationToken);

        if (user == null)
            return (404, "Kullanıcı bulunamadı.");


        Session? session
            = await sessionRepository.FindOneAsync(x => x.RefreshToken ==
                                                        encryptionService.Encrypt(request.RefreshToken), cancellationToken);

        IEnumerable<Workspace?> workspaces = await workspaceRepository.FindAsync(x => x.UserId == user.Id, cancellationToken);

        if (session == null)
            return (404, "Oturum bulunamadı.");

        if (session.RefreshTokenExpiryTime < DateTime.UtcNow)
            return (404, "Oturumun süresi dolmuş.");

        var enumerable = workspaces.ToList();
        TokenDto token = await jwtProvider.GenerateJwtToken(user, enumerable, enumerable.First()!.Id);

        session.RefreshTokenExpiryTime = token.RefreshTokenExpiryTime;
        session.ExpiryTime = token.ExpiryTime;
        session.RefreshToken = encryptionService.Encrypt(token.RefreshToken);
        session.Token = encryptionService.Encrypt(token.Token);

        await sessionRepository.ReplaceOneAsync(x => x.Id == session.Id, session, cancellationToken);

        return "Başarılı bir şekilde oturum yenilendi.";
    }
}