﻿using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.EntityFrameworkCore.Relational.KrakenarDb;
using Logitar;

namespace Krakenar.EntityFrameworkCore.Relational.Entities;

public sealed class UniqueIndex : ISegregatedEntity
{
  public const char KeySeparator = '|';
  public const int MaximumLength = byte.MaxValue;

  public int UniqueIndexId { get; private set; }

  public Realm? Realm { get; private set; }
  public int? RealmId { get; private set; }
  public Guid? RealmUid { get; private set; }

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

  public long Version { get; private set; }
  public ContentStatus Status { get; private set; }

  public string Value { get; private set; } = string.Empty;
  public string ValueNormalized
  {
    get => Helper.Normalize(Value);
    private set { }
  }

  public string Key
  {
    get => CreateKey(FieldDefinitionUid, ValueNormalized);
    private set { }
  }

  public Content? Content { get; private set; }
  public int ContentId { get; private set; }
  public Guid ContentUid { get; private set; }

  public ContentLocale? ContentLocale { get; private set; }
  public int ContentLocaleId { get; private set; }
  public string ContentLocaleName { get; private set; } = string.Empty;

  public UniqueIndex(
    Realm? realm,
    ContentType contentType,
    Language? language,
    FieldType fieldType,
    FieldDefinition fieldDefinition,
    Content content,
    ContentLocale contentLocale,
    ContentStatus status,
    string value)
  {
    if (realm is not null)
    {
      Realm = realm;
      RealmId = realm.RealmId;
      RealmUid = realm.Id;
    }

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

    Status = status;

    long version = (status == ContentStatus.Published ? contentLocale.PublishedVersion : null) ?? contentLocale.Version;
    Update(version, value);

    Content = content;
    ContentId = content.ContentId;
    ContentUid = content.Id;

    ContentLocale = contentLocale;
    ContentLocaleId = contentLocale.ContentLocaleId;
    ContentLocaleName = contentLocale.UniqueNameNormalized;
  }

  private UniqueIndex()
  {
  }

  public static string CreateKey(KeyValuePair<Guid, FieldValue> fieldValue) => CreateKey(fieldValue.Key, fieldValue.Value.Value);
  public static string CreateKey(Guid fieldDefinitionId, string value) => string.Join(KeySeparator,
    Convert.ToBase64String(fieldDefinitionId.ToByteArray()).TrimEnd('='),
    Helper.Normalize(value));

  public void Update(long version, string value)
  {
    Version = version;

    Value = value.Truncate(MaximumLength);
  }
}
