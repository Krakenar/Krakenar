using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Core.Contents.Commands;
using Krakenar.Core.Contents.Queries;
using Logitar.CQRS;
using ContentDto = Krakenar.Contracts.Contents.Content;
using ContentLocaleDto = Krakenar.Contracts.Contents.ContentLocale;

namespace Krakenar.Core.Contents;

public class ContentService : IContentService
{
  protected virtual ICommandBus CommandBus { get; }
  protected virtual IQueryBus QueryBus { get; }

  public ContentService(ICommandBus commandBus, IQueryBus queryBus)
  {
    CommandBus = commandBus;
    QueryBus = queryBus;
  }

  public virtual async Task<ContentDto> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken)
  {
    CreateContent command = new(payload);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> DeleteAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    DeleteContent command = new(id, language);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishContent command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    PublishContent command = new(id, language);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ReadContent query = new(id);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<ContentDto?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    SaveContentLocale command = new(id, payload, language);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<SearchResults<ContentLocaleDto>> SearchLocalesAsync(SearchContentLocalesPayload payload, CancellationToken cancellationToken)
  {
    SearchContentLocales query = new(payload);
    return await QueryBus.ExecuteAsync(query, cancellationToken);
  }

  public virtual async Task<ContentDto?> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    UnpublishContent command = new(id);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    UnpublishContent command = new(id, language);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }

  public virtual async Task<ContentDto?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    UpdateContentLocale command = new(id, payload, language);
    return await CommandBus.ExecuteAsync(command, cancellationToken);
  }
}
