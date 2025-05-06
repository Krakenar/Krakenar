using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates;

public interface ITemplateQuerier
{
  Task<TemplateId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<TemplateDto> ReadAsync(Template template, CancellationToken cancellationToken = default);
  Task<TemplateDto?> ReadAsync(TemplateId id, CancellationToken cancellationToken = default);
  Task<TemplateDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TemplateDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<TemplateDto>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
}
