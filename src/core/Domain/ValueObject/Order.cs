using Domain.Enums;
using Domain.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.ValueObject
{
    public sealed class Order
    {
        public string ReferenceId     { get; set; } = string.Empty;
        public string BillOfLandingId { get; set; } = string.Empty;
        public string Description     { get; set; } = string.Empty;
        [BsonSerializer(typeof(SmartEnumBsonSerializer<CodEnum>))]
        public int             IsCod           { get; set; } = CodEnum.NOT_COD;
        public decimal             CodAmount       { get; set; } = 0;
        [BsonSerializer(typeof(SmartEnumBsonSerializer<ShippingProviderEnum>))]
        public int ShipmentService { get; set; } = ShipmentServiceEnum.Standard;

    }
}