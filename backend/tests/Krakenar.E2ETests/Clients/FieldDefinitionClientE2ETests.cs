using Krakenar.Client.Contents;
using Krakenar.Client.Fields;
using Krakenar.Contracts;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Fields.Settings;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class FieldDefinitionClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public FieldDefinitionClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Field Definitions should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    KrakenarSettings.Realm = Realm.UniqueSlug;

    using HttpClient fieldTypeClient = new();
    FieldTypeClient fieldTypes = new(fieldTypeClient, KrakenarSettings);
    using HttpClient contentTypeClient = new();
    ContentTypeClient contentTypes = new(contentTypeClient, KrakenarSettings);
    using HttpClient fieldDefinitionClient = new();
    FieldDefinitionClient fieldDefinitions = new(fieldDefinitionClient, KrakenarSettings);

    FieldType? fieldType = await fieldTypes.ReadAsync(id: null, "Price", _cancellationToken);
    if (fieldType is null)
    {
      CreateOrReplaceFieldTypePayload fieldTypePayload = new("Price")
      {
        Number = new NumberSettings(minimumValue: 0.01d, maximumValue: 999999.99d, step: 0.01d)
      };
      CreateOrReplaceFieldTypeResult fieldTypeResult = await fieldTypes.CreateOrReplaceAsync(fieldTypePayload, id: null, version: null, _cancellationToken);
      Assert.True(fieldTypeResult.Created);
      Assert.NotNull(fieldTypeResult.FieldType);
      fieldType = fieldTypeResult.FieldType;
    }

    ContentType? contentType = await contentTypes.ReadAsync(id: null, "Product", _cancellationToken);
    if (contentType is null)
    {
      CreateOrReplaceContentTypePayload contentTypePayload = new("Product");
      CreateOrReplaceContentTypeResult contentTypeResult = await contentTypes.CreateOrReplaceAsync(contentTypePayload, id: null, version: null, _cancellationToken);
      Assert.True(contentTypeResult.Created);
      Assert.NotNull(contentTypeResult.ContentType);
      contentType = contentTypeResult.ContentType;
    }
    Guid contentTypeId = contentType.Id;

    CreateOrReplaceFieldDefinitionPayload createOrReplace = new("ProductPrice", fieldType.UniqueName);
    Guid fieldId = Guid.NewGuid();
    contentType = await fieldDefinitions.CreateOrReplaceAsync(contentTypeId, createOrReplace, fieldId, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Equal(contentTypeId, contentType.Id);
    FieldDefinition field = Assert.Single(contentType.Fields);
    Assert.Equal(fieldId, field.Id);
    Assert.Equal(0, field.Order);
    Assert.Equal(fieldType, field.FieldType);
    Assert.False(field.IsInvariant);
    Assert.False(field.IsRequired);
    Assert.False(field.IsIndexed);
    Assert.False(field.IsUnique);
    Assert.Equal(createOrReplace.UniqueName, field.UniqueName);
    Assert.Null(field.DisplayName);
    Assert.Null(field.Description);
    Assert.Null(field.Placeholder);
    Assert.Equal(DateTime.UtcNow, field.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(field.CreatedOn, field.UpdatedOn);

    UpdateFieldDefinitionPayload update = new()
    {
      IsInvariant = true,
      IsRequired = true,
      IsIndexed = true,
      DisplayName = new Change<string>(" Price "),
      Description = new Change<string>("  The current product price.  ")
    };
    contentType = await fieldDefinitions.UpdateAsync(contentTypeId, fieldId, update, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Equal(contentTypeId, contentType.Id);
    field = Assert.Single(contentType.Fields);
    Assert.Equal(fieldId, field.Id);
    Assert.Equal(0, field.Order);
    Assert.Equal(fieldType, field.FieldType);
    Assert.Equal(update.IsInvariant, field.IsInvariant);
    Assert.Equal(update.IsRequired, field.IsRequired);
    Assert.Equal(update.IsInvariant, field.IsIndexed);
    Assert.False(field.IsUnique);
    Assert.Equal(createOrReplace.UniqueName, field.UniqueName);
    Assert.Equal(update.DisplayName.Value?.Trim(), field.DisplayName);
    Assert.Equal(update.Description.Value?.Trim(), field.Description);
    Assert.Null(field.Placeholder);
    Assert.True(field.CreatedOn < field.UpdatedOn);
    Assert.Equal(DateTime.UtcNow, field.UpdatedOn, TimeSpan.FromSeconds(10));

    contentType = await fieldDefinitions.DeleteAsync(contentTypeId, fieldId, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Equal(contentTypeId, contentType.Id);
    Assert.Empty(contentType.Fields);
  }
}
