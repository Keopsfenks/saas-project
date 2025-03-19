using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Customers.v1
{
    public record ResultCustomerRequest() : IRequest<Result<List<CustomerDto>>>
    {
        public string? Filter            { get; set; } = null;
        public int?    Skip              { get; set; } = null;
        public int?    Top               { get; set; } = null;
        public string? Expand            { get; set; } = null;
        public string? OrderBy           { get; set; } = null;
        public string? ThenBy            { get; set; } = null;
        public string? OrderByDescending { get; set; } = null;
        public string? ThenByDescending  { get; set; } = null;
    }


    internal sealed record ResultCustomerHandler(
        IRepositoryService<Customer> customerRepository) : IRequestHandler<ResultCustomerRequest, Result<List<CustomerDto>>>
    {
        public async Task<Result<List<CustomerDto>>> Handle(ResultCustomerRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Customer?> results
                = await customerRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top, request.Expand,
                                                  request.OrderBy, request.ThenBy, request.OrderByDescending,
                                                  request.ThenByDescending, cancellationToken);




            return results.Select(x => new CustomerDto(x!)).ToList();
        }
    }
}