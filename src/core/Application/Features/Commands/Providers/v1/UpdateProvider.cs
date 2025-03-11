using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Providers.v1
{
    public sealed record UpdateProviderRequest(
        string  Id,
        string? Username,
        string? Password) : IRequest<Result<ProviderDto>>;

    internal sealed record UpdateProviderHandler(
        IRepositoryService<Provider> providerRepository,
        IEncryptionService           encrptionService) : IRequestHandler<UpdateProviderRequest, Result<ProviderDto>>
    {
        public async Task<Result<ProviderDto>> Handle(UpdateProviderRequest request, CancellationToken cancellationToken)
        {
            Provider? provider = await providerRepository.FindOneAsync(x => x.Id == request.Id);

            if (provider is null)
                return (404, "Kargo sağlayıcısı bulunamadı.");

            ProviderDto providerDto = new(provider);

            if (request.Username is not null)
            {
                providerDto.Username = request.Username;
                provider.Username = request.Username;
            }

            if (request.Password is not null)
            {
                providerDto.Password = request.Password;
                provider.Password = encrptionService.Encrypt(request.Password);
            }

            await providerRepository.ReplaceOneAsync(x => x.Id == provider.Id, provider, cancellationToken);

            return providerDto;
        }
    }
}