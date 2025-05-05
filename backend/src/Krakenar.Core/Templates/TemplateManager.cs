using Krakenar.Core.Templates.Events;

namespace Krakenar.Core.Templates;

public interface ITemplateManager
{
  Task SaveAsync(Template template, CancellationToken cancellationToken = default);
}

public class TemplateManager : ITemplateManager
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ITemplateQuerier TemplateQuerier { get; }
  protected virtual ITemplateRepository TemplateRepository { get; }

  public TemplateManager(IApplicationContext applicationContext, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    ApplicationContext = applicationContext;
    TemplateQuerier = templateQuerier;
    TemplateRepository = templateRepository;
  }

  public virtual async Task SaveAsync(Template template, CancellationToken cancellationToken)
  {
    bool hasUniqueNameChanged = template.Changes.Any(change => change is TemplateCreated || change is TemplateUniqueNameChanged);
    if (hasUniqueNameChanged)
    {
      TemplateId? conflictId = await TemplateQuerier.FindIdAsync(template.UniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(template.Id))
      {
        throw new UniqueNameAlreadyUsedException(template, conflictId.Value);
      }
    }

    await TemplateRepository.SaveAsync(template, cancellationToken);
  }
}
