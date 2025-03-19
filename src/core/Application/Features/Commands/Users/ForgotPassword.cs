using Application.Services;
using Domain.EmailPatterns;
using Domain.Entities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Users;

public sealed record ForgotPasswordRequest(
    string Email) : IRequest<Result<string>>;


public sealed class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("E-posta boş olamaz")
           .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz");
    }
}

internal sealed record ForgotPasswordHandler(
    IEmailService emailService,
    ICacheService cacheService,
    IRepositoryService<User> userRepository) : IRequestHandler<ForgotPasswordRequest, Result<string>>
{
    public async Task<Result<string>> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.FindOneAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null)
            return (404, "Girdiğiniz mail adresi ile kayıtlı bir kullanıcı bulunamadı.");

        string otp = emailService.GenerateOtp("forgot_" + request.Email, TimeSpan.FromMinutes(5));

        ForgotPasswordMail mail = new(otp);

        await emailService.SendEmailAsync(request.Email, mail.Subject, mail.Body, cancellationToken);

        return "Şifre sıfırlama maili gönderildi.";

    }
}