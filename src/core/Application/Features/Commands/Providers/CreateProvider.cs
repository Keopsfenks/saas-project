using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers
{
    public sealed record CreateProviderRequest(
        string Username,
        string Password,
        int    ShippingProviderCode) : IRequest<Result<ProviderDto>>;


    internal sealed record CreateProviderHandler(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encrptionService) : IRequestHandler<CreateProviderRequest, Result<ProviderDto>>
    {
        public async Task<Result<ProviderDto>> Handle(CreateProviderRequest request, CancellationToken cancellationToken)
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
                                    ShippingProvider = ShippingProviderEnum.FromValue(request.ShippingProviderCode)
                                };

            ProviderDto providerDto = new(provider);

            provider.Password = encrptionService.Encrypt(request.Password);
            await providerRepository.InsertOneAsync(provider, cancellationToken);

            return providerDto;
        }
    }
}