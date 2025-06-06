﻿using Krakenar.Core.Contents;
using Krakenar.Core.Contents.Events;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Dictionaries.Events;
using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Logitar.EventSourcing;
using Moq;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteLanguageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<IContentQuerier> _contentQuerier = new();
  private readonly Mock<IContentRepository> _contentRepository = new();
  private readonly Mock<IDictionaryQuerier> _dictionaryQuerier = new();
  private readonly Mock<IDictionaryRepository> _dictionaryRepository = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly DeleteLanguageHandler _handler;

  public DeleteLanguageHandlerTests()
  {
    _handler = new(_applicationContext.Object, _contentQuerier.Object, _contentRepository.Object, _dictionaryQuerier.Object, _dictionaryRepository.Object, _languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "It should delete the contents associated to the language.")]
  public async Task Given_Contents_When_HandleAsync_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Language language = new(new Locale("en"), isDefault: false, actorId, LanguageId.NewId(realmId));
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    ContentType contentType = new(new Identifier("BlogArticle"), isInvariant: false, actorId, ContentTypeId.NewId(realmId));
    ContentLocale invariant = new(new UniqueName(new UniqueNameSettings(), "my-blog-article"));
    Content content = new(contentType, invariant, actorId);
    content.SetLocale(language, invariant, actorId);
    _contentQuerier.Setup(x => x.FindIdsAsync(language.Id, _cancellationToken)).ReturnsAsync([content.Id]);
    _contentRepository.Setup(x => x.LoadAsync(It.Is<IEnumerable<ContentId>>(y => y.Single() == content.Id), _cancellationToken)).ReturnsAsync([content]);

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    DeleteLanguage command = new(language.EntityId);
    LanguageDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(language.IsDeleted);
    Assert.Contains(language.Changes, change => change is LanguageDeleted deleted && deleted.ActorId == actorId);

    Assert.Empty(content.Locales);
    Assert.Contains(content.Changes, change => change is ContentLocaleRemoved removed && removed.LanguageId == language.Id && removed.ActorId == actorId);

    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
    _contentRepository.Verify(x => x.SaveAsync(It.Is<IEnumerable<Content>>(y => y.Single() == content), _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should delete the dictionary associated to the language.")]
  public async Task Given_Dictionary_When_HandleAsync_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Language language = new(new Locale("en"), isDefault: false, actorId, LanguageId.NewId(realmId));
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    Dictionary dictionary = new(language, actorId, DictionaryId.NewId(realmId));
    _dictionaryQuerier.Setup(x => x.FindIdAsync(dictionary.LanguageId, _cancellationToken)).ReturnsAsync(dictionary.Id);
    _dictionaryRepository.Setup(x => x.LoadAsync(dictionary.Id, _cancellationToken)).ReturnsAsync(dictionary);

    _contentQuerier.Setup(x => x.FindIdsAsync(language.Id, _cancellationToken)).ReturnsAsync([]);

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    DeleteLanguage command = new(language.EntityId);
    LanguageDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(language.IsDeleted);
    Assert.Contains(language.Changes, change => change is LanguageDeleted deleted && deleted.ActorId == actorId);

    Assert.True(dictionary.IsDeleted);
    Assert.Contains(dictionary.Changes, change => change is DictionaryDeleted deleted && deleted.ActorId == actorId);

    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
    _dictionaryRepository.Verify(x => x.SaveAsync(dictionary, _cancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should delete the language.")]
  public async Task Given_NotDefault_When_HandleAsync_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Language language = new(new Locale("en"), isDefault: false, actorId, LanguageId.NewId(realmId));
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    _contentQuerier.Setup(x => x.FindIdsAsync(language.Id, _cancellationToken)).ReturnsAsync([]);

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    DeleteLanguage command = new(language.EntityId);
    LanguageDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(language.IsDeleted);
    Assert.Contains(language.Changes, change => change is LanguageDeleted deleted && deleted.ActorId == actorId);

    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);

    _dictionaryRepository.Verify(x => x.LoadAsync(It.IsAny<DictionaryId>(), It.IsAny<CancellationToken>()), Times.Never);
    _dictionaryRepository.Verify(x => x.SaveAsync(It.IsAny<Dictionary>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should return null when the language was not found.")]
  public async Task Given_NotFound_When_HandleAsync_Then_NullReturned()
  {
    DeleteLanguage command = new(Guid.Empty);
    Assert.Null(await _handler.HandleAsync(command, _cancellationToken));
  }

  [Fact(DisplayName = "It should throw CannotDeleteDefaultLanguageException when the language is default.")]
  public async Task Given_IsDefault_When_HandleAsync_Then_CannotDeleteDefaultLanguageException()
  {
    Language language = new(new Locale("en"), isDefault: true);
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    DeleteLanguage command = new(language.EntityId);
    var exception = await Assert.ThrowsAsync<CannotDeleteDefaultLanguageException>(async () => await _handler.HandleAsync(command, _cancellationToken));

    Assert.Equal(language.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(language.EntityId, exception.LanguageId);
  }
}
