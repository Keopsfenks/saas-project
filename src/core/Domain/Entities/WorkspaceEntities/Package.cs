using Domain.Abstractions;
using Domain.Enums;
using Domain.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.WorkspaceEntities
{

    public sealed class Volume
    {
        public decimal Height { get; set; }
        public decimal Width  { get; set; }
        public decimal Lenght { get; set; }

        public decimal Desi   => (Width * Lenght * Height) / 3000;
        public decimal Weight { get; set; }
    }


    public sealed class Package : WorkspaceEntity
    {
        public string  Name        { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        [BsonSerializer(typeof(SmartEnumBsonSerializer<UnitOfMeasureEnum>))]
        public int MassUnit => UnitOfMeasureEnum.Kilogram;
        [BsonSerializer(typeof(SmartEnumBsonSerializer<UnitOfMeasureEnum>))]
        public int DistanceUnit => UnitOfMeasureEnum.Centimeter;

        public Volume Volume { get; set; } = new();
    }
}