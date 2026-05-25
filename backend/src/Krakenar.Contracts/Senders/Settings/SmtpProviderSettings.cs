namespace Krakenar.Contracts.Senders.Settings;

public record SmtpProviderSettings : ISmtpProviderSettings
{
  public string Host { get; set; }
  public ushort Port { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }

  public SmtpProviderSettings() : this(string.Empty, 0, string.Empty, string.Empty)
  {
  }

  [JsonConstructor]
  public SmtpProviderSettings(string host, ushort port, string username, string password)
  {
    Host = host;
    Port = port;
    Username = username;
    Password = password;
  }

  public SmtpProviderSettings(ISmtpProviderSettings smtp) : this(smtp.Host, smtp.Port, smtp.Username, smtp.Password)
  {
  }
}
