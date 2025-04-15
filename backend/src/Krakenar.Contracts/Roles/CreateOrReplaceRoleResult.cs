namespace Krakenar.Contracts.Roles;

public record CreateOrReplaceRoleResult
{
  public Role? Role { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceRoleResult()
  {
  }

  public CreateOrReplaceRoleResult(Role? role, bool created)
  {
    Role = role;
    Created = created;
  }
}
