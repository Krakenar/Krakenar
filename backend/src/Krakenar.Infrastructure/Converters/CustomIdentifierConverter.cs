using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

public class CustomIdentifierConverter : JsonConverter<CustomIdentifier>
{
  public override CustomIdentifier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return CustomIdentifier.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, CustomIdentifier customidentifier, JsonSerializerOptions options)
  {
    writer.WriteStringValue(customidentifier.Value);
  }
}
