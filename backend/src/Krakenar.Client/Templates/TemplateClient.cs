using Krakenar.Contracts;
using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;

namespace Krakenar.Client.Templates;

public class TemplateClient : BaseClient, ITemplateService
{
  protected virtual Uri Path { get; } = new("/api/templates", UriKind.Relative);

  public TemplateClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<CreateOrReplaceTemplateResult> CreateOrReplaceAsync(CreateOrReplaceTemplatePayload payload, Guid? id, long? version, CancellationToken cancellationToken)
  {
    ApiResult<Template> result = id is null
      ? await PostAsync<Template>(Path, payload, cancellationToken)
      : await PutAsync<Template>(new Uri($"{Path}/{id}?version={version}", UriKind.Relative), payload, cancellationToken);
    return new CreateOrReplaceTemplateResult(result.Value, result.StatusCode == HttpStatusCode.Created);
  }

  public virtual async Task<Template?> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await DeleteAsync<Template>(uri, cancellationToken)).Value;
  }

  public virtual async Task<Template?> ReadAsync(Guid? id, string? uniqueName, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Template> templates = new(capacity: 2);

    if (id.HasValue)
    {
      Uri uri = new($"{Path}/{id}", UriKind.Relative);
      Template? template = (await GetAsync<Template>(uri, cancellationToken)).Value;
      if (template is not null)
      {
        templates[template.Id] = template;
      }
    }

    if (!string.IsNullOrWhiteSpace(uniqueName))
    {
      Uri uri = new($"{Path}/name:{uniqueName}", UriKind.Relative);
      Template? template = (await GetAsync<Template>(uri, cancellationToken)).Value;
      if (template is not null)
      {
        templates[template.Id] = template;
      }
    }

    if (templates.Count > 1)
    {
      throw TooManyResultsException<Template>.ExpectedSingle(templates.Count);
    }

    return templates.SingleOrDefault().Value;
  }

  public virtual async Task<SearchResults<Template>> SearchAsync(SearchTemplatesPayload payload, CancellationToken cancellationToken)
  {
    Dictionary<string, List<object?>> parameters = payload.ToQueryParameters();
    parameters["type"] = [payload.ContentType];
    parameters["sort"] = payload.Sort.Select(sort => (object?)(sort.IsDescending ? $"DESC.{sort.Field}" : sort)).ToList();

    Uri uri = new($"{Path}?{parameters.ToQueryString()}", UriKind.Relative);
    return (await GetAsync<SearchResults<Template>>(uri, cancellationToken)).Value
      ?? throw CreateInvalidApiResponseException(nameof(SearchAsync), HttpMethod.Get, uri);
  }

  public virtual async Task<Template?> UpdateAsync(Guid id, UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    Uri uri = new($"{Path}/{id}", UriKind.Relative);
    return (await PatchAsync<Template>(uri, payload, cancellationToken)).Value;
  }
}
