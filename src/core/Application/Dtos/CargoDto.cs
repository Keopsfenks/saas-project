using Domain.Entities.WorkspaceEntities;
using Domain.Enums;

namespace Application.Dtos
{
    public sealed class CargoDto
    {
        public CargoDto(Cargo cargo)
        {
            Id = cargo.Id;
            Name = cargo.Name;
            Description = cargo.Description;
            MassUnit = cargo.MassUnit;
            DistanceUnit = cargo.DistanceUnit;
            Height = cargo.Height;
            Length = cargo.Length;
            Width = cargo.Width;
            CreatedAt = cargo.CreateAt;
            UpdatedAt = cargo.UpdateAt;
        }
        public string            Id           { get; set; }
        public string            Name         { get; set; }
        public string?           Description  { get; set; }
        public UnitOfMeasureEnum MassUnit     { get; set; }
        public UnitOfMeasureEnum DistanceUnit { get; set; }

        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Width  { get; set; }
        public decimal Volume => Width  * Length * Height;
        public decimal Desi   => Volume / 3000;

        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}