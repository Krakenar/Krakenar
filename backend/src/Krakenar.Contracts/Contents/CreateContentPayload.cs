namespace Krakenar.Contracts.Contents;

public record CreateContentPayload
{
  public Guid? Id { get; set; }

  public string ContentType { get; set; }
  public string? Language { get; set; }

  public string UniqueName { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateContentPayload() : this(string.Empty, string.Empty)
  {
  }

  public CreateContentPayload(string uniqueName, string contentType)
  {
    ContentType = contentType;

    UniqueName = uniqueName;
  }
}
