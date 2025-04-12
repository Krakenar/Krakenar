using Krakenar.Core;

namespace Krakenar.Infrastructure.Converters;

public class IdentifierConverter : JsonConverter<Identifier>
{
  public override Identifier? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Identifier.TryCreate(reader.GetString());
  }

  public override Identifier ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Read(ref reader, typeToConvert, options) ?? throw new InvalidOperationException("The identifier could not be read.");
  }

  public override void Write(Utf8JsonWriter writer, Identifier identifier, JsonSerializerOptions options)
  {
    writer.WriteStringValue(identifier.Value);
  }

  public override void WriteAsPropertyName(Utf8JsonWriter writer, [DisallowNull] Identifier identifier, JsonSerializerOptions options)
  {
    writer.WritePropertyName(identifier.Value);
  }
}
