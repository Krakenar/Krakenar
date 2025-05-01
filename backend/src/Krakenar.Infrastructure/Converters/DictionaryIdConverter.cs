using Krakenar.Core.Dictionaries;

namespace Krakenar.Infrastructure.Converters;

public class DictionaryIdConverter : JsonConverter<DictionaryId>
{
  public override DictionaryId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new DictionaryId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, DictionaryId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
