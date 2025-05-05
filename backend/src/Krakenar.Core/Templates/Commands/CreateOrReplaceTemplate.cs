using FluentValidation;
using Krakenar.Contracts.Settings;
using Krakenar.Contracts.Templates;
using Krakenar.Core.Templates.Validators;
using Logitar.EventSourcing;
using TemplateDto = Krakenar.Contracts.Templates.Template;

namespace Krakenar.Core.Templates.Commands;

public record CreateOrReplaceTemplate(Guid? Id, CreateOrReplaceTemplatePayload Payload, long? Version) : ICommand<CreateOrReplaceTemplateResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceTemplateHandler : ICommandHandler<CreateOrReplaceTemplate, CreateOrReplaceTemplateResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual ITemplateManager TemplateManager { get; }
  protected virtual ITemplateQuerier TemplateQuerier { get; }
  protected virtual ITemplateRepository TemplateRepository { get; }

  public CreateOrReplaceTemplateHandler(IApplicationContext applicationContext, ITemplateManager templateManager, ITemplateQuerier templateQuerier, ITemplateRepository templateRepository)
  {
    ApplicationContext = applicationContext;
    TemplateManager = templateManager;
    TemplateQuerier = templateQuerier;
    TemplateRepository = templateRepository;
  }

  public virtual async Task<CreateOrReplaceTemplateResult> HandleAsync(CreateOrReplaceTemplate command, CancellationToken cancellationToken)
  {
    IUniqueNameSettings uniqueNameSettings = ApplicationContext.UniqueNameSettings;

    CreateOrReplaceTemplatePayload payload = command.Payload;
    new CreateOrReplaceTemplateValidator(uniqueNameSettings).ValidateAndThrow(payload);

    TemplateId templateId = TemplateId.NewId(ApplicationContext.RealmId);
    Template? template = null;
    if (command.Id.HasValue)
    {
      templateId = new(command.Id.Value, templateId.RealmId);
      template = await TemplateRepository.LoadAsync(templateId, cancellationToken);
    }

    UniqueName uniqueName = new(uniqueNameSettings, payload.UniqueName);
    Subject subject = new(payload.Subject);
    Content content = new(payload.Content);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (template is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceTemplateResult();
      }

      template = new(uniqueName, subject, content, actorId, templateId);
      created = true;
    }

    Template reference = (command.Version.HasValue
      ? await TemplateRepository.LoadAsync(templateId, command.Version, cancellationToken)
      : null) ?? template;

    if (reference.UniqueName != uniqueName)
    {
      template.SetUniqueName(uniqueName, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      template.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      template.Description = description;
    }

    if (reference.Subject != subject)
    {
      template.Subject = subject;
    }
    if (reference.Content != content)
    {
      template.Content = content;
    }

    template.Update(actorId);
    await TemplateManager.SaveAsync(template, cancellationToken);

    TemplateDto dto = await TemplateQuerier.ReadAsync(template, cancellationToken);
    return new CreateOrReplaceTemplateResult(dto, created);
  }
}
