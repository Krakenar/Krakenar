using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Krakenar.Core.Dictionaries.Commands;
using Krakenar.Core.Dictionaries.Queries;
using Logitar.CQRS;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries;

public class DictionaryService : IDictionaryService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public DictionaryService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceDictionaryResult> CreateOrReplaceAsync(CreateOrReplaceDictionaryPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionary command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<DictionaryDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteDictionary command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<DictionaryDto?> ReadAsync(Guid? id, Guid? languageId, CancellationToken cancellationToken)
  {
    ReadDictionary query = new(id, languageId);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<DictionaryDto>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    SearchDictionaries query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<DictionaryDto?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    UpdateDictionary command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
