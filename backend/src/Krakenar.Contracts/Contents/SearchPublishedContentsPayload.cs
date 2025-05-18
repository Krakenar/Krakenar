using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public record SearchPublishedContentsPayload
{
  public ContentFilter Content { get; set; } = new();
  public ContentTypeFilter ContentType { get; set; } = new();
  public LanguageFilter Language { get; set; } = new();
  public TextSearch Search { get; set; } = new();

  public List<PublishedContentSortOption> Sort { get; set; } = [];

  public int Skip { get; set; }
  public int Limit { get; set; }
}
