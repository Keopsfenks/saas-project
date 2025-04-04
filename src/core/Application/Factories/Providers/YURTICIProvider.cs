using Application.Dtos;
using Application.Factories.Abstractions;
using Application.Factories.Parameters.Requests;
using Application.Features.Commands.Shipments.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using Domain.ValueObject;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class YURTICIProvider<TProvider>(
        IRepositoryService<Provider> providerRepository,
        IRepositoryService<Shipment> shipmentRepository,
        IHttpClientFactory           httpClientFactory,
        IEncryptionService           encryptionService) : AProvider<TProvider>(providerRepository,
                                                                               shipmentRepository,
                                                                               httpClientFactory,
                                                                               encryptionService) where TProvider : class
    {

        private string _auth = string.Empty;


        public override Task<HttpClient> GetClient(Provider provider, string? apiVersion = null, CancellationToken cancellationToken = default,
                                                         bool     isRefresh = true,
                                                         string?  api      = null)
        {
            HttpClient client = HttpClientFactory.CreateClient();

            client.BaseAddress
                = new Uri("http" +
                          "://testwebservices.yurticikargo.com:9090/KOPSWebServices/ShippingOrderDispatcherServices");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            _auth = $@"<wsUserName>{provider.Username}</wsUserName>
                      <wsPassword>{EncryptionService.Decrypt(provider.Password)}</wsPassword>
                      <userLanguage>TR</userLanguage>";

            return Task.FromResult(client);
        }

        public override async Task<Result<ShipmentDto>> UpdateShipmentAsync(UpdateShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment
                = await ShipmentRepository.FindOneAsync(x => x.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return (404, "Kargo bulunamadı.");

            CargoStatusEnum status = CargoStatusEnum.FromValue(shipment.Status);
            if (shipment.Status != CargoStatusEnum.DRAFT)
                return (400, $"Yurtiçi kargo gönderi durumu ({status}) olduğundan güncellenemez.");

            shipment.Dispatch = request.Dispatch ?? shipment.Dispatch;
            shipment.Cargo    = request.Cargo    ?? shipment.Cargo;

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return new ShipmentDto(shipment);
        }

        public override async Task<Result<ShipmentDto>> CancelShipmentAsync(CancelShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment
                = await ShipmentRepository.FindOneAsync(x => x.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return (404, "Kargo bulunamadı.");

            HttpClient client = await GetClient(shipment.Provider, cancellationToken: cancellationToken);

            string body = YURTICIRequest.APICancelOrder.Request(_auth, shipment);

            HttpRequestMessage req     = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            HttpContent        content = new StringContent(body, Encoding.UTF8, "text/xml");

            req.Content = content;

            HttpResponseMessage response = await client.SendAsync(req, cancellationToken);

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(await response.Content.ReadAsStringAsync(cancellationToken));

            string? err     = xmlDocument.SelectSingleNode("//errMessage")?.InnerText;
            string? errCode = xmlDocument.SelectSingleNode("//errCode")?.InnerText;

            if (errCode is not null && errCode != "0")
                return (500, $"{errCode}: {err ?? "Hata oluştu."}");

            shipment.Status       = CargoStatusEnum.CANCELLED;

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return (new ShipmentDto(shipment));
        }

        public override async Task<Result<ShipmentDto>> ConfirmShipmentAsync(ConfirmShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment
                = await ShipmentRepository.FindOneAsync(x => x.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return (404, "Kargo bulunamadı.");


            HttpClient client = await GetClient(shipment.Provider, cancellationToken: cancellationToken);

            string body = YURTICIRequest.APIConfirmOrder.Request(_auth, shipment);

            HttpRequestMessage req     = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            HttpContent        content = new StringContent(body, Encoding.UTF8, "text/xml");

            req.Content = content;

            HttpResponseMessage response = await client.SendAsync(req, cancellationToken);

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(await response.Content.ReadAsStringAsync(cancellationToken));

            string? err     = xmlDocument.SelectSingleNode("//errMessage")?.InnerText;
            string? errCode = xmlDocument.SelectSingleNode("//errCode")?.InnerText;

            if (errCode is null && errCode != "0")
                return (500, $"{errCode}: {err ?? "Hata oluştu."}");

            var jobId = xmlDocument.SelectSingleNode("//jobId")?.InnerText;

            shipment.Status       = CargoStatusEnum.SEND_TO_PROVIDER;
            shipment.ProviderInfo = new() { { "jobId", jobId ?? string.Empty }, };

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return (new ShipmentDto(shipment));
        }
    }
}