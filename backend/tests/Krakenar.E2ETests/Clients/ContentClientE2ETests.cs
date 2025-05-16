using Krakenar.Client.Contents;
using Krakenar.Contracts.Contents;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ContentClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public ContentClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Contents should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    KrakenarSettings.Realm = Realm.UniqueSlug;
    using HttpClient contentTypeClient = new();
    ContentTypeClient contentTypes = new(contentTypeClient, KrakenarSettings);
    using HttpClient contentClient = new();
    ContentClient contents = new(contentClient, KrakenarSettings);

    Guid contentTypeId = Guid.Parse("82b2cff6-4fb1-4a48-8f9c-faaeba44f106");
    ContentType? contentType = await contentTypes.ReadAsync(contentTypeId, uniqueName: null, _cancellationToken);
    if (contentType is null)
    {
      CreateOrReplaceContentTypePayload contentTypePayload = new("Item");
      CreateOrReplaceContentTypeResult contentTypeResult = await contentTypes.CreateOrReplaceAsync(contentTypePayload, contentTypeId, version: null, _cancellationToken);
      Assert.True(contentTypeResult.Created);
      Assert.NotNull(contentTypeResult.ContentType);
      contentType = contentTypeResult.ContentType;
    }

    Guid contentId = Guid.NewGuid();
    CreateContentPayload payload = new(contentId.ToString(), contentTypeId.ToString())
    {
      Id = contentId,
      Language = "en"
    };
    Content content = await contents.CreateAsync(payload, _cancellationToken);
    Assert.Equal(contentId, content.Id);
    Assert.Equal(contentTypeId, content.ContentType.Id);
    Assert.Equal(payload.UniqueName, content.Invariant.UniqueName);
    ContentLocale locale = Assert.Single(content.Locales);
    Assert.Equal(payload.Language, locale.Language?.Locale.Code);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
  }
}
