using Application.Services;
using Domain.Entities.WorkspaceEntities;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Products.v1
{
    public sealed record DeleteProductRequest(
        string Id) : IRequest<Result<string>>;


    internal sealed record DeleteProductHandler(
        IRepositoryService<Product> productRepository) : IRequestHandler<DeleteProductRequest, Result<string>>
    {
        public async Task<Result<string>> Handle(DeleteProductRequest request, CancellationToken cancellationToken)
        {
            bool isProductExist = await productRepository.ExistsAsync(c => c.Id == request.Id, cancellationToken);

            if (!isProductExist)
                return (404, "Sistemde belirtilen ürün bulunamadı.");


            await productRepository.SoftDeleteOneAsync(c => c.Id == request.Id, cancellationToken);

            return "Sistemden ürün başarıyla silindi.";

        }
    }
}