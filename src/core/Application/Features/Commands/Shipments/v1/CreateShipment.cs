using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using Domain.ValueObject;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record CreateShipmentRequest(
        int       ShippingProviderCode,
        Order     Order,
        int?      StatusCode,
        CargoList Cargo,
        Member    Recipient,
        Member?   Shipper,
        string    ProviderId) : IRequest<Result<object>>;


    public sealed class CreateShipmentRequestValidator : AbstractValidator<CreateShipmentRequest>
    {
        public CreateShipmentRequestValidator()
        {
            RuleFor(x => x.ShippingProviderCode)
               .GreaterThan(0).WithMessage("Nakliye sağlayıcı kodu sıfırdan büyük olmalıdır");

            RuleFor(x => x.Order)
               .NotNull().WithMessage("Sipariş bilgisi boş olamaz")
               .Must(order => !string.IsNullOrEmpty(order.ReferenceId)).WithMessage("Sipariş referans numarası boş olamaz")
               .Must(order => !string.IsNullOrEmpty(order.BillOfLandingId)).WithMessage("İrsaliye bilgisi boş olamaz");

            RuleFor(x => x.Cargo)
               .NotNull().WithMessage("Kargo bilgisi boş olamaz");

            RuleFor(x => x.Recipient)
               .NotNull().WithMessage("Alıcı bilgisi boş olamaz")
               .Must(recipient => !string.IsNullOrEmpty(recipient.Name)).WithMessage("Alıcı adı boş olamaz")
               .Must(recipient => !string.IsNullOrEmpty(recipient.Email)).WithMessage("Alıcı e-posta adresi boş olamaz");

            RuleFor(x => x.Shipper)
               .NotNull().When(x => x.ShippingProviderCode != 0).WithMessage("Gönderen bilgisi boş olamaz");

            RuleFor(x => x.ProviderId)
               .NotEmpty().WithMessage("Sağlayıcı ID'si boş olamaz");
        }
    }

    internal sealed record CreateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<CreateShipmentRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(CreateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                          serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.CreateShipmentAsync<object>(request, cancellationToken);

        }
    }
}