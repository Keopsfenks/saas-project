using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record DeleteProviderRequest(
        string Id) : IRequest<Result<string>>;


    internal sealed record DeleteProviderHandler(
        IRepositoryService<Provider> providerRepository) : IRequestHandler<DeleteProviderRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteProviderRequest request, CancellationToken cancellationToken)
        {
            bool isProviderExist = await providerRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isProviderExist)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            await providerRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Kargo sağlayıcısı başarıyla silindi.";
        }
    }
}