using Application.Services;
using Domain.Entities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record ResetPasswordRequest(
    string Email,
    string Otp,
    string Password) : IRequest<Result<string>>;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("E-posta boş olamaz")
           .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");

        RuleFor(x => x.Otp)
           .NotEmpty().WithMessage("OTP (One-Time Password) boş olamaz")
           .Matches(@"^\d{6}$").WithMessage("OTP 6 haneli bir sayısal değer olmalıdır");

    }
}


internal sealed record ResetPasswordHandler(
    IRepositoryService<User> userRepository,
    ICacheService cacheService,
    IEncryptionService encryptionService) : IRequestHandler<ResetPasswordRequest, Result<string>>
{
    public async Task<Result<string>> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.FindOneAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null)
            return (404, "Kullanıcı Bulunamadı");

        string? otp = cacheService.Get<string>("forgot_" + request.Email);

        if (otp == null)
            return (404, "Doğrulama kodu bulunamadı.");

        if (otp != request.Otp)
            return (400, "Girdiğiniz doğrulama kodu hatalı.");

        user.Password = encryptionService.Encrypt(request.Password);

        await userRepository.ReplaceOneAsync(x => x.Email == request.Email, user, cancellationToken);

        cacheService.Remove("forgot_" + request.Email);

        return "Şifreniz başarıyla değiştirildi.";
    }
}