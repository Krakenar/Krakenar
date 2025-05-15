namespace Krakenar.Contracts.Contents;

public record CreateOrReplaceContentTypeResult
{
  public ContentType? ContentType { get; set; }
  public bool Created { get; set; }

  public CreateOrReplaceContentTypeResult()
  {
  }

  public CreateOrReplaceContentTypeResult(ContentType? contentType, bool created)
  {
    ContentType = contentType;
    Created = created;
  }
}
