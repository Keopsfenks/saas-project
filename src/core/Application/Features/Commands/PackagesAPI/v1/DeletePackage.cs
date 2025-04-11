using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Packages.v1
{
    public sealed record DeletePackageRequest(
        string Id) : IRequest<Result<string>>;



    internal sealed record DeletePackageHandler(
        IRepositoryService<Package> packageRepository) : IRequestHandler<DeletePackageRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeletePackageRequest request, CancellationToken cancellationToken)
        {

            bool isPackageExist = await packageRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isPackageExist)
                return (404, "Paket bulunamadı.");


            await packageRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Paket başarıyla silindi.";
        }
    }
}