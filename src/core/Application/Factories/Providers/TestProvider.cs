using Application.Factories.Abstractions;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class TestProvider<TProvider, TShipment>(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService encryptionService) : AProvider<TProvider, TShipment>(providerRepository, encryptionService)
        where TProvider : class
        where TShipment : class
    {
        public override Task<Result<T>> CheckConnectionAsync<T>(Provider?         provider,
                                                                CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Result<T>.Succeed(("Bağlantı başarılı." as T)!));
        }
    }
}