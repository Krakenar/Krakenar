using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Queries;

public record SearchTemplates(SearchTemplatesPayload Payload) : IQuery<SearchResults<TemplateDto>>;

public class SearchTemplatesHandler : IQueryHandler<SearchTemplates, SearchResults<TemplateDto>>
{
  protected virtual ITemplateQuerier TemplateQuerier { get; }

  public SearchTemplatesHandler(ITemplateQuerier templateQuerier)
  {
    TemplateQuerier = templateQuerier;
  }

  public virtual async Task<SearchResults<TemplateDto>> HandleAsync(SearchTemplates query, CancellationToken cancellationToken)
  {
    return await TemplateQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
