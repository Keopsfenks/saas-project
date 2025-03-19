using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using FluentValidation;
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

    public sealed class CreateProductValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Ad alanı boş olamaz")
               .MinimumLength(3).WithMessage("Ad en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(50).WithMessage("Ad en fazla 50 karakter uzunluğunda olmalıdır");

            RuleFor(x => x.Description)
               .MaximumLength(200).WithMessage("Açıklama en fazla 200 karakter uzunluğunda olmalıdır")
               .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Barcode)
               .Matches(@"^\d{12}$").WithMessage("Barkod 12 haneli bir sayı olmalıdır")
               .When(x => !string.IsNullOrEmpty(x.Barcode));

            RuleFor(x => x.Stock)
               .GreaterThanOrEqualTo(0)
               .WithMessage("Stok negatif olamaz");

        }
    }

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