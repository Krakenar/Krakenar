using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Templates;
using Krakenar.Core.Templates.Validators;
using Logitar.CQRS;
using Logitar.EventSourcing;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Commands;

public record UpdateTemplate(Guid Id, UpdateTemplatePayload Payload) : ICommand<TemplateDto?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateTemplateHandler : ICommandHandler<UpdateTemplate, TemplateDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ITemplateManager TemplateManager { get; }
  protected virtual ITemplateQuerier TemplateQuerier { get; }
  protected virtual ITemplateRepository TemplateRepository { get; }

  public UpdateTemplateHandler(
    IApplicationContext applicationContext,
    ITemplateManager templateManager,
    ITemplateQuerier templateQuerier,
    ITemplateRepository templateRepository)
  {
    ApplicationContext = applicationContext;
    TemplateManager = templateManager;
    TemplateQuerier = templateQuerier;
    TemplateRepository = templateRepository;
  }

  public virtual async Task<TemplateDto?> HandleAsync(UpdateTemplate command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    UpdateTemplatePayload payload = command.Payload;
    new UpdateTemplateValidator(uniqueNameSettings).ValidateAndThrow(payload);

    TemplateId templateId = new(command.Id, ApplicationContext.RealmId);
    Template? template = await TemplateRepository.LoadAsync(templateId, cancellationToken);
    if (template is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
      template.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      template.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      template.Description = Description.TryCreate(payload.Description.Value);
    }

    if (!string.IsNullOrWhiteSpace(payload.Subject))
    {
      template.Subject = new Subject(payload.Subject);
    }
    if (payload.Content is not null)
    {
      template.Content = new Content(payload.Content);
    }

    template.Update(actorId);
    await TemplateManager.SaveAsync(template, cancellationToken);

    return await TemplateQuerier.ReadAsync(template, cancellationToken);
  }
}
