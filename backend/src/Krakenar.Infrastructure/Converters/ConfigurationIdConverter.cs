using Krakenar.Core.Configurations;
using Logitar.EventSourcing;

namespace Krakenar.Infrastructure.Converters;

public class ConfigurationIdConverter : JsonConverter<ConfigurationId>
{
  public override ConfigurationId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ConfigurationId() : new(new StreamId(value));
  }

  public override void Write(Utf8JsonWriter writer, ConfigurationId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
