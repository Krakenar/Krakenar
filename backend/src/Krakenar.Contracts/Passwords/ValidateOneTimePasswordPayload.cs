namespace Krakenar.Contracts.Passwords;

public record ValidateOneTimePasswordPayload
{
  public string Password { get; set; } = string.Empty;
  public List<CustomAttribute> CustomAttributes { get; set; } = [];
}
