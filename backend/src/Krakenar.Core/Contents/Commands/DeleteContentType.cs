using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents.Commands;

public record DeleteContentType(Guid Id) : ICommand<ContentTypeDto?>;

public class DeleteContentTypeHandler : ICommandHandler<DeleteContentType, ContentTypeDto?>
{
  protected virtual IApplicationContext ApplicationContext { get; }
  protected virtual IContentTypeQuerier ContentTypeQuerier { get; }
  protected virtual IContentTypeRepository ContentTypeRepository { get; }

  public DeleteContentTypeHandler(IApplicationContext applicationContext, IContentTypeQuerier contentTypeQuerier, IContentTypeRepository contentTypeRepository)
  {
    ApplicationContext = applicationContext;
    ContentTypeQuerier = contentTypeQuerier;
    ContentTypeRepository = contentTypeRepository;
  }

  public virtual async Task<ContentTypeDto?> HandleAsync(DeleteContentType command, CancellationToken cancellationToken)
  {
    ContentTypeId roleId = new(command.Id, ApplicationContext.RealmId);
    ContentType? role = await ContentTypeRepository.LoadAsync(roleId, cancellationToken);
    if (role is null)
    {
      return null;
    }
    ContentTypeDto dto = await ContentTypeQuerier.ReadAsync(role, cancellationToken);

    role.Delete(ApplicationContext.ActorId);
    await ContentTypeRepository.SaveAsync(role, cancellationToken);

    return dto;
  }
}
