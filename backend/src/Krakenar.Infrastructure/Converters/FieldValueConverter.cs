using Krakenar.Core.Fields;

namespace Krakenar.Infrastructure.Converters;

public class FieldValueConverter : JsonConverter<FieldValue>
{
  public override FieldValue? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new FieldValue(value);
  }

  public override void Write(Utf8JsonWriter writer, FieldValue fieldValue, JsonSerializerOptions options)
  {
    writer.WriteStringValue(fieldValue.Value);
  }
}
