using Application.Dtos;
using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Profile;

public sealed record GetProfileRequest : IRequest<Result<ProfileDto>>;

internal sealed record GetProfileHandler(
    IRepositoryService<User> userRepository,
    IRepositoryService<Session> sessionRepository,
    IEncryptionService encryptionService,
    IAuthorizeService AuthorizeService
) : IRequestHandler<GetProfileRequest, Result<ProfileDto>>
{
    public async Task<Result<ProfileDto>> Handle(GetProfileRequest request, CancellationToken cancellationToken)
    {

        User? user = await AuthorizeService.FindUserAsync(cancellationToken);

        if (user == null)
            return (404, "Kullanıcı bulunamadı");

        IEnumerable<Session?> sessions
            = await sessionRepository.FindAsync(x => x.UserId == user.Id, cancellationToken: cancellationToken);

        List<TokenDto> tokenDtos = sessions
                                  .OrderBy(cr => cr!.ExpiryTime)
                                  .Select(x => new TokenDto(encryptionService.Decrypt(x!.Token), encryptionService
                                                               .Decrypt(x.RefreshToken), x.RefreshTokenExpiryTime,
                                                            x.ExpiryTime)).ToList();


        ProfileDto profile = new(user, tokenDtos);

        return profile;
    }
}
