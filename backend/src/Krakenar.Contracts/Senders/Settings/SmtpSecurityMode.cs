namespace Krakenar.Contracts.Senders.Settings;

public enum SmtpSecurityMode
{
  None = 0,
  Auto = 1,
  SslOnConnect = 2,
  StartTls = 3,
  StartTlsWhenAvailable = 4
}
