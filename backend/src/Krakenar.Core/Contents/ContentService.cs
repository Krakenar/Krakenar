using Krakenar.Contracts.Contents;
using Krakenar.Core.Contents.Commands;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents;

public class ContentService : IContentService
{
  protected virtual ICommandHandler<CreateContent, ContentDto> CreateContent { get; }

  public ContentService(
    ICommandHandler<CreateContent, ContentDto> createContent)
  {
    CreateContent = createContent;
  }

  public virtual async Task<ContentDto> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken)
  {
    CreateContent command = new(payload);
    return await CreateContent.HandleAsync(command, cancellationToken);
  }
}
