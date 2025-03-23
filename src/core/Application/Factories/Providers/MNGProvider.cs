using Application.Dtos;
using Application.Factories.Abstractions;
using Application.Factories.Parameters.Requests;
using Application.Factories.Parameters.Response;
using Application.Features.Commands.Orders.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class MNGProvider<TProvider> : AProvider<TProvider>
        where TProvider : class
    {

        public MNGProvider(IRepositoryService<Provider> providerRepository,
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
            HttpClient client = HttpClientFactory.CreateClient();

            MNGRequestProvider? mngParameterProvider
                = ParametersFactory.Deserialize<MNGRequestProvider>(provider.Parameters);

            MNGResponseToken? mngResponseToken
                = ParametersFactory.Deserialize<MNGResponseToken>(provider.Session);


            if (mngParameterProvider is null)
                throw new ArgumentNullException(nameof(mngParameterProvider));

            client.BaseAddress = new Uri("https://testapi.mngkargo.com.tr/mngapi/api/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", EncryptionService.Decrypt(mngResponseToken!.Jwt) ?? "");
            client.DefaultRequestHeaders.Add("X-IBM-Client-Id",     mngParameterProvider.ClientId);
            client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", mngParameterProvider.ClientSecret);

            if (!string.IsNullOrEmpty(apiVersion))
                client.DefaultRequestHeaders.Add("x-api-version", apiVersion);

            return client;
        }

        public override async Task<Result<string>> CreateConnectionAsync(Provider provider, CancellationToken cancellationToken = default)
        {
            HttpClient client = GetClient(provider);

            MNGRequestToken request = new(provider.Username, provider.Password);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("token", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(), response.ReasonPhrase);

            string contentResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            MNGResponseToken? mngResponse = JsonConvert.DeserializeObject<MNGResponseToken>(contentResponse);

            if (mngResponse is null)
                return (500, "Beklenmeyen bir hata oluştu.");

            mngResponse.Jwt          = EncryptionService.Encrypt(mngResponse.Jwt);
            mngResponse.RefreshToken = EncryptionService.Encrypt(mngResponse.RefreshToken);

            provider.Session = ParametersFactory.Serialize(mngResponse);

            await ProviderRepository.ReplaceOneAsync(x => x.Id == provider.Id, provider, cancellationToken);

            return "İşlem başarılı";
        }

        public override async Task<Result<ShipmentDto>> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
        {
            Provider? provider
                = await ProviderRepository.FindOneAsync(x => x.Id == request.ProviderId, cancellationToken);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            await CreateConnectionAsync(provider, cancellationToken);

            int cod;
            int package;
            int payment;

            if (request.Order.IsCod == CodEnum.COD)
                cod = 1;
            else
                cod = 0;

            if (request.Order.PackagingType == PackagingTypeEnum.File)
                package = 1;
            else if (request.Order.PackagingType == PackagingTypeEnum.Package)
                package = 3;
            else
                package = 4;

            if (request.Order.PaymentType == PaymentTypeEnum.Sender)
                payment = 1;
            else if (request.Order.PaymentType == PaymentTypeEnum.Receiver)
                payment = 2;
            else
                payment = 3;

            MNGRequestOrder            order  = new(cod, cod == 0 ? 0 : request.Order.CodPrice, package, payment);
            List<MNGRequestOrderCargo> cargos = new();
            foreach (var cargoList in request.Cargo)
            {
                MNGRequestOrderCargo cargo = new(order.referenceId, cargoList);
                cargos.Add(cargo);
            }
            MNGRequestOrderMember shipper   = new(request.Shipper);
            MNGRequestOrderMember recipient = new(request.Recipient);
            MNGRequestFullOrder fullOrder = new(order, cargos, shipper, recipient);


            object     orderTest = JsonConvert.SerializeObject(fullOrder);
            HttpClient client    = GetClient(provider);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(fullOrder), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("createDetailedOrder", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(), response.ReasonPhrase);

            string contentResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            MNGResponseOrder? mngResponse = JsonConvert.DeserializeObject<MNGResponseOrder>(contentResponse);

            if (mngResponse is null)
                return (500, "Beklenmeyen bir hata oluştu.");

            Shipment shipment = new()
                                {
                                    Name = order.referenceId,
                                    Description
                                        = $"MNG Kargo {mngResponse.OrderInvoiceId} - {mngResponse.OrderInvoiceDetailId} fatura numaralı kargo",
                                    Order                = request.Order,
                                    Cargo                = request.Cargo,
                                    Status               = CargoStatusEnum.SEND_TO_PROVIDER,
                                    Recipient            = request.Recipient,
                                    Shipper              = request.Shipper,
                                    ShippingProviderCode = ShippingProviderEnum.MNG,
                                    ProviderId           = provider.Id,
                                    OrderDetail          = ParametersFactory.Serialize(mngResponse),
                                    Provider             = provider
                                };

            await ShipmentRepository.InsertOneAsync(shipment, cancellationToken);

            return new ShipmentDto(shipment);
        }

        public override async Task<Result<ShipmentDto>> CancelOrderAsync(CancelOrderRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment = await ShipmentRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

            if (shipment is null)
                return (404, "Kargo bulunamadı.");


            HttpClient client = GetClient(shipment.Provider);

            MNGRequestCancelOrder cancelOrder = new(shipment.Name, "Kargo iptal edildi.");

            HttpContent content = new StringContent(JsonConvert.SerializeObject(cancelOrder), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("cancelOrderDelivery", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(), response.ReasonPhrase);

            string contentResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            MNGResponseCancelOrder? mngResponse = JsonConvert.DeserializeObject<MNGResponseCancelOrder>(contentResponse);

            if (mngResponse is null)
                return (500, "Beklenmeyen bir hata oluştu.");

            shipment.Status = CargoStatusEnum.CANCELED;

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return new ShipmentDto(shipment);
        }
    }
}
