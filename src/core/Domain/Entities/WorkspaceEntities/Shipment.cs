using Domain.Abstractions;
using Domain.Enums;
using Domain.Serializers;
using Domain.ValueObject;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Shipment : WorkspaceEntity
    {
        public string  Name        { get; set; } = string.Empty;
        public string? Description { get; set; } = null;

        [BsonSerializer(typeof(SmartEnumBsonSerializer<CargoStatusEnum>))]
        public int Status { get; set; } = CargoStatusEnum.DRAFT;

        public Order           Order     { get; set; } = new Order();
        public List<CargoList> Cargo     { get; set; } = new List<CargoList>();
        public Member          Recipient { get; set; } = new Member();
        public Member          Shipper   { get; set; } = new Member();

        public BsonDocument? OrderDetail { get; set; } = new BsonDocument();

        [BsonSerializer(typeof(SmartEnumBsonSerializer<ShippingProviderEnum>))]
        public int ShippingProviderCode { get; set; } = ShippingProviderEnum.None;
        public string ProviderId { get; set; } = string.Empty;
        [BsonIgnore]
        public Provider Provider   { get; set; } = new Provider();
    }
}