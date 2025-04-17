namespace Krakenar.Contracts.Roles;

public record CreateOrReplaceRolePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public CreateOrReplaceRolePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceRolePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
