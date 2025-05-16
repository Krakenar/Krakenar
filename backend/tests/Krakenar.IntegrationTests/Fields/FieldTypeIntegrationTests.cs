using Krakenar.Contracts;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Fields;
using Krakenar.Core.Fields.Settings;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using BooleanSettingsDto = Krakenar.Contracts.Fields.Settings.BooleanSettings;
using FieldType = Krakenar.Core.Fields.FieldType;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;
using MediaTypeNames = System.Net.Mime.MediaTypeNames;
using NumberSettingsDto = Krakenar.Contracts.Fields.Settings.NumberSettings;
using RelatedContentSettingsDto = Krakenar.Contracts.Fields.Settings.RelatedContentSettings;
using RichTextSettingsDto = Krakenar.Contracts.Fields.Settings.RichTextSettings;
using SelectOptionDto = Krakenar.Contracts.Fields.Settings.SelectOption;
using SelectSettingsDto = Krakenar.Contracts.Fields.Settings.SelectSettings;
using StringSettingsDto = Krakenar.Contracts.Fields.Settings.StringSettings;

namespace Krakenar.Fields;

[Trait(Traits.Category, Categories.Integration)]
public class FieldTypeIntegrationTests : IntegrationTests
{
  private readonly IFieldTypeRepository _fieldTypeRepository;
  private readonly IFieldTypeService _fieldTypeService;

  public FieldTypeIntegrationTests()
  {
    _fieldTypeRepository = ServiceProvider.GetRequiredService<IFieldTypeRepository>();
    _fieldTypeService = ServiceProvider.GetRequiredService<IFieldTypeService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();
  }

