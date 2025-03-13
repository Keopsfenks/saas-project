using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Queries.Products.v1
{
    public record ResultProductRequest() : IRequest<Result<List<ProductDto>>>
    {
        public int     PageSize   { get; set; } = 10;
        public int     PageNumber { get; set; } = 0;
        public string? Search     { get; set; } = null;
    }


    internal sealed record ResultProductHandler(
        IRepositoryService<Product> productRepository) : IRequestHandler<ResultProductRequest, Result<List<ProductDto>>>
    {
        public async Task<Result<List<ProductDto>>> Handle(ResultProductRequest request, CancellationToken cancellationToken)
        {
            int     PageSize   = request.PageSize;
            int     PageNumber = request.PageNumber;
            string? Search     = request.Search;

            IEnumerable<Product?> products = await productRepository.FindAsync(x => true, cancellationToken);

            List<ProductDto> productsList = products
                                           .OrderBy(x => x.Name)
                                           .Where(x => Search == null || x.Name.ToLower()
                                                                          .Contains(Search.ToLower()))
                                           .Skip(PageNumber * PageSize)
                                           .Take(PageSize)
                                           .Select(x => new ProductDto(x!))
                                           .ToList();
            return productsList;
        }
    }
}