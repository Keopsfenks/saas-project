using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Users;

public sealed record DeleteUserRequest : IRequest<Result<string>>;


internal sealed record DeleteUserHandler(
    IRepositoryService<User> userRepository,
    IRepositoryService<Session> sessionRepository,
    IAuthorizeService AuthorizeService,
    ICacheService cacheService) : IRequestHandler<DeleteUserRequest, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteUserRequest request, CancellationToken cancellationToken)
    {
        User? user = await AuthorizeService.FindUserAsync(cancellationToken);

        if (user is null)
            return (404, "Kullanıcı bulunamadı.");

        IEnumerable<Session?> sessions = await sessionRepository.FindAsync(x => x.UserId == user.Id, cancellationToken);

        foreach (var session in sessions)
        {
            if (session is null)
                continue;
            await sessionRepository.DeleteOneAsync(x => x.Id == session.Id, cancellationToken);
        }

        user.IsDeleted = true;

        await userRepository.ReplaceOneAsync(x => x.Id == user.Id, user, cancellationToken);

        if (cacheService.Remove(user.Email))
            return "Kullanıcı başarıyla silindi ve bellek temizlendi.";

        return "Kullanıcı başarıyla silindi.";
    }
}