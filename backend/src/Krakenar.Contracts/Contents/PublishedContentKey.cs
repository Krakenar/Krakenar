namespace Krakenar.Contracts.Contents;

public record PublishedContentKey
{
  public string ContentType { get; set; }
  public string UniqueName { get; set; }
  public string? Language { get; set; }

  public PublishedContentKey() : this(string.Empty, string.Empty)
  {
  }

  public PublishedContentKey(string contentType, string uniqueName, string? language = null)
  {
    ContentType = contentType;
    UniqueName = uniqueName;
    Language = language;
  }
}
