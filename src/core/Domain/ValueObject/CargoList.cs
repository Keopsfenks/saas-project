using Domain.Enums;
using Domain.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.ValueObject
{
    public sealed class CargoList
    {
        public string  Name        { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;

        [BsonSerializer(typeof(SmartEnumBsonSerializer<UnitOfMeasureEnum>))]
        public int MassUnit { get; set; } = UnitOfMeasureEnum.Kilogram;

        [BsonSerializer(typeof(SmartEnumBsonSerializer<UnitOfMeasureEnum>))]
        public int DistanceUnit { get; set; } = UnitOfMeasureEnum.Centimeter;

        public decimal Height { get; set; } = 0;
        public decimal Length { get; set; } = 0;
        public decimal Width  { get; set; } = 0;
        public decimal Volume => Width  * Length * Height;
        public decimal Desi   => Volume / 3000;

        public List<Item>? Items  { get; set; } = null;
        public decimal     Weight { get; set; } = 0;
    }
}