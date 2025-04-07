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


public sealed class ResetPasswordValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Email alanı boş olamaz.")
           .EmailAddress().WithMessage("Girdiğiniz email hatalı.");

        RuleFor(x => x.Otp)
           .NotEmpty().WithMessage("Tek kullanımlık şifre boş olamaz");

        RuleFor(x => x.Password)
           .NotEmpty().WithMessage("Şifre boş olamaz.")
           .MinimumLength(6).WithMessage("Şifre en az 6 karakter olmalıdır.")
           .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter olabilir.")
           .Matches(@"[A-Z]").WithMessage("Şifre en az bir büyük harf içermelidir.")
           .Matches(@"[a-z]").WithMessage("Şifre en az bir küçük harf içermelidir.")
           .Matches(@"[0-9]").WithMessage("Şifre en az bir rakam içermelidir.")
           .Matches(@"[\W_]").WithMessage("Şifre en az bir özel karakter içermelidir.");
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