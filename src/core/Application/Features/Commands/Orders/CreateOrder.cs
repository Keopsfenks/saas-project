using Application.Dtos;
using Domain.ValueObject;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Orders
{

    public sealed class CreateOrderValidator : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.Order)
               .NotNull()
               .NotEmpty()
               .DependentRules(() => RuleFor(x => x.ShipmentId).Null());

            RuleFor(x => x.Cargo)
               .NotNull()
               .NotEmpty()
               .DependentRules(() => RuleFor(x => x.ShipmentId).Null());;

            RuleFor(x => x.Recipient)
               .NotNull()
               .NotEmpty()
               .DependentRules(() => RuleFor(x => x.ShipmentId).Null());

            RuleFor(x => x.Shipper)
               .NotNull()
               .NotEmpty()
               .DependentRules(() => RuleFor(x => x.ShipmentId).Null());
        }

    }

    public sealed record CreateOrderRequest(
        string?    ShipmentId,
        Order     Order,
        CargoList Cargo,
        Member    Recipient,
        Member    Shipper) : IRequest<Result<OrderDto>>;
}