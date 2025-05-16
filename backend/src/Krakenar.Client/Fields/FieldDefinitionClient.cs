using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;

namespace Krakenar.Client.Fields;

public class FieldDefinitionClient : BaseClient, IFieldDefinitionService
{
  public FieldDefinitionClient(HttpClient httpClient, IKrakenarSettings settings) : base(httpClient, settings)
  {
  }

  public virtual async Task<ContentType?> CreateOrReplaceAsync(Guid contentTypeId, CreateOrReplaceFieldDefinitionPayload payload, Guid? fieldId, CancellationToken cancellationToken)
  {
    Uri uri = GetPath(contentTypeId, fieldId);
    ApiResult<ContentType> result = fieldId is null
      ? await PostAsync<ContentType>(uri, payload, cancellationToken)
      : await PutAsync<ContentType>(uri, payload, cancellationToken);
    return result.Value;
  }

  public virtual async Task<ContentType?> DeleteAsync(Guid contentTypeId, Guid fieldId, CancellationToken cancellationToken)
  {
    Uri uri = GetPath(contentTypeId, fieldId);
    return (await DeleteAsync<ContentType>(uri, cancellationToken)).Value;
  }

  public virtual async Task<ContentType?> UpdateAsync(Guid contentTypeId, Guid fieldId, UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    Uri uri = GetPath(contentTypeId, fieldId);
    return (await PatchAsync<ContentType>(uri, payload, cancellationToken)).Value;
  }

  protected virtual Uri GetPath(Guid contentTypeId, Guid? fieldId = null)
  {
    StringBuilder path = new();
    path.Append("/api/contents/types/").Append(contentTypeId).Append("/fields");
    if (fieldId.HasValue)
    {
      path.Append('/').Append(fieldId.Value);
    }
    return new Uri(path.ToString(), UriKind.Relative);
  }
}
