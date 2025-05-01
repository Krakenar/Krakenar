using Krakenar.Core.Dictionaries.Events;

namespace Krakenar.Core.Dictionaries;

public interface IDictionaryManager
{
  Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken = default);
}

public class DictionaryManager : IDictionaryManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }
  protected virtual IDictionaryRepository DictionaryRepository { get; }

  public DictionaryManager(IApplicationContext applicationContext, IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    ApplicationContext = applicationContext;
    DictionaryQuerier = dictionaryQuerier;
    DictionaryRepository = dictionaryRepository;
  }

  public virtual async Task SaveAsync(Dictionary dictionary, CancellationToken cancellationToken)
  {
    bool hasLanguageChanged = dictionary.Changes.Any(change => change is DictionaryCreated || change is DictionaryLanguageChanged);
    if (hasLanguageChanged)
    {
      DictionaryId? conflictId = await DictionaryQuerier.FindIdAsync(dictionary.LanguageId, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(dictionary.Id))
      {
        throw new NotImplementedException(); // TODO(fpion): implement
      }
    }

    await DictionaryRepository.SaveAsync(dictionary, cancellationToken);
  }
}
