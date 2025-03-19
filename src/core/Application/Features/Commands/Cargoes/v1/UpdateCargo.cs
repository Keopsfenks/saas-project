using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Cargoes.v1
{
    public sealed record UpdateCargoRequest(
        string   Id,
        string?  Name,
        string?  Description,
        int?     MassUnit,
        int?     DistanceUnit,
        decimal? Height,
        decimal? Lenght,
        decimal? Width) : IRequest<Result<CargoDto>>;


    public sealed class UpdateCargoValidator : AbstractValidator<UpdateCargoRequest>
    {
        public UpdateCargoValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Kargo idsi boş olamaz")
               .NotNull().WithMessage("Kargo idsi boş olamaz");

            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Kargo ismi boş olamaz")
               .MinimumLength(3).WithMessage("Kargo ismi en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(30).WithMessage("Kargo ismi en fazla 30 karakter uzunluğunda olmalıdır")
               .When(x => x.Name is not null);

            RuleFor(x => x.Description)
               .MaximumLength(100).WithMessage("Kargo açıklaması en fazla 100 karakter uzunluğunda olmalıdır")
               .When(x => x.Description is not null);
        }
    }


    internal sealed record UpdateCargoHandler(
        IRepositoryService<Cargo> cargoRepository) : IRequestHandler<UpdateCargoRequest, Result<CargoDto>>
    {
        public async Task<Result<CargoDto>> Handle(UpdateCargoRequest request, CancellationToken cancellationToken)
        {
            Cargo? cargo = await cargoRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

            if (cargo is null)
                return (404, "Kargo şablonu bulunamadı.");


            if (request.Name is not null)
                cargo.Name = request.Name;
            if (request.Description is not null)
                cargo.Description = request.Description;
            if (request.Height is not null)
                cargo.Height = (decimal)request.Height;
            if (request.Lenght is not null)
                cargo.Length = (decimal)request.Lenght;
            if (request.Width is not null)
                cargo.Width = (decimal)request.Width;

            await cargoRepository.ReplaceOneAsync(x => x.Id == request.Id, cargo, cancellationToken);

            return new CargoDto(cargo);
        }
    }
}