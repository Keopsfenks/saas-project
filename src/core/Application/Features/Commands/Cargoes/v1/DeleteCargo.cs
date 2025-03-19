using Application.Services;
using Domain.Entities.WorkspaceEntities;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Cargoes.v1
{
    public sealed record DeleteCargoRequest(
        string Id) : IRequest<Result<string>>;


    public sealed class DeleteCargoValidator : AbstractValidator<DeleteCargoRequest>
    {
        public DeleteCargoValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Kargo idsi boş olamaz")
               .NotNull().WithMessage("Kargo idsi boş olamaz");
        }
    }


    internal sealed record DeleteCargoHandler(
        IRepositoryService<Cargo> cargoRepository) : IRequestHandler<DeleteCargoRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteCargoRequest request, CancellationToken cancellationToken)
        {
            bool isCargoExist = await cargoRepository.ExistsAsync(x => x.Id == request.Id);

            if (!isCargoExist)
                return (404, "Kargo şablonu bulunamadı.");

            await cargoRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Kargo şablonu başarıyla silindi.";
        }
    }
}