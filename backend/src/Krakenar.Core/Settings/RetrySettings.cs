using Microsoft.Extensions.Configuration;

namespace Krakenar.Core.Settings;

public record RetrySettings
{
  public const string SectionKey = "Retry";

  public RetryAlgorithm Algorithm { get; set; }
  public int Delay { get; set; }
  public int ExponentialBase { get; set; }
  public int RandomVariation { get; set; }

  public int MaximumRetries { get; set; }
  public int MaximumDelay { get; set; }

  public static RetrySettings Initialize(IConfiguration configuration)
  {
    RetrySettings settings = configuration.GetSection(SectionKey).Get<RetrySettings>() ?? new();

    string? algorithmValue = Environment.GetEnvironmentVariable("RETRY_ALGORITHM");
    if (!string.IsNullOrWhiteSpace(algorithmValue) && Enum.TryParse(algorithmValue, out RetryAlgorithm algorithm))
    {
      settings.Algorithm = algorithm;
    }

    settings.Delay = EnvironmentHelper.GetInt32("RETRY_DELAY", settings.Delay);
    settings.ExponentialBase = EnvironmentHelper.GetInt32("RETRY_EXPONENTIAL_BASE", settings.ExponentialBase);
    settings.RandomVariation = EnvironmentHelper.GetInt32("RETRY_RANDOM_VARIATION", settings.RandomVariation);

    settings.MaximumRetries = EnvironmentHelper.GetInt32("RETRY_MAXIMUM_RETRIES", settings.MaximumRetries);
    settings.MaximumDelay = EnvironmentHelper.GetInt32("RETRY_MAXIMUM_DELAY", settings.MaximumDelay);

    return settings;
  }
}
