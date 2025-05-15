namespace Krakenar.Contracts.Fields.Settings;

public record StringSettings : IStringSettings
{
  public int? MinimumLength { get; set; }
  public int? MaximumLength { get; set; }
  public string? Pattern { get; set; }

  public StringSettings()
  {
  }

  [JsonConstructor]
  public StringSettings(int? minimumLength, int? maximumLength, string? pattern)
  {
    MinimumLength = minimumLength;
    MaximumLength = maximumLength;
    Pattern = pattern;
  }

  public StringSettings(IStringSettings @string) : this(@string.MinimumLength, @string.MaximumLength, @string.Pattern)
  {
  }
}
