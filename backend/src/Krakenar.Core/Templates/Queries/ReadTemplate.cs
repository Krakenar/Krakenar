using Krakenar.Contracts;
using Logitar.CQRS;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Queries;

public record ReadTemplate(Guid? Id, string? UniqueName) : IQuery<TemplateDto?>;

/// <exception cref="TooManyResultsException{T}"></exception>
public class ReadTemplateHandler : IQueryHandler<ReadTemplate, TemplateDto?>
{
  protected virtual ITemplateQuerier TemplateQuerier { get; }

  public ReadTemplateHandler(ITemplateQuerier templateQuerier)
  {
    TemplateQuerier = templateQuerier;
  }

  public virtual async Task<TemplateDto?> HandleAsync(ReadTemplate query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, TemplateDto> templates = new(capacity: 2);

    if (query.Id.HasValue)
    {
      TemplateDto? template = await TemplateQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (template is not null)
      {
        templates[template.Id] = template;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueName))
    {
      TemplateDto? template = await TemplateQuerier.ReadAsync(query.UniqueName, cancellationToken);
      if (template is not null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw TooManyResultsException<TemplateDto>.ExpectedSingle(templates.Count);
    }

    return templates.SingleOrDefault().Value;
  }
}
