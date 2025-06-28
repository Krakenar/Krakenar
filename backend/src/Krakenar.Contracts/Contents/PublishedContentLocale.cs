using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Localization;

namespace Krakenar.Contracts.Contents;

public record PublishedContentLocale
{
  public PublishedContent Content { get; set; }
  public Language? Language { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];

  public long Version { get; set; }
  public Actor PublishedBy { get; set; } = new();
  public DateTime PublishedOn { get; set; }

  public PublishedContentLocale()
  {
    Content = new PublishedContent(this);
  }

  public PublishedContentLocale(PublishedContent content)
  {
    Content = content;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName}";
}
