using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shippers.v1
{
    public sealed record DeleteShipperRequest(
        string Id) : IRequest<Result<string>>;

    internal sealed record DeleteShipperHandler(
        IRepositoryService<Shipper> shipperRepository) : IRequestHandler<DeleteShipperRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteShipperRequest request, CancellationToken cancellationToken)
        {
            bool isShipperExist = await shipperRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isShipperExist)
                return (404, "Müşteri bulunamadı.");

            await shipperRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Müşteri başarıyla silindi.";
        }
    }
}