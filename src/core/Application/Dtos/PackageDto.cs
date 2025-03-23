using Domain.Entities.WorkspaceEntities;
using Domain.Enums;

namespace Application.Dtos
{
    public sealed class PackageDto
    {
        public PackageDto(Package package)
        {
            Id           = package.Id;
            Name         = package.Name;
            Description  = package.Description;
            MassUnit     = UnitOfMeasureEnum.FromValue(package.MassUnit);
            DistanceUnit = UnitOfMeasureEnum.FromValue(package.DistanceUnit);
            Volume       = package.Volume;

            CreatedAt = package.CreateAt;
            UpdatedAt = package.UpdateAt;

        }
        public string            Id           { get; set; }
        public string            Name         { get; set; }
        public string?           Description  { get; set; }
        public UnitOfMeasureEnum MassUnit     { get; set; }
        public UnitOfMeasureEnum DistanceUnit { get; set; }
        public Volume            Volume       { get; set; }

        public DateTimeOffset  CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}