using FluentValidation;
using Krakenar.Contracts.Senders;
using Krakenar.Contracts.Senders.Settings;
using Krakenar.Core.Senders.Validators;

namespace Krakenar.Core.Senders.Settings;

public record SmtpProviderSettings : SenderSettings, ISmtpProviderSettings
{
  public override SenderProvider Provider { get; } = SenderProvider.SmtpProvider;

  public string Host { get; }
  public ushort Port { get; }
  public string Username { get; }
  public string Password { get; }

  [JsonConstructor]
  public SmtpProviderSettings(string host, ushort port, string username, string password)
  {
    Host = host.Trim();
    Port = port;
    Username = username.Trim();
    Password = password.Trim();
    new SmtpProviderSettingsValidator().ValidateAndThrow(this);
  }

  public SmtpProviderSettings(ISmtpProviderSettings smtp) : this(smtp.Host, smtp.Port, smtp.Username, smtp.Password)
  {
  }
}
