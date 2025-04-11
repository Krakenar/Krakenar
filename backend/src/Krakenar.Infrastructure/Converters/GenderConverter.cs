using Krakenar.Core.Users;

namespace Krakenar.Infrastructure.Converters;

public class GenderConverter : JsonConverter<Gender>
{
  public override Gender? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    return Gender.TryCreate(reader.GetString());
  }

  public override void Write(Utf8JsonWriter writer, Gender gender, JsonSerializerOptions options)
  {
    writer.WriteStringValue(gender.Value);
  }
}
