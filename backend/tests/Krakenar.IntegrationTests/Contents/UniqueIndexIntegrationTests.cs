using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Content = Krakenar.Core.Contents.Content;
using ContentLocale = Krakenar.Core.Contents.ContentLocale;
using ContentType = Krakenar.Core.Contents.ContentType;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;
using FieldDefinition = Krakenar.Core.Fields.FieldDefinition;
using FieldType = Krakenar.Core.Fields.FieldType;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;
using FieldValue = Krakenar.Core.Fields.FieldValue;
using UniqueIndexEntity = Krakenar.EntityFrameworkCore.Relational.Entities.UniqueIndex;

namespace Krakenar.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class UniqueIndexIntegrationTests : IntegrationTests
{
  private const string Sku = "CC18DACH";

  private readonly IContentManager _contentManager;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IContentTypeService _contentTypeService;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly IFieldTypeService _fieldTypeService;

  private FieldType _fieldType = null!;
  private FieldDefinition _fieldDefinition = null!;
  private ContentType _contentType = null!;

  public UniqueIndexIntegrationTests() : base()
  {
    _contentManager = ServiceProvider.GetRequiredService<IContentManager>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _contentTypeService = ServiceProvider.GetRequiredService<IContentTypeService>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();
    _fieldTypeService = ServiceProvider.GetRequiredService<IFieldTypeService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    UniqueName uniqueName = new(Realm.UniqueNameSettings, "StockKeepingUnit");
    StringSettings settings = new(minimumLength: 3, maximumLength: 32, pattern: null);
    _fieldType = new FieldType(uniqueName, settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(_fieldType);

    _fieldDefinition = new FieldDefinition(Guid.NewGuid(), _fieldType.Id, true, true, false, true, new Identifier("Sku"), null, null, null);
    _contentType = new ContentType(new Identifier("Product"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    _contentType.SetField(_fieldDefinition, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);
  }

  [Fact(DisplayName = "Creating content should populate indices.")]
  public async Task Given_FieldValues_When_ContentCreated_Then_IndicesPopulated()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueIndexEntity uniqueIndex = Assert.Single(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
    Assert.Equal(_contentType.EntityId, uniqueIndex.ContentTypeUid);
    Assert.Equal(_contentType.UniqueName.Value.ToUpperInvariant(), uniqueIndex.ContentTypeName);
    Assert.Null(uniqueIndex.LanguageId);
    Assert.Null(uniqueIndex.LanguageUid);
    Assert.Null(uniqueIndex.LanguageCode);
    Assert.False(uniqueIndex.LanguageIsDefault);
    Assert.Equal(_fieldType.EntityId, uniqueIndex.FieldTypeUid);
    Assert.Equal(_fieldType.UniqueName.Value.ToUpperInvariant(), uniqueIndex.FieldTypeName);
    Assert.Equal(_fieldDefinition.Id, uniqueIndex.FieldDefinitionUid);
    Assert.Equal(_fieldDefinition.UniqueName.Value.ToUpperInvariant(), uniqueIndex.FieldDefinitionName);
    Assert.Equal(content.EntityId, uniqueIndex.ContentUid);
    Assert.Equal(invariant.UniqueName.Value.ToUpperInvariant(), uniqueIndex.ContentLocaleName);
    Assert.Equal(content.Version, uniqueIndex.Version);
    Assert.Equal(ContentStatus.Latest, uniqueIndex.Status);
    Assert.Equal(Sku, uniqueIndex.Value);
    Assert.Equal(Sku.ToUpperInvariant(), uniqueIndex.ValueNormalized);
  }

  [Fact(DisplayName = "Deleting content should clear indices.")]
  public async Task Given_FieldValues_When_ContentDeleted_Then_IndicesCleared()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Assert.NotEmpty(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());

    content.Delete(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Assert.Empty(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Deleting content type should clear indices.")]
  public async Task Given_FieldValues_When_ContentTypeDeleted_Then_IndicesCleared()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    ContentTypeDto? contentType = await _contentTypeService.DeleteAsync(_contentType.EntityId);
    Assert.NotNull(contentType);

    Assert.Empty(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Deleting field type should clear indices.")]
  public async Task Given_FieldValues_When_FieldTypeDeleted_Then_IndicesCleared()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    FieldTypeDto? fieldType = await _fieldTypeService.DeleteAsync(_fieldType.EntityId);
    Assert.NotNull(fieldType);

    Assert.Empty(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Publishing content should populate indices.")]
  public async Task Given_FieldValues_When_ContentPublished_Then_IndicesPopulated()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueIndexEntity[] indices = await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync();
    Assert.Equal(2, indices.Length);
    Assert.Contains(indices, index => index.Status == ContentStatus.Latest);

    UniqueIndexEntity uniqueIndex = Assert.Single(indices, index => index.Status == ContentStatus.Published);
    Assert.Equal(_contentType.EntityId, uniqueIndex.ContentTypeUid);
    Assert.Equal(_contentType.UniqueName.Value.ToUpperInvariant(), uniqueIndex.ContentTypeName);
    Assert.Null(uniqueIndex.LanguageId);
    Assert.Null(uniqueIndex.LanguageUid);
    Assert.Null(uniqueIndex.LanguageCode);
    Assert.False(uniqueIndex.LanguageIsDefault);
    Assert.Equal(_fieldType.EntityId, uniqueIndex.FieldTypeUid);
    Assert.Equal(_fieldType.UniqueName.Value.ToUpperInvariant(), uniqueIndex.FieldTypeName);
    Assert.Equal(_fieldDefinition.Id, uniqueIndex.FieldDefinitionUid);
    Assert.Equal(_fieldDefinition.UniqueName.Value.ToUpperInvariant(), uniqueIndex.FieldDefinitionName);
    Assert.Equal(content.EntityId, uniqueIndex.ContentUid);
    Assert.Equal(invariant.UniqueName.Value.ToUpperInvariant(), uniqueIndex.ContentLocaleName);
    Assert.Equal(content.Version, uniqueIndex.Version);
    Assert.Equal(Sku, uniqueIndex.Value);
    Assert.Equal(Sku.ToUpperInvariant(), uniqueIndex.ValueNormalized);
  }

  [Fact(DisplayName = "Removing field definitions should clear indices.")]
  public async Task Given_FieldValues_When_FieldDefinitionRemoved_Then_IndicesCleared()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    _contentType.RemoveField(_fieldDefinition.Id, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);

    Assert.Empty(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Removing field values should clear indices.")]
  public async Task Given_FieldValues_When_ValueRemoved_Then_IndicesCleared()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, new Dictionary<Guid, FieldValue>());
    content.SetInvariant(invariant, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Assert.Empty(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
  }

  [Fact(DisplayName = "Renaming content locale should update indices.")]
  public async Task Given_FieldValues_When_ContentLocaleRenamed_Then_IndicesUpdated()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueName uniqueName = new(Realm.UniqueNameSettings, Sku);
    invariant = new(uniqueName, null, null, fieldValues);
    content.SetInvariant(invariant, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueIndexEntity[] indices = await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync();
    foreach (UniqueIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.ContentLocaleName);
    }
  }

  [Fact(DisplayName = "Renaming content type should update indices.")]
  public async Task Given_FieldValues_When_ContentTypeRenamed_Then_IndicesUpdated()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Identifier uniqueName = new("Products");
    _contentType.SetUniqueName(uniqueName, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);

    UniqueIndexEntity[] indices = await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync();
    foreach (UniqueIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.ContentTypeName);
    }
  }

  [Fact(DisplayName = "Renaming field definition should update indices.")]
  public async Task Given_FieldValues_When_FieldDefinitionRenamed_Then_IndicesUpdated()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    Identifier uniqueName = new("StockKeepingUnit");
    FieldDefinition fieldDefinition = new(_fieldDefinition.Id, _fieldType.Id, true, true, false, true, uniqueName, null, null, null);
    _contentType.SetField(fieldDefinition, ActorId);
    await _contentTypeRepository.SaveAsync(_contentType);

    UniqueIndexEntity[] indices = await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync();
    foreach (UniqueIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.FieldDefinitionName);
    }
  }

  [Fact(DisplayName = "Renaming field type should update indices.")]
  public async Task Given_FieldValues_When_FieldTypeRenamed_Then_IndicesUpdated()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Stock_Keeping_Unit");
    _fieldType.SetUniqueName(uniqueName, ActorId);
    await _fieldTypeRepository.SaveAsync(_fieldType);

    UniqueIndexEntity[] indices = await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync();
    foreach (UniqueIndexEntity index in indices)
    {
      Assert.Equal(uniqueName.Value.ToUpperInvariant(), index.FieldTypeName);
    }
  }

  [Fact(DisplayName = "Unpublishing content should clear indices.")]
  public async Task Given_FieldValues_When_ContentUnpublished_Then_IndicesCleared()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    content.Publish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueIndexEntity[] indices = await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync();
    Assert.Equal(2, indices.Length);

    content.Unpublish(ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueIndexEntity index = Assert.Single(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
    Assert.Equal(ContentStatus.Latest, index.Status);
  }

  [Fact(DisplayName = "Updating content should update indices.")]
  public async Task Given_FieldValues_When_ContentUpdated_Then_IndicesUpdated()
  {
    Dictionary<Guid, FieldValue> fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue("CC18DATRCH")
    };
    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    Content content = new(_contentType, invariant, ActorId);
    fieldValues = new()
    {
      [_fieldDefinition.Id] = new FieldValue(Sku)
    };
    invariant = new(new UniqueName(Realm.UniqueNameSettings, "classics-custom-18-dark-china"), null, null, fieldValues);
    content.SetInvariant(invariant, ActorId);
    await _contentManager.SaveAsync(content, _contentType);

    UniqueIndexEntity uniqueIndex = Assert.Single(await KrakenarContext.UniqueIndex.AsNoTracking().ToArrayAsync());
    Assert.Equal(_contentType.EntityId, uniqueIndex.ContentTypeUid);
    Assert.Equal(_contentType.UniqueName.Value.ToUpperInvariant(), uniqueIndex.ContentTypeName);
    Assert.Null(uniqueIndex.LanguageId);
    Assert.Null(uniqueIndex.LanguageUid);
    Assert.Null(uniqueIndex.LanguageCode);
    Assert.False(uniqueIndex.LanguageIsDefault);
    Assert.Equal(_fieldType.EntityId, uniqueIndex.FieldTypeUid);
    Assert.Equal(_fieldType.UniqueName.Value.ToUpperInvariant(), uniqueIndex.FieldTypeName);
    Assert.Equal(_fieldDefinition.Id, uniqueIndex.FieldDefinitionUid);
    Assert.Equal(_fieldDefinition.UniqueName.Value.ToUpperInvariant(), uniqueIndex.FieldDefinitionName);
    Assert.Equal(content.EntityId, uniqueIndex.ContentUid);
    Assert.Equal(invariant.UniqueName.Value.ToUpperInvariant(), uniqueIndex.ContentLocaleName);
    Assert.Equal(content.Version, uniqueIndex.Version);
    Assert.Equal(ContentStatus.Latest, uniqueIndex.Status);
    Assert.Equal(Sku, uniqueIndex.Value);
    Assert.Equal(Sku.ToUpperInvariant(), uniqueIndex.ValueNormalized);
  }
}
