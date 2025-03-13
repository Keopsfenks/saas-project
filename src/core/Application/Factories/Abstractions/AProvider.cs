using Application.Dtos;
using Application.Factories.Interfaces;
using Application.Factories.Parameters;
using Application.Features.Commands.Providers.v1;
using Application.Features.Commands.Shipments.v1;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using TS.Result;

namespace Application.Factories.Abstractions
{

    public abstract class AProvider<TProvider, TShipment>(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encryptionService)
        where TProvider : IProvider
        where TShipment : IProvider
    {

        public async Task<Result<ProviderDto>> CreateProviderAsync(CreateProviderRequest request, CancellationToken cancellationToken = default)
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
            provider.Password = encryptionService.Encrypt(request.Password);
            await providerRepository.InsertOneAsync(provider, cancellationToken);

            return providerDto;
        }

        public async Task<Result<ProviderDto>> UpdateProviderAsync(UpdateProviderRequest request, CancellationToken cancellationToken = default)
        {
            Provider? provider = await providerRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

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
                providerDto.Parameters = ParametersFactory<TProvider>.Parameters(request.Parameters);
                provider.Parameters    = ParametersFactory<TProvider>.Parameters(request.Parameters);
            }

            await providerRepository.ReplaceOneAsync(x => x.Id == provider.Id, provider, cancellationToken);

            return providerDto;
        }

        public async Task<Result<string>> DeleteProviderAsync(DeleteProviderRequest request, CancellationToken cancellationToken = default)
        {
            bool isProviderExist = await providerRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isProviderExist)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            await providerRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Kargo sağlayıcısı başarıyla silindi.";
        }

        public abstract Task<Result<string>> CheckConnectionAsync(CancellationToken cancellationToken = default);

    }
}