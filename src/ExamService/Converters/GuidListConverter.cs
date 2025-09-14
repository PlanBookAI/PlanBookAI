using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExamService.Converters
{
    public class GuidListConverter : JsonConverter<List<Guid>>
    {
        public override List<Guid> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected start of array");
            }

            var guidList = new List<Guid>();
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return guidList;
                }

                if (reader.TokenType == JsonTokenType.String)
                {
                    var stringValue = reader.GetString();
                    if (Guid.TryParse(stringValue, out var guid))
                    {
                        guidList.Add(guid);
                    }
                    else
                    {
                        throw new JsonException($"Invalid GUID format: {stringValue}");
                    }
                }
            }

            throw new JsonException("Unexpected end of JSON");
        }

        public override void Write(Utf8JsonWriter writer, List<Guid> value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var guid in value)
            {
                writer.WriteStringValue(guid.ToString());
            }
            writer.WriteEndArray();
        }
    }
}
