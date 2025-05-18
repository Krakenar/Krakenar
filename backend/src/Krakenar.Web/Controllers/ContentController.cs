using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Content;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/contents")]
public class ContentController : ControllerBase
{
  protected virtual IContentService ContentService { get; }

  public ContentController(IContentService contentService)
  {
    ContentService = contentService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Content>> CreateAsync([FromBody] CreateContentPayload payload, CancellationToken cancellationToken)
  {
    Content content = await ContentService.CreateAsync(payload, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/contents/{content.Id}", UriKind.Absolute);
    return Created(location, content);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<Content>> DeleteAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.DeleteAsync(id, language, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpPatch("{id}/publish/all")]
  public virtual async Task<ActionResult<Content>> PublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.PublishAllAsync(id, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpPatch("{id}/publish")]
  public virtual async Task<ActionResult<Content>> PublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.PublishAsync(id, language, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Content>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.ReadAsync(id, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<Content>> SaveLocaleAsync(Guid id, [FromBody] SaveContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.SaveLocaleAsync(id, payload, language, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<ContentLocale>>> SearchAsync([FromQuery] SearchContentLocalesParameters parameters, CancellationToken cancellationToken)
  {
    SearchContentLocalesPayload payload = parameters.ToPayload();
    SearchResults<ContentLocale> contentLocales = await ContentService.SearchLocalesAsync(payload, cancellationToken);
    return Ok(contentLocales);
  }

  [HttpPatch("{id}/unpublish/all")]
  public virtual async Task<ActionResult<Content>> UnpublishAllAsync(Guid id, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.UnpublishAllAsync(id, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpPatch("{id}/unpublish")]
  public virtual async Task<ActionResult<Content>> UnpublishAsync(Guid id, string? language, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.UnpublishAsync(id, language, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<Content>> UpdateLocaleAsync(Guid id, [FromBody] UpdateContentLocalePayload payload, string? language, CancellationToken cancellationToken)
  {
    Content? content = await ContentService.UpdateLocaleAsync(id, payload, language, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }
}
