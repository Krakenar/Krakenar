using Krakenar.Core.Localization;

namespace Krakenar.Infrastructure.Converters;

public class LocaleConverter : JsonConverter<Locale>
{
  public override Locale? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new Locale(value);
  }

  public override void Write(Utf8JsonWriter writer, Locale locale, JsonSerializerOptions options)
  {
    writer.WriteStringValue(locale.Code);
  }
}
