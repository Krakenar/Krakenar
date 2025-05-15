using Krakenar.Core.Fields;

namespace Krakenar.Infrastructure.Converters;

public class FieldTypeIdConverter : JsonConverter<FieldTypeId>
{
  public override FieldTypeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new FieldTypeId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, FieldTypeId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
