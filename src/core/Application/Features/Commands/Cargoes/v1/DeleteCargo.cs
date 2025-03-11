using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Cargoes.v1
{
    public sealed record DeleteCargoRequest(
        string Id) : IRequest<Result<string>>;


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