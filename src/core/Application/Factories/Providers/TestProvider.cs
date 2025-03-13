using Application.Factories.Abstractions;
using Application.Factories.Interfaces;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class TestProvider<TProvider, TShipment>(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService encryptionService) : AProvider<TProvider, TShipment>(providerRepository, encryptionService)
        where TProvider : IProvider
        where TShipment : IProvider
    {
        public override Task<Result<string>> CheckConnectionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}