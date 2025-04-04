using Domain.Enums;
using Domain.Serializers;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.ValueObject
{
    public sealed class Dispatch
    {
        [BsonSerializer(typeof(SmartEnumBsonSerializer<CodEnum>))]
        public int IsCod { get; set; } = CodEnum.NOT_COD;
        [BsonSerializer(typeof(SmartEnumBsonSerializer<PackagingTypeEnum>))]
        public int PackagingType { get; set; } = PackagingTypeEnum.Box;
        [BsonSerializer(typeof(SmartEnumBsonSerializer<PaymentTypeEnum>))]
        public int PaymentType { get; set; } = PaymentTypeEnum.Sender;
        public decimal  CodPrice      { get; set; } = 0;
    }
}