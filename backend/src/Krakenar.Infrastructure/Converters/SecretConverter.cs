using Krakenar.Core.Tokens;

namespace Krakenar.Infrastructure.Converters;

public class SecretConverter : JsonConverter<Secret>
{
  public override Secret? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new Secret(value);
  }

  public override void Write(Utf8JsonWriter writer, Secret secret, JsonSerializerOptions options)
  {
    writer.WriteStringValue(secret.Value);
  }
}
