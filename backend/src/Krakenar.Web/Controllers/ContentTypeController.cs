using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.ContentType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/contents/types")]
public class ContentTypeController : ControllerBase
{
  protected virtual IContentTypeService ContentTypeService { get; }

  public ContentTypeController(IContentTypeService contentTypeService)
  {
    ContentTypeService = contentTypeService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<ContentType>> CreateAsync([FromBody] CreateOrReplaceContentTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypeResult result = await ContentTypeService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<ContentType>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentType? contentType = await ContentTypeService.DeleteAsync(id, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<ContentType>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    ContentType? contentType = await ContentTypeService.ReadAsync(id, uniqueName: null, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }

  [HttpGet("name:{uniqueName}")]
  public virtual async Task<ActionResult<ContentType>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    ContentType? contentType = await ContentTypeService.ReadAsync(id: null, uniqueName, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<ContentType>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceContentTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceContentTypeResult result = await ContentTypeService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<ContentType>>> SearchAsync([FromQuery] SearchContentTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchContentTypesPayload payload = parameters.ToPayload();
    SearchResults<ContentType> contentTypes = await ContentTypeService.SearchAsync(payload, cancellationToken);
    return Ok(contentTypes);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<ContentType>> UpdateAsync(Guid id, [FromBody] UpdateContentTypePayload payload, CancellationToken cancellationToken)
  {
    ContentType? contentType = await ContentTypeService.UpdateAsync(id, payload, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }

  protected virtual ActionResult<ContentType> ToActionResult(CreateOrReplaceContentTypeResult result)
  {
    if (result.ContentType is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/contents/types/{result.ContentType.Id}", UriKind.Absolute);
      return Created(location, result.ContentType);
    }

    return Ok(result.ContentType);
  }
}
