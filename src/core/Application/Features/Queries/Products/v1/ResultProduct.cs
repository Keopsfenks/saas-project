using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Products.v1
{
    public record ResultProductRequest() : IRequest<Result<List<ProductDto>>>
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


    internal sealed record ResultProductHandler(
        IRepositoryService<Product> productRepository) : IRequestHandler<ResultProductRequest, Result<List<ProductDto>>>
    {
        public async Task<Result<List<ProductDto>>> Handle(ResultProductRequest request, CancellationToken cancellationToken)
        {
            IEnumerable<Product?> results
                = await productRepository.FindAsync(x => true, request.Filter, request.Skip, request.Top, request.Expand,
                                                  request.OrderBy, request.ThenBy, request.OrderByDescending,
                                                  request.ThenByDescending, cancellationToken);




            return results.Select(x => new ProductDto(x!)).ToList();
        }
    }
}