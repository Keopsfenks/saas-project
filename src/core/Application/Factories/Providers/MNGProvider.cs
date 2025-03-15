using Application.Factories.Abstractions;
using Application.Factories.Parameters;
using Application.Factories.Parameters.Requests;
using Application.Factories.Parameters.Response;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using TS.Result;

namespace Application.Factories.Providers
{
    public sealed class MNGProvider<TProvider, TShipment> : AProvider<TProvider, TShipment>
        where TProvider : class
        where TShipment : class
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

            MNGParameterProvider? mngParameterProvider
                = ParametersFactory.Deserialize<MNGParameterProvider>(provider.Parameters);

            if (mngParameterProvider is null)
                throw new ArgumentNullException(nameof(mngParameterProvider));

            client.BaseAddress = new Uri("https://api.mngkargo.com.tr/mngapi/api/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token ?? "");
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

        public override Task<Result<T>> RefreshTokenAsync<T>(Provider provider, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
