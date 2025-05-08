using FluentValidation;
using Krakenar.Core.Realms;
using Krakenar.Core.Settings;
using Krakenar.Core.Templates.Events;

namespace Krakenar.Core.Templates;

public interface ITemplateManager
{
  Task<Template> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken = default);
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

  public virtual async Task<Template> FindAsync(string idOrUniqueName, string propertyName, CancellationToken cancellationToken)
  {
    RealmId? realmId = ApplicationContext.RealmId;
    Template? template = null;

    if (Guid.TryParse(idOrUniqueName, out Guid entityId))
    {
      TemplateId templateId = new(entityId, realmId);
      template = await TemplateRepository.LoadAsync(templateId, cancellationToken);
    }

    if (template is null)
    {
      try
      {
        UniqueNameSettings settings = new(allowedCharacters: null);
        UniqueName uniqueName = new(settings, idOrUniqueName);
        TemplateId? templateId = await TemplateQuerier.FindIdAsync(uniqueName, cancellationToken);
        if (templateId.HasValue)
        {
          template = await TemplateRepository.LoadAsync(templateId.Value, cancellationToken);
        }
      }
      catch (ValidationException)
      {
      }
    }

    return template ?? throw new TemplateNotFoundException(realmId, idOrUniqueName, propertyName);
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
