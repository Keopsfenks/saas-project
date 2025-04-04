using Application.Dtos;
using Application.Factories.Abstractions;
using Application.Factories.Parameters.Requests;
using Application.Features.Commands.Shipments.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class ARASProvider<TProvider>(
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

            client.BaseAddress
                = new Uri($"https://customerws.araskargo.com.tr/arascargoservice.asmx?op=" + $"{api}");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

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
                return (400, $"Aras kargo gönderi durumu ({status}) olduğundan güncellenemez.");

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

            string body = ARASRequest.APICancelOrder.Request(shipment.Provider, shipment);

            HttpRequestMessage req     = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            HttpContent        content = new StringContent(body, Encoding.UTF8, "text/xml");

            req.Content = content;

            HttpResponseMessage response = await client.SendAsync(req, cancellationToken);

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(await response.Content.ReadAsStringAsync(cancellationToken));

            string? err     = xmlDocument.SelectSingleNode("//ResultMessage")?.InnerText;
            string? errCode = xmlDocument.SelectSingleNode("//ResultCode")?.InnerText;

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
                return (404, "Kargo bulunamadı");

            HttpClient client
                = await GetClient(shipment.Provider, cancellationToken: cancellationToken, api: "SetOrder");

            string body = ARASRequest.APIConfirmOrder.Request(shipment.Provider, shipment);

            HttpRequestMessage req     = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            HttpContent        content = new StringContent(body, Encoding.UTF8, "text/xml");

            req.Content = content;

            HttpResponseMessage response = await client.SendAsync(req, cancellationToken);

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(await response.Content.ReadAsStringAsync(cancellationToken));

            string? err     = xmlDocument.SelectSingleNode("//ResultMessage")?.InnerText;
            string? errCode = xmlDocument.SelectSingleNode("//ResultCode")?.InnerText;

            if (errCode is not null && errCode != "0")
                return (500, $"{errCode}: {err ?? "Hata oluştu."}");

            var jobId = xmlDocument.SelectSingleNode("//InvoiceKey")?.InnerText;

            shipment.Status       = CargoStatusEnum.SEND_TO_PROVIDER;
            shipment.ProviderInfo = new() { { "InvoiceKey", jobId ?? string.Empty }, };

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return (new ShipmentDto(shipment));
        }
    }
}