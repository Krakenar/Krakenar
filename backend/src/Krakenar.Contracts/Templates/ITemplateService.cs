using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Templates;

public interface ITemplateService
{
  Task<CreateOrReplaceTemplateResult> CreateOrReplaceAsync(CreateOrReplaceTemplatePayload payload, Guid? id = null, long? version = null, CancellationToken cancellationToken = default);
  Task<Template?> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<Template?> ReadAsync(Guid? id = null, string? uniqueName = null, CancellationToken cancellationToken = default);
  Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken = default);
  Task<Template?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken = default);
}
