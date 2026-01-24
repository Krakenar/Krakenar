using Krakenar.Contracts;
using Logitar.CQRS;
using DictionaryDto = Krakenar.Contracts.Dictionaries.Dictionary;

namespace Krakenar.Core.Dictionaries.Queries;

public record ReadDictionary(Guid? Id, Guid? LanguageId) : IQuery<DictionaryDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadDictionaryHandler : IQueryHandler<ReadDictionary, DictionaryDto?>
{
  protected virtual IDictionaryQuerier DictionaryQuerier { get; }

  public ReadDictionaryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    DictionaryQuerier = dictionaryQuerier;
  }

  public virtual async Task<DictionaryDto?> HandleAsync(ReadDictionary query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, DictionaryDto> dictionaries = new(capacity: 2);

    if (query.Id.HasValue)
    {
      DictionaryDto? dictionary = await DictionaryQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (dictionary is not null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (query.LanguageId.HasValue)
    {
      DictionaryDto? dictionary = await DictionaryQuerier.ReadByLanguageAsync(query.LanguageId.Value, cancellationToken);
      if (dictionary is not null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw TooManyResultsException<DictionaryDto>.ExpectedSingle(dictionaries.Count);
    }

    return dictionaries.SingleOrDefault().Value;
  }
}
