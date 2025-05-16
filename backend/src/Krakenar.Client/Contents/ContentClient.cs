using Krakenar.Contracts.Contents;

namespace Krakenar.Client.Contents;

public class ContentClient : BaseClient, IContentService
{
  protected virtual Uri Path { get; } = new("/api/contents", UriKind.Relative);

  public ContentClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<Content> CreateAsync(CreateContentPayload payload, CancellationToken cancellationToken)
  {
    ApiResult<Content> result = await PostAsync<Content>(Path, payload, cancellationToken);
    return result.Value ?? throw CreateInvalidApiResponseException(nameof(CreateAsync), HttpMethod.Post, Path, payload);
  }

  public virtual async Task<Content?> SaveLocaleAsync(Guid id, SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}?language={language}", UriKind.Relative);
    return (await PutAsync<Content>(uri, payload, cancellationToken)).Value;
  }
}
