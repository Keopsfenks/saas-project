using Domain.Abstractions;
using Domain.Enums;
using Domain.Serializers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Provider : WorkspaceEntity
    {
        public string               Username         { get; set; } = string.Empty;
        public string               Password         { get; set; } = string.Empty;
        [BsonSerializer(typeof(SmartEnumBsonSerializer<ShippingProviderEnum>))]
        public int ShippingProvider { get; set; } = ShippingProviderEnum.None;

        public BsonDocument? Parameters { get; set; }
        public BsonDocument? Session    { get; set; }

    }
}