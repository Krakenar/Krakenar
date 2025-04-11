using Krakenar.Core.Passwords;

namespace Krakenar.Infrastructure.Converters;

public class PasswordConverter : JsonConverter<Password>
{
  protected virtual IPasswordService PasswordService { get; }

  public PasswordConverter(IPasswordService passwordService)
  {
    PasswordService = passwordService;
  }

  public override Password? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    string? value = reader.GetString();
    return string.IsNullOrWhiteSpace(value) ? null : PasswordService.Decode(value);
  }

  public override void Write(Utf8JsonWriter writer, Password password, JsonSerializerOptions options)
  {
    writer.WriteStringValue(password.Encode());
  }
}
