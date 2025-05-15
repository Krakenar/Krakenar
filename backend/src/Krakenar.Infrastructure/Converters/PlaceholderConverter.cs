using Krakenar.Core.Fields;

namespace Krakenar.Infrastructure.Converters;

public class PlaceholderConverter : JsonConverter<Placeholder>
{
  public override Placeholder? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Placeholder.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Placeholder placeholder, JsonSerializerOptions options)
  {
    writer.WriteStringValue(placeholder.Value);
  }
}
