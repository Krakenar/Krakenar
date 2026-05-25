namespace Krakenar.Contracts.Senders.Settings;

public interface ISmtpProviderSettings
{
  string Host { get; }
  ushort Port { get; }
  SmtpSecurityMode Security { get; }
  string Username { get; }
  string Password { get; }
}
