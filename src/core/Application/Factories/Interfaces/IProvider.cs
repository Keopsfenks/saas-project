using Application.Dtos;
using Application.Features.Commands.Providers.v1;
using Application.Features.Commands.Shipments.v1;
using Domain.Entities.WorkspaceEntities;
using TS.Result;

namespace Application.Factories.Interfaces
{
    public interface IProvider
    {
        Task<Result<T>> CreateProviderAsync<T>(CreateProviderRequest request,
                                               CancellationToken     cancellationToken = default) where T : class;

        Task<Result<T>> UpdateProviderAsync<T>(UpdateProviderRequest request,
                                               CancellationToken     cancellationToken = default) where T : class;

        Task<Result<T>> DeleteProviderAsync<T>(DeleteProviderRequest request,
                                               CancellationToken     cancellationToken = default) where T : class;

        Task<Result<ShipmentDto>> CreateShipmentAsync(CreateShipmentRequest request,
                                                   CancellationToken  cancellationToken = default);

        Task<Result<ShipmentDto>> UpdateShipmentAsync(UpdateShipmentRequest request,
                                                      CancellationToken     cancellationToken = default);

        Task<Result<ShipmentDto>> CancelShipmentAsync(CancelShipmentRequest request,
                                                      CancellationToken     cancellationToken = default);

        Task<Result<string>> DeleteShipmentAsync(DeleteShipmentRequest request,
                                                 CancellationToken     cancellationToken = default);

        Task<Result<ShipmentDto>> ConfirmShipmentAsync(ConfirmShipmentRequest request,
                                                       CancellationToken      cancellationToken = default);
    }
}