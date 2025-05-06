using FluentValidation;
using Krakenar.Contracts.Senders;
using Krakenar.Core.Senders.Validators;

namespace Krakenar.Core.Senders.Settings;

public record SendGridSettings : SenderSettings, ISendGridSettings
{
  public override SenderProvider Provider => SenderProvider.SendGrid;

  public string ApiKey { get; }

  [JsonConstructor]
  public SendGridSettings(string apiKey)
  {
    ApiKey = apiKey.Trim();
    new SendGridSettingsValidator().ValidateAndThrow(this);
  }

  public SendGridSettings(ISendGridSettings sendGrid) : this(sendGrid.ApiKey)
  {
  }
}
