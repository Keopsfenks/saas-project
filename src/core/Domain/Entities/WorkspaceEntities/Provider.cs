using Domain.Abstractions;
using Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities.WorkspaceEntities
{
    public sealed class Provider : WorkspaceEntity
    {
        public string               Username         { get; set; } = string.Empty;
        public string               Password         { get; set; } = string.Empty;
        public ShippingProviderEnum ShippingProvider { get; set; } = ShippingProviderEnum.None;

        public BsonDocument? Parameters { get; set; } = null;
        [BsonIgnore]
        public BsonDocument? Session    { get; set; } = null;

    }
}