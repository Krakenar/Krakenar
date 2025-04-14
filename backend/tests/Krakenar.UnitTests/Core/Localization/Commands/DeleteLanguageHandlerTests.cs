using Krakenar.Core.Localization.Events;
using Krakenar.Core.Realms;
using Logitar.EventSourcing;
using Moq;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

[Trait(Traits.Category, Categories.Unit)]
public class DeleteLanguageHandlerTests
{
  private readonly CancellationToken _cancellationToken = default;

  private readonly Mock<IApplicationContext> _applicationContext = new();
  private readonly Mock<ILanguageQuerier> _languageQuerier = new();
  private readonly Mock<ILanguageRepository> _languageRepository = new();

  private readonly DeleteLanguageHandler _handler;

  public DeleteLanguageHandlerTests()
  {
    _handler = new(_applicationContext.Object, _languageQuerier.Object, _languageRepository.Object);
  }

  [Fact(DisplayName = "It should delete the language.")]
  public async Task Givne_NotDefault_When_HandleAsync_Then_Deleted()
  {
    ActorId actorId = ActorId.NewId();
    _applicationContext.SetupGet(x => x.ActorId).Returns(actorId);

    RealmId realmId = RealmId.NewId();
    _applicationContext.SetupGet(x => x.RealmId).Returns(realmId);

    Language language = new(new Locale("en"), isDefault: false, actorId, LanguageId.NewId(realmId));
    _languageRepository.Setup(x => x.LoadAsync(language.Id, _cancellationToken)).ReturnsAsync(language);

    LanguageDto dto = new();
    _languageQuerier.Setup(x => x.ReadAsync(language, _cancellationToken)).ReturnsAsync(dto);

    DeleteLanguage command = new(language.EntityId);
    LanguageDto? result = await _handler.HandleAsync(command, _cancellationToken);
    Assert.NotNull(result);
    Assert.Same(dto, result);

    Assert.True(language.IsDeleted);
    Assert.Contains(language.Changes, change => change is LanguageDeleted deleted && deleted.ActorId == actorId);

    _languageRepository.Verify(x => x.SaveAsync(language, _cancellationToken), Times.Once);
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
