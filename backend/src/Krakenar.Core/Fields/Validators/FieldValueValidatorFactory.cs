using Krakenar.Contracts.Fields;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields.Settings;

namespace Krakenar.Core.Fields.Validators;

public interface IFieldValueValidatorFactory
{
  IFieldValueValidator Create(FieldType fieldType);
}

public class FieldValueValidatorFactory : IFieldValueValidatorFactory
{
  protected virtual IContentQuerier ContentQuerier { get; }

  public FieldValueValidatorFactory(IContentQuerier contentQuerier)
  {
    ContentQuerier = contentQuerier;
  }

  public virtual IFieldValueValidator Create(FieldType fieldType) => fieldType.DataType switch
  {
    DataType.Boolean => new BooleanValueValidator((BooleanSettings)fieldType.Settings),
    DataType.DateTime => new DateTimeValueValidator((DateTimeSettings)fieldType.Settings),
    DataType.Number => new NumberValueValidator((NumberSettings)fieldType.Settings),
    DataType.RelatedContent => new RelatedContentValueValidator(ContentQuerier, (RelatedContentSettings)fieldType.Settings),
    DataType.RichText => new RichTextValueValidator((RichTextSettings)fieldType.Settings),
    DataType.Select => new SelectValueValidator((SelectSettings)fieldType.Settings),
    DataType.String => new StringValueValidator((StringSettings)fieldType.Settings),
    DataType.Tags => new TagsValueValidator((TagsSettings)fieldType.Settings),
    _ => throw new DataTypeNotSupportedException(fieldType.DataType),
  };
}
