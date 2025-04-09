namespace Krakenar.Contracts.Sessions;

public record CreateSessionPayload
{
  public Guid? Id { get; set; }

  public string User { get; set; }
  public bool IsPersistent { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public CreateSessionPayload() : this(string.Empty)
  {
  }

  public CreateSessionPayload(string user, bool isPersistent = false, IEnumerable<CustomAttribute>? customAttributes = null)
  {
    User = user;
    IsPersistent = isPersistent;

    if (customAttributes is not null)
    {
      CustomAttributes.AddRange(customAttributes);
    }
  }
}
