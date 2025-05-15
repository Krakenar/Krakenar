using FluentValidation;
using Krakenar.Contracts.Contents;
using Krakenar.Core.Contents.Validators;
using Logitar.EventSourcing;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents.Commands;

public record UpdateContentType(Guid Id, UpdateContentTypePayload Payload) : ICommand<ContentTypeDto?>;

/// <exception cref="UniqueNameAlreadyUsedException"></exception>
/// <exception cref="ValidationException"></exception>
public class UpdateContentTypeHandler : ICommandHandler<UpdateContentType, ContentTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentManager ContentManager { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public UpdateContentTypeHandler(
    IApplicationContext applicationContext,
    IContentManager contentManager,
    IContentTypeQuerier contentTypeQuerier,
    IContentTypeRepository contentTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentManager = contentManager;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
  }

  public virtual async Task<ContentTypeDto?> HandleAsync(UpdateContentType command, CancellationToken cancellationToken)
  {
    UpdateContentTypePayload payload = command.Payload;
    new UpdateContentTypeValidator().ValidateAndThrow(payload);

    ContentTypeId contentTypeId = new(command.Id, ApplicationContext.RealmId);
    ContentType? contentType = await ContentTypeRepository.LoadAsync(contentTypeId, cancellationToken);
    if (contentType is null)
    {
      return null;
    }

    ActorId? actorId = ApplicationContext.ActorId;

    if (payload.IsInvariant.HasValue)
    {
      contentType.IsInvariant = payload.IsInvariant.Value;
    }

    if (!string.IsNullOrWhiteSpace(payload.UniqueName))
    {
      Identifier uniqueName = new(payload.UniqueName);
      contentType.SetUniqueName(uniqueName, actorId);
    }
    if (payload.DisplayName is not null)
    {
      contentType.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description is not null)
    {
      contentType.Description = Description.TryCreate(payload.Description.Value);
    }

    contentType.Update(actorId);
    await ContentManager.SaveAsync(contentType, cancellationToken);

    return await ContentTypeQuerier.ReadAsync(contentType, cancellationToken);
  }
}
