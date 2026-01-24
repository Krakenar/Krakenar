using Logitar.CQRS;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries.Commands;

public record DeleteDictionary(Guid Id) : ICommand<DictionaryDto?>;

public class DeleteDictionaryHandler : ICommandHandler<DeleteDictionary, DictionaryDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }
  protected virtual IDictionaryRepository DictionaryRepository { get; }

  public DeleteDictionaryHandler(IApplicationContext applicationContext, IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    ApplicationContext = applicationContext;
    DictionaryQuerier = dictionaryQuerier;
    DictionaryRepository = dictionaryRepository;
  }

  public virtual async Task<DictionaryDto?> HandleAsync(DeleteDictionary command, CancellationToken cancellationToken)
  {
    DictionaryId dictionaryId = new(command.Id, ApplicationContext.RealmId);
    Dictionary? dictionary = await DictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    if (dictionary is null)
    {
      return null;
    }
    DictionaryDto dto = await DictionaryQuerier.ReadAsync(dictionary, cancellationToken);

    dictionary.Delete(ApplicationContext.ActorId);
    await DictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return dto;
  }
}
