namespace Krakenar.Contracts.Sessions;

public record SignInSessionPayload
{
  public Guid? Id { get; set; }

  public string UniqueName { get; set; }
  public string Password { get; set; }
  public bool IsPersistent { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public SignInSessionPayload() : this(string.Empty, string.Empty)
  {
  }

  public SignInSessionPayload(string uniqueName, string password, bool isPersistent = false, IEnumerable<CustomAttribute>? customAttributes = null)
  {
    UniqueName = uniqueName;
    Password = password;
    IsPersistent = isPersistent;

    if (customAttributes is not null)
    {
      CustomAttributes.AddRange(customAttributes);
    }
  }
}
