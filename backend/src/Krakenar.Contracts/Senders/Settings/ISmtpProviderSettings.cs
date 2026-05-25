namespace Krakenar.Contracts.Senders.Settings;

public interface ISmtpProviderSettings
{
  string Host { get; }
  ushort Port { get; }
  string Username { get; }
  string Password { get; }
  // TODO(fpion): SSL/TLS
}
