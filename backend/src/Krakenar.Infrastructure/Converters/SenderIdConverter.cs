using Krakenar.Core.Senders;

namespace Krakenar.Infrastructure.Converters;

public class SenderIdConverter : JsonConverter<SenderId>
{
  public override SenderId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? new SenderId() : new(value);
  }

  public override void Write(Utf8JsonWriter writer, SenderId id, JsonSerializerOptions options)
  {
    writer.WriteStringValue(id.Value);
  }
}
