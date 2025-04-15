namespace Krakenar.Contracts.Roles;

public record RoleChange
{
  public string Role { get; set; }
  public CollectionAction Action { get; set; }

  public RoleChange() : this(string.Empty)
  {
  }

  public RoleChange(string role, CollectionAction action = CollectionAction.Add)
  {
    Role = role;
    Action = action;
  }
}
