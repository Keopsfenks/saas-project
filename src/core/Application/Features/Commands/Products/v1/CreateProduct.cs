using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Products.v1
{
    public sealed record CreateProductRequest(
        string  Name,
        string? Description,
        string? Barcode,
        int     UnitOfMeasure,
        decimal Weight,
        decimal Stock,
        decimal Price) : IRequest<Result<ProductDto>>;



    internal sealed record CreateProductHandler(
        IRepositoryService<Product> productRepository) : IRequestHandler<CreateProductRequest, Result<ProductDto>>
    {
        public async Task<Result<ProductDto>> Handle(CreateProductRequest request, CancellationToken cancellationToken)
        {
            bool isProductExist = await productRepository.ExistsAsync(c => c.Name == request.Name, cancellationToken);

            if (isProductExist)
                return (409, "Sistemde aynı isimde bir ürün bulunmaktadır.");

            Product product = new()
                              {
                                  Name          = request.Name,
                                  Description   = request.Description,
                                  UnitOfMeasure = UnitOfMeasureEnum.FromValue(request.UnitOfMeasure),
                                  Weight        = request.Weight,
                                  Stock         = request.Stock,
                                  Price         = request.Price,
                                  Barcode       = request.Barcode
                              };

            await productRepository.InsertOneAsync(product, cancellationToken);

            return new ProductDto(product);
        }
    }
}