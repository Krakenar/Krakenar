using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public record SearchContentLocalesPayload : SearchPayload
{
  public Guid? ContentTypeId { get; set; }
  public Guid? LanguageId { get; set; }

  public new List<ContentSortOption> Sort { get; set; } = [];
}
