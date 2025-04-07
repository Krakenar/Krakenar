namespace Krakenar.Contracts.Logging;

public interface ILoggingSettings
{
  LoggingExtent Extent { get; }
  bool OnlyErrors { get; }
}

public record LoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; set; } = LoggingExtent.ActivityOnly;
  public bool OnlyErrors { get; set; }

  public LoggingSettings()
  {
  }

  public LoggingSettings(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  public LoggingSettings(LoggingExtent extent, bool onlyErrors = false)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
  }
}
