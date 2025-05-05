using Krakenar.Core.ApiKeys;

namespace Krakenar.Infrastructure.Converters;

public class ApiKeyIdConverter : JsonConverter<ApiKeyId>
{
  public override ApiKeyId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new ApiKeyId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, ApiKeyId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
