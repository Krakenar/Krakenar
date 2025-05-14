using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Contracts.Fields;

public record CreateOrReplaceFieldTypePayload
{
  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public BooleanSettings? Boolean { get; set; }
  public DateTimeSettings? DateTime { get; set; }
  public NumberSettings? Number { get; set; }
  public RelatedContentSettings? RelatedContent { get; set; }
  public RichTextSettings? RichText { get; set; }
  public SelectSettings? Select { get; set; }
  public StringSettings? String { get; set; }
  public TagsSettings? Tags { get; set; }

  public CreateOrReplaceFieldTypePayload() : this(string.Empty)
  {
  }

  public CreateOrReplaceFieldTypePayload(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
