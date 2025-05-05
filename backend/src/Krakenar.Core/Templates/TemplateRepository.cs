using Logitar.EventSourcing;

namespace Krakenar.Core.Templates;

public interface ITemplateRepository
{
  Task<Template?> LoadAsync(TemplateId id, CancellationToken cancellationToken = default);
  Task<Template?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken = default);

  Task SaveAsync(Template template, CancellationToken cancellationToken = default);
  Task SaveAsync(IEnumerable<Template> templates, CancellationToken cancellationToken = default);
}

public class TemplateRepository : Repository, ITemplateRepository
{
  public TemplateRepository(IEventStore eventStore) : base(eventStore)
  {
  }

  public virtual async Task<Template?> LoadAsync(TemplateId id, CancellationToken cancellationToken)
  {
    return await LoadAsync(id, version: null, cancellationToken);
  }
  public virtual async Task<Template?> LoadAsync(TemplateId id, long? version, CancellationToken cancellationToken)
  {
    return await LoadAsync<Template>(id.StreamId, version, cancellationToken);
  }

  public virtual async Task SaveAsync(Template template, CancellationToken cancellationToken)
  {
    await base.SaveAsync(template, cancellationToken);
  }
  public virtual async Task SaveAsync(IEnumerable<Template> templates, CancellationToken cancellationToken)
  {
    await base.SaveAsync(templates, cancellationToken);
  }
}
