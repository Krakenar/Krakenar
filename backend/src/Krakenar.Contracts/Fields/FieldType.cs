using Krakenar.Contracts.Fields.Settings;
using Krakenar.Contracts.Realms;

namespace Krakenar.Contracts.Fields;

public class FieldType : Aggregate
{
  public Realm? Realm { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public DataType DataType { get; set; }
  public BooleanSettings? Boolean { get; set; }
  public DateTimeSettings? DateTime { get; set; }
  public NumberSettings? Number { get; set; }
  public RelatedContentSettings? RelatedContent { get; set; }
  public RichTextSettings? RichText { get; set; }
  public SelectSettings? Select { get; set; }
  public StringSettings? String { get; set; }
  public TagsSettings? Tags { get; set; }

  public FieldType() : this(string.Empty)
  {
  }

  public FieldType(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName} | {base.ToString()}";
}
