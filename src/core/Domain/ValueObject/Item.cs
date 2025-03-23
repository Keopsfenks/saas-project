using Domain.Enums;
using Domain.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.ValueObject
{
    public sealed class Item
    {
        public string  Name        { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}