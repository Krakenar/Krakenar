namespace Krakenar.Client;

public interface IKrakenarSettings
{
  string? BaseUrl { get; }

  BasicCredentials? Basic { get; }

  string? Realm { get; }
  string? User { get; }
}
