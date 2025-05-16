namespace Krakenar.Contracts.Contents;

public interface IContentService
{
  Task<Content> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken = default);
}
