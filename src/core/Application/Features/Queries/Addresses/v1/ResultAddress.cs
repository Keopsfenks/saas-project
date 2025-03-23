using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Addresses.v1
{
    public sealed record ResultAddressRequest() : IRequest<Result<List<AddressDto>>>
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


    internal sealed record ResultAddressHandler(
        IRepositoryService<Address> addressRepository) : IRequestHandler<ResultAddressRequest, Result<List<AddressDto>>>
    {
        public async Task<Result<List<AddressDto>>> Handle(ResultAddressRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Address?> results
                = await addressRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top,
                                                    request.Expand,
                                                    request.OrderBy, request.ThenBy, request.OrderByDescending,
                                                    request.ThenByDescending, cancellationToken);

            return results.Select(x => new AddressDto(x!)).ToList();
        }
    }
}