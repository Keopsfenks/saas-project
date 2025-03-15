using Domain.Abstractions;
using Domain.Enums;
using Domain.ValueObject;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Shipment : WorkspaceEntity
    {
        public BsonDocument    Order  { get; set; } = new BsonDocument();

        public CargoStatusEnum Status { get; set; } = CargoStatusEnum.DRAFT;

        public CargoList Cargo { get; set; } = new CargoList();

        public Member  Recipient { get; set; } = new Member();
        public Member? Shipper   { get; set; } = null;

        public string ProviderId { get; set; } = string.Empty;
        [BsonIgnore]
        public Provider Provider   { get; set; } = null!;
    }
}