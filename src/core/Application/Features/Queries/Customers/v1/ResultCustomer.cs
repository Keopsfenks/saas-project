using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Customers.v1
{
    public record ResultCustomerRequest() : IRequest<Result<List<CustomerDto>>>
    {
        public int     PageSize   { get; set; } = 10;
        public int     PageNumber { get; set; } = 0;
        public string? Search     { get; set; } = null;
    }


    internal sealed record ResultCustomerHandler(
        IRepositoryService<Customer> customerRepository) : IRequestHandler<ResultCustomerRequest, Result<List<CustomerDto>>>
    {
        public async Task<Result<List<CustomerDto>>> Handle(ResultCustomerRequest request, CancellationToken cancellationToken)
        {
            int     PageSize   = request.PageSize;
            int     PageNumber = request.PageNumber;
            string? Search     = request.Search;

            IEnumerable<Customer?> customers = await customerRepository.FindAsync(x => true, cancellationToken);

            List<CustomerDto> customersList = customers
                                             .OrderBy(x => x.Name)
                                             .Where(x => Search == null || x.Name.ToLower()
                                                                            .Contains(Search.ToLower()))
                                             .Skip(PageNumber * PageSize)
                                             .Take(PageSize)
                                             .Select(x => new CustomerDto(x!))
                                             .ToList();

            return customersList;
        }
    }
}