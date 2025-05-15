using Krakenar.Client.Fields;
using Krakenar.Contracts;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;
using Krakenar.Contracts.Search;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class FieldTypeClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public FieldTypeClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Field Types should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    FieldTypeClient fieldTypes = new(httpClient, KrakenarSettings);

    Guid id = Guid.Parse("c829611f-8abb-499f-92c4-37e5c37a83eb");
    FieldType? fieldType = await fieldTypes.ReadAsync(id, uniqueName: null, _cancellationToken);

    CreateOrReplaceFieldTypePayload createOrReplaceFieldType = new("Title")
    {
      String = new StringSettings(minimumLength: 3, maximumLength: 100, pattern: null)
    };
    CreateOrReplaceFieldTypeResult fieldTypeResult = await fieldTypes.CreateOrReplaceAsync(createOrReplaceFieldType, id, version: null, _cancellationToken);
    Assert.Equal(fieldTypeResult.Created, fieldType is null);
    fieldType = fieldTypeResult.FieldType;
    Assert.NotNull(fieldType);
    Assert.Equal(id, fieldType.Id);
    Assert.Equal(createOrReplaceFieldType.UniqueName, fieldType.UniqueName);
    Assert.Equal(createOrReplaceFieldType.String, fieldType.String);

    UpdateFieldTypePayload updateFieldType = new()
    {
      DisplayName = new Change<string>(" Title "),
      Description = new Change<string>("  This is the field type for titles.  ")
    };
    fieldType = await fieldTypes.UpdateAsync(id, updateFieldType, _cancellationToken);
    Assert.NotNull(fieldType);
    Assert.Equal(createOrReplaceFieldType.UniqueName, fieldType.UniqueName);
    Assert.Equal(updateFieldType.DisplayName.Value?.Trim(), fieldType.DisplayName);
    Assert.Equal(updateFieldType.Description.Value?.Trim(), fieldType.Description);

    fieldType = await fieldTypes.ReadAsync(id: null, fieldType.UniqueName, _cancellationToken);
    Assert.NotNull(fieldType);
    Assert.Equal(id, fieldType.Id);

    SearchFieldTypesPayload searchFieldTypes = new()
    {
      DataType = DataType.String,
      Ids = [fieldType.Id],
      Search = new TextSearch([new SearchTerm("%tit%")])
    };
    SearchResults<FieldType> results = await fieldTypes.SearchAsync(searchFieldTypes, _cancellationToken);
    Assert.Equal(1, results.Total);
    fieldType = Assert.Single(results.Items);
    Assert.Equal(id, fieldType.Id);

    createOrReplaceFieldType.UniqueName = "Brand";
    fieldTypeResult = await fieldTypes.CreateOrReplaceAsync(createOrReplaceFieldType, id: null, version: null, _cancellationToken);
    fieldType = fieldTypeResult.FieldType;
    Assert.NotNull(fieldType);

    fieldType = await fieldTypes.DeleteAsync(fieldType.Id, _cancellationToken);
    Assert.NotNull(fieldType);
    Assert.Equal(createOrReplaceFieldType.UniqueName, fieldType.UniqueName);
  }
}
