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


    public sealed class UpdateProvider : AbstractValidator<CreateProviderRequest>
    {
        public UpdateProvider()
        {
            RuleFor(x => x.Username)
               .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
               .When(x => !string.IsNullOrWhiteSpace(x.Username));

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Şifre boş olamaz.")
               .When(x => !string.IsNullOrWhiteSpace(x.Password));

            RuleFor(x => x.ShippingProviderCode)
               .GreaterThan(0).WithMessage("Kargo sağlayıcı kodu geçerli olmalıdır.");
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