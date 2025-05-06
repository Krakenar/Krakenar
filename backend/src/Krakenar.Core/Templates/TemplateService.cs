using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using Krakenar.Core.Templates.Commands;
using Krakenar.Core.Templates.Queries;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates;

public class TemplateService : ITemplateService
{
  protected virtual ICommandHandler<CreateOrReplaceTemplate, CreateOrReplaceTemplateResult> CreateOrReplaceTemplate { get; }
  protected virtual ICommandHandler<DeleteTemplate, TemplateDto?> DeleteTemplate { get; }
  protected virtual IQueryHandler<ReadTemplate, TemplateDto?> ReadTemplate { get; }
  protected virtual IQueryHandler<SearchTemplates, SearchResults<TemplateDto>> SearchTemplates { get; }
  protected virtual ICommandHandler<UpdateTemplate, TemplateDto?> UpdateTemplate { get; }

  public TemplateService(
    ICommandHandler<CreateOrReplaceTemplate, CreateOrReplaceTemplateResult> createOrReplaceTemplate,
    ICommandHandler<DeleteTemplate, TemplateDto?> deleteTemplate,
    IQueryHandler<ReadTemplate, TemplateDto?> readTemplate,
    IQueryHandler<SearchTemplates, SearchResults<TemplateDto>> searchTemplates,
    ICommandHandler<UpdateTemplate, TemplateDto?> updateTemplate)
  {
    CreateOrReplaceTemplate = createOrReplaceTemplate;
    DeleteTemplate = deleteTemplate;
    ReadTemplate = readTemplate;
    SearchTemplates = searchTemplates;
    UpdateTemplate = updateTemplate;
  }

  public virtual async Task<CreateOrReplaceTemplateResult> CreateOrReplaceAsync(CreateOrReplaceTemplatePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceTemplate command = new(id, payload, version);
    return await CreateOrReplaceTemplate.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<TemplateDto?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    DeleteTemplate command = new(id);
    return await DeleteTemplate.HandleAsync(command, cancellationToken);
  }

  public virtual async Task<TemplateDto?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    ReadTemplate query = new(id, uniqueName);
    return await ReadTemplate.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<SearchResults<TemplateDto>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    SearchTemplates query = new(payload);
    return await SearchTemplates.HandleAsync(query, cancellationToken);
  }

  public virtual async Task<TemplateDto?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    UpdateTemplate command = new(id, payload);
    return await UpdateTemplate.HandleAsync(command, cancellationToken);
  }
}
