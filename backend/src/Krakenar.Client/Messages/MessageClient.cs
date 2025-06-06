﻿using Krakenar.Contracts.Messages;
using Krakenar.Contracts.Search;

namespace Krakenar.Client.Messages;

public class MessageClient : BaseClient, IMessageService
{
  protected virtual Uri Path { get; } = new("/api/messages", UriKind.Relative);

  public MessageClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<Message?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await GetAsync<Message>(uri, cancellationToken)).Value;
  }

  public virtual async Task<SearchResults<Message>> SearchAsync(SearchMessagesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["template"] = [payload.TemplateId];
    parameters["demo"] = [payload.IsDemo];
    parameters["status"] = [payload.Status];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Message>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<SentMessages> SendAsync(SendMessagePayload payload, CancellationToken cancellationToken)
  {
    return (await PostAsync<SentMessages>(Path, payload, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SendAsync), HttpMethod.Post, Path, payload);
  }
}
