using Krakenar.Contracts;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Krakenar.Core;
using Krakenar.Core.Dictionaries;
using Krakenar.Core.Localization;
using Logitar;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Dictionary = Krakenar.Core.Dictionaries.Dictionary;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Dictionaries;

[Trait(Traits.Category, Categories.Integration)]
public class DictionaryIntegrationTests : IntegrationTests
{
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IDictionaryService _dictionaryService;
  private readonly ILanguageRepository _languageRepository;

  private readonly Language _french;
  private readonly Dictionary _dictionary;

  public DictionaryIntegrationTests() : base()
  {
    _dictionaryRepository = ServiceProvider.GetRequiredService<IDictionaryRepository>();
    _dictionaryService = ServiceProvider.GetRequiredService<IDictionaryService>();
    _languageRepository = ServiceProvider.GetRequiredService<ILanguageRepository>();

    _french = new(new Locale("fr"), isDefault: false, actorId: null, LanguageId.NewId(Realm.Id));
    _dictionary = new(_french, actorId: null, DictionaryId.NewId(Realm.Id));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _languageRepository.SaveAsync(_french);
    await _dictionaryRepository.SaveAsync(_dictionary);
  }

  [Fact(DisplayName = "It should create a new dictionary.")]
  public async Task Given_NotExist_When_CreateOrReplace_Then_Created()
  {
    Language spanish = new(new Locale("es"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(spanish);

    CreateOrReplaceDictionaryPayload payload = new()
    {
      Language = spanish.Locale.Code
    };
    payload.Entries.Add(new DictionaryEntry("Blue", "Azul"));
    payload.Entries.Add(new DictionaryEntry("Green", "Verde"));
    payload.Entries.Add(new DictionaryEntry("Red", "Rojo"));

    Guid id = Guid.NewGuid();
    CreateOrReplaceDictionaryResult result = await _dictionaryService.CreateOrReplaceAsync(payload, id);
    Assert.True(result.Created);

    DictionaryDto? dictionary = result.Dictionary;
    Assert.NotNull(dictionary);
    Assert.Equal(id, dictionary.Id);
    Assert.Equal(2, dictionary.Version);
    Assert.Equal(Actor, dictionary.CreatedBy);
    Assert.Equal(DateTime.UtcNow, dictionary.CreatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));
    Assert.Equal(Actor, dictionary.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dictionary.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(RealmDto, dictionary.Language.Realm);
    Assert.Equal(spanish.Id.EntityId, dictionary.Language.Id);
    Assert.Equal(dictionary.Entries.Count, dictionary.EntryCount);
    Assert.Equal(payload.Entries, dictionary.Entries);
  }

  [Fact(DisplayName = "It should delete the dictionary.")]
  public async Task Given_Dictionary_When_Delete_Then_Deleted()
  {
    DictionaryDto? dictionary = await _dictionaryService.DeleteAsync(_dictionary.EntityId);
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.EntityId, dictionary.Id);

    Assert.Empty(await KrakenarContext.Dictionaries.AsNoTracking().Where(x => x.StreamId == _dictionary.Id.Value).ToArrayAsync());
  }

  [Fact(DisplayName = "It should read the dictionary by ID.")]
  public async Task Given_Id_When_Read_Then_Found()
  {
    DictionaryDto? dictionary = await _dictionaryService.ReadAsync(_dictionary.EntityId);
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.EntityId, dictionary.Id);
  }

