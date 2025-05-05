using Krakenar.Contracts.Roles;

namespace Krakenar.Contracts.ApiKeys;

public record UpdateApiKeyPayload
{
  public string? Name { get; set; }
  public Change<string>? Description { get; set; }
  public DateTime? ExpiresOn { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];
  public List<RoleChange> Roles { get; set; } = [];
}
