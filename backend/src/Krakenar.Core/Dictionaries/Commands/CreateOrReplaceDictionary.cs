using FluentValidation;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Core.Dictionaries.Validators;
using Krakenar.Core.Localization;
using Logitar.CQRS;
using Logitar.EventSourcing;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries.Commands;

public record CreateOrReplaceDictionary(Guid? Id, CreateOrReplaceDictionaryPayload Payload, long? Version) : ICommand<CreateOrReplaceDictionaryResult>;

/// <exception cref="LanguageAlreadyUsedException"></exception>
/// <exception cref="LanguageNotFoundException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceDictionaryHandler : ICommandHandler<CreateOrReplaceDictionary, CreateOrReplaceDictionaryResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IDictionaryManager DictionaryManager { get; }
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }
  protected virtual IDictionaryRepository DictionaryRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public CreateOrReplaceDictionaryHandler(
    IApplicationContext applicationContext,
    IDictionaryManager dictionaryManager,
    IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository,
    ILanguageManager languageManager)
  {
    ApplicationContext = applicationContext;
    DictionaryManager = dictionaryManager;
    DictionaryQuerier = dictionaryQuerier;
    DictionaryRepository = dictionaryRepository;
    LanguageManager = languageManager;
  }

  public virtual async Task<CreateOrReplaceDictionaryResult> HandleAsync(CreateOrReplaceDictionary command, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionaryPayload payload = command.Payload;
    new CreateOrReplaceDictionaryValidator().ValidateAndThrow(payload);

    DictionaryId dictionaryId = DictionaryId.NewId(ApplicationContext.RealmId);
    Dictionary? dictionary = null;
    if (command.Id.HasValue)
    {
      dictionaryId = new(command.Id.Value, dictionaryId.RealmId);
      dictionary = await DictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    }

    Language language = await LanguageManager.FindAsync(payload.Language, nameof(payload.Language), cancellationToken);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (dictionary is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceDictionaryResult();
      }

      dictionary = new(language, actorId, dictionaryId);
      created = true;
    }

    Dictionary reference = (command.Version.HasValue
      ? await DictionaryRepository.LoadAsync(dictionaryId, command.Version, cancellationToken)
      : null) ?? dictionary;

    if (reference.LanguageId != language.Id)
    {
      dictionary.SetLanguage(language, actorId);
    }

    SetEntries(dictionary, payload.Entries, reference);

    dictionary.Update(actorId);
    await DictionaryManager.SaveAsync(dictionary, cancellationToken);

    DictionaryDto dto = await DictionaryQuerier.ReadAsync(dictionary, cancellationToken);
    return new CreateOrReplaceDictionaryResult(dto, created);
  }

  private static void SetEntries(Dictionary dictionary, IEnumerable<DictionaryEntry> entries, Dictionary reference)
  {
    HashSet<Identifier> keys = entries.Select(customAttribute => new Identifier(customAttribute.Key)).ToHashSet();
    foreach (Identifier key in reference.Entries.Keys)
    {
      if (!keys.Contains(key))
      {
        dictionary.RemoveEntry(key);
      }
    }

    foreach (DictionaryEntry entry in entries)
    {
      Identifier key = new(entry.Key);
      if (!reference.Entries.TryGetValue(key, out string? existingValue) || existingValue != entry.Value)
      {
        dictionary.SetEntry(key, entry.Value);
      }
    }
  }
}
