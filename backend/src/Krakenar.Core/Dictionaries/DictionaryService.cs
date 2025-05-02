using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Krakenar.Core.Dictionaries.Commands;
using Krakenar.Core.Dictionaries.Queries;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries;

public class DictionaryService : IDictionaryService
{
  protected virtual ICommandHandler<CreateOrReplaceDictionary, CreateOrReplaceDictionaryResult> CreateOrReplaceDictionary { get; }
  protected virtual ICommandHandler<DeleteDictionary, DictionaryDto?> DeleteDictionary { get; }
  protected virtual IQueryHandler<ReadDictionary, DictionaryDto?> ReadDictionary { get; }
  protected virtual IQueryHandler<SearchDictionaries, SearchResults<DictionaryDto>> SearchDictionaries { get; }
  protected virtual ICommandHandler<UpdateDictionary, DictionaryDto?> UpdateDictionary { get; }

  public DictionaryService(
    ICommandHandler<CreateOrReplaceDictionary, CreateOrReplaceDictionaryResult> createOrReplaceDictionary,
    ICommandHandler<DeleteDictionary, DictionaryDto?> deleteDictionary,
    IQueryHandler<ReadDictionary, DictionaryDto?> readDictionary,
    IQueryHandler<SearchDictionaries, SearchResults<DictionaryDto>> searchDictionaries,
    ICommandHandler<UpdateDictionary, DictionaryDto?> updateDictionary)
  {
    CreateOrReplaceDictionary = createOrReplaceDictionary;
    DeleteDictionary = deleteDictionary;
    ReadDictionary = readDictionary;
    SearchDictionaries = searchDictionaries;
    UpdateDictionary = updateDictionary;
  }

  public virtual async Task<CreateOrReplaceDictionaryResult> CreateOrReplaceAsync(CreateOrReplaceDictionaryPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionary command = new(id, payload, version);
    return await CreateOrReplaceDictionary.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<DictionaryDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteDictionary command = new(id);
    return await DeleteDictionary.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<DictionaryDto?> ReadAsync(Guid? id, Guid? languageId, CancellationToken cancellationToken)
  {
    ReadDictionary query = new(id, languageId);
    return await ReadDictionary.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<DictionaryDto>> SearchAsync(SearchDictionariesPayload payload, CancellationToken cancellationToken)
  {
    SearchDictionaries query = new(payload);
    return await SearchDictionaries.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<DictionaryDto?> UpdateAsync(Guid id, UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    UpdateDictionary command = new(id, payload);
    return await UpdateDictionary.HandleAsync(command, cancellationToken);
  }
}
