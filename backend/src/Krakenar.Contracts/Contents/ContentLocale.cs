using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Localization;

namespace Krakenar.Contracts.Contents;

public record ContentLocale
{
  public Content Content { get; set; }
  public Language? Language { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];

  public long Version { get; set; }
  public Actor CreatedBy { get; set; } = new();
  public DateTime CreatedOn { get; set; }
  public Actor UpdatedBy { get; set; } = new();
  public DateTime UpdatedOn { get; set; }

  public bool IsPublished { get; set; }
  public long? PublishedVersion { get; set; }
  public Actor? PublishedBy { get; set; }
  public DateTime? PublishedOn { get; set; }

  public ContentLocale()
  {
    Content = new Content(this);
  }

  public ContentLocale(Content content)
  {
    Content = content;
  }
}