  [Fact(DisplayName = "It should read the dictionary by language ID.")]
  public async Task Given_LanguageId_When_Read_Then_Found()
  {
    DictionaryDto? dictionary = await _dictionaryService.ReadAsync(id: null, _french.EntityId);
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.EntityId, dictionary.Id);
  }

  [Fact(DisplayName = "It should replace an existing dictionary.")]
  public async Task Given_NoVersion_When_CreateOrReplace_Then_Replaced()
  {
    Language spanish = new(new Locale("es"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(spanish);

    _dictionary.SetEntry(new Identifier("Blue"), "Azul");
    _dictionary.SetEntry(new Identifier("Green"), "Verde");
    _dictionary.SetEntry(new Identifier("Red"), "Rojo");
    _dictionary.Update(ActorId);
    await _dictionaryRepository.SaveAsync(_dictionary);

    CreateOrReplaceDictionaryPayload payload = new()
    {
      Language = spanish.EntityId.ToString()
    };
    payload.Entries.Add(new DictionaryEntry("HelloWorld", "¡Hola Mundo!"));

    CreateOrReplaceDictionaryResult result = await _dictionaryService.CreateOrReplaceAsync(payload, _dictionary.EntityId);
    Assert.False(result.Created);

    DictionaryDto? dictionary = result.Dictionary;
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.EntityId, dictionary.Id);
    Assert.Equal(_dictionary.Version + 2, dictionary.Version);
    Assert.Equal(Actor, dictionary.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dictionary.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(spanish.EntityId, dictionary.Language.Id);
    Assert.Equal(dictionary.Entries.Count, dictionary.Entries.Count);
    Assert.Equal(payload.Entries, dictionary.Entries);
  }

  [Fact(DisplayName = "It should replace an existing dictionary from reference.")]
  public async Task Given_Version_When_CreateOrReplace_Then_Replaced()
  {
    _dictionary.SetEntry(new Identifier("Blue"), "Bleu");
    _dictionary.SetEntry(new Identifier("Green"), "Vert");
    _dictionary.SetEntry(new Identifier("Red"), "Rojo");
    _dictionary.SetEntry(new Identifier("Yellow"), "Jaune");
    _dictionary.Update(ActorId);
    long version = _dictionary.Version;

    _dictionary.RemoveEntry(new Identifier("Yellow"));
    _dictionary.SetEntry(new Identifier("Silver"), "Argent");
    _dictionary.SetEntry(new Identifier("Red"), "Red");
    _dictionary.Update(ActorId);
    await _dictionaryRepository.SaveAsync(_dictionary);

    CreateOrReplaceDictionaryPayload payload = new()
    {
      Language = _french.EntityId.ToString()
    };
    payload.Entries.Add(new DictionaryEntry("Green", "Vert"));
    payload.Entries.Add(new DictionaryEntry("Red", "Rouge"));
    payload.Entries.Add(new DictionaryEntry("White", "Blanc"));
    payload.Entries.Add(new DictionaryEntry("Yellow", "Jaune"));

    CreateOrReplaceDictionaryResult result = await _dictionaryService.CreateOrReplaceAsync(payload, _dictionary.EntityId, version);
    Assert.False(result.Created);

    DictionaryDto? dictionary = result.Dictionary;
    Assert.NotNull(dictionary);
    Assert.Equal(_dictionary.EntityId, dictionary.Id);
    Assert.Equal(_dictionary.Version + 1, dictionary.Version);
    Assert.Equal(Actor, dictionary.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dictionary.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(_french.EntityId, dictionary.Language.Id);
    Assert.Equal(dictionary.Entries.Count, dictionary.EntryCount);
    Assert.Equal(4, dictionary.Entries.Count);
    Assert.Contains(dictionary.Entries, e => e.Key == "Green" && e.Value == "Vert");
    Assert.Contains(dictionary.Entries, e => e.Key == "Red" && e.Value == "Rouge");
    Assert.Contains(dictionary.Entries, e => e.Key == "Silver" && e.Value == "Argent");
    Assert.Contains(dictionary.Entries, e => e.Key == "White" && e.Value == "Blanc");
  }

  [Fact(DisplayName = "It should return null when the dictionary cannot be found.")]
  public async Task Given_NotFound_When_Read_Then_NullReturned()
  {
    Assert.Null(await _dictionaryService.ReadAsync(Guid.Empty, Guid.Empty));
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task Given_Dictionaries_When_Search_Then_CorrectResults()
  {
    ILanguageQuerier languageQuerier = ServiceProvider.GetRequiredService<ILanguageQuerier>();
    LanguageId defaultId = await languageQuerier.FindDefaultIdAsync();
    Language? english = await _languageRepository.LoadAsync(defaultId);
    Assert.NotNull(english);

    Language german = new(new Locale("de"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    Language spanish = new(new Locale("es"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync([german, spanish]);

    _dictionary.SetEntry(new Identifier("Blue"), "Bleu");
    _dictionary.SetEntry(new Identifier("Green"), "Vert");
    _dictionary.SetEntry(new Identifier("Red"), "Rouge");

    Dictionary englishDictionary = new(english, ActorId, DictionaryId.NewId(Realm.Id));
    Dictionary germanDictionary = new(german, ActorId, DictionaryId.NewId(Realm.Id));
    Dictionary spanishDictionary = new(spanish, ActorId, DictionaryId.NewId(Realm.Id));
    await _dictionaryRepository.SaveAsync([_dictionary, englishDictionary, germanDictionary, spanishDictionary]);

    SearchDictionariesPayload payload = new()
    {
      Ids = [_dictionary.EntityId, englishDictionary.EntityId, germanDictionary.EntityId, Guid.Empty],
      Search = new TextSearch([new SearchTerm("English"), new SearchTerm("français"), new SearchTerm("es")], SearchOperator.Or),
      Sort = [new DictionarySortOption(DictionarySort.Language, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchResults<DictionaryDto> results = await _dictionaryService.SearchAsync(payload);
    Assert.Equal(2, results.Total);

    DictionaryDto dictionary = Assert.Single(results.Items);
    Assert.Equal(englishDictionary.EntityId, dictionary.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when multiple dictionaries were read.")]
  public async Task Given_MultipleResults_When_Read_Then_TooManyResultsException()
  {
    Language spanish = new(new Locale("es"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(spanish);

    Dictionary dictionary = new(spanish, ActorId, DictionaryId.NewId(Realm.Id));
    await _dictionaryRepository.SaveAsync(dictionary);

    var exception = await Assert.ThrowsAsync<TooManyResultsException<DictionaryDto>>(async () => await _dictionaryService.ReadAsync(_dictionary.EntityId, spanish.EntityId));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }

  [Fact(DisplayName = "It should throw LanguageAlreadyUsedException when a language conflict occurs.")]
  public async Task Given_LanguageConflict_When_CreateOrReplace_Then_LanguageAlreadyUsedException()
  {
    CreateOrReplaceDictionaryPayload payload = new()
    {
      Language = _french.EntityId.ToString()
    };
    Guid id = Guid.NewGuid();
    var exception = await Assert.ThrowsAsync<LanguageAlreadyUsedException>(async () => await _dictionaryService.CreateOrReplaceAsync(payload, id));

    Assert.Equal(_dictionary.RealmId?.ToGuid(), exception.RealmId);
    Assert.Equal(id, exception.DictionaryId);
    Assert.Equal(_dictionary.EntityId, exception.ConflictId);
    Assert.Equal(_french.EntityId, exception.LanguageId);
    Assert.Equal("LanguageId", exception.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing dictionary.")]
  public async Task Given_Exists_When_Update_Then_Updated()
  {
    Language spanish = new(new Locale("es"), isDefault: false, ActorId, LanguageId.NewId(Realm.Id));
    await _languageRepository.SaveAsync(spanish);

    UpdateDictionaryPayload payload = new()
    {
      Language = spanish.Locale.Code
    };
    payload.Entries.Add(new DictionaryEntry("HelloWorld", $"  ¡Hola Mundo!  "));
    payload.Entries.Add(new DictionaryEntry("removed", string.Empty));
    DictionaryDto? dictionary = await _dictionaryService.UpdateAsync(_dictionary.EntityId, payload);
    Assert.NotNull(dictionary);

    Assert.Equal(_dictionary.EntityId, dictionary.Id);
    Assert.Equal(_dictionary.Version + 2, dictionary.Version);
    Assert.Equal(Actor, dictionary.UpdatedBy);
    Assert.Equal(DateTime.UtcNow, dictionary.UpdatedOn.AsUniversalTime(), TimeSpan.FromSeconds(10));

    Assert.Equal(spanish.EntityId, dictionary.Language.Id);
    Assert.Equal(dictionary.Entries.Count, dictionary.EntryCount);
    Assert.Single(dictionary.Entries);
    Assert.Contains(dictionary.Entries, c => c.Key == "HelloWorld" && c.Value == "¡Hola Mundo!");
  }
}
