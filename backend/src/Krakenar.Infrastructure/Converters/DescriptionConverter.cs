using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

public class DescriptionConverter : JsonConverter<Description>
{
  public override Description? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Description.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Description description, JsonSerializerOptions options)
  {
    writer.WriteStringValue(description.Value);
  }
}
