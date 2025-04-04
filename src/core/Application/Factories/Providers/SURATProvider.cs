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
    public sealed class SURATProvider<TProvider>(
        IRepositoryService<Provider> providerRepository,
        IRepositoryService<Shipment> shipmentRepository,
        IHttpClientFactory           httpClientFactory,
        IEncryptionService           encryptionService) : AProvider<TProvider>(providerRepository,
                                                                               shipmentRepository,
                                                                               httpClientFactory,
                                                                               encryptionService) where TProvider : class
    {

        private string _auth = string.Empty;

        public override Task<HttpClient> GetClient(Provider provider,         string? apiVersion = null, CancellationToken cancellationToken = default,
                                                   bool     isRefresh = true, string? api        = null)
        {
            HttpClient client = HttpClientFactory.CreateClient();

            client.BaseAddress
                = new Uri($"https://webservices.suratkargo.com.tr/services.asmx?op={api}");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));

            _auth = $@"<KullaniciAdi>{provider.Username}</KullaniciAdi>
                      <Sifre>{EncryptionService.Decrypt(provider.Password)}</Sifre>";

            return Task.FromResult(client);
        }

        public override Task<Result<ShipmentDto>> UpdateShipmentAsync(UpdateShipmentRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task<Result<ShipmentDto>> CancelShipmentAsync(CancelShipmentRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override async Task<Result<ShipmentDto>> ConfirmShipmentAsync(ConfirmShipmentRequest request, CancellationToken cancellationToken = default)
        {
            Shipment? shipment
                = await ShipmentRepository.FindOneAsync(x => x.Id == request.ShipmentId, cancellationToken);

            if (shipment is null)
                return (404, "Kargo bulunamadı.");

            HttpClient client = await GetClient(shipment.Provider, cancellationToken: cancellationToken);

            string body = SURATRequest.APIConfirmOrder.Request(_auth, shipment);

            HttpRequestMessage req     = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress);
            HttpContent        content = new StringContent(body, Encoding.UTF8, "text/xml");

            req.Content = content;

            HttpResponseMessage response = await client.SendAsync(req, cancellationToken);

            var xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(await response.Content.ReadAsStringAsync(cancellationToken));

            string? err     = xmlDocument.SelectSingleNode("//GonderiyiKargoyaGonderYeniResult")?.InnerText;

            if (err is null && err != "Tamam")
                return (500, $"500: {err}");

            var jobId = xmlDocument.SelectSingleNode("//jobId")?.InnerText;

            shipment.Status       = CargoStatusEnum.SEND_TO_PROVIDER;
            shipment.ProviderInfo = new() { { "jobId", jobId ?? string.Empty }, };

            await ShipmentRepository.ReplaceOneAsync(x => x.Id == shipment.Id, shipment, cancellationToken);

            return (new ShipmentDto(shipment));
        }
    }
}