using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using Krakenar.Core.Templates.Commands;
using Krakenar.Core.Templates.Queries;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates;

public class TemplateService : ITemplateService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public TemplateService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<CreateOrReplaceTemplateResult> CreateOrReplaceAsync(CreateOrReplaceTemplatePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceTemplate command = new(id, payload, version);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<TemplateDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteTemplate command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<TemplateDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadTemplate query = new(id, uniqueName);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<TemplateDto>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    SearchTemplates query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<TemplateDto?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    UpdateTemplate command = new(id, payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
