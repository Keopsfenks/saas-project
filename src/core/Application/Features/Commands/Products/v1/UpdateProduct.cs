using Application.Dtos;
using Application.Services;
using Domain.Entities.WorkspaceEntities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using TS.Result;

namespace Application.Features.Commands.Products.v1
{
    public sealed record UpdateProductRequest(
        string   Id,
        string?  Name,
        string?  Description,
        string?  Barcode,
        int?     UnitOfMeasure,
        decimal? Weight,
        decimal? Stock,
        decimal? Price) : IRequest<Result<ProductDto>>;

    public sealed class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Id)
               .NotEmpty().WithMessage("Id alanı boş olamaz")
               .NotNull().WithMessage("Id alanı null olamaz");

            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Ad alanı boş olamaz")
               .MinimumLength(3).WithMessage("Ad en az 3 karakter uzunluğunda olmalıdır")
               .MaximumLength(50).WithMessage("Ad en fazla 50 karakter uzunluğunda olmalıdır")
               .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
               .MaximumLength(200).WithMessage("Açıklama en fazla 200 karakter uzunluğunda olmalıdır")
               .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Barcode)
               .Matches(@"^\d{12}$").WithMessage("Barkod 12 haneli bir sayı olmalıdır")
               .When(x => !string.IsNullOrEmpty(x.Barcode));

            RuleFor(x => x.Stock)
               .GreaterThanOrEqualTo(0)
               .WithMessage("Stok negatif olamaz")
               .When(x => x.Stock is not null);

        }
    }


    internal sealed record UpdateProductHandler(
        IRepositoryService<Product> productRepository) : IRequestHandler<UpdateProductRequest, Result<ProductDto>>
    {
        public async Task<Result<ProductDto>> Handle(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            Product? product = await productRepository.FindOneAsync(c => c.Id == request.Id, cancellationToken);

            if (product is null)
                return (404, "Sistemde kayıtlı ürün bulunamadı.");

            if (request.Name is not null)
                product.Name = request.Name;
            if (request.Description is not null)
                product.Description = request.Description;
            if (request.UnitOfMeasure is not null)
                product.UnitOfMeasure = UnitOfMeasureEnum.FromValue((int)request.UnitOfMeasure);
            if (request.Weight is not null)
                product.Weight = (decimal)request.Weight;
            if (request.Stock is not null)
                product.Stock = (decimal)request.Stock;
            if (request.Price is not null)
                product.Price = (decimal)request.Price;
            if (request.Barcode is not null)
                product.Barcode = request.Barcode;

            await productRepository.ReplaceOneAsync(c => c.Id == request.Id, product, cancellationToken);

            return new ProductDto(product);

        }
    }
}