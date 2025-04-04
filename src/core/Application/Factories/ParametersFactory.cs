using Domain.Enums;
using MongoDB.Bson;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Application.Factories
{
    public static class ParametersFactory
    {
        public static BsonDocument? Serialize(object? parameters)
        {
            if (parameters == null)
                return null;

            BsonDocument bsonDocument = new BsonDocument();

            var json = JsonSerializer.Serialize(parameters);

            bsonDocument.AddRange(BsonDocument.Parse(json));

            return bsonDocument;
        }

        public static T? Deserialize<T>(BsonDocument? bsonDocument) where T : class
        {
            if (bsonDocument == null)
                return null;

            var json = bsonDocument.ToJson();

            return JsonSerializer.Deserialize<T>(json);
        }

        public static string CreateId(string prefix)
        {

            string id = $"{prefix}_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            return id;
        }

        public static int CreateNumber()
        {
            Random random = new Random();

            return random.Next(100000, 1000000);
        }

        public static bool Validate<T>(BsonDocument? bsonDocument) where T : class
        {
            if (bsonDocument == null)
                return false;

            var obj = Deserialize<T>(bsonDocument);
            if (obj == null)
                return false;

            if (!HasValidProperties(obj))
                return false;

            return true;
        }

        private static bool HasValidProperties<T>(T obj) where T : class
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (IsNullable(property))
                    continue;

                var value = property.GetValue(obj);
                if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool IsNullable(PropertyInfo property)
        {
            if (!property.PropertyType.IsValueType)
                return true;

            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                return true;

            return false;
        }


    }
}