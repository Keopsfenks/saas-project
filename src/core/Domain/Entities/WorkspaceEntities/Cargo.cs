using Domain.Abstractions;
using Domain.Enums;
using Domain.Serializers;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Cargo : WorkspaceEntity
    {
        public string  Name        { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        [BsonSerializer(typeof(SmartEnumBsonSerializer<UnitOfMeasureEnum>))]
        public int MassUnit     { get; set; } = UnitOfMeasureEnum.Kilogram;
        [BsonSerializer(typeof(SmartEnumBsonSerializer<UnitOfMeasureEnum>))]
        public int DistanceUnit { get; set; } = UnitOfMeasureEnum.Centimeter;

        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Width  { get; set; }
        public decimal Volume => Width  * Length * Height;
        public decimal Desi   => Volume / 3000;
        public decimal Weight { get; set; } = 0;
    }
}