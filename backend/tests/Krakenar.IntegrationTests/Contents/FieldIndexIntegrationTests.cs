using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Localization;
using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Settings;
using Krakenar.Core.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Content = Krakenar.Core.Contents.Content;
using ContentLocale = Krakenar.Core.Contents.ContentLocale;
using ContentType = Krakenar.Core.Contents.ContentType;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;
using FieldDefinition = Krakenar.Core.Fields.FieldDefinition;
using FieldIndexEntity = Krakenar.EntityFrameworkCore.Relational.Entities.FieldIndex;
using FieldType = Krakenar.Core.Fields.FieldType;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;
using FieldValue = Krakenar.Core.Fields.FieldValue;
using Language = Krakenar.Core.Localization.Language;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using Locale = Krakenar.Core.Localization.Locale;

namespace Krakenar.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class FieldIndexIntegrationTests : IntegrationTests
{
  private const string ShortDescription = "Dark sound qualities and a big-sized lip offer a trashy sound. The short sustain offers cutting and quick accents.";

  private readonly IContentManager _contentManager;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IContentTypeService _contentTypeService;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly IFieldTypeService _fieldTypeService;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILanguageService _languageService;

  private Language _language = null!;
  private FieldType _fieldType = null!;
  private FieldDefinition _fieldDefinition = null!;
  private ContentType _contentType = null!;

