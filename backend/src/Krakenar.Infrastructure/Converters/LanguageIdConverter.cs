using Krakenar.Core.Localization;

namespace Krakenar.Infrastructure.Converters;

public class LanguageIdConverter : JsonConverter<LanguageId>
{
  public override LanguageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new LanguageId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, LanguageId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
