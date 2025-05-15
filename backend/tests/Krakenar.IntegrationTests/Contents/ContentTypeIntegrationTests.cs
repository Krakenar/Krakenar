using Krakenar.Contracts;
using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Contents;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ContentType = Krakenar.Core.Contents.ContentType;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class ContentTypeIntegrationTests : IntegrationTests
{
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly IContentTypeService _contentTypeService;

  public ContentTypeIntegrationTests()
  {
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _contentTypeService = ServiceProvider.GetRequiredService<IContentTypeService>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();
  }

  [Fact(DisplayName = "It should create a new content type without ID.")]
  public async Task Given_NoId_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceContentTypePayload payload = new()
    {
      UniqueName = "BlogArticle",
      DisplayName = " Blog Article ",
      Description = "  This is the content type for blog articles.  "
    };

    CreateOrReplaceContentTypeResult result = await _contentTypeService.CreateOrReplaceAsync(payload);
    Assert.True(result.Created);

    ContentTypeDto? contentType = result.ContentType;
    Assert.NotNull(contentType);
    Assert.NotEqual(Guid.Empty, contentType.Id);
    Assert.Equal(2, contentType.Version);
    Assert.Equal(Actor, contentType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, contentType.Realm);
    Assert.False(contentType.IsInvariant);
    Assert.Equal(payload.UniqueName, contentType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), contentType.DisplayName);
    Assert.Equal(payload.Description.Trim(), contentType.Description);
  }

  [Fact(DisplayName = "It should create a new content type with ID.")]
  public async Task Given_Id_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = true,
      UniqueName = "BlogCategory",
      DisplayName = " Blog Category ",
      Description = "  This is the content type for blog categories.  "
    };

    Guid id = Guid.NewGuid();
    CreateOrReplaceContentTypeResult result = await _contentTypeService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    ContentTypeDto? contentType = result.ContentType;
    Assert.NotNull(contentType);
    Assert.Equal(id, contentType.Id);
    Assert.Equal(2, contentType.Version);
    Assert.Equal(Actor, contentType.CreatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, contentType.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, contentType.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, contentType.Realm);
    Assert.True(contentType.IsInvariant);
    Assert.Equal(payload.UniqueName, contentType.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), contentType.DisplayName);
    Assert.Equal(payload.Description.Trim(), contentType.Description);
  }

  [Fact(DisplayName = "It should delete an existing content type.")]
  public async Task Given_ContentType_When_Delete_Then_Deleted()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(contentType);

    ContentTypeDto? dto = await _contentTypeService.DeleteAsync(contentType.EntityId);
    Assert.NotNull(dto);
    Assert.Equal(contentType.EntityId, dto.Id);

    Assert.Empty(await KrakenarContext.ContentTypes.AsNoTracking().Where(x => x.StreamId == contentType.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the content type by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(contentType);

    ContentTypeDto? dto = await _contentTypeService.ReadAsync(contentType.EntityId);
    Assert.NotNull(dto);
    Assert.Equal(contentType.EntityId, dto.Id);
  }

  [Fact(DisplayName = "It should read the content type by unique name.")]
  public async Task Given_UniqueName_When_Read_Then_Found()
  {
    ContentType contentType = new(new Identifier("BlogAuthor"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(contentType);

    ContentTypeDto? dto = await _contentTypeService.ReadAsync(id: null, contentType.UniqueName.Value);
    Assert.NotNull(dto);
    Assert.Equal(contentType.EntityId, dto.Id);
  }

  [Fact(DisplayName = "It should replace an existing content type.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    ContentType contentType = new(new Identifier("Article"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(contentType);

    CreateOrReplaceContentTypePayload payload = new()
    {
      IsInvariant = false,
      UniqueName = "BlogArticle",
      DisplayName = " Blog Article ",
      Description = "  This is the content type for blog articles.  "
    };

    CreateOrReplaceContentTypeResult result = await _contentTypeService.CreateOrReplaceAsync(payload, contentType.EntityId);
    Assert.False(result.Created);

    ContentTypeDto? dto = result.ContentType;
    Assert.NotNull(dto);
    Assert.Equal(contentType.EntityId, dto.Id);
    Assert.Equal(contentType.Version + 2, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.IsInvariant, dto.IsInvariant);
    Assert.Equal(payload.UniqueName, dto.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), dto.DisplayName);
    Assert.Equal(payload.Description.Trim(), dto.Description);
  }

  [Fact(DisplayName = "It should replace an existing contentType from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    ContentType contentType = new(new Identifier("Author"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));

    long version = contentType.Version;

    Description description = new("  This is the content type for blog authors.  ");
    contentType.Description = description;
    contentType.IsInvariant = true;
    contentType.Update(ActorId);

    await _contentTypeRepository.SaveAsync(contentType);

    CreateOrReplaceContentTypePayload payload = new()
    {
      UniqueName = "BlogAuthor",
      DisplayName = " Blog Author ",
    };

    CreateOrReplaceContentTypeResult result = await _contentTypeService.CreateOrReplaceAsync(payload, contentType.EntityId, version);
    Assert.False(result.Created);

    ContentTypeDto? dto = result.ContentType;
    Assert.NotNull(dto);
    Assert.Equal(contentType.EntityId, dto.Id);
    Assert.Equal(contentType.Version + 2, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.True(dto.IsInvariant);
    Assert.Equal(payload.UniqueName, dto.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), dto.DisplayName);
    Assert.Equal(description.Value, dto.Description);
  }

  [Fact(DisplayName = "It should return null when the content type cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _contentTypeService.ReadAsync(Guid.Empty, "not-found"));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_ContentTypes_When_Search_Then_CorrectResults()
  {
    ContentType blogCategory = new(new Identifier("BlogCategory"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    ContentType blogAuthor = new(new Identifier("BlogAuthor"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    ContentType product = new(new Identifier("Product"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    ContentType productCategory = new(new Identifier("ProductCategory"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync([blogCategory, blogArticle, blogAuthor, product, productCategory]);

    SearchContentTypesPayload payload = new()
    {
      IsInvariant = false,
      Ids = [blogCategory.EntityId, blogArticle.EntityId, blogAuthor.EntityId, product.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("Blog%"), new SearchTerm("%category")], SearchOperator.Or),
      Sort = [new ContentTypeSortOption(ContentTypeSort.UniqueName, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<ContentTypeDto> results = await _contentTypeService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    ContentTypeDto contentType = Assert.Single(results.Items);
    Assert.Equal(blogArticle.EntityId, contentType.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple content types were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    ContentType article = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    ContentType author = new(new Identifier("BlogAuthor"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync([article, author]);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<ContentTypeDto>>(
      async () => await _contentTypeService.ReadAsync(article.EntityId, author.UniqueName.Value));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw UniqueNameAlreadyUsedException when a unique name conflict occurs.")]
  public async Task Given_UniqueNameConflict_When_CreateOrReplace_Then_UniqueNameAlreadyUsedException()
  {
    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(contentType);

    CreateOrReplaceContentTypePayload payload = new()
    {
      UniqueName = contentType.UniqueName.Value
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<UniqueNameAlreadyUsedException>(async () => await _contentTypeService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(contentType.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal("ContentType", exception.EntityType);
    Assert.Equal(id, exception.EntityId);
    Assert.Equal(contentType.EntityId, exception.ConflictId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing content type.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    ContentType contentType = new(new Identifier("Article"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(contentType);

    UpdateContentTypePayload payload = new()
    {
      IsInvariant = false,
      DisplayName = new Contracts.Change<string>(" Blog Article ")
    };
    ContentTypeDto? dto = await _contentTypeService.UpdateAsync(contentType.EntityId, payload);
    Assert.NotNull(dto);

    Assert.Equal(contentType.EntityId, dto.Id);
    Assert.Equal(contentType.Version + 1, dto.Version);
    Assert.Equal(Actor, dto.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dto.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(payload.IsInvariant, dto.IsInvariant);
    Assert.Equal(contentType.UniqueName.Value, dto.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), dto.DisplayName);
    Assert.Equal(contentType.Description?.Value, dto.Description);
  }
}
