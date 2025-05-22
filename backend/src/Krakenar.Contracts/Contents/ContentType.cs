using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Contents;

public class ContentType : Aggregate
{
  public Realm? Realm { get; set; }

  public bool IsInvariant { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int FieldCount { get; set; }
  public List<FieldDefinition> Fields { get; set; } = [];

  public ContentType() : this(string.Empty)
  {
  }

  public ContentType(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
