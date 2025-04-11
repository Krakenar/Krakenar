using Krakenar.Core.Roles;

namespace Krakenar.Infrastructure.Converters;

public class RoleIdConverter : JsonConverter<RoleId>
{
  public override RoleId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new RoleId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, RoleId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
