using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

public class SlugConverter : JsonConverter<Slug>
{
  public override Slug? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Slug.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Slug slug, JsonSerializerOptions options)
  {
    writer.WriteStringValue(slug.Value);
  }
}
