using Domain.Entities.WorkspaceEntities;
using Domain.Enums;

namespace Application.Dtos
{
    public sealed class ProductDto
    {
        public ProductDto(Product product)
        {
            Id            = product.Id;
            Name          = product.Name;
            Description   = product.Description;
            UnitOfMeasure = UnitOfMeasureEnum.FromValue(product.UnitOfMeasure);
            Weight        = product.Weight;
            Stock         = product.Stock;
            Price         = product.Price;
            CreatedAt     = product.CreateAt;
            UpdatedAt     = product.UpdateAt;
        }

        public string  Id          { get; set; }
        public string  Name        { get; set; }
        public string? Description { get; set; }
        public string? Barcode     { get; set; }

        public UnitOfMeasureEnum UnitOfMeasure { get; set; }
        public decimal           Weight        { get; set; }
        public decimal           Stock         { get; set; }

        public decimal Price { get; set; }

        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}