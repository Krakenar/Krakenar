namespace Krakenar.Contracts.Passwords;

public record ValidateOneTimePasswordPayload
{
  public string Password { get; set; }
  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public ValidateOneTimePasswordPayload() : this(string.Empty)
  {
  }

  public ValidateOneTimePasswordPayload(string password, IEnumerable<CustomAttribute>? customAttributes = null)
  {
    Password = password;

    if (customAttributes is not null)
    {
      CustomAttributes.AddRange(customAttributes);
    }
  }
}
