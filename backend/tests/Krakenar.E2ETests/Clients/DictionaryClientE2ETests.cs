using Krakenar.Client.Dictionaries;
using Krakenar.Client.Localization;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;

namespace Krakenar.Clients;

[Trait(Traits.Category, Categories.EndToEnd)]
public class DictionaryClientE2ETests : E2ETests
{
  private readonly CancellationToken _cancellationToken = default;

  public DictionaryClientE2ETests() : base()
  {
  }

  [Fact(DisplayName = "Dictionaries should be managed correctly through the API client.")]
  public async Task Given_Realm_When_Client_Then_Success()
  {
    await InitializeRealmAsync(_cancellationToken);

    using HttpClient languageClient = new();
    using HttpClient dictionaryClient = new();
    KrakenarSettings.Realm = Realm.UniqueSlug;
    LanguageClient languages = new(languageClient, KrakenarSettings);
    DictionaryClient dictionaries = new(dictionaryClient, KrakenarSettings);

    Guid frenchId = Guid.Parse("69e7eeda-0d03-4f95-95ee-4ec6ffa866a9");
    CreateOrReplaceLanguagePayload createOrReplaceLanguage = new("fr");
    CreateOrReplaceLanguageResult languageResult = await languages.CreateOrReplaceAsync(createOrReplaceLanguage, frenchId, version: null, _cancellationToken);
    Language? french = languageResult.Language;
    Assert.NotNull(french);

    Guid spanishId = Guid.Parse("6c820639-f75a-4bbd-abee-c5ece0e1f05e");
    createOrReplaceLanguage = new("es");
    languageResult = await languages.CreateOrReplaceAsync(createOrReplaceLanguage, spanishId, version: null, _cancellationToken);
    Language? spanish = languageResult.Language;
    Assert.NotNull(spanish);

    Language? german = await languages.ReadAsync(id: null, "de", isDefault: false, _cancellationToken);
    if (german is null)
    {
      createOrReplaceLanguage = new("de");
      languageResult = await languages.CreateOrReplaceAsync(createOrReplaceLanguage, id: null, version: null, _cancellationToken);
      german = languageResult.Language;
      Assert.NotNull(german);
    }

    Guid id = Guid.Parse("cd1ac3dd-c675-416e-91fa-495c8136b052");
    Dictionary? dictionary = await dictionaries.ReadAsync(id, languageId: null, _cancellationToken);

    CreateOrReplaceDictionaryPayload createOrReplaceDictionary = new(french.Id.ToString());
    CreateOrReplaceDictionaryResult dictionaryResult = await dictionaries.CreateOrReplaceAsync(createOrReplaceDictionary, id, version: null, _cancellationToken);
    Assert.Equal(dictionaryResult.Created, dictionary is null);
    dictionary = dictionaryResult.Dictionary;
    Assert.NotNull(dictionary);
    Assert.Equal(id, dictionary.Id);
    Assert.Equal(createOrReplaceDictionary.Language, dictionary.Language.Id.ToString());

    UpdateDictionaryPayload updateDictionary = new();
    updateDictionary.Entries.Add(new DictionaryEntry("Blue", "Bleu"));
    updateDictionary.Entries.Add(new DictionaryEntry("Green", "Vert"));
    updateDictionary.Entries.Add(new DictionaryEntry("Red", "Rouge"));
    dictionary = await dictionaries.UpdateAsync(id, updateDictionary, _cancellationToken);
    Assert.NotNull(dictionary);
    Assert.Equal(updateDictionary.Entries, dictionary.Entries);

    dictionary = await dictionaries.ReadAsync(id: null, dictionary.Language.Id, _cancellationToken);
    Assert.NotNull(dictionary);
    Assert.Equal(id, dictionary.Id);

    SearchDictionariesPayload searchDictionaries = new()
    {
      Ids = [dictionary.Id],
      Search = new TextSearch([new SearchTerm("%ançai%")])
    };
    SearchResults<Dictionary> results = await dictionaries.SearchAsync(searchDictionaries, _cancellationToken);
    Assert.Equal(1, results.Total);
    dictionary = Assert.Single(results.Items);
    Assert.Equal(id, dictionary.Id);

    createOrReplaceDictionary = new(spanish.Locale.Code);
    dictionaryResult = await dictionaries.CreateOrReplaceAsync(createOrReplaceDictionary, id: null, version: null, _cancellationToken);
    Assert.True(dictionaryResult.Created);
    Assert.NotNull(dictionaryResult.Dictionary);

    dictionary = await dictionaries.DeleteAsync(dictionaryResult.Dictionary.Id, _cancellationToken);
    Assert.NotNull(dictionary);
    Assert.Equal(dictionaryResult.Dictionary.Id, dictionary.Id);

    createOrReplaceDictionary = new(german.Id.ToString());
    dictionaryResult = await dictionaries.CreateOrReplaceAsync(createOrReplaceDictionary, id: null, version: null, _cancellationToken);
    Assert.True(dictionaryResult.Created);
    dictionary = dictionaryResult.Dictionary;
    Assert.NotNull(dictionary);

    german = await languages.DeleteAsync(german.Id, _cancellationToken);
    Assert.NotNull(german);

    dictionary = await dictionaries.ReadAsync(dictionary.Id, german.Id, _cancellationToken);
    Assert.Null(dictionary);
  }
}
