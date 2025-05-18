using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;

namespace Krakenar.Core.Contents;

public interface IPublishedContentQuerier
{
  Task<PublishedContent?> ReadAsync(int id, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<PublishedContent?> ReadAsync(PublishedContentKey key, CancellationToken cancellationToken = default);

  Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken = default);
}
