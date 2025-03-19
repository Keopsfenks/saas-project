using Application.Factories;
using Application.Factories.Interfaces;
using Domain.Enums;
using Domain.ValueObject;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record UpdateShipmentRequest(
        string     Id,
        int        ShippingProviderCode,
        Order?     Order,
        CargoList? Cargo,
        Member?    Recipient,
        Member?    Shipper) : IRequest<Result<object>>;


    public sealed class UpdateShipmentRequestValidator : AbstractValidator<UpdateShipmentRequest>
    {
        public UpdateShipmentRequestValidator()
        {

            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Gönderi, ID'si boş olamaz")
               .NotNull().WithMessage("Gönderi, ID'si boş olamaz");

            RuleFor(x => x.ShippingProviderCode)
               .GreaterThan(0).WithMessage("Nakliye sağlayıcı kodu sıfırdan büyük olmalıdır");

            RuleFor(x => x.Order)
               .NotNull().WithMessage("Sipariş bilgisi boş olamaz")
               .Must(order => !string.IsNullOrEmpty(order.ReferenceId)).WithMessage("Sipariş referans numarası boş olamaz")
               .Must(order => !string.IsNullOrEmpty(order.BillOfLandingId)).WithMessage("İrsaliye bilgisi boş olamaz")
               .When(x => x.Order != null);

            RuleFor(x => x.Cargo)
               .NotNull().WithMessage("Kargo bilgisi boş olamaz")
               .When(x => x.Cargo != null);

            RuleFor(x => x.Recipient)
               .NotNull().WithMessage("Alıcı bilgisi boş olamaz")
               .Must(recipient => !string.IsNullOrEmpty(recipient.Name)).WithMessage("Alıcı adı boş olamaz")
               .Must(recipient => !string.IsNullOrEmpty(recipient.Email)).WithMessage("Alıcı e-posta adresi boş olamaz")
               .When(x => x.Recipient != null);

            RuleFor(x => x.Shipper)
               .NotNull().When(x => x.ShippingProviderCode != 0).WithMessage("Gönderen bilgisi boş olamaz")
               .When(x => x.Shipper != null);
        }
    }


    internal sealed record UpdateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<UpdateShipmentRequest, Result<object>>
    {
        public async Task<Result<object>> Handle(UpdateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                          serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.UpdateShipmentAsync<object>(request, cancellationToken);
        }
    }
}