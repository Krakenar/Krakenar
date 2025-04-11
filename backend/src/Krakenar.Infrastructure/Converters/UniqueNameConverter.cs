using Krakenar.Core;
using Krakenar.Core.Settings;

namespace Krakenar.Infrastructure.Converters;

public class UniqueNameConverter : JsonConverter<UniqueName>
{
  private readonly UniqueNameSettings _settings = new(allowedCharacters: null);

  public override UniqueName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : new UniqueName(_settings, value);
  }

  public override void Write(Utf8JsonWriter writer, UniqueName uniqueName, JsonSerializerOptions options)
  {
    writer.WriteStringValue(uniqueName.Value);
  }
}
