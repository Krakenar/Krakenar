using Krakenar.Client.Localization;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class LanguageClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public LanguageClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Languages should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient httpClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    LanguageClient languages = new(httpClient, KrakenarSettings);

    Language? @default = await languages.ReadAsync(id: null, locale: null, isDefault: true, _cancellationToken);
    Assert.NotNull(@default);
    Assert.True(@default.IsDefault);
    Assert.Equal("en", @default.Locale.Code);

    Guid id = Guid.Parse("78bddfb8-de17-4a8a-8bc8-f01ee5216cc5");
    Language? language = await languages.ReadAsync(id, locale: null, isDefault: false, _cancellationToken);

    CreateOrReplaceLanguagePayload createOrReplaceLanguage = new("fr");
    CreateOrReplaceLanguageResult languageResult = await languages.CreateOrReplaceAsync(createOrReplaceLanguage, id, version: null, _cancellationToken);
    Assert.Equal(languageResult.Created, language is null);
    language = languageResult.Language;
    Assert.NotNull(language);
    Assert.Equal(id, language.Id);
    Assert.False(language.IsDefault);
    Assert.Equal(createOrReplaceLanguage.Locale, language.Locale.Code);

    UpdateLanguagePayload updateLanguage = new()
    {
      Locale = "fr-CA"
    };
    language = await languages.UpdateAsync(id, updateLanguage, _cancellationToken);
    Assert.NotNull(language);
    Assert.Equal(updateLanguage.Locale, language.Locale.Code);

    language = await languages.SetDefaultAsync(id, _cancellationToken);
    Assert.NotNull(language);
    Assert.True(language.IsDefault);

    _ = await languages.SetDefaultAsync(@default.Id, _cancellationToken);
    language = await languages.ReadAsync(id: null, language.Locale.Code, isDefault: false, _cancellationToken);
    Assert.NotNull(language);
    Assert.Equal(id, language.Id);
    Assert.False(language.IsDefault);

    SearchLanguagesPayload searchLanguages = new()
    {
      Ids = [@default.Id, language.Id],
      Search = new TextSearch([new SearchTerm("en"), new SearchTerm("fr%")], SearchOperator.Or),
      Sort = [new LanguageSortOption(LanguageSort.Code)],
      Skip = 1,
      Limit = -1
    };
    SearchResults<Language> results = await languages.SearchAsync(searchLanguages, _cancellationToken);
    Assert.Equal(2, results.Total);
    language = Assert.Single(results.Items);
    Assert.Equal(id, language.Id);

    createOrReplaceLanguage = new("en-US");
    languageResult = await languages.CreateOrReplaceAsync(createOrReplaceLanguage, id: null, version: null, _cancellationToken);
    Assert.True(languageResult.Created);
    Assert.NotNull(languageResult.Language);

    language = await languages.DeleteAsync(languageResult.Language.Id, _cancellationToken);
    Assert.NotNull(language);
    Assert.Equal(languageResult.Language.Id, language.Id);
  }
}
