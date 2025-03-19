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
            var type = context.Reader.GetCurrentBsonType();

            if (type == BsonType.Document)
            {
                context.Reader.ReadStartDocument();

                // Read the document until we find the "Value" field
                while (context.Reader.ReadBsonType() != BsonType.EndOfDocument)
                {
                    var name = context.Reader.ReadName(new Utf8NameDecoder());
                    if (name == "Value")
                    {
                        var value = context.Reader.ReadInt32();
                        context.Reader.ReadEndDocument();
                        return value;
                    }
                    else
                    {
                        // Skip other fields
                        context.Reader.SkipValue();
                    }
                }

                context.Reader.ReadEndDocument();
                throw new BsonSerializationException("Missing 'Value' field in SmartEnum document.");
            }
            else if (type == BsonType.Int32)
            {
                // Handle case where only the value is serialized
                return context.Reader.ReadInt32();
            }

            throw new BsonSerializationException($"Cannot deserialize {typeof(T).Name} from BsonType {type}");
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, int value)
        {
            var smartEnum = SmartEnum<T>.TryFromValue(value, out var result);

            if (result is null)
                throw new BsonSerializationException($"Value {value} is not a valid {typeof(T).Name} value.");

            BsonDocument doc = new BsonDocument
            {
                { "Value", result.Value },
                { "Name", result.Name },
            };

            context.Writer.WriteStartDocument();
            context.Writer.WriteName("Name");
            context.Writer.WriteString(result.Name);
            context.Writer.WriteName("Value");
            context.Writer.WriteInt32(result.Value);
            context.Writer.WriteEndDocument();
        }
    }
}