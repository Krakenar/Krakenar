using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Templates;

public class Template : Aggregate
{
  public Realm? Realm { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public string Subject { get; set; } = string.Empty;
  public Content Content { get; set; } = new();

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
