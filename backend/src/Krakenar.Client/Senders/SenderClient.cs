using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Senders;

namespace Krakenar.Client.Senders;

public class SenderClient : BaseClient, ISenderService
{
  protected virtual Uri Path { get; } = new("/api/senders", UriKind.Relative);

  public SenderClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<CreateOrReplaceSenderResult> CreateOrReplaceAsync(CreateOrReplaceSenderPayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<Sender> result = id is null
      ? await PostAsync<Sender>(Path, payload, cancellationToken)
      : await PutAsync<Sender>(new Uri($"{Path}/{id}?version={version}", UriKind.Relative), payload, cancellationToken);
    return new CreateOrReplaceSenderResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<Sender?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<Sender>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Sender?> ReadAsync(Guid? id, SenderKind? kind, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Sender> senders = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Sender? sender = (await GetAsync<Sender>(uri, cancellationToken)).Value;
      if (sender is not null)
      {
        senders[sender.Id] = sender;
      }
    }

    if (kind.HasValue)
    {
      Uri uri = new($"{Path}/default/{kind}", UriKind.Relative);
      Sender? sender = (await GetAsync<Sender>(uri, cancellationToken)).Value;
      if (sender is not null)
      {
        senders[sender.Id] = sender;
      }
    }

    if (senders.Count > 1)
    {
      throw TooManyResultsException<Sender>.ExpectedSingle(senders.Count);
    }

    return senders.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<Sender>> SearchAsync(SearchSendersPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["kind"] = [payload.Kind];
    parameters["provider"] = [payload.Provider];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Sender>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Sender?> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}/default", UriKind.Relative);
    return (await PatchAsync<Sender>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Sender?> UpdateAsync(Guid id, UpdateSenderPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<Sender>(uri, payload, cancellationToken)).Value;
  }
}
