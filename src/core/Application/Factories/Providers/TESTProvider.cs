using Application.Factories.Abstractions;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class TESTProvider<TProvider> : AProvider<TProvider>
        where TProvider : class
    {
        public TESTProvider(IRepositoryService<Provider> providerRepository,
                            IRepositoryService<Shipment> shipmentRepository,
                            IHttpClientFactory           httpClientFactory,
                            IEncryptionService           encryptionService) : base(
            providerRepository,
            shipmentRepository,
            httpClientFactory,
            encryptionService)
        {
        }

        public override HttpClient GetClient(Provider provider, string? apiVersion = null)
        {
            throw new NotImplementedException();
        }

        public override Task<Result<string>> CreateConnectionAsync(Provider provider, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

    }
}