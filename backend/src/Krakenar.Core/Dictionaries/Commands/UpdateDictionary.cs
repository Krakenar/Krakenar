using FluentValidation;
using Krakenar.Contracts.Dictionaries;
using Krakenar.Core.Dictionaries.Validators;
using Krakenar.Core.Localization;
using Logitar.CQRS;
using Logitar.EventSourcing;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries.Commands;

public record UpdateDictionary(Guid Id, UpdateDictionaryPayload Payload) : ICommand<DictionaryDto?>;

/// <exception cref="LanguageAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateDictionaryHandler : ICommandHandler<UpdateDictionary, DictionaryDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IDictionaryManager DictionaryManager { get; }
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }
  protected virtual IDictionaryRepository DictionaryRepository { get; }
  protected virtual ILanguageManager LanguageManager { get; }

  public UpdateDictionaryHandler(
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

  public virtual async Task<DictionaryDto?> HandleAsync(UpdateDictionary command, CancellationToken cancellationToken)
  {
    UpdateDictionaryPayload payload = command.Payload;
    new UpdateDictionaryValidator().ValidateAndThrow(payload);

    DictionaryId dictionaryId = new(command.Id, ApplicationContext.RealmId);
    Dictionary? dictionary = await DictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    if (dictionary is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.Language))
    {
      Language language = await LanguageManager.FindAsync(payload.Language, nameof(payload.Language), cancellationToken);
      dictionary.SetLanguage(language, actorId);
    }

    foreach (DictionaryEntry entry in payload.Entries)
    {
      Identifier key = new(entry.Key);
      dictionary.SetEntry(key, entry.Value);
    }

    dictionary.Update(actorId);
    await DictionaryManager.SaveAsync(dictionary, cancellationToken);

    return await DictionaryQuerier.ReadAsync(dictionary, cancellationToken);
  }
}
