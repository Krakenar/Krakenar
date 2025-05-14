
namespace Krakenar.Contracts.Fields.Settings;

public record RelatedContentSettings : IRelatedContentSettings
{
  public Guid ContentTypeId { get; set; }
  public bool IsMultiple { get; set; }

  public RelatedContentSettings()
  {
  }

  [JsonConstructor]
  public RelatedContentSettings(Guid contentTypeId, bool isMultiple = false)
  {
    ContentTypeId = contentTypeId;
    IsMultiple = isMultiple;
  }

  public RelatedContentSettings(IRelatedContentSettings relatedContent) : this(relatedContent.ContentTypeId, relatedContent.IsMultiple)
  {
  }
}
