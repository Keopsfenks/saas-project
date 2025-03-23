using Application.Dtos;
using Application.Features.Commands.Orders.v1;
using Application.Features.Commands.Providers.v1;
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

        Task<Result<string>> CreateConnectionAsync(Provider          provider,
                                                                 CancellationToken cancellationToken = default);

        public abstract Task<Result<ShipmentDto>> CreateOrderAsync(CreateOrderRequest request,
                                                                   CancellationToken  cancellationToken = default);

        public abstract Task<Result<ShipmentDto>> CancelOrderAsync(CancelOrderRequest request,
                                                              CancellationToken  cancellationToken = default);
    }
}