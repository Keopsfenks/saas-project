using Domain.Abstractions;
using Domain.Enums;
using Domain.Serializers;
using Domain.ValueObject;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Shipment : WorkspaceEntity
    {
        public Order        Order    { get; set; } = new Order();

        [BsonSerializer(typeof(SmartEnumBsonSerializer<CargoStatusEnum>))]
        public int Status { get; set; } = CargoStatusEnum.DRAFT;

        public CargoList Cargo { get; set; } = new CargoList();

        public Member        Recipient { get; set; } = new Member();
        public Member?       Shipper   { get; set; } = null;

        public string ProviderId { get; set; } = string.Empty;
        [BsonIgnore]
        public Provider Provider   { get; set; } = null!;
    }
}