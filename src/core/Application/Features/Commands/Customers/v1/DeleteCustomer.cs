using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Customers.v1
{
    public sealed record DeleteCustomerRequest(
        string Id) : IRequest<Result<string>>;

    internal sealed record DeleteCustomerHandler(
        IRepositoryService<Customer> customerRepository) : IRequestHandler<DeleteCustomerRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteCustomerRequest request, CancellationToken cancellationToken)
        {
            bool isCustomerExist = await customerRepository.ExistsAsync(x => x.Id == request.Id, cancellationToken);

            if (!isCustomerExist)
                return (404, "Müşteri bulunamadı.");

            await customerRepository.SoftDeleteOneAsync(x => x.Id == request.Id, cancellationToken);

            return "Müşteri başarıyla silindi.";
        }
    }
}