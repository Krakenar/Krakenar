using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using FieldTypeDto = Krakenar.Contracts.Fields.FieldType;

namespace Krakenar.Core.Fields.Queries;

public record SearchFieldTypes(SearchFieldTypesPayload Payload) : IQuery<SearchResults<FieldTypeDto>>;

public class SearchFieldTypesHandler : IQueryHandler<SearchFieldTypes, SearchResults<FieldTypeDto>>
{
  protected virtual IFieldTypeQuerier FieldTypeQuerier { get; }

  public SearchFieldTypesHandler(IFieldTypeQuerier roleQuerier)
  {
    FieldTypeQuerier = roleQuerier;
  }

  public virtual async Task<SearchResults<FieldTypeDto>> HandleAsync(SearchFieldTypes query, CancellationToken cancellationToken)
  {
    return await FieldTypeQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
