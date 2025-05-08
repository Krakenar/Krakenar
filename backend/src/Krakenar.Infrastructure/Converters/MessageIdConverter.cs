using Krakenar.Core.Messages;

namespace Krakenar.Infrastructure.Converters;

public class MessageIdConverter : JsonConverter<MessageId>
{
  public override MessageId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new MessageId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, MessageId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
