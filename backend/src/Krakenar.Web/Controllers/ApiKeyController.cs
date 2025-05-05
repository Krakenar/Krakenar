using Krakenar.Contracts.ApiKeys;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.ApiKey;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/keys")]
public class ApiKeyController : ControllerBase
{
  protected virtual IApiKeyService ApiKeyService { get; }

  public ApiKeyController(IApiKeyService apiKeyService)
  {
    ApiKeyService = apiKeyService;
  }

  [HttpPatch("authenticate")]
  public virtual async Task<ActionResult<ApiKey>> AuthenticateAsync([FromBody] AuthenticateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey apiKey = await ApiKeyService.AuthenticateAsync(payload, cancellationToken);
    return Ok(apiKey);
  }

  [HttpPost]
  public virtual async Task<ActionResult<ApiKey>> CreateAsync([FromBody] CreateOrReplaceApiKeyPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKeyResult result = await ApiKeyService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<ApiKey>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await ApiKeyService.DeleteAsync(id, cancellationToken);
    return apiKey is null ? NotFound() : Ok(apiKey);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<ApiKey>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await ApiKeyService.ReadAsync(id, cancellationToken);
    return apiKey is null ? NotFound() : Ok(apiKey);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<ApiKey>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceApiKeyPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceApiKeyResult result = await ApiKeyService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<ApiKey>>> SearchAsync([FromQuery] SearchApiKeysParameters parameters, CancellationToken cancellationToken)
  {
    SearchApiKeysPayload payload = parameters.ToPayload();
    SearchResults<ApiKey> apiKeys = await ApiKeyService.SearchAsync(payload, cancellationToken);
    return Ok(apiKeys);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<ApiKey>> UpdateAsync(Guid id, [FromBody] UpdateApiKeyPayload payload, CancellationToken cancellationToken)
  {
    ApiKey? apiKey = await ApiKeyService.UpdateAsync(id, payload, cancellationToken);
    return apiKey is null ? NotFound() : Ok(apiKey);
  }

  protected virtual ActionResult<ApiKey> ToActionResult(CreateOrReplaceApiKeyResult result)
  {
    if (result.ApiKey is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/keys/{result.ApiKey.Id}", UriKind.Absolute);
      return Created(location, result.ApiKey);
    }

    return Ok(result.ApiKey);
  }
}
