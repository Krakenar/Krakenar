namespace Krakenar.Contracts.Senders.Settings;

public record SendGridSettings : ISendGridSettings
{
  public string ApiKey { get; set; }

  public SendGridSettings() : this(string.Empty)
  {
  }

  [JsonConstructor]
  public SendGridSettings(string apiKey)
  {
    ApiKey = apiKey;
  }

  public SendGridSettings(ISendGridSettings sendGrid) : this(sendGrid.ApiKey)
  {
  }
}
