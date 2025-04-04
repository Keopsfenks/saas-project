using Ardalis.SmartEnum;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Domain.Serializers
{
    public class SmartEnumBsonSerializer<T> : SerializerBase<int> where T : SmartEnum<T>
    {
        public override int Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var bsonReader = context.Reader;
            var value = bsonReader.ReadString();


            var smartEnum = SmartEnum<T>.TryFromName(value, ignoreCase: true, out var result);

            if (result is not null)
                return result.Value;

            throw new BsonSerializationException($"Value {value} is not a valid {typeof(T).Name} value.");
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int value)
        {
            var smartEnum = SmartEnum<T>.TryFromValue(value, out var result);

            if (result is null)
                throw new BsonSerializationException($"Value {value} is not a valid {typeof(T).Name} value.");


            context.Writer.WriteString($"{result.Name}");
        }
    }
}