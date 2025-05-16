using Krakenar.Contracts.Localization;

namespace Krakenar.Contracts.Contents;

public record ContentLocale
{
  public Content? Content { get; set; }
  public Language? Language { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public ContentLocale() : this(string.Empty)
  {
  }

  public ContentLocale(string uniqueName)
  {
    UniqueName = uniqueName;
  }
}
