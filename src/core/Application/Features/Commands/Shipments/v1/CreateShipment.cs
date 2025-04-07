using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using Application.Validations;
using Domain.ValueObject;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record CreateShipmentRequest(
        int       ProviderEnum,
        int       Type,
        string?   Description,
        Dispatch  Dispatch,
        CargoList Cargo,
        Member    Recipient,
        Member    Shipper,
        string    ProviderId) : IRequest<Result<ShipmentDto>>;


    public sealed class CreateShipmentValidator : AbstractValidator<CreateShipmentRequest>
    {
        public CreateShipmentValidator()
        {
            RuleFor(x => x.ProviderEnum)
               .GreaterThan(0).WithMessage("Geçerli bir kargo sağlayıcı seçilmelidir.");

            RuleFor(x => x.Type)
               .GreaterThan(0).WithMessage("Gönderi tipi belirtilmelidir.");

            RuleFor(x => x.Description)
               .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olmalıdır.")
               .When(x => !string.IsNullOrWhiteSpace(x.Description));


            RuleFor(x => x.ProviderId)
               .NotEmpty().WithMessage("Sağlayıcı ID alanı boş olamaz.");

            RuleFor(x => x.Dispatch)
               .NotNull().WithMessage("Dispatch bilgileri zorunludur.")
               .SetValidator(new DispatchValidator());

            RuleFor(x => x.Cargo)
               .NotNull().WithMessage("Kargo bilgileri zorunludur.")
               .SetValidator(new CargoListValidator());

            RuleFor(x => x.Recipient)
               .NotNull().WithMessage("Alıcı bilgileri zorunludur.")
               .SetValidator(new MemberValidator());

            RuleFor(x => x.Shipper)
               .NotNull().WithMessage("Gönderici bilgileri zorunludur.")
               .SetValidator(new MemberValidator());
        }
    }

    internal sealed record CreateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<CreateShipmentRequest, Result<ShipmentDto>>
    {
        public async Task<Result<ShipmentDto>> Handle(CreateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(request.ProviderEnum, serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.CreateShipmentAsync(request, cancellationToken);
        }
    }
}