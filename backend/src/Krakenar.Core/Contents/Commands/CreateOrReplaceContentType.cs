using FluentValidation;
using Krakenar.Contracts.Contents;
using Krakenar.Core.Contents.Validators;
using Logitar.EventSourcing;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents.Commands;

public record CreateOrReplaceContentType(Guid? Id, CreateOrReplaceContentTypePayload Payload, long? Version) : ICommand<CreateOrReplaceContentTypeResult>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class CreateOrReplaceContentTypeHandler : ICommandHandler<CreateOrReplaceContentType, CreateOrReplaceContentTypeResult>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentManager ContentManager { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public CreateOrReplaceContentTypeHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentTypeQuerier contenttypeQuerier,
    IContentTypeRepository contenttypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentManager = contentManager;
    ContentTypeQuerier = contenttypeQuerier;
    ContentTypeRepository = contenttypeRepository;
  }

  public virtual async Task<CreateOrReplaceContentTypeResult> HandleAsync(CreateOrReplaceContentType command, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypePayload payload = command.Payload;
    new CreateOrReplaceContentTypeValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = ContentTypeId.NewId(ApplicationContext.RealmId);
    ContentType? contentType = null;
    if (command.Id.HasValue)
    {
      contentTypeId = new(command.Id.Value, contentTypeId.RealmId);
      contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    }

    Identifier uniqueName = new(payload.UniqueName);
    ActorId? actorId = ApplicationContext.ActorId;

    bool created = false;
    if (contentType is null)
    {
      if (command.Version.HasValue)
      {
        return new CreateOrReplaceContentTypeResult();
      }

      contentType = new(uniqueName, payload.IsInvariant, actorId, contentTypeId);
      created = true;
    }

    ContentType reference = (command.Version.HasValue
      ? await ContentTypeRepository.LoadAsync(contentTypeId, command.Version, cancellationToken)
      : null) ?? contentType;

    if (reference.IsInvariant != payload.IsInvariant)
    {
      contentType.IsInvariant = payload.IsInvariant;
    }

    if (reference.UniqueName != uniqueName)
    {
      contentType.SetUniqueName(uniqueName, actorId);
    }
    DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
    if (reference.DisplayName != displayName)
    {
      contentType.DisplayName = displayName;
    }
    Description? description = Description.TryCreate(payload.Description);
    if (reference.Description != description)
    {
      contentType.Description = description;
    }

    contentType.Update(actorId);
    await ContentManager.SaveAsync(contentType, cancellationToken);

    ContentTypeDto dto = await ContentTypeQuerier.ReadAsync(contentType, cancellationToken);
    return new CreateOrReplaceContentTypeResult(dto, created);
  }
}
