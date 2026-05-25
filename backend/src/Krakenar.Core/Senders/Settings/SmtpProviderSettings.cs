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
  public SmtpSecurityMode Security { get; }
  public string Username { get; }
  public string Password { get; }

  [JsonConstructor]
  public SmtpProviderSettings(string host, ushort port, SmtpSecurityMode security, string username, string password)
  {
    Host = host.Trim();
    Port = port;
    Security = security;
    Username = username.Trim();
    Password = password.Trim();
    new SmtpProviderSettingsValidator().ValidateAndThrow(this);
  }

  public SmtpProviderSettings(ISmtpProviderSettings smtp) : this(smtp.Host, smtp.Port, smtp.Security, smtp.Username, smtp.Password)
  {
  }
}
