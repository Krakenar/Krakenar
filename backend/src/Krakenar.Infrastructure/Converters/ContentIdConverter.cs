using Krakenar.Core.Contents;

namespace Krakenar.Infrastructure.Converters;

public class ContentIdConverter : JsonConverter<ContentId>
{
  public override ContentId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ContentId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ContentId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
