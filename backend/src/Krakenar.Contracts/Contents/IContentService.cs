namespace Krakenar.Contracts.Contents;

public interface IContentService
{
  Task<Content> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken = default);
  Task<Content?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language = null, CancellationToken cancellationToken = default);
  Task<Content?> UpdateLocaleAsync(Guid id, UpdateContentLocalePayload payload, string? language = null, CancellationToken cancellationToken = default);
}
