using Krakenar.Core.Templates;

namespace Krakenar.Infrastructure.Converters;

public class TemplateIdConverter : JsonConverter<TemplateId>
{
  public override TemplateId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new TemplateId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, TemplateId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
