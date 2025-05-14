using FluentValidation;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Core.Fields.Validators;

namespace Krakenar.Core.Fields.Settings;

public record RelatedContentSettings : FieldTypeSettings, IRelatedContentSettings
{
  public override DataType DataType => DataType.RelatedContent;

  public Guid ContentTypeId { get; }
  public bool IsMultiple { get; }

  [JsonConstructor]
  public RelatedContentSettings(Guid contentTypeId, bool isMultiple = false)
  {
    ContentTypeId = contentTypeId;
    IsMultiple = isMultiple;
    new RelatedContentSettingsValidator().ValidateAndThrow(this);
  }

  public RelatedContentSettings(IRelatedContentSettings relatedContent) : this(relatedContent.ContentTypeId, relatedContent.IsMultiple)
  {
  }
}
