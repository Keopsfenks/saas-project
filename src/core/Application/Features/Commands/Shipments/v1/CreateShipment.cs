using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.ValueObject;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Shipments.v1
{
    public sealed record CreateShipmentRequest(
        Dictionary<string, object> Order,
        CargoList                  Cargo,
        Member                     Recipient,
        Member?                    Shipper,
        string                     ProviderId) : IRequest<Result<ShipmentDto>>;


    internal sealed record CreateShipmentHandler(
        IRepositoryService<Shipment> shipmentRepository,
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encryptionService,
        IEmailService                emailService) : IRequestHandler<CreateShipmentRequest, Result<ShipmentDto>>
    {
        public Task<Result<ShipmentDto>> Handle(CreateShipmentRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}