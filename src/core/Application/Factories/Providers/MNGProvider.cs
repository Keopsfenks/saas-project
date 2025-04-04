using Application.Dtos;
using Application.Factories.Abstractions;
using Application.Factories.Parameters.Requests;
using Application.Factories.Parameters.Response;
using Application.Features.Commands.Shipments.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Newtonsoft.Json;
using System.Globalization;
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

        public override async Task<HttpClient> GetClient(Provider provider, string? apiVersion = null, CancellationToken cancellationToken = default, bool isRefresh = true, string? api = null)
        {
            HttpClient client = HttpClientFactory.CreateClient();

            MNGRequest.Provider? mngParameterProvider
                = ParametersFactory.Deserialize<MNGRequest.Provider>(provider.Parameters);

            MNGResponse.APIToken? mngResponseToken
                = ParametersFactory.Deserialize<MNGResponse.APIToken>(provider.Session);

            if (isRefresh)
            {
                if (mngResponseToken is not null &&
                    DateTime.TryParseExact(mngResponseToken.JwtExpireDate, "dd.MM.yyyy HH:mm:ss",
                                           CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime expireDate) &&
                    expireDate < DateTime.Now)
                {
                    await CreateConnectionAsync(provider, cancellationToken);
                    mngResponseToken = ParametersFactory.Deserialize<MNGResponse.APIToken>(provider.Session);
                }
            }

            client.BaseAddress = new Uri("https://testapi.mngkargo.com.tr/mngapi/api/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer", mngResponseToken is null ? "" : $"{EncryptionService.Decrypt(mngResponseToken.Jwt)}");
            client.DefaultRequestHeaders.Add("X-IBM-Client-Id",     mngParameterProvider?.ClientId);
            client.DefaultRequestHeaders.Add("X-IBM-Client-Secret", mngParameterProvider?.ClientSecret);

            if (!string.IsNullOrEmpty(apiVersion))
                client.DefaultRequestHeaders.Add("x-api-version", apiVersion);

            return client;
        }

        public async Task<Result<string>> CreateConnectionAsync(Provider provider, CancellationToken cancellationToken = default)
        {
            HttpClient client = await GetClient(provider, cancellationToken: cancellationToken, isRefresh: false);

            MNGRequest.APIToken request = new(provider.Username, EncryptionService.Decrypt(provider.Password));

            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("token", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(), response.ReasonPhrase);

            string contentResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            MNGResponse.APIToken? mngResponse = JsonConvert.DeserializeObject<MNGResponse.APIToken>(contentResponse);

            if (mngResponse is null)
                return (500, "Beklenmeyen bir hata oluştu.");

            token                    = mngResponse.Jwt;
            mngResponse.Jwt          = EncryptionService.Encrypt(mngResponse.Jwt);
            mngResponse.RefreshToken = EncryptionService.Encrypt(mngResponse.RefreshToken);

            provider.Session = ParametersFactory.Serialize(mngResponse);

            await ProviderRepository.ReplaceOneAsync(x => x.Id == provider.Id, provider, cancellationToken);

            return "İşlem başarılı";
        }

        public override async Task<Result<ShipmentDto>> CreateShipmentAsync(CreateShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Provider? provider
                = await ProviderRepository.FindOneAsync(x => x.Id == request.ProviderId, cancellationToken);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            MNGRequest.APICreateOrder order = new(request);

            HttpClient client = await GetClient(provider, cancellationToken: cancellationToken);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync("standardcmdapi/createOrder", content, cancellationToken);

            string contentResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(),
                        $"MNG Kargo Hatası: {response.Content.ReadAsStringAsync(cancellationToken).Result}");

            MNGResponse.APICreateOrder? mngResponse = JsonConvert.DeserializeObject<List<MNGResponse.APICreateOrder>>(contentResponse)!.FirstOrDefault();

            if (mngResponse is null)
                return (500, "Beklenmeyen bir hata oluştu.");

            Shipment shipment = new()
                                {
                                    Name        = $"{order.order.referenceId}: gönderi numaralı kargo",
                                    CargoId     = order.order.referenceId,
                                    WaybillId   = Convert.ToInt32(order.order.billOfLandingId),
                                    Type        = request.Type,
                                    Description = request.Description,
                                    Dispatch    = request.Dispatch,
                                    Cargo       = request.Cargo,
                                    Status      = CargoStatusEnum.DRAFT,
                                    Recipient   = request.Recipient,
                                    ProviderId  = provider.Id,
                                    Provider    = provider,
                                    ProviderInfo =
                                    {
                                        { "orderInvoiceId", mngResponse.orderInvoiceId },
                                        { "orderInvoiceDetailId", mngResponse.orderInvoiceDetailId },
                                        { "shipperBranchCode", mngResponse.shipperBranchCode }
                                    },
                                };

            await ShipmentRepository.InsertOneAsync(shipment, cancellationToken);
            return new ShipmentDto(shipment);
        }

        public override async Task<Result<ShipmentDto>> UpdateShipmentAsync(UpdateShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment
                = await ShipmentRepository.FindOneAsync(x => x.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return (404, "Gönderi Bulanamadı");


            shipment.Dispatch = request.Dispatch ?? shipment.Dispatch;
            shipment.Cargo    = request.Cargo ?? shipment.Cargo;


            HttpClient client = await GetClient(shipment.Provider, cancellationToken: cancellationToken);

            MNGRequest.APIUpdateOrder order = new(shipment);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync("standardcmdapi/updateorder", content, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(),
                        $"MNG Kargo Hatası: {response.Content.ReadAsStringAsync(cancellationToken).Result}");


            if (shipment.Status != CargoStatusEnum.DRAFT)
            {
                //DOLDUR
            }

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);
            return new ShipmentDto(shipment);
        }

        public override async Task<Result<ShipmentDto>> CancelShipmentAsync(CancelShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment
                = await ShipmentRepository.FindOneAsync(x => x.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return (404, "Gönderi Bulanamadı");

            HttpClient client = await GetClient(shipment.Provider, cancellationToken: cancellationToken);

            HttpResponseMessage response
                = await client.PutAsync($"standardcmdapi/cancelorder/{shipment.CargoId}", null, cancellationToken);

            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(),
                        $"MNG Kargo Hatası: {response.Content.ReadAsStringAsync(cancellationToken).Result}");

            if (shipment.Status != CargoStatusEnum.DRAFT)
            {
                //DOLDUR
            }

            shipment.Status = CargoStatusEnum.CANCELLED;

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return new ShipmentDto(shipment);
        }

        public override async Task<Result<ShipmentDto>> ConfirmShipmentAsync(ConfirmShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment
                = await ShipmentRepository.FindOneAsync(x => x.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return (404, "Gönderi Bulanamadı.");

            HttpClient client = await GetClient(shipment.Provider, cancellationToken: cancellationToken);

            MNGRequest.APICreateBarcode order = new(shipment);
            HttpContent content = new StringContent(JsonConvert.SerializeObject(order), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response
                = await client.PostAsync("barcodecmdapi/createbarcode", content, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return (response.StatusCode.GetHashCode(),
                        $"MNG Kargo Hatası: {response.Content.ReadAsStringAsync(cancellationToken).Result}");

            string contentResponse = await response.Content.ReadAsStringAsync(cancellationToken);
            MNGResponse.APICreateBarcode? mngResponse = JsonConvert.DeserializeObject<MNGResponse.APICreateBarcode>(contentResponse);

            if (mngResponse is null)
                return (500, "Beklenmeyen bir hata oluştu.");

            shipment.Status    = CargoStatusEnum.SEND_TO_PROVIDER;
            shipment.InvoiceId = Convert.ToInt32(mngResponse.invoiceId);

            shipment.ProviderInfo.TryAdd("_referenceId", mngResponse.referenceId);
            shipment.ProviderInfo.TryAdd("_invoiceId", mngResponse.invoiceId);
            shipment.ProviderInfo.TryAdd("_shipmentId", mngResponse.shipmentId);
            shipment.ProviderInfo.TryAdd("_barcodes", string.Join(",", mngResponse.barcodes.Select(x => x.value)));

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return new ShipmentDto(shipment);
        }
    }
}
