using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Templates;

public record SearchTemplatesPayload : SearchPayload
{
  public string? ContentType { get; set; }

  public new List<TemplateSortOption> Sort { get; set; } = [];
}
