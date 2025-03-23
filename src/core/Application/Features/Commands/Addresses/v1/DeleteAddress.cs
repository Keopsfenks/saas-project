using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Addresses.v1
{
    public sealed record DeleteAddressRequest(
        string Id) : IRequest<Result<string>>;


    internal sealed record DeleteAddressHandler(IRepositoryService<Address> addressRepository) : IRequestHandler<DeleteAddressRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteAddressRequest request, CancellationToken cancellationToken)
        {
            bool isAddressExist = await addressRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isAddressExist)
                return (404, "Adres bulunamadı.");


            await addressRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Adres başarıyla silindi.";
        }
    }
}