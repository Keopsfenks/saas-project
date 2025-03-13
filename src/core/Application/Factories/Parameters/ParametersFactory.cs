using Application.Factories.Interfaces;
using MongoDB.Bson;

namespace Application.Factories.Parameters
{
    public static class ParametersFactory<T> where T : IProvider
    {
        public static BsonDocument? Parameters(Dictionary<string, string>? parameters)
        {
            if (parameters == null)
                return null;

            BsonDocument parametersBsonDocument = new BsonDocument();

            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var name = property.Name;
                var type = property.PropertyType;

                if (parameters.TryGetValue(name, out var value))
                {
                    if (string.IsNullOrEmpty(value))
                        parametersBsonDocument.Add(name, BsonNull.Value);
                    else
                    {
                        if (type == typeof(string))
                            parametersBsonDocument.Add(name, value.ToString());
                        else if (type == typeof(int))
                            parametersBsonDocument.Add(name, Convert.ToInt32(value));
                        else if (type == typeof(bool))
                            parametersBsonDocument.Add(name, Convert.ToBoolean(value));
                        else if (type == typeof(double))
                            parametersBsonDocument.Add(name, Convert.ToDouble(value));
                        else if (type == typeof(decimal))
                            parametersBsonDocument.Add(name, Convert.ToDecimal(value));
                        else if (type == typeof(DateTime))
                            parametersBsonDocument.Add(name, Convert.ToDateTime(value));
                    }
                }
                else
                    throw new ArgumentException($"{name} adında bir parametre bulunamadı.");
            }
            return parametersBsonDocument;
        }

        public static bool ValidationParameters(Dictionary<string, string> parameters)
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var name = property.Name;
                if (!parameters.ContainsKey(name))
                    return false;
            }
            return true;
        }
    }
}