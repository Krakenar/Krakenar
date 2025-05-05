using Krakenar.Core.Passwords;

namespace Krakenar.Infrastructure.Converters;

public class OneTimePasswordIdConverter : JsonConverter<OneTimePasswordId>
{
  public override OneTimePasswordId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new OneTimePasswordId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, OneTimePasswordId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
