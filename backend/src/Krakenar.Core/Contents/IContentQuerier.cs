using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents;

public interface IContentQuerier
{
  Task<ContentDto> ReadAsync(Content content, CancellationToken cancellationToken = default);
  Task<ContentDto?> ReadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<ContentDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
