using Application.Dtos;
using Application.Factories.Abstractions;
using Application.Features.Commands.Shipments.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class HEPSIJETProvider<TProvider>(
        IRepositoryService<Provider> providerRepository,
        IRepositoryService<Shipment> shipmentRepository,
        IHttpClientFactory           httpClientFactory,
        IEncryptionService           encryptionService) : AProvider<TProvider>(providerRepository,
                                                                               shipmentRepository,
                                                                               httpClientFactory,
                                                                               encryptionService) where TProvider : class
    {
        public override Task<HttpClient> GetClient(Provider provider,         string? apiVersion = null, CancellationToken cancellationToken = default,
                                                   bool     isRefresh = true, string? api        = null)
        {
            HttpClient client = HttpClientFactory.CreateClient();

            string account       = $"{provider.Username}:{provider.Password}";
            string accountBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(account));



            throw new NotImplementedException();
        }

        public override Task<Result<ShipmentDto>> UpdateShipmentAsync(UpdateShipmentRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<Result<ShipmentDto>> CancelShipmentAsync(CancelShipmentRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<Result<ShipmentDto>> ConfirmShipmentAsync(ConfirmShipmentRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}