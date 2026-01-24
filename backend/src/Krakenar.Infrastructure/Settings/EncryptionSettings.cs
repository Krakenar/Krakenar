using Logitar;
using Microsoft.Extensions.Configuration;

namespace Krakenar.Infrastructure.Settings;

public record EncryptionSettings
{
  public const string SectionKey = "Encryption";

  public string Key { get; set; } = string.Empty;

  public static EncryptionSettings Initialize(IConfiguration configuration)
  {
    EncryptionSettings settings = configuration.GetSection(SectionKey).Get<EncryptionSettings>() ?? new();

    settings.Key = EnvironmentHelper.GetString("ENCRYPTION_KEY", settings.Key);

    return settings;
  }
}
