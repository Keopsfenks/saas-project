using Domain.Abstractions;
using Domain.Enums;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Cargo : WorkspaceEntity
    {
        public string  Name        { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        public UnitOfMeasureEnum MassUnit     { get; set; } = UnitOfMeasureEnum.Kilogram;
        public UnitOfMeasureEnum DistanceUnit { get; set; } = UnitOfMeasureEnum.Centimeter;

        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Width  { get; set; }
        public decimal Volume => Width * Length * Height;
    }
}