  [Fact(DisplayName = "It should create a new field type without ID.")]
  public async Task Given_NoId_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "Title",
      DisplayName = " Title ",
      Description = "  This is the field type for titles.  ",
      String = new StringSettingsDto(minimumLength: 3, maximumLength: 100, pattern: "    ")
    };

    CreateOrReplaceFieldTypeResult result = await _fieldTypeService.CreateOrReplaceAsync(payload);
    Assert.True(result.Created);

    FieldTypeDto? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    Assert.NotEqual(Guid.Empty, fieldType.Id);
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, fieldType.Realm);
    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.String, fieldType.DataType);
    Assert.NotNull(fieldType.String);
    Assert.Equal(payload.String.MinimumLength, fieldType.String.MinimumLength);
    Assert.Equal(payload.String.MaximumLength, fieldType.String.MaximumLength);
    Assert.Null(fieldType.String.Pattern);
  }

  [Fact(DisplayName = "It should create a new field type with ID.")]
  public async Task Given_Id_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "Contents",
      DisplayName = " Contents ",
      Description = "  This is the field type for contents.  ",
      RichText = new RichTextSettingsDto(contentType: MediaTypeNames.Text.Html, minimumLength: null, maximumLength: 9999)
    };

    Guid id = Guid.NewGuid();
    CreateOrReplaceFieldTypeResult result = await _fieldTypeService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    FieldTypeDto? fieldType = result.FieldType;
    Assert.NotNull(fieldType);
    Assert.Equal(id, fieldType.Id);
    Assert.Equal(3, fieldType.Version);
    Assert.Equal(Actor, fieldType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, fieldType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, fieldType.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, fieldType.Realm);
    Assert.Equal(payload.UniqueName, fieldType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), fieldType.DisplayName);
    Assert.Equal(payload.Description.Trim(), fieldType.Description);
    Assert.Equal(DataType.RichText, fieldType.DataType);
    Assert.Equal(payload.RichText, fieldType.RichText);
  }

  [Fact(DisplayName = "It should delete an existing field type.")]
  public async Task Given_FieldType_When_Delete_Then_Deleted()
  {
    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Birthdate");
    DateTimeSettings settings = new(minimumValue: new DateTime(1900, 1, 1), maximumValue: new DateTime(1999, 12, 31));
    FieldType fieldType = new(uniqueName, settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    FieldTypeDto? dto = await _fieldTypeService.DeleteAsync(fieldType.EntityId);
    Assert.NotNull(dto);
    Assert.Equal(fieldType.EntityId, dto.Id);

    Assert.Empty(await KrakenarContext.FieldTypes.AsNoTracking().Where(x => x.StreamId == fieldType.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the field type by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Boolean");
    BooleanSettings settings = new();
    FieldType fieldType = new(uniqueName, settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    FieldTypeDto? dto = await _fieldTypeService.ReadAsync(fieldType.EntityId);
    Assert.NotNull(dto);
    Assert.Equal(fieldType.EntityId, dto.Id);
  }

  [Fact(DisplayName = "It should read the field type by unique name.")]
  public async Task Given_UniqueName_When_Read_Then_Found()
  {
    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Keywords");
    TagsSettings settings = new();
    FieldType fieldType = new(uniqueName, settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    FieldTypeDto? dto = await _fieldTypeService.ReadAsync(id: null, fieldType.UniqueName.Value);
    Assert.NotNull(dto);
    Assert.Equal(fieldType.EntityId, dto.Id);
  }

  [Fact(DisplayName = "It should replace an existing field type.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Price");
    NumberSettings settings = new(minimumValue: 0.00d, maximumValue: null, step: 0.01d);
    FieldType fieldType = new(uniqueName, settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "Price",
      DisplayName = " Price ",
      Description = "  This is the field type for product prices.  ",
      Number = new NumberSettingsDto(settings.MinimumValue, maximumValue: 999999.99d, settings.Step)
    };

    CreateOrReplaceFieldTypeResult result = await _fieldTypeService.CreateOrReplaceAsync(payload, fieldType.EntityId);
    Assert.False(result.Created);

    FieldTypeDto? dto = result.FieldType;
    Assert.NotNull(dto);
    Assert.Equal(fieldType.EntityId, dto.Id);
    Assert.Equal(fieldType.Version + 2, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, dto.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), dto.DisplayName);
    Assert.Equal(payload.Description.Trim(), dto.Description);
    Assert.Equal(payload.Number, dto.Number);
  }

  [Fact(DisplayName = "It should replace an existing field tppe from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Author");
    RelatedContentSettings oldSettings = new(Guid.Empty, isMultiple: false);
    FieldType fieldType = new(uniqueName, oldSettings, ActorId, FieldTypeId.NewId(Realm.Id));

    long version = fieldType.Version;

    Description description = new("  This is the field type for blog authors.  ");
    fieldType.Description = description;
    fieldType.Update(ActorId);

    RelatedContentSettings newSettings = new(Guid.NewGuid(), isMultiple: true);
    fieldType.SetSettings(newSettings, ActorId);

    await _fieldTypeRepository.SaveAsync(fieldType);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = "Authors",
      DisplayName = " Authors ",
      RelatedContent = new RelatedContentSettingsDto(oldSettings)
    };

    CreateOrReplaceFieldTypeResult result = await _fieldTypeService.CreateOrReplaceAsync(payload, fieldType.EntityId, version);
    Assert.False(result.Created);

    FieldTypeDto? dto = result.FieldType;
    Assert.NotNull(dto);
    Assert.Equal(fieldType.EntityId, dto.Id);
    Assert.Equal(fieldType.Version + 2, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.UniqueName, dto.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), dto.DisplayName);
    Assert.Equal(description.Value, dto.Description);
    Assert.NotNull(dto.RelatedContent);
    Assert.Equal(newSettings, new RelatedContentSettings(dto.RelatedContent));
  }

  [Fact(DisplayName = "It should return null when the field type cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _fieldTypeService.ReadAsync(Guid.Empty, "not-found"));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_FieldTypes_When_Search_Then_CorrectResults()
  {
    FieldType brand = new(new UniqueName(Realm.UniqueNameSettings, "Brand"), new StringSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    FieldType model = new(new UniqueName(Realm.UniqueNameSettings, "Model"), new StringSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    FieldType sku = new(new UniqueName(Realm.UniqueNameSettings, "SKU"), new StringSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    FieldType title = new(new UniqueName(Realm.UniqueNameSettings, "Title"), new StringSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    FieldType contents = new(new UniqueName(Realm.UniqueNameSettings, "Contents"), new RichTextSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync([brand, model, sku, title, contents]);

    SearchFieldTypesPayload payload = new()
    {
      DataType = DataType.String,
      Ids = [brand.EntityId, sku.EntityId, title.EntityId, contents.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("%d%"), new SearchTerm("%t%")], SearchOperator.Or),
      Sort = [new FieldTypeSortOption(FieldTypeSort.UniqueName)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<FieldTypeDto> results = await _fieldTypeService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    FieldTypeDto fieldType = Assert.Single(results.Items);
    Assert.Equal(title.EntityId, fieldType.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple field types were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    FieldType boolean = new(new UniqueName(Realm.UniqueNameSettings, "Boolean"), new BooleanSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    FieldType tags = new(new UniqueName(Realm.UniqueNameSettings, "Keywords"), new TagsSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync([boolean, tags]);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<FieldTypeDto>>(async () => await _fieldTypeService.ReadAsync(boolean.EntityId, tags.UniqueName.Value));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when a unique name conflict occurs.")]
  public async Task Given_UniqueNameConflict_When_CreateOrReplace_Then_UniqueNameAlreadyUsedException()
  {
    FieldType fieldType = new(new UniqueName(Realm.UniqueNameSettings, "Boolean"), new BooleanSettings(), ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    CreateOrReplaceFieldTypePayload payload = new()
    {
      UniqueName = fieldType.UniqueName.Value,
      Boolean = new BooleanSettingsDto()
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _fieldTypeService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(fieldType.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("FieldType", exception.EntityType);
    Assert.Equal(id, exception.EntityId);
    Assert.Equal(fieldType.EntityId, exception.ConflictId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing field type.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    UniqueName uniqueName = new(Realm.UniqueNameSettings, "Colors");
    SelectOption[] options =
    [
      new("Black", value: "black", isDisabled: true),
      new("Cyan", value: "cyan", label: "blue"),
      new("Magenta",  label: "magenta"),
      new("Yellow", value: "yellow")
    ];
    SelectSettings settings = new(options, isMultiple: true);
    FieldType fieldType = new(uniqueName, settings, ActorId, FieldTypeId.NewId(Realm.Id));
    await _fieldTypeRepository.SaveAsync(fieldType);

    UpdateFieldTypePayload payload = new()
    {
      DisplayName = new Contracts.Change<string>(" Couleurs ")
    };
    FieldTypeDto? dto = await _fieldTypeService.UpdateAsync(fieldType.EntityId, payload);
    Assert.NotNull(dto);

    Assert.Equal(fieldType.EntityId, dto.Id);
    Assert.Equal(fieldType.Version + 1, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(fieldType.UniqueName.Value, dto.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), dto.DisplayName);
    Assert.Equal(fieldType.Description?.Value, dto.Description);

    Assert.NotNull(dto.Select);
    SelectSettingsDto select = new(settings.Options.Select(option => new SelectOptionDto(option)).ToList(), settings.IsMultiple);
    Assert.Equal(select.Options, dto.Select.Options);
    Assert.Equal(select.IsMultiple, dto.Select.IsMultiple);
  }
}
