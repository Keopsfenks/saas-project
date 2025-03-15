using Application.Factories.Abstractions;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using System.Net.Http.Headers;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class TESTProvider<TProvider, TShipment> : AProvider<TProvider, TShipment>
        where TProvider : class
        where TShipment : class
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

        public override Task<Result<T>> CheckConnectionAsync<T>(Provider         provider,
                                                                CancellationToken cancellationToken = default)
        {

            return Task.FromResult(Result<T>.Succeed(("Bağlantı başarılı." as T)!));
        }

        public override Task<Result<T>> RefreshTokenAsync<T>(Provider provider, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}