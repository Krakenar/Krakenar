using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Localization;
using Krakenar.Core.Localization.Commands;
using Krakenar.Core.Localization.Queries;
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
  private readonly ILanguageRepository _languageRepository;

  private readonly ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult> _createOrReplaceLanguage;
  private readonly ICommandHandler<DeleteLanguage, LanguageDto?> _deleteLanguage;
  private readonly IQueryHandler<ReadLanguage, LanguageDto?> _readLanguage;
  private readonly IQueryHandler<SearchLanguages, SearchResults<LanguageDto>> _searchLanguages;
  private readonly ICommandHandler<SetDefaultLanguage, LanguageDto?> _setDefaultLanguage;
  private readonly ICommandHandler<UpdateLanguage, LanguageDto?> _updateLanguage;

  private readonly Language _language;

  public LanguageIntegrationTests() : base()
  {
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _createOrReplaceLanguage = ServiceProvider.GetRequiredService<ICommandHandler<CreateOrReplaceLanguage, CreateOrReplaceLanguageResult>>();
    _deleteLanguage = ServiceProvider.GetRequiredService<ICommandHandler<DeleteLanguage, LanguageDto?>>();
    _readLanguage = ServiceProvider.GetRequiredService<IQueryHandler<ReadLanguage, LanguageDto?>>();
    _searchLanguages = ServiceProvider.GetRequiredService<IQueryHandler<SearchLanguages, SearchResults<LanguageDto>>>();
    _setDefaultLanguage = ServiceProvider.GetRequiredService<ICommandHandler<SetDefaultLanguage, LanguageDto?>>();
    _updateLanguage = ServiceProvider.GetRequiredService<ICommandHandler<UpdateLanguage, LanguageDto?>>();

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
    CreateOrReplaceLanguage command = new(Guid.NewGuid(), payload, Version: null);
    CreateOrReplaceLanguageResult result = await _createOrReplaceLanguage.HandleAsync(command);
    Assert.True(result.Created);

    LanguageDto? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
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
    DeleteLanguage command = new(_language.EntityId);
    LanguageDto? language = await _deleteLanguage.HandleAsync(command);
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);

    Assert.Empty(await KrakenarContext.Languages.AsNoTracking().Where(x => x.StreamId == _language.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the language by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    ReadLanguage query = new(_language.EntityId, Locale: null, IsDefault: false);
    LanguageDto? language = await _readLanguage.HandleAsync(query);
    Assert.NotNull(language);
    Assert.Equal(query.Id, language.Id);
  }

  [Fact(DisplayName = "It should read the language by locale.")]
  public async Task Given_Locale_When_Read_Then_Found()
  {
    ReadLanguage query = new(Id: null, _language.Locale.Code, IsDefault: false);
    LanguageDto? language = await _readLanguage.HandleAsync(query);
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
    CreateOrReplaceLanguage command = new(_language.EntityId, payload, Version: null);
    CreateOrReplaceLanguageResult result = await _createOrReplaceLanguage.HandleAsync(command);
    Assert.False(result.Created);

    LanguageDto? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
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
    CreateOrReplaceLanguage command = new(_language.EntityId, payload, version);
    CreateOrReplaceLanguageResult result = await _createOrReplaceLanguage.HandleAsync(command);
    Assert.False(result.Created);

    LanguageDto? language = result.Language;
    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
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
    ReadLanguage query = new(Guid.Empty, "fr-CA", IsDefault: false);
    Assert.Null(await _readLanguage.HandleAsync(query));
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
    SearchLanguages command = new(payload);
    SearchResults<LanguageDto> results = await _searchLanguages.HandleAsync(command);
    Assert.Equal(2, results.Total);

    LanguageDto language = Assert.Single(results.Items);
    Assert.Equal(canadianEnglish.EntityId, language.Id);
  }

  [Fact(DisplayName = "It should set the default language.")]
  public async Task Given_NotDefault_When_SetDefault_Then_SetDefault()
  {
    SetDefaultLanguage command = new(_language.EntityId);
    LanguageDto? language = await _setDefaultLanguage.HandleAsync(command);

    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
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
    CreateOrReplaceLanguage command = new(Guid.NewGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<LocaleAlreadyUsedException>(async () => await _createOrReplaceLanguage.HandleAsync(command));

    Assert.Equal(Realm.Id.ToGuid(), exception.RealmId);
    Assert.Equal(command.Id, exception.LanguageId);
    Assert.Equal(_language.EntityId, exception.ConflictId);
    Assert.Equal(_language.Locale.ToString(), exception.Locale);
    Assert.Equal("Locale", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple languages were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    Language language = new(new Locale("es"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(language);

    ReadLanguage query = new(_language.EntityId, language.Locale.Code, IsDefault: true);
    var exception = await Assert.ThrowsAsync<TooManyResultsException<LanguageDto>>(async () => await _readLanguage.HandleAsync(query));
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
    UpdateLanguage command = new(_language.EntityId, payload);
    LanguageDto? language = await _updateLanguage.HandleAsync(command);

    Assert.NotNull(language);
    Assert.Equal(command.Id, language.Id);
    Assert.Equal(2, language.Version);
    Assert.Equal(Actor, language.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, language.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, language.Realm);
    Assert.False(language.IsDefault);
    Assert.Equal(payload.Locale, language.Locale.Code);
  }
}