  public FieldIndexIntegrationTests() : base()
  {
    _contentManager = ServiceProvider.GetRequiredService<IContentManager>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _contentTypeService = ServiceProvider.GetRequiredService<IContentTypeService>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();
    _fieldTypeService = ServiceProvider.GetRequiredService<IFieldTypeService>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _languageService = ServiceProvider.GetRequiredService<ILanguageService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    _language = new(new Locale("en-CA"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(_language);

    UniqueName uniqueName = new(Realm.UniqueNameSettings, "ShortDescription");
    StringSettings settings = new();
    _fieldType = new FieldType(uniqueName, settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(_fieldType);

    _fieldDefinition = new FieldDefinition(Guid.NewGuid(), _fieldType.Id, false, true, true, false, new Identifier("ShortDescription"), null, null, null);
    _contentType = new ContentType(new Identifier("Product"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    _contentType.SetField(_fieldDefinition, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);
  }

  [Fact(DisplayName = "Changing language locale should update indices.")]
  public async Task Given_FieldValues_When_LanguageLocaleChanged_Then_IndicesUpdated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    _language.SetLocale(new Locale("en-US"), ActorId);
    await _languageRepository.SaveAsync(_language);

    FieldIndexEntity fieldIndex = Assert.Single(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
    Assert.Equal(_language.Locale.Code.ToUpperInvariant(), fieldIndex.LanguageCode);
  }

  [Fact(DisplayName = "Creating content should populate indices.")]
  public async Task Given_FieldValues_When_ContentCreated_Then_IndicesPopulated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldIndexEntity fieldIndex = Assert.Single(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
    Assert.Equal(_contentType.EntityId, fieldIndex.ContentTypeUid);
    Assert.Equal(_contentType.UniqueName.Value.ToUpperInvariant(), fieldIndex.ContentTypeName);
    Assert.Equal(_language.EntityId, fieldIndex.LanguageUid);
    Assert.Equal(_language.Locale.Code.ToUpperInvariant(), fieldIndex.LanguageCode);
    Assert.Equal(_language.IsDefault, fieldIndex.LanguageIsDefault);
    Assert.Equal(_fieldType.EntityId, fieldIndex.FieldTypeUid);
    Assert.Equal(_fieldType.UniqueName.Value.ToUpperInvariant(), fieldIndex.FieldTypeName);
    Assert.Equal(_fieldDefinition.Id, fieldIndex.FieldDefinitionUid);
    Assert.Equal(_fieldDefinition.UniqueName.Value.ToUpperInvariant(), fieldIndex.FieldDefinitionName);
    Assert.Equal(content.EntityId, fieldIndex.ContentUid);
    Assert.Equal(invariant.UniqueName.Value.ToUpperInvariant(), fieldIndex.ContentLocaleName);
    Assert.Equal(content.Version, fieldIndex.Version);
    Assert.Equal(ContentStatus.Latest, fieldIndex.Status);
    Assert.Equal(ShortDescription, fieldIndex.String);
  }

  [Fact(DisplayName = "Deleting content type should clear indices.")]
  public async Task Given_FieldValues_When_ContentTypeDeleted_Then_IndicesCleared()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    ContentTypeDto? contentType = await _contentTypeService.DeleteAsync(_contentType.EntityId);
    Assert.NotNull(contentType);

    Assert.Empty(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Deleting field type should clear indices.")]
  public async Task Given_FieldValues_When_FieldTypeDeleted_Then_IndicesCleared()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldTypeDto? fieldType = await _fieldTypeService.DeleteAsync(_fieldType.EntityId);
    Assert.NotNull(fieldType);

    Assert.Empty(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Deleting language should clear indices.")]
  public async Task Given_FieldValues_When_LanguageDeleted_Then_IndicesCleared()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    LanguageDto? language = await _languageService.DeleteAsync(_language.EntityId);
    Assert.NotNull(language);

    Assert.Empty(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Publishing content should populate indices.")]
  public async Task Given_FieldValues_When_ContentPublished_Then_IndicesPopulated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldIndexEntity[] indices = await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync();
    Assert.Equal(2, indices.Length);
    Assert.Contains(indices, index => index.Status == ContentStatus.Latest);

    FieldIndexEntity fieldIndex = Assert.Single(indices, index => index.Status == ContentStatus.Published);
    Assert.Equal(_contentType.EntityId, fieldIndex.ContentTypeUid);
    Assert.Equal(_contentType.UniqueName.Value.ToUpperInvariant(), fieldIndex.ContentTypeName);
    Assert.Equal(_language.EntityId, fieldIndex.LanguageUid);
    Assert.Equal(_language.Locale.Code.ToUpperInvariant(), fieldIndex.LanguageCode);
    Assert.Equal(_language.IsDefault, fieldIndex.LanguageIsDefault);
    Assert.Equal(_fieldType.EntityId, fieldIndex.FieldTypeUid);
    Assert.Equal(_fieldType.UniqueName.Value.ToUpperInvariant(), fieldIndex.FieldTypeName);
    Assert.Equal(_fieldDefinition.Id, fieldIndex.FieldDefinitionUid);
    Assert.Equal(_fieldDefinition.UniqueName.Value.ToUpperInvariant(), fieldIndex.FieldDefinitionName);
    Assert.Equal(content.EntityId, fieldIndex.ContentUid);
    Assert.Equal(invariant.UniqueName.Value.ToUpperInvariant(), fieldIndex.ContentLocaleName);
    Assert.Equal(content.Version, fieldIndex.Version);
    Assert.Equal(ShortDescription, fieldIndex.String);
  }

  [Fact(DisplayName = "Removing content locales should clear indices.")]
  public async Task Given_FieldValues_When_ContentLocaleRemoved_Then_IndicesCleared()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    content.RemoveLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Assert.Empty(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Removing field definitions should clear indices.")]
  public async Task Given_FieldValues_When_FieldDefinitionRemoved_Then_dIndicesCleare()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    _contentType.RemoveField(_fieldDefinition.Id, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);

    Assert.Empty(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Removing field values should clear indices.")]
  public async Task Given_FieldValues_When_ValueRemoved_Then_IndicesCleared()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    locale = new(locale.UniqueName, locale.DisplayName, locale.Description);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Assert.Empty(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Renaming content locale should update indices.")]
  public async Task Given_FieldValues_When_ContentLocaleRenamed_Then_IndicesUpdated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueName uniqueName = new(Realm.UniqueNameSettings, "CC18DACH");
    locale = new(uniqueName, null, null, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldIndexEntity[] indices = await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync();
    foreach (FieldIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.ContentLocaleName);
    }
  }

  [Fact(DisplayName = "Renaming content type should update indices.")]
  public async Task Given_FieldValues_When_ContentTypeRenamed_Then_IndicesUpdated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Identifier uniqueName = new("Products");
    _contentType.SetUniqueName(uniqueName, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);

    FieldIndexEntity[] indices = await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync();
    foreach (FieldIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.ContentTypeName);
    }
  }

  [Fact(DisplayName = "Renaming field definition should update indices.")]
  public async Task Given_FieldValues_When_FieldDefinitionRenamed_Then_IndicesUpdated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Identifier uniqueName = new("ShortDescription");
    FieldDefinition fieldDefinition = new(_fieldDefinition.Id, _fieldType.Id, true, true, false, true, uniqueName, null, null, null);
    _contentType.SetField(fieldDefinition, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);

    FieldIndexEntity[] indices = await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync();
    foreach (FieldIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.FieldDefinitionName);
    }
  }

  [Fact(DisplayName = "Renaming field type should update indices.")]
  public async Task Given_FieldValues_When_FieldTypeRenamed_Then_IndicesUpdated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Short_Description");
    _fieldType.SetUniqueName(uniqueName, ActorId);
    await _fieldTypeRepository.SaveAsync(_fieldType);

    FieldIndexEntity[] indices = await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync();
    foreach (FieldIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.FieldTypeName);
    }
  }

  [Fact(DisplayName = "Setting default language should update indices.")]
  public async Task Given_FieldValues_When_LanguageSetDefault_Then_IndicesUpdated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    _language.SetDefault(isDefault: true, ActorId);
    await _languageRepository.SaveAsync(_language);

    FieldIndexEntity fieldIndex = Assert.Single(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
    Assert.True(fieldIndex.LanguageIsDefault);
  }

  [Fact(DisplayName = "Unpublishing content should clear indices.")]
  public async Task Given_FieldValues_When_ContentUnpublished_Then_IndicesCleared()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(ShortDescription)
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    content.PublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldIndexEntity[] indices = await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync();
    Assert.Equal(2, indices.Length);

    content.UnpublishLocale(_language, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldIndexEntity index = Assert.Single(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
    Assert.Equal(ContentStatus.Latest, index.Status);
  }

  [Fact(DisplayName = "Updating content should update indices.")]
  public async Task Given_FieldValues_When_ContentUpdated_Then_IndicesUpdated()
  {
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"));
    Content content = new(_contentType, invariant, ActorId);
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue("- B12 Bronze Alloy\n- High-tech computerized manufacturing\n- Outstanding sound qualities and dark look")
    };
    ContentLocale locale = new(invariant.UniqueName, invariant.DisplayName, invariant.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    fieldValues[_fieldDefinition.Id] = new FieldValue(ShortDescription);
    locale = new(locale.UniqueName, locale.DisplayName, locale.Description, fieldValues);
    content.SetLocale(_language, locale, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldIndexEntity fieldIndex = Assert.Single(await KrakenarContext.FieldIndex.AsNoTracking().ToArrayAsync());
    Assert.Equal(_contentType.EntityId, fieldIndex.ContentTypeUid);
    Assert.Equal(_contentType.UniqueName.Value.ToUpperInvariant(), fieldIndex.ContentTypeName);
    Assert.Equal(_language.EntityId, fieldIndex.LanguageUid);
    Assert.Equal(_language.Locale.Code.ToUpperInvariant(), fieldIndex.LanguageCode);
    Assert.Equal(_language.IsDefault, fieldIndex.LanguageIsDefault);
    Assert.Equal(_fieldType.EntityId, fieldIndex.FieldTypeUid);
    Assert.Equal(_fieldType.UniqueName.Value.ToUpperInvariant(), fieldIndex.FieldTypeName);
    Assert.Equal(_fieldDefinition.Id, fieldIndex.FieldDefinitionUid);
    Assert.Equal(_fieldDefinition.UniqueName.Value.ToUpperInvariant(), fieldIndex.FieldDefinitionName);
    Assert.Equal(content.EntityId, fieldIndex.ContentUid);
    Assert.Equal(invariant.UniqueName.Value.ToUpperInvariant(), fieldIndex.ContentLocaleName);
    Assert.Equal(content.Version, fieldIndex.Version);
    Assert.Equal(ContentStatus.Latest, fieldIndex.Status);
    Assert.Equal(ShortDescription, fieldIndex.String);
  }
}
