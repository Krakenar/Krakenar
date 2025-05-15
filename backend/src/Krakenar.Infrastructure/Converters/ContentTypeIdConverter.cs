using Krakenar.Core.Contents;

namespace Krakenar.Infrastructure.Converters;

public class ContentTypeIdConverter : JsonConverter<ContentTypeId>
{
  public override ContentTypeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ContentTypeId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ContentTypeId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
