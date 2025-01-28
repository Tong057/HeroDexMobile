using HeroDex.Models.Enums;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class HeroTypeConverter : JsonConverter<HeroType>
{
    public override HeroType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string enumValue = reader.GetString();
        if (Enum.TryParse<HeroType>(enumValue, true, out var result))
        {
            return result;
        }

        throw new JsonException($"Unable to convert \"{enumValue}\" to {nameof(HeroType)}.");
    }

    public override void Write(Utf8JsonWriter writer, HeroType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToUpper());
    }
}
