using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Roles;

public class Role : Aggregate
{
  public Realm? Realm { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<CustomAttribute> CustomAttributes { get; set; } = [];

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
