﻿using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Contents;
using Krakenar.Core.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Content = Krakenar.Core.Contents.Content;
using ContentDto = Krakenar.Contracts.Contents.Content;
using ContentLocale = Krakenar.Core.Contents.ContentLocale;
using ContentLocaleDto = Krakenar.Contracts.Contents.ContentLocale;
using ContentType = Krakenar.Core.Contents.ContentType;

namespace Krakenar.Contents;

[Trait(Traits.Category, Categories.Integration)]
public class ContentIntegrationTests : IntegrationTests
{
  private readonly IContentRepository _contentRepository;
  private readonly IContentService _contentService;
  private readonly IContentTypeRepository _contentTypeRepository;
  private readonly ILanguageRepository _languageRepository;

  public ContentIntegrationTests() : base()
  {
    _contentRepository = ServiceProvider.GetRequiredService<IContentRepository>();
    _contentService = ServiceProvider.GetRequiredService<IContentService>();
    _contentTypeRepository = ServiceProvider.GetRequiredService<IContentTypeRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();
  }

  [Fact(DisplayName = "It should create a new invariant content.")]
  public async Task Given_Invariant_When_Create_Then_Created()
  {
    ContentType blogCategory = new(new Identifier("BlogCategory"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogCategory);

    CreateContentPayload payload = new("software-archicture", $" {blogCategory.EntityId} ");
    ContentDto content = await _contentService.CreateAsync(payload);

    Assert.NotEqual(Guid.Empty, content.Id);
    Assert.Equal(1, content.Version);
    Assert.Equal(Actor, content.CreatedBy);
    Assert.Equal(DateTime.UtcNow, content.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(blogCategory.EntityId, content.ContentType.Id);

    ContentLocaleDto invariant = content.Invariant;
    Assert.Same(content, invariant.Content);
    Assert.Null(invariant.Language);
    Assert.Equal(payload.UniqueName, invariant.UniqueName);
    Assert.Null(invariant.DisplayName);
    Assert.Null(invariant.Description);
    Assert.Equal(Actor, invariant.CreatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Empty(content.Locales);
  }

  [Fact(DisplayName = "It should create a new localized content.")]
  public async Task Given_Localized_When_Create_Then_Created()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    CreateContentPayload payload = new("the-clean-architecture", $"  {blogArticle.UniqueName}  ")
    {
      Id = Guid.NewGuid(),
      Language = $" {language.EntityId} ",
      DisplayName = " The Clean Architecture ",
      Description = "  Over the last several years we’ve seen a whole range of ideas regarding the architecture of systems. These include:  "
    };
    ContentDto content = await _contentService.CreateAsync(payload);

    Assert.Equal(payload.Id.Value, content.Id);
    Assert.Equal(2, content.Version);
    Assert.Equal(Actor, content.CreatedBy);
    Assert.Equal(DateTime.UtcNow, content.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromSeconds(10));

    Assert.Equal(blogArticle.EntityId, content.ContentType.Id);

    ContentLocaleDto invariant = content.Invariant;
    Assert.Same(content, invariant.Content);
    Assert.Null(invariant.Language);
    Assert.Equal(payload.UniqueName, invariant.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), invariant.DisplayName);
    Assert.Equal(payload.Description.Trim(), invariant.Description);
    Assert.Equal(Actor, invariant.CreatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, invariant.UpdatedOn, TimeSpan.FromSeconds(10));

    ContentLocaleDto locale = Assert.Single(content.Locales);
    Assert.Same(content, locale.Content);
    Assert.Equal(language.EntityId, locale.Language?.Id);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), locale.DisplayName);
    Assert.Equal(payload.Description.Trim(), locale.Description);
    Assert.Equal(Actor, locale.CreatedBy);
    Assert.Equal(DateTime.UtcNow, locale.CreatedOn, TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, invariant.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, locale.UpdatedOn, TimeSpan.FromSeconds(10));
  }

  [Fact(DisplayName = "It should delete an existing content.")]
  public async Task Given_Invariant_When_Delete_Then_Deleted()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content article = new(blogArticle, invariant, ActorId);
    article.SetLocale(language, invariant, ActorId);
    await _contentRepository.SaveAsync(article);

    ContentDto? content = await _contentService.DeleteAsync(article.EntityId);
    Assert.NotNull(content);
    Assert.Equal(article.EntityId, content.Id);

    Assert.Empty(await KrakenarContext.Contents.AsNoTracking().Where(x => x.StreamId == article.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read an existing content by ID.")]
  public async Task Given_Id_When_Read_Then_Returned()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content article = new(blogArticle, invariant, ActorId);
    article.SetLocale(language, invariant, ActorId);
    await _contentRepository.SaveAsync(article);

    ContentDto? content = await _contentService.ReadAsync(article.EntityId);
    Assert.NotNull(content);
    Assert.Equal(article.EntityId, content.Id);
  }

  [Fact(DisplayName = "It should remove an existing content locale.")]
  public async Task Given_Locale_When_Save_Then_Removed()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content article = new(blogArticle, invariant, ActorId);
    article.SetLocale(language, invariant, ActorId);
    await _contentRepository.SaveAsync(article);

    ContentDto? content = await _contentService.DeleteAsync(article.EntityId, $" {language.Locale.Code} ");
    Assert.NotNull(content);
    Assert.Equal(article.EntityId, content.Id);
    Assert.Equal(3, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromSeconds(10));

    ContentLocaleDto invariantDto = content.Invariant;
    Assert.Equal(invariant.UniqueName.Value, invariantDto.UniqueName);
    Assert.Equal(invariant.DisplayName?.Value, invariantDto.DisplayName);
    Assert.Equal(invariant.Description?.Value, invariantDto.Description);

    Assert.Empty(content.Locales);
  }

  [Fact(DisplayName = "It should replace an existing content invariant.")]
  public async Task Given_Invariant_When_Save_Then_Replaced()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content article = new(blogArticle, invariant, ActorId);
    article.SetLocale(language, invariant, ActorId);
    await _contentRepository.SaveAsync(article);

    SaveContentLocalePayload payload = new("The_Clean_Architecture")
    {
      DisplayName = " The Clean Architecture ",
      Description = "  Over the last several years we’ve seen a whole range of ideas regarding the architecture of systems. These include:  "
    };
    ContentDto? content = await _contentService.SaveLocaleAsync(article.EntityId, payload);
    Assert.NotNull(content);
    Assert.Equal(article.EntityId, content.Id);
    Assert.Equal(3, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromSeconds(10));

    ContentLocaleDto invariantDto = content.Invariant;
    Assert.Equal(payload.UniqueName, invariantDto.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), invariantDto.DisplayName);
    Assert.Equal(payload.Description.Trim(), invariantDto.Description);

    ContentLocaleDto locale = Assert.Single(content.Locales);
    Assert.Equal(invariant.UniqueName.Value, locale.UniqueName);
    Assert.Equal(invariant.DisplayName?.Value, locale.DisplayName);
    Assert.Equal(invariant.Description?.Value, locale.Description);
  }

  [Fact(DisplayName = "It should replace an existing content locale.")]
  public async Task Given_Locale_When_Save_Then_Replaced()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content article = new(blogArticle, invariant, ActorId);
    await _contentRepository.SaveAsync(article);

    SaveContentLocalePayload payload = new("The_Clean_Architecture")
    {
      DisplayName = " The Clean Architecture ",
      Description = "  Over the last several years we’ve seen a whole range of ideas regarding the architecture of systems. These include:  "
    };
    ContentDto? content = await _contentService.SaveLocaleAsync(article.EntityId, payload, language.EntityId.ToString());
    Assert.NotNull(content);
    Assert.Equal(article.EntityId, content.Id);
    Assert.Equal(2, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromSeconds(10));

    ContentLocaleDto invariantDto = content.Invariant;
    Assert.Equal(invariant.UniqueName.Value, invariantDto.UniqueName);
    Assert.Equal(invariant.DisplayName?.Value, invariantDto.DisplayName);
    Assert.Equal(invariant.Description?.Value, invariantDto.Description);

    ContentLocaleDto locale = Assert.Single(content.Locales);
    Assert.Equal(payload.UniqueName, locale.UniqueName);
    Assert.Equal(payload.DisplayName.Trim(), locale.DisplayName);
    Assert.Equal(payload.Description.Trim(), locale.Description);
  }

  [Fact(DisplayName = "It should return null when the content could not be found.")]
  public async Task Given_NotFound_When_Delete_Then_NullReturned()
  {
    Assert.Null(await _contentService.ReadAsync(Guid.Empty));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_ContentLocales_When_Search_Then_CorrectResults()
  {
    Language french = new(new Locale("fr"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(french);

    ContentType blogCategory = new(new Identifier("BlogCategory"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync([blogCategory, blogArticle]);

    Content softwareArchitecture = new(blogCategory, new ContentLocale(new UniqueName(Realm.UniqueNameSettings, "software-architecture")), ActorId);
    Content cleanCode = new(blogArticle, new ContentLocale(new UniqueName(Realm.UniqueNameSettings, "the-clean-code")), ActorId);
    Content onion = new(blogArticle, new ContentLocale(new UniqueName(Realm.UniqueNameSettings, "onion-architecture")), ActorId);
    Content screaming = new(blogArticle, new ContentLocale(new UniqueName(Realm.UniqueNameSettings, "screaming-architecture")), ActorId);
    Content hexagonal = new(blogArticle, new ContentLocale(new UniqueName(Realm.UniqueNameSettings, "hexagonal-architecture")), ActorId);
    hexagonal.SetLocale(french, new ContentLocale(new UniqueName(Realm.UniqueNameSettings, "architecture-hexagonale")), ActorId);
    await _contentRepository.SaveAsync([softwareArchitecture, cleanCode, onion, screaming, hexagonal]);

    SearchContentLocalesPayload payload = new()
    {
      ContentTypeId = blogArticle.EntityId,
      LanguageId = null,
      Ids = [softwareArchitecture.EntityId, cleanCode.EntityId, onion.EntityId, hexagonal.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("%architecture%")]),
      Sort = [new ContentSortOption(ContentSort.UniqueName, isDescending: true)],
      Limit = 1,
      Skip = 1
    };
    SearchResults<ContentLocaleDto> results = await _contentService.SearchLocalesAsync(payload);
    Assert.Equal(2, results.Total);

    ContentLocaleDto locale = Assert.Single(results.Items);
    Assert.Equal(hexagonal.EntityId, locale.Content?.Id);
    Assert.Null(locale.Language);
  }

  [Fact(DisplayName = "It should throw ContentUniqueNameAlreadyUsedException when there is a content invariant unique name conflict.")]
  public async Task Given_InvariantUniqueNameConflict_When_Save_Then_ContentUniqueNameAlreadyUsedException()
  {
    ContentType blogCategory = new(new Identifier("BlogCategory"), isInvariant: true, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogCategory);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "architecture-logicielle"));
    Content content = new(blogCategory, invariant, ActorId);
    await _contentRepository.SaveAsync(content);

    CreateContentPayload payload = new(invariant.UniqueName.Value, $" {blogCategory.EntityId} ");
    var exception = await Assert.ThrowsAsync<ContentUniqueNameAlreadyUsedException>(async () => await _contentService.CreateAsync(payload));
    Assert.Equal(Realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal(blogCategory.EntityId, exception.ContentTypeId);
    Assert.NotEqual(Guid.Empty, exception.ContentId);
    Assert.Null(exception.LanguageId);
    Assert.Equal(content.EntityId, exception.ConflictId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ContentUniqueNameAlreadyUsedException when there is a content locale unique name conflict.")]
  public async Task Given_LocaleUniqueNameConflict_When_Save_Then_ContentUniqueNameAlreadyUsedException()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content content = new(blogArticle, invariant, ActorId);
    ContentLocale locale = new(new UniqueName(Realm.UniqueNameSettings, "software-architecture"));
    content.SetLocale(language, locale, ActorId);
    await _contentRepository.SaveAsync(content);

    CreateContentPayload payload = new(locale.UniqueName.Value, $"  {blogArticle.UniqueName}  ")
    {
      Id = Guid.NewGuid(),
      Language = $"  {language.Locale.Code}  "
    };
    var exception = await Assert.ThrowsAsync<ContentUniqueNameAlreadyUsedException>(async () => await _contentService.CreateAsync(payload));
    Assert.Equal(Realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal(blogArticle.EntityId, exception.ContentTypeId);
    Assert.Equal(payload.Id.Value, exception.ContentId);
    Assert.Equal(language.EntityId, exception.LanguageId);
    Assert.Equal(content.EntityId, exception.ConflictId);
    Assert.Equal(payload.UniqueName, exception.UniqueName);
    Assert.Equal("UniqueName", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing content invariant.")]
  public async Task Given_Invariant_When_Save_Then_Updated()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content article = new(blogArticle, invariant, ActorId);
    article.SetLocale(language, invariant, ActorId);
    await _contentRepository.SaveAsync(article);

    UpdateContentLocalePayload payload = new()
    {
      DisplayName = new Contracts.Change<string>(" The Clean Architecture "),
      Description = new Contracts.Change<string>("  Over the last several years we’ve seen a whole range of ideas regarding the architecture of systems. These include:  ")
    };
    ContentDto? content = await _contentService.UpdateLocaleAsync(article.EntityId, payload);
    Assert.NotNull(content);
    Assert.Equal(article.EntityId, content.Id);
    Assert.Equal(3, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromSeconds(10));

    ContentLocaleDto invariantDto = content.Invariant;
    Assert.Equal(invariant.UniqueName.Value, invariantDto.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), invariantDto.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), invariantDto.Description);

    ContentLocaleDto locale = Assert.Single(content.Locales);
    Assert.Equal(invariant.UniqueName.Value, locale.UniqueName);
    Assert.Equal(invariant.DisplayName?.Value, locale.DisplayName);
    Assert.Equal(invariant.Description?.Value, locale.Description);
  }

  [Fact(DisplayName = "It should update an existing content locale.")]
  public async Task Given_Locale_When_Save_Then_Updated()
  {
    Language language = new(new Locale("en-US"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ContentType blogArticle = new(new Identifier("BlogArticle"), isInvariant: false, ActorId, ContentTypeId.NewId(Realm.Id));
    await _contentTypeRepository.SaveAsync(blogArticle);

    ContentLocale invariant = new(new UniqueName(Realm.UniqueNameSettings, "the-clean-architecture"));
    Content article = new(blogArticle, invariant, ActorId);
    article.SetLocale(language, invariant, ActorId);
    await _contentRepository.SaveAsync(article);

    UpdateContentLocalePayload payload = new()
    {
      DisplayName = new Contracts.Change<string>(" The Clean Architecture "),
      Description = new Contracts.Change<string>("  Over the last several years we’ve seen a whole range of ideas regarding the architecture of systems. These include:  ")
    };
    ContentDto? content = await _contentService.UpdateLocaleAsync(article.EntityId, payload, language.Locale.Code);
    Assert.NotNull(content);
    Assert.Equal(article.EntityId, content.Id);
    Assert.Equal(3, content.Version);
    Assert.Equal(Actor, content.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, content.UpdatedOn, TimeSpan.FromSeconds(10));

    ContentLocaleDto invariantDto = content.Invariant;
    Assert.Equal(invariant.UniqueName.Value, invariantDto.UniqueName);
    Assert.Equal(invariant.DisplayName?.Value, invariantDto.DisplayName);
    Assert.Equal(invariant.Description?.Value, invariantDto.Description);

    ContentLocaleDto locale = Assert.Single(content.Locales);
    Assert.Equal(invariant.UniqueName.Value, locale.UniqueName);
    Assert.Equal(payload.DisplayName.Value?.Trim(), locale.DisplayName);
    Assert.Equal(payload.Description.Value?.Trim(), locale.Description);
  }
}
