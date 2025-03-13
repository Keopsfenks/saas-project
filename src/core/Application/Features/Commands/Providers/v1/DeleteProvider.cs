using Application.Factories;
using Application.Factories.Interfaces;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record DeleteProviderRequest(
        string Id) : IRequest<Result<string>>;


    internal sealed record DeleteProviderHandler(
        IRepositoryService<Provider> providerRepository,
        IServiceProvider             serviceProvider) : IRequestHandler<DeleteProviderRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteProviderRequest request, CancellationToken cancellationToken)
        {
            Provider? provider = await providerRepository.FindOneAsync(x => x.Id == request.Id, cancellationToken);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            ProviderFactory providerFactory = new(provider.ShippingProvider, serviceProvider);

            IProvider providerService = providerFactory.GetProvider();

            return await providerService.DeleteProviderAsync<string>(request, cancellationToken);
        }
    }
}