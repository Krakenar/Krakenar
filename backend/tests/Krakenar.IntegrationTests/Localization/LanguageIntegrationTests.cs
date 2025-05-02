using Krakenar.Contracts;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Localization;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Language = Krakenar.Core.Localization.Language;
using LanguageDto = Krakenar.Contracts.Localization.Language;
using Locale = Krakenar.Core.Localization.Locale;

namespace Krakenar.Localization;

[Trait(Traits.Category, Categories.Integration)]
public class LanguageIntegrationTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly ILanguageRepository _languageRepository;
  private readonly ILanguageService _languageService;

  private readonly Language _language;

  public LanguageIntegrationTests() : base()
  {
    _dictionaryRepository = ServiceProvider.GetRequiredService<IDictionaryRepository>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();
    _languageService = ServiceProvider.GetRequiredService<ILanguageService>();

    _language = new Language(new Locale("fr"), isDefault: false, actorId: null, LanguageId.NewId(Realm.Id));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync(_language);
  }

  [Fact(DisplayName = "It should create a new language.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "fr-CA"
    };
    Guid id = Guid.NewGuid();
    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    LanguageDto? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(id, language.Id);
    Assert.Equal(1, language.Version);
    Assert.Equal(Actor, language.CreatedBy);
    Assert.Equal(DateTime.UtcNow, language.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(language.CreatedOn, language.UpdatedOn);

    Assert.Equal(RealmDto, language.Realm);
    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }

  [Fact(DisplayName = "It should delete the language.")]
  public async Task Given_Language_When_Delete_Then_Deleted()
  {
    Dictionary dictionary = new(_language, ActorId, DictionaryId.NewId(Realm.Id));
    await _dictionaryRepository.SaveAsync(dictionary);

    LanguageDto? language = await _languageService.DeleteAsync(_language.EntityId);
    Assert.NotNull(language);
    Assert.Equal(_language.EntityId, language.Id);

    Assert.Empty(await KrakenarContext.Languages.AsNoTracking().Where(x => x.StreamId == _language.Id.Value).ToArrayAsync());
    Assert.Empty(await KrakenarContext.Dictionaries.AsNoTracking().Include(x => x.Language).Where(x => x.Language!.StreamId == _language.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the language by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    LanguageDto? language = await _languageService.ReadAsync(_language.EntityId);
    Assert.NotNull(language);
    Assert.Equal(_language.EntityId, language.Id);
  }

  [Fact(DisplayName = "It should read the language by locale.")]
  public async Task Given_Locale_When_Read_Then_Found()
  {
    LanguageDto? language = await _languageService.ReadAsync(id: null, _language.Locale.Code);
    Assert.NotNull(language);
    Assert.Equal(_language.EntityId, language.Id);
  }

  [Fact(DisplayName = "It should replace an existing language.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "fr-CA"
    };
    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, _language.EntityId);
    Assert.False(result.Created);

    LanguageDto? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(_language.EntityId, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, language.Realm);
    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }

  [Fact(DisplayName = "It should replace an existing language from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    long version = _language.Version;

    _language.SetLocale(new Locale("fr-CA"), ActorId);
    await _languageRepository.SaveAsync(_language);

    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = "fr-CA"
    };
    CreateOrReplaceLanguageResult result = await _languageService.CreateOrReplaceAsync(payload, _language.EntityId, version);
    Assert.False(result.Created);

    LanguageDto? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(_language.EntityId, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, language.Realm);
    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }

  [Fact(DisplayName = "It should return null when the language cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _languageService.ReadAsync(Guid.Empty, "fr-CA"));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Languages_When_Search_Then_CorrectResults()
  {
    Language canadianEnglish = new(new Locale("en-CA"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    Language canadianFrench = new(new Locale("fr-CA"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync([canadianEnglish, canadianFrench]);

    SearchLanguagesPayload payload = new()
    {
      Ids = [_language.EntityId, canadianEnglish.EntityId, canadianFrench.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("en"), new SearchTerm("__-CA")], SearchOperator.Or),
      Sort = [new LanguageSortOption(LanguageSort.DisplayName, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<LanguageDto> results = await _languageService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    LanguageDto language = Assert.Single(results.Items);
    Assert.Equal(canadianEnglish.EntityId, language.Id);
  }

  [Fact(DisplayName = "It should set the default language.")]
  public async Task Given_NotDefault_When_SetDefault_Then_SetDefault()
  {
    LanguageDto? language = await _languageService.SetDefaultAsync(_language.EntityId);

    Assert.NotNull(language);
    Assert.Equal(_language.EntityId, language.Id);
    Assert.Equal(_language.Version + 1, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn, TimeSpan.FromSeconds(10));
    Assert.True(language.IsDefault);
  }

  [Fact(DisplayName = "It should throw LocaleAlreadyUsedException when a locale conflict occurs.")]
  public async Task Given_LocaleConflict_When_CreateOrReplace_Then_LocaleAlreadyUsedException()
  {
    CreateOrReplaceLanguagePayload payload = new()
    {
      Locale = _language.Locale.Code
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _languageService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(Realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal(id, exception.LanguageId);
    Assert.Equal(_language.EntityId, exception.ConflictId);
    Assert.Equal(_language.Locale.ToString(), exception.Locale);
    Assert.Equal("Locale", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple languages were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    Language language = new(new Locale("es"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<LanguageDto>>(async () => await _languageService.ReadAsync(_language.EntityId, language.Locale.Code, isDefault: true));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(3, exception.ActualCount);
  }

  [Fact(DisplayName = "It should update an existing language.")]
  public async Task Given_Language_When_Update_Then_Updated()
  {
    UpdateLanguagePayload payload = new()
    {
      Locale = "fr-CA"
    };
    LanguageDto? language = await _languageService.UpdateAsync(_language.EntityId, payload);

    Assert.NotNull(language);
    Assert.Equal(_language.EntityId, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, language.Realm);
    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }
}
