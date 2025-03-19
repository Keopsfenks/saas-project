using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record UpdateProviderRequest(
        int     ShippingProviderCode,
        string  Id,
        string? Username,
        string? Password,
        object? Parameters) : IRequest<Result<object>>;


    public sealed class UpdateProviderRequestValidator : AbstractValidator<UpdateProviderRequest>
    {
        public UpdateProviderRequestValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Sağlayıcı idsi boş olamaz")
               .NotNull().WithMessage("sağlayıcı idsi boş olamaz");

            RuleFor(x => x.Username)
               .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
               .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter uzunluğunda olmalıdır")
               .When(x => !string.IsNullOrEmpty(x.Username));

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Şifre boş olamaz")
               .When(x => !string.IsNullOrEmpty(x.Password));

            RuleFor(x => x.ShippingProviderCode)
               .GreaterThan(0).WithMessage("Nakliye sağlayıcı kodu sıfırdan büyük olmalıdır")
               .When(x => x.ShippingProviderCode > 0);
        }
    }

    internal sealed record UpdateProviderHandler(
        IServiceProvider             serviceProvider) : IRequestHandler<UpdateProviderRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(UpdateProviderRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory providerFactory
                = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode), serviceProvider);

            IProvider providerService = providerFactory.GetProvider();

            return await providerService.UpdateProviderAsync<object>(request, cancellationToken);
        }
    }
}