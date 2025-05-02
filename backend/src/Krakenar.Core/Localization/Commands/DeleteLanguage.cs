using Krakenar.Core.Dictionaries;
using Logitar.EventSourcing;
using LanguageDto = Krakenar.Contracts.Localization.Language;

namespace Krakenar.Core.Localization.Commands;

public record DeleteLanguage(Guid Id) : ICommand<LanguageDto?>;

public class DeleteLanguageHandler : ICommandHandler<DeleteLanguage, LanguageDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }
  protected virtual IDictionaryRepository DictionaryRepository { get; }
  protected virtual ILanguageQuerier LanguageQuerier { get; }
  protected virtual ILanguageRepository LanguageRepository { get; }

  public DeleteLanguageHandler(
    IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository,
    ILanguageQuerier languageQuerier,
    ILanguageRepository languageRepository)
  {
    ApplicationContext = applicationContext;
    DictionaryQuerier = dictionaryQuerier;
    DictionaryRepository = dictionaryRepository;
    LanguageQuerier = languageQuerier;
    LanguageRepository = languageRepository;
  }

  public virtual async Task<LanguageDto?> HandleAsync(DeleteLanguage command, CancellationToken cancellationToken)
  {
    LanguageId languageId = new(command.Id, ApplicationContext.RealmId);
    Language? language = await LanguageRepository.LoadAsync(languageId, cancellationToken);
    if (language is null)
    {
      return null;
    }
    else if (language.IsDefault)
    {
      throw new CannotDeleteDefaultLanguageException(language);
    }
    LanguageDto dto = await LanguageQuerier.ReadAsync(language, cancellationToken);

    ActorId? actorId = ApplicationContext.ActorId;

    DictionaryId? dictionaryId = await DictionaryQuerier.FindIdAsync(language.Id, cancellationToken);
    if (dictionaryId.HasValue)
    {
      Dictionary dictionary = await DictionaryRepository.LoadAsync(dictionaryId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The dictionary 'Id={dictionaryId}' was not loaded.");
      dictionary.Delete(actorId);
      await DictionaryRepository.SaveAsync(dictionary, cancellationToken);
    }

    language.Delete(actorId);
    await LanguageRepository.SaveAsync(language, cancellationToken);

    return dto;
  }
}
