using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using ContentLocaleDto = Krakenar.Contracts.Contents.ContentLocale;

namespace Krakenar.Core.Contents;

public interface IContentLocaleQuerier
{
  Task<SearchResults<ContentLocaleDto>> SearchAsync(SearchContentLocalesPayload payload, CancellationToken cancellationToken = default);
}
