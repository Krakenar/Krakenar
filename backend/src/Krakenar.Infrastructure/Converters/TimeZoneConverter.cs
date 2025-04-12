using TimeZone = Krakenar.Core.Localization.TimeZone;

namespace Krakenar.Infrastructure.Converters;

public class TimeZoneConverter : JsonConverter<TimeZone>
{
  public override TimeZone? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return TimeZone.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, TimeZone timeZone, JsonSerializerOptions options)
  {
    writer.WriteStringValue(timeZone.Id);
  }
}
