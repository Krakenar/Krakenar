using Krakenar.Contracts.Fields;
using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Settings;
using Microsoft.Extensions.DependencyInjection;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;
using FieldDefinition = Krakenar.Core.Fields.FieldDefinition;
using FieldDefinitionDto = Krakenar.Contracts.Fields.FieldDefinition;
using FieldType = Krakenar.Core.Fields.FieldType;

namespace Krakenar.Fields;

[Trait(Traits.Category, Categories.Integration)]
public class FieldDefinitionIntegrationTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly IFieldDefinitionService _fieldDefinitionService;

  public FieldDefinitionIntegrationTests() : base()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();
    _fieldDefinitionService = ServiceProvider.GetRequiredService<IFieldDefinitionService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();
  }

  [Fact(DisplayName = "It should create a new field definition without ID.")]
  public async Task Given_NoId_When_CreateOrReplace_Then_Created()
  {
    StringSettings settings = new(minimumLength: 3, maximumLength: 100, pattern: null);
    FieldType fieldType = new(new UniqueName(Realm.UniqueNameSettings, "ArticleTitle"), settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      FieldType = $" {fieldType.EntityId} ",
      IsInvariant = false,
      IsRequired = true,
      IsIndexed = true,
      IsUnique = true,
      UniqueName = "Title",
      DisplayName = " Title ",
      Description = "  This is the article title.  ",
      Placeholder = " Enter your article title. "
    };
    ContentTypeDto? contentType = await _fieldDefinitionService.CreateOrReplaceAsync(blogArticle.EntityId, payload);
    Assert.NotNull(contentType);
    Assert.Equal(blogArticle.EntityId, contentType.Id);
    Assert.Equal(blogArticle.Version + 1, contentType.Version);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromSeconds(10));

    FieldDefinitionDto field = Assert.Single(contentType.Fields);
    Assert.NotEqual(Guid.Empty, field.Id);
    Assert.Equal(0, field.Order);
    Assert.Equal(fieldType.EntityId, field.FieldType.Id);
    Assert.Equal(payload.IsInvariant, field.IsInvariant);
    Assert.Equal(payload.IsRequired, field.IsRequired);
    Assert.Equal(payload.IsIndexed, field.IsIndexed);
    Assert.Equal(payload.IsUnique, field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), field.DisplayName);
    Assert.Equal(payload.Description.Trim(), field.Description);
    Assert.Equal(payload.Placeholder.Trim(), field.Placeholder);

    Assert.Equal(Actor, field.CreatedBy);
    Assert.Equal(contentType.UpdatedOn, field.CreatedOn);
    Assert.Equal(Actor, field.UpdatedBy);
    Assert.Equal(contentType.UpdatedOn, field.UpdatedOn);
  }

  [Fact(DisplayName = "It should create a new field definition with ID.")]
  public async Task Given_Id_When_CreateOrReplace_Then_Created()
  {
    StringSettings settings = new(minimumLength: 3, maximumLength: 100, pattern: null);
    FieldType fieldType = new(new UniqueName(Realm.UniqueNameSettings, "Slug"), settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    ContentType blogCategory = new(new Identifier("BlogCategory"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    blogCategory.SetField(new FieldDefinition(Guid.NewGuid(), fieldType.Id, true, true, true, true, new Identifier("externalSlug"), null, null, null), ActorId);
    blogCategory.SetField(new FieldDefinition(Guid.NewGuid(), fieldType.Id, true, true, true, true, new Identifier("ExternalSlug"), null, null, null), ActorId);
    await _contentTypeRepository.SaveAsync(blogCategory);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      FieldType = $"  {fieldType.UniqueName}  ",
      IsInvariant = true,
      IsRequired = true,
      IsIndexed = true,
      IsUnique = true,
      UniqueName = "Slug",
      DisplayName = " Slug ",
      Description = "  This is the slug of the category.  ",
      Placeholder = " Enter a value composed of alphanumeric words separated by hyphens (-). "
    };
    Guid fieldId = Guid.NewGuid();
    ContentTypeDto? contentType = await _fieldDefinitionService.CreateOrReplaceAsync(blogCategory.EntityId, payload, fieldId);
    Assert.NotNull(contentType);
    Assert.Equal(blogCategory.EntityId, contentType.Id);
    Assert.Equal(blogCategory.Version + 1, contentType.Version);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromSeconds(10));

    FieldDefinitionDto field = Assert.Single(contentType.Fields, f => f.Id == fieldId);
    Assert.Equal(fieldId, field.Id);
    Assert.Equal(1, field.Order);
    Assert.Equal(fieldType.EntityId, field.FieldType.Id);
    Assert.Equal(payload.IsInvariant, field.IsInvariant);
    Assert.Equal(payload.IsRequired, field.IsRequired);
    Assert.Equal(payload.IsIndexed, field.IsIndexed);
    Assert.Equal(payload.IsUnique, field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), field.DisplayName);
    Assert.Equal(payload.Description.Trim(), field.Description);
    Assert.Equal(payload.Placeholder.Trim(), field.Placeholder);

    Assert.Equal(Actor, field.CreatedBy);
    Assert.Equal(contentType.UpdatedOn, field.CreatedOn);
    Assert.Equal(Actor, field.UpdatedBy);
    Assert.Equal(contentType.UpdatedOn, field.UpdatedOn);
  }

  [Fact(DisplayName = "It should delete an existing field definition.")]
  public async Task Given_FieldDefinition_When_Delete_Then_Deleted()
  {
    FieldType booleanType = new(new UniqueName(Realm.UniqueNameSettings, "Boolean"), new BooleanSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    FieldType keywordsType = new(new UniqueName(Realm.UniqueNameSettings, "Keywords"), new TagsSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync([booleanType, keywordsType]);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    Guid booleanId = Guid.NewGuid();
    blogArticle.SetField(new FieldDefinition(booleanId, booleanType.Id, false, true, false, true, new Identifier("Featured"), null, null, null), ActorId);
    Guid keywordsId = Guid.NewGuid();
    blogArticle.SetField(new FieldDefinition(keywordsId, keywordsType.Id, true, false, false, false, new Identifier("Keywords"), null, null, null), ActorId);
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentTypeDto? contentType = await _fieldDefinitionService.DeleteAsync(blogArticle.EntityId, booleanId);
    Assert.NotNull(contentType);
    Assert.Equal(blogArticle.EntityId, contentType.Id);
    Assert.Equal(blogArticle.Version + 1, contentType.Version);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromSeconds(10));

    FieldDefinitionDto field = Assert.Single(contentType.Fields);
    Assert.Equal(keywordsId, field.Id);
    Assert.Equal(0, field.Order);
  }

  [Fact(DisplayName = "It should replace an existing field definition.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    FieldType fieldType = new(new UniqueName(Realm.UniqueNameSettings, "Boolean"), new BooleanSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    Guid fieldId = Guid.NewGuid();
    Placeholder placeholder = new("Tick or Untick!");
    blogArticle.SetField(new FieldDefinition(fieldId, fieldType.Id, false, true, false, true, new Identifier("Featured"), null, null, placeholder), ActorId);
    await _contentTypeRepository.SaveAsync(blogArticle);

    CreateOrReplaceFieldDefinitionPayload payload = new()
    {
      FieldType = "unknown",
      IsInvariant = true,
      IsRequired = false,
      IsIndexed = true,
      IsUnique = false,
      UniqueName = "IsFeatured",
      DisplayName = " Is Featured? ",
      Description = "  When enabled, the article will appear in the feature articles section.  "
    };
    ContentTypeDto? contentType = await _fieldDefinitionService.CreateOrReplaceAsync(blogArticle.EntityId, payload, fieldId);
    Assert.NotNull(contentType);
    Assert.Equal(blogArticle.EntityId, contentType.Id);
    Assert.Equal(blogArticle.Version + 1, contentType.Version);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromSeconds(10));

    FieldDefinitionDto field = Assert.Single(contentType.Fields);
    Assert.Equal(fieldId, field.Id);
    Assert.Equal(0, field.Order);
    Assert.Equal(fieldType.EntityId, field.FieldType.Id);
    Assert.Equal(payload.IsInvariant, field.IsInvariant);
    Assert.Equal(payload.IsRequired, field.IsRequired);
    Assert.Equal(payload.IsIndexed, field.IsIndexed);
    Assert.Equal(payload.IsUnique, field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), field.DisplayName);
    Assert.Equal(payload.Description.Trim(), field.Description);
    Assert.Null(field.Placeholder);

    Assert.Equal(Actor, field.CreatedBy);
    Assert.True(field.CreatedOn < field.UpdatedOn);
    Assert.Equal(Actor, field.UpdatedBy);
    Assert.Equal(contentType.UpdatedOn, field.UpdatedOn);
  }

  [Fact(DisplayName = "It should update an existing field definition.")]
  public async Task Given_FieldDefinition_When_Update_Then_Updated()
  {
    FieldType fieldType = new(new UniqueName(Realm.UniqueNameSettings, "Boolean"), new BooleanSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    Guid fieldId = Guid.NewGuid();
    Placeholder placeholder = new("Tick or Untick!");
    blogArticle.SetField(new FieldDefinition(fieldId, fieldType.Id, false, true, false, true, new Identifier("Featured"), null, null, placeholder), ActorId);
    await _contentTypeRepository.SaveAsync(blogArticle);

    UpdateFieldDefinitionPayload payload = new()
    {
      IsInvariant = true,
      IsRequired = false,
      IsIndexed = true,
      IsUnique = false,
      UniqueName = "IsFeatured",
      DisplayName = new Contracts.Change<string>(" Is Featured? "),
      Description = new Contracts.Change<string>("  When enabled, the article will appear in the feature articles section.  ")
    };
    ContentTypeDto? contentType = await _fieldDefinitionService.UpdateAsync(blogArticle.EntityId, fieldId, payload);
    Assert.NotNull(contentType);
    Assert.Equal(blogArticle.EntityId, contentType.Id);
    Assert.Equal(blogArticle.Version + 1, contentType.Version);
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn, TimeSpan.FromSeconds(10));

    FieldDefinitionDto field = Assert.Single(contentType.Fields);
    Assert.Equal(fieldId, field.Id);
    Assert.Equal(0, field.Order);
    Assert.Equal(fieldType.EntityId, field.FieldType.Id);
    Assert.Equal(payload.IsInvariant, field.IsInvariant);
    Assert.Equal(payload.IsRequired, field.IsRequired);
    Assert.Equal(payload.IsIndexed, field.IsIndexed);
    Assert.Equal(payload.IsUnique, field.IsUnique);
    Assert.Equal(payload.UniqueName, field.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), field.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), field.Description);
    Assert.Equal(placeholder.Value, field.Placeholder);

    Assert.Equal(Actor, field.CreatedBy);
    Assert.True(field.CreatedOn < field.UpdatedOn);
    Assert.Equal(Actor, field.UpdatedBy);
    Assert.Equal(contentType.UpdatedOn, field.UpdatedOn);
  }
}
