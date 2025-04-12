using Krakenar.Core.Sessions;

namespace Krakenar.Infrastructure.Converters;

public class SessionIdConverter : JsonConverter<SessionId>
{
  public override SessionId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new SessionId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, SessionId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
