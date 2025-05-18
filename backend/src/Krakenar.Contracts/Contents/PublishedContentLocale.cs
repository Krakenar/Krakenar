using Krakenar.Contracts.Actors;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Localization;

namespace Krakenar.Contracts.Contents;

public record PublishedContentLocale
{
  public PublishedContent Content { get; set; } = new();
  public Language? Language { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public List<FieldValue> FieldValues { get; set; } = [];

  public long Revision { get; set; }
  public Actor PublishedBy { get; set; } = new();
  public DateTime PublishedOn { get; set; }

  public PublishedContentLocale() : this(string.Empty)
  {
  }

  public PublishedContentLocale(string uniqueName)
  {
    UniqueName = uniqueName;
  }

  public override string ToString() => $"{DisplayName ?? UniqueName}";
}
