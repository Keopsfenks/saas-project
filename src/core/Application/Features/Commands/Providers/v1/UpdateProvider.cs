using Application.Dtos;
using Application.Factories;
using Application.Factories.Interfaces;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record UpdateProviderRequest(
        string                      Id,
        string?                     Username,
        string?                     Password,
        Dictionary<string, string>? Parameters) : IRequest<Result<ProviderDto>>;

    internal sealed record UpdateProviderHandler(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encrptionService,
        IServiceProvider             serviceProvider) : IRequestHandler<UpdateProviderRequest, Result<ProviderDto>>
    {
        public async Task<Result<ProviderDto>> Handle(UpdateProviderRequest request, CancellationToken cancellationToken)
        {
            Provider? provider = await providerRepository.FindOneAsync(x => x.Id == request.Id);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            ProviderFactory providerFactory = new(provider.ShippingProvider, serviceProvider);


            IProvider providerService = providerFactory.GetProvider();

            return await providerService.UpdateProviderAsync<ProviderDto>(request, cancellationToken);
        }
    }
}