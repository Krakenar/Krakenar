namespace Krakenar.Contracts.Roles;

public record UpdateRolePayload
{
  public string? UniqueName { get; set; }
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
}
