using Krakenar.Core.Realms;

namespace Krakenar.Infrastructure.Converters;

public class RealmIdConverter : JsonConverter<RealmId>
{
  public override RealmId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RealmId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, RealmId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
