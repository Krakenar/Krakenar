using Krakenar.Contracts.Fields;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Logitar;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class FieldIndex
{
  public const int MaximumLength = byte.MaxValue;

  public int FieldIndexId { get; private set; }

  public ContentType? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public Guid ContentTypeUid { get; private set; }
  public string ContentTypeName { get; private set; } = string.Empty;

  public Language? Language { get; private set; }
  public int? LanguageId { get; private set; }
  public Guid? LanguageUid { get; private set; }
  public string? LanguageCode { get; private set; }
  public bool LanguageIsDefault { get; private set; }

  public FieldType? FieldType { get; private set; }
  public int FieldTypeId { get; private set; }
  public Guid FieldTypeUid { get; private set; }
  public string FieldTypeName { get; private set; } = string.Empty;

  public FieldDefinition? FieldDefinition { get; private set; }
  public int FieldDefinitionId { get; private set; }
  public Guid FieldDefinitionUid { get; private set; }
  public string FieldDefinitionName { get; private set; } = string.Empty;

  public Content? Content { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }

  public ContentLocale? ContentLocale { get; private set; }
  public int ContentLocaleId { get; private set; }
  public string ContentLocaleName { get; private set; } = string.Empty;

  public long Version { get; private set; }
  public ContentStatus Status { get; private set; }

  public bool? Boolean { get; private set; }
  public DateTime? DateTime { get; private set; }
  public double? Number { get; private set; }
  public string? RelatedContent { get; private set; }
  public string? RichText { get; private set; }
  public string? Select { get; private set; }
  public string? String { get; private set; }
  public string? Tags { get; private set; }

  public FieldIndex(
    ContentType contentType,
    Language? language,
    FieldType fieldType,
    FieldDefinition fieldDefinition,
    Content content,
    ContentLocale contentLocale,
    ContentStatus status,
    string value)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;
    ContentTypeUid = contentType.Id;
    ContentTypeName = contentType.UniqueNameNormalized;

    if (language is not null)
    {
      Language = language;
      LanguageId = language.LanguageId;
      LanguageUid = language.Id;
      LanguageCode = language.CodeNormalized;
      LanguageIsDefault = language.IsDefault;
    }

    FieldType = fieldType;
    FieldTypeId = fieldType.FieldTypeId;
    FieldTypeUid = fieldType.Id;
    FieldTypeName = fieldType.UniqueNameNormalized;

    FieldDefinition = fieldDefinition;
    FieldDefinitionId = fieldDefinition.FieldDefinitionId;
    FieldDefinitionUid = fieldDefinition.Id;
    FieldDefinitionName = fieldDefinition.UniqueNameNormalized;

    Content = content;
    ContentId = content.ContentId;
    ContentUid = content.Id;

    ContentLocale = contentLocale;
    ContentLocaleId = contentLocale.ContentLocaleId;
    ContentLocaleName = contentLocale.UniqueNameNormalized;

    Status = status;

    long version = (status == ContentStatus.Published ? contentLocale.PublishedVersion : null) ?? contentLocale.Version;
    Update(version, value);
  }

  private FieldIndex()
  {
  }

  public void Update(long version, string value)
  {
    if (FieldType == null)
    {
      throw new InvalidOperationException($"The {nameof(FieldType)} is required.");
    }

    Version = version;

    switch (FieldType.DataType)
    {
      case DataType.Boolean:
        Boolean = bool.Parse(value);
        break;
      case DataType.DateTime:
        DateTime = System.DateTime.Parse(value);
        break;
      case DataType.Number:
        Number = double.Parse(value);
        break;
      case DataType.RelatedContent:
        RelatedContent = value;
        break;
      case DataType.RichText:
        RichText = value;
        break;
      case DataType.Select:
        Select = value;
        break;
      case DataType.String:
        String = value.Truncate(MaximumLength);
        break;
      case DataType.Tags:
        Tags = value;
        break;
      default:
        throw new DataTypeNotSupportedException(FieldType.DataType);
    }
  }
}
