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
    public sealed record UpdateShipmentRequest(
        string     ShipmentId,
        int        ProviderEnum,
        Dispatch?  Dispatch,
        CargoList? Cargo) : IRequest<Result<ShipmentDto>>;

    public sealed class UpdateShipmentValidator : AbstractValidator<UpdateShipmentRequest>
    {
        public UpdateShipmentValidator()
        {
            RuleFor(x => x.ShipmentId)
               .NotEmpty().WithMessage("Gönderi ID'si boş olamaz.");

            When(x => x.Dispatch is not null, () => RuleFor(x => x.Dispatch!)
                    .SetValidator(new DispatchValidator()));

            When(x => x.Cargo is not null, () => RuleFor(x => x.Cargo!)
                    .SetValidator(new CargoListValidator()));
        }
    }


    internal sealed record UpdateShipmentHandler(
        IServiceProvider serviceProvider) : IRequestHandler<UpdateShipmentRequest, Result<ShipmentDto>>
    {
        public async Task<Result<ShipmentDto>> Handle(UpdateShipmentRequest request, CancellationToken cancellationToken)
        {
            ProviderFactory factory = new(request.ProviderEnum, serviceProvider);

            IProvider provider = factory.GetProvider();

            return await provider.UpdateShipmentAsync(request, cancellationToken);
        }
    }
}