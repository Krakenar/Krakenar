using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Localization;
using Microsoft.Extensions.DependencyInjection;
using Content = Krakenar.Core.Contents.Content;
using ContentLocale = Krakenar.Core.Contents.ContentLocale;
using ContentType = Krakenar.Core.Contents.ContentType;

namespace Krakenar.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class PublishedContentIntegrationTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageQuerier _languageQuerier;
  private readonly ILanguageRepository _languageRepository;
  private readonly IPublishedContentQuerier _publishedContentQuerier;

  public PublishedContentIntegrationTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageQuerier = ServiceProvider.GetRequiredService<ILanguageQuerier>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _publishedContentQuerier = ServiceProvider.GetRequiredService<IPublishedContentQuerier>();
  }

  [Theory(DisplayName = "It should return the correct search results (Language.IsDefault).")]
  [InlineData(false)]
  [InlineData(true)]
  public async Task Given_LanguageIsDefault_When_SearchAsync_Then_CorrectPublishedContents(bool isDefault)
  {
    Guid defaultId = (await _languageQuerier.ReadDefaultAsync()).Id;
    LanguageId englishId = new(defaultId, Realm.Id);

    Language french = new(new Locale("fr"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(french);

    ContentType contentType = new(new Identifier("Attribute"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(contentType);

    ContentLocale english = new(new UniqueName(Realm.UniqueNameSettings, "Dexterity"));
    Content content = new(contentType, english, ActorId, ContentId.NewId(Realm.Id));
    content.SetLocale(englishId, english, ActorId);
    content.SetLocale(french, new ContentLocale(new UniqueName(Realm.UniqueNameSettings, "Adresse")), ActorId);
    content.PublishLocale(englishId, ActorId);
    content.PublishLocale(french, ActorId);
    await _contentRepository.SaveAsync(content);

    SearchPublishedContentsPayload payload = new();
    payload.Language.IsDefault = isDefault;
    SearchResults<PublishedContentLocale> results = await _publishedContentQuerier.SearchAsync(payload);

    Assert.Equal(1, results.Total);
    PublishedContentLocale publishedContent = Assert.Single(results.Items);
    Assert.Equal(content.EntityId, publishedContent.Content.Id);
    Assert.NotNull(publishedContent.Language);
    Assert.Equal(isDefault, publishedContent.Language.IsDefault);
    Assert.Equal(isDefault ? englishId.EntityId : french.EntityId, publishedContent.Language.Id);
  }
}
