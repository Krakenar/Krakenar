using Logitar.CQRS;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Commands;

public record DeleteTemplate(Guid Id) : ICommand<TemplateDto?>;

public class DeleteTemplateHandler : ICommandHandler<DeleteTemplate, TemplateDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ITemplateQuerier TemplateQuerier { get; }
  protected virtual ITemplateRepository TemplateRepository { get; }

  public DeleteTemplateHandler(IApplicationContext applicationContext, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    ApplicationContext = applicationContext;
    TemplateQuerier = templateQuerier;
    TemplateRepository = templateRepository;
  }

  public virtual async Task<TemplateDto?> HandleAsync(DeleteTemplate command, CancellationToken cancellationToken)
  {
    TemplateId templateId = new(command.Id, ApplicationContext.RealmId);
    Template? template = await TemplateRepository.LoadAsync(templateId, cancellationToken);
    if (template is null)
    {
      return null;
    }
    TemplateDto dto = await TemplateQuerier.ReadAsync(template, cancellationToken);

    template.Delete(ApplicationContext.ActorId);
    await TemplateRepository.SaveAsync(template, cancellationToken);

    return dto;
  }
}
