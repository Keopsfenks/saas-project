using Application.Dtos;
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

        public abstract Task<Result<T>> CheckConnectionAsync<T>(Provider?         provider,
                                                                CancellationToken cancellationToken = default)
            where T : class;
    }
}