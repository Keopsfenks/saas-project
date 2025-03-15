using Application.Dtos;
using Application.Factories.Interfaces;
using Application.Factories.Parameters.Response;
using Application.Features.Commands.Providers.v1;
using Application.Features.Commands.Shipments.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using System.Net;
using TS.Result;

namespace Application.Factories.Abstractions
{

    public abstract class AProvider<TProvider, TShipment>(
        IRepositoryService<Provider> providerRepository,
        IRepositoryService<Shipment> shipmentRepository,
        IHttpClientFactory           httpClientFactory,
        IEncryptionService           encryptionService) : IProvider
        where TProvider : class
        where TShipment : class
    {

        protected string? token = null;

        protected readonly IRepositoryService<Provider> ProviderRepository = providerRepository;
        protected readonly IRepositoryService<Shipment> ShipmentRepository = shipmentRepository;
        protected readonly IHttpClientFactory           HttpClientFactory  = httpClientFactory;
        protected readonly IEncryptionService           EncryptionService  = encryptionService;

        public abstract HttpClient GetClient(Provider provider, string? apiVersion = null);

        public async Task<Result<T>> CreateProviderAsync<T>(CreateProviderRequest request, CancellationToken cancellationToken = default)
            where T : class
        {
            bool isProviderExist
                = await ProviderRepository.ExistsAsync(x => x.ShippingProvider ==
                                                            ShippingProviderEnum.FromValue(
                                                                request.ShippingProviderCode), cancellationToken);
            if (isProviderExist)
                return (409, "Kargo sağlayıcı zaten mevcut.");


            Provider provider = new()
                                {
                                    Username         = request.Username,
                                    Password         = request.Password,
                                    ShippingProvider = ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                    Parameters       = ParametersFactory.Serialize(request.Parameters),
                                };


            ProviderDto<TProvider> providerDto = new(provider);

            await CreateConnectionAsync(provider, cancellationToken);

            provider.Password = EncryptionService.Encrypt(request.Password);

            await ProviderRepository.InsertOneAsync(provider, cancellationToken);

            return (providerDto as T)!;
        }

        public async Task<Result<T>> UpdateProviderAsync<T>(UpdateProviderRequest request, CancellationToken cancellationToken = default)
            where T : class
        {
            Provider? provider = await ProviderRepository.FindOneAsync(x => x.Id == request.Id);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");


            provider.Username    = request.Username ?? provider.Username;
            provider.Password    = request.Password ?? provider.Password;
            provider.Parameters    = ParametersFactory.Serialize(request.Parameters) ?? provider.Parameters;

            ProviderDto<TProvider> providerDto = new(provider);

            if (request.Password is not null)
                provider.Password = EncryptionService.Encrypt(request.Password);

            await ProviderRepository.ReplaceOneAsync(x => x.Id == provider.Id, provider, cancellationToken);

            return (providerDto as T)!;
        }

        public async Task<Result<T>> DeleteProviderAsync<T>(DeleteProviderRequest request, CancellationToken cancellationToken = default)
            where T : class
        {
            bool isProviderExist = await ProviderRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isProviderExist)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            await ProviderRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return ("Kargo sağlayıcısı başarıyla silindi." as T)!;
        }

        public async Task<Result<T>> CreateShipmentAsync<T>(CreateShipmentRequest request,
                                                            CancellationToken     cancellationToken = default)
            where T : class
        {
            Provider? provider
                = await ProviderRepository.FindOneAsync(x => x.Id == request.ProviderId, cancellationToken);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            Shipment shipment = new()
                                {
                                    Provider   = provider,
                                    ProviderId = provider.Id,
                                    Order = ParametersFactory.Serialize(request.Order) ??
                                            throw new InvalidOperationException(),
                                    Cargo     = request.Cargo,
                                    Shipper   = request.Shipper,
                                    Recipient = request.Recipient,
                                    Status    = CargoStatusEnum.DRAFT,
                                };


            ShipmentDto<TShipment> shipmentDto = new(shipment);

            await ShipmentRepository.InsertOneAsync(shipment, cancellationToken);

            return (shipmentDto as T)!;
        }

        public async Task<Result<T>> UpdateShipmentAsync<T>(UpdateShipmentRequest request,
                                                            CancellationToken     cancellationToken = default)
            where T : class
        {
            Shipment? shipment = await ShipmentRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

            if (shipment is null)
                return (404, "Gönderi bulunamadı.");

            shipment.Order     = ParametersFactory.Serialize(request.Order) ?? shipment.Order;
            shipment.Cargo     = request.Cargo                                         ?? shipment.Cargo;
            shipment.Shipper   = request.Shipper                                       ?? shipment.Shipper;
            shipment.Recipient = request.Recipient                                     ?? shipment.Recipient;


            ShipmentDto<TShipment> shipmentDto = new(shipment);

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return (shipmentDto as T)!;
        }

        public async Task<Result<T>> DeleteShipmentAsync<T>(DeleteShipmentRequest request,
                                                            CancellationToken     cancellationToken = default)
            where T : class
        {
            bool isShipmentExist = await ShipmentRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isShipmentExist)
                return (404, "Gönderi bulunamadı.");


            await ShipmentRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return ("Gönderi başarıyla silindi." as T)!;
        }


        public abstract Task<Result<string>> CreateConnectionAsync(Provider          provider,
                                                                 CancellationToken cancellationToken = default);

        public abstract Task<Result<T>> RefreshTokenAsync<T>(Provider          provider,
                                                             CancellationToken cancellationToken = default)
            where T : class;

    }
}