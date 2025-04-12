using Krakenar.Core.Users;

namespace Krakenar.Infrastructure.Converters;

public class UserIdConverter : JsonConverter<UserId>
{
  public override UserId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new UserId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, UserId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
