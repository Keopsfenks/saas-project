using Application.Dtos;
using Application.Factories.Interfaces;
using Application.Factories.Parameters;
using Application.Features.Commands.Providers.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using TS.Result;

namespace Application.Factories.Abstractions
{

    public abstract class AProvider<TProvider, TShipment>(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encryptionService) : IProvider
        where TProvider : class
        where TShipment : class
    {

        public async Task<Result<T>> CreateProviderAsync<T>(CreateProviderRequest request, CancellationToken cancellationToken = default)
            where T : class
        {
            bool isProviderExist
                = await providerRepository.ExistsAsync(x => x.ShippingProvider ==
                                                            ShippingProviderEnum.FromValue(
                                                                request.ShippingProviderCode), cancellationToken);
            if (isProviderExist)
                return (409, "Kargo sağlayıcı zaten mevcut.");


            Provider provider = new()
                                {
                                    Username         = request.Username,
                                    Password         = request.Password,
                                    ShippingProvider = ShippingProviderEnum.FromValue(request.ShippingProviderCode),
                                    Parameters       = ParametersFactory<TProvider>.Parameters(request.Parameters),
                                };

            ProviderDto providerDto = new(provider);

            if (!ParametersFactory<TProvider>.ValidationParameters(request.Parameters))
                return (400, "Parametreler eksik veya hatalı.");
            provider.Password = encryptionService.Encrypt(request.Password);

            await providerRepository.InsertOneAsync(provider, cancellationToken);

            return (providerDto as T)!;
        }

        public async Task<Result<T>> UpdateProviderAsync<T>(UpdateProviderRequest request, CancellationToken cancellationToken = default)
            where T : class
        {
            Provider? provider = await providerRepository.FindOneAsync(x => x.Id == request.Id);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            ProviderDto providerDto = new(provider);

            if (request.Username is not null)
            {
                providerDto.Username = request.Username;
                provider.Username    = request.Username;
            }

            if (request.Password is not null)
            {
                providerDto.Password = request.Password;
                provider.Password    = encryptionService.Encrypt(request.Password);
            }
            if (request.Parameters is not null)
            {
                providerDto.Parameters = ParametersFactory<TProvider>.Parameters(request.Parameters)!.ToDictionary();
                provider.Parameters    = ParametersFactory<TProvider>.Parameters(request.Parameters);
            }

            if (!ParametersFactory<TProvider>.ValidationParameters(request.Parameters))
                return (400, "Parametreler eksik veya hatalı.");

            await providerRepository.ReplaceOneAsync(x => x.Id == provider.Id, provider, cancellationToken);

            return (providerDto as T)!;
        }

        public async Task<Result<T>> DeleteProviderAsync<T>(DeleteProviderRequest request, CancellationToken cancellationToken = default)
            where T : class
        {
            bool isProviderExist = await providerRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isProviderExist)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            await providerRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return ("Kargo sağlayıcısı başarıyla silindi." as T)!;
        }

        public abstract Task<Result<T>> CheckConnectionAsync<T>(Provider?         provider,
                                                                CancellationToken cancellationToken = default)
            where T : class;


    }
}