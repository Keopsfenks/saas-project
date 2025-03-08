using Application.Services;
using Domain.Entities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record AccountVerificationRequest(
    string Email,
    string otp) : IRequest<Result<string>>;


internal sealed record AccountVerificationHandler(
    ICacheService cacheService,
    IRepositoryService<User> userRepository) : IRequestHandler<AccountVerificationRequest, Result<string>>
{
    public async Task<Result<string>> Handle(AccountVerificationRequest request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.FindOneAsync(x => x.Email == request.Email, cancellationToken);

        if (user == null)
            return (404, "Kullanıcı bulunamadı.");

        string? otp = cacheService.Get<string>(request.Email);

        if (otp == null)
            return (400, "Doğrulama kodu bulunamadı.");

        if (otp != request.otp)
            return (400, "Girdiğiniz doğrulama kodu hatalı.");

        if (user.EmailConfirmed)
            return (400, "Hesabınız zaten doğrulanmış.");

        user.EmailConfirmed = true;

        await userRepository.ReplaceOneAsync(x => x.Email == request.Email, user, cancellationToken);

        cacheService.Remove(request.Email);

        return "Hesabınız başarıyla doğrulandı.";
    }
}