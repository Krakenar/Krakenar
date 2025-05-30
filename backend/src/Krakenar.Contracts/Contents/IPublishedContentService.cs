﻿using Krakenar.Contracts.Search;

namespace Krakenar.Contracts.Contents;

public interface IPublishedContentService
{
  Task<PublishedContent?> ReadAsync(int? id = null, Guid? uid = null, PublishedContentKey? key = null, CancellationToken cancellationToken = default);
  Task<SearchResults<PublishedContentLocale>> SearchAsync(SearchPublishedContentsPayload payload, CancellationToken cancellationToken = default);
}
