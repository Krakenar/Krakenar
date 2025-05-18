using Krakenar.Core.Localization;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents;

public interface IContentQuerier
{
  Task<IReadOnlyDictionary<Guid, Guid>> FindContentTypeIdsAsync(IEnumerable<Guid> contentIds, CancellationToken cancellationToken = default);
  Task<ContentId?> FindIdAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueName uniqueName, CancellationToken cancellationToken = default);

  Task<ContentDto> ReadAsync(Content content, CancellationToken cancellationToken = default);
  Task<ContentDto?> ReadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<ContentDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
