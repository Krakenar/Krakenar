using Krakenar.Core.Fields;
using Krakenar.Core.Localization;
using ContentDto = Krakenar.Contracts.Contents.Content;

namespace Krakenar.Core.Contents;

public interface IContentQuerier
{
  Task<IReadOnlyDictionary<Guid, ContentId>> FindConflictsAsync(
    ContentTypeId contentTypeId,
    LanguageId? languageId,
    ContentStatus status,
    IReadOnlyDictionary<Guid, FieldValue> fieldValues,
    ContentId contentId,
    CancellationToken cancellationToken = default);
  Task<IReadOnlyDictionary<Guid, Guid>> FindContentTypeIdsAsync(IEnumerable<Guid> contentIds, CancellationToken cancellationToken = default);
  Task<ContentId?> FindIdAsync(ContentTypeId contentTypeId, LanguageId? languageId, UniqueName uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentId>> FindIdsAsync(ContentTypeId contentTypeId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentId>> FindIdsAsync(LanguageId languageId, CancellationToken cancellationToken = default);

  Task<ContentDto> ReadAsync(Content content, CancellationToken cancellationToken = default);
  Task<ContentDto?> ReadAsync(ContentId id, CancellationToken cancellationToken = default);
  Task<ContentDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
}
