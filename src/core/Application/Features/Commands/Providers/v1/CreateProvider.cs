using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record CreateProviderRequest(
        string  Username,
        string  Password,
        int     ShippingProviderCode,

        object? Parameters) : IRequest<Result<object>>;

    public sealed class CreateProviderRequestValidator : AbstractValidator<CreateProviderRequest>
    {
        public CreateProviderRequestValidator()
        {
            RuleFor(x => x.Username)
               .NotEmpty().WithMessage("Kullanıcı adı boş olamaz")
               .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter uzunluğunda olmalıdır");

            RuleFor(x => x.Password)
               .NotEmpty().WithMessage("Şifre boş olamaz");

            RuleFor(x => x.ShippingProviderCode)
               .GreaterThan(0).WithMessage("Nakliye sağlayıcı kodu sıfırdan büyük olmalıdır");
        }
    }


    internal sealed record CreateProviderHandler(
        IServiceProvider             serviceProvider) : IRequestHandler<CreateProviderRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(CreateProviderRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory providerFactory
                = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode), serviceProvider);

            IProvider provider = providerFactory.GetProvider();

            return await provider.CreateProviderAsync<object>(request, cancellationToken);

        }
    }
}