﻿using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core.Fields;
using ContentTypeDto = Krakenar.Contracts.Contents.ContentType;

namespace Krakenar.Core.Contents;

public interface IContentTypeQuerier
{
  Task<ContentTypeId?> FindIdAsync(Identifier uniqueName, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentTypeId>> FindIdsAsync(FieldTypeId fieldTypeId, CancellationToken cancellationToken = default);
  Task<IReadOnlyCollection<ContentTypeId>> FindIdsAsync(IEnumerable<FieldTypeId> fieldTypeIds, CancellationToken cancellationToken = default);

  Task<ContentTypeDto> ReadAsync(ContentType contentType, CancellationToken cancellationToken = default);
  Task<ContentTypeDto?> ReadAsync(ContentTypeId id, CancellationToken cancellationToken = default);
  Task<ContentTypeDto?> ReadAsync(Guid id, CancellationToken cancellationToken = default);
  Task<ContentTypeDto?> ReadAsync(string uniqueName, CancellationToken cancellationToken = default);

  Task<SearchResults<ContentTypeDto>> SearchAsync(SearchContentTypesPayload payload, CancellationToken cancellationToken = default);
}
