using Domain.Abstractions;
using Domain.Enums;
using Domain.Serializers;
using Domain.ValueObject;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Shipment : WorkspaceEntity
    {
        [BsonSerializer(typeof(SmartEnumBsonSerializer<ShipmentTypeEnum>))]
        public int Type { get; set; } = ShipmentTypeEnum.Order;

        [BsonSerializer(typeof(SmartEnumBsonSerializer<CargoStatusEnum>))]
        public int Status { get;          set; } = CargoStatusEnum.DRAFT;
        public string  CargoId     { get; set; } = string.Empty;
        public int     InvoiceId   { get; set; }
        public int     WaybillId   { get; set; }
        public string  Name        { get; set; } = string.Empty;
        public string? Description { get; set; }

        public Dispatch  Dispatch  { get; set; } = new Dispatch();
        public CargoList Cargo     { get; set; } = new CargoList();
        public Member    Recipient { get; set; } = new Member();
        public Member    Shipper   { get; set; } = new Member();

        public string                     ProviderId   { get; set; } = string.Empty;
        public Provider                   Provider     { get; set; } = new Provider();
        public Dictionary<string, string> ProviderInfo { get; set; } = new Dictionary<string, string>();
    }
}