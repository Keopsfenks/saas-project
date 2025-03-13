using Domain.Enums;

namespace Domain.ValueObject
{
    public sealed class Item
    {
        public string Name        { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public UnitOfMeasureEnum UnitOfMeasure { get; set; } = UnitOfMeasureEnum.Kilogram;

        public decimal Weight { get; set; }
        public decimal Stock  { get; set; }

        public string? Barcode { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}