using Krakenar.Client.Contents;
using Krakenar.Contracts;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class ContentTypeClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public ContentTypeClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Content Types should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    ContentTypeClient contentTypes = new(httpClient, KrakenarSettings);

    Guid id = Guid.Parse("776cb869-5a91-4ea6-bbd9-b9e40250e099");
    ContentType? contentType = await contentTypes.ReadAsync(id, uniqueName: null, _cancellationToken);

    CreateOrReplaceContentTypePayload createOrReplaceContentType = new("BlogArticle")
    {
      IsInvariant = true
    };
    CreateOrReplaceContentTypeResult contentTypeResult = await contentTypes.CreateOrReplaceAsync(createOrReplaceContentType, id, version: null, _cancellationToken);
    Assert.Equal(contentTypeResult.Created, contentType is null);
    contentType = contentTypeResult.ContentType;
    Assert.NotNull(contentType);
    Assert.Equal(id, contentType.Id);
    Assert.True(contentType.IsInvariant);
    Assert.Equal(createOrReplaceContentType.UniqueName, contentType.UniqueName);

    UpdateContentTypePayload updateContentType = new()
    {
      IsInvariant = false,
      DisplayName = new Change<string>(" Blog Article "),
      Description = new Change<string>("  This is the content type for blog articles.  ")
    };
    contentType = await contentTypes.UpdateAsync(id, updateContentType, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.False(contentType.IsInvariant);
    Assert.Equal(createOrReplaceContentType.UniqueName, contentType.UniqueName);
    Assert.Equal(updateContentType.DisplayName.Value?.Trim(), contentType.DisplayName);
    Assert.Equal(updateContentType.Description.Value?.Trim(), contentType.Description);

    contentType = await contentTypes.ReadAsync(id: null, contentType.UniqueName, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Equal(id, contentType.Id);

    SearchContentTypesPayload searchContentTypes = new()
    {
      IsInvariant = false,
      Ids = [contentType.Id],
      Search = new TextSearch([new SearchTerm("%arti%")])
    };
    SearchResults<ContentType> results = await contentTypes.SearchAsync(searchContentTypes, _cancellationToken);
    Assert.Equal(1, results.Total);
    contentType = Assert.Single(results.Items);
    Assert.Equal(id, contentType.Id);

    createOrReplaceContentType.UniqueName = "BlogCategory";
    contentTypeResult = await contentTypes.CreateOrReplaceAsync(createOrReplaceContentType, id: null, version: null, _cancellationToken);
    contentType = contentTypeResult.ContentType;
    Assert.NotNull(contentType);

    contentType = await contentTypes.DeleteAsync(contentType.Id, _cancellationToken);
    Assert.NotNull(contentType);
    Assert.Equal(createOrReplaceContentType.UniqueName, contentType.UniqueName);
  }
}
