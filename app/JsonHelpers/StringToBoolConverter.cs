using Newtonsoft.Json;

namespace GHelper.JsonHelpers;

public class StringToBoolConverter : JsonConverter<bool>
{
    public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
    {
        writer.WriteValue(value ? "1" : "0");
    }

    public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        return reader.Value?.ToString() == "1";
    }
}