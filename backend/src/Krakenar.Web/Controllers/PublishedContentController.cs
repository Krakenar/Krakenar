using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.PublishedContent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/published/contents")]
public class PublishedContentController : ControllerBase
{
  protected virtual IPublishedContentService PublishedContentService { get; }

  public PublishedContentController(IPublishedContentService publishedContentService)
  {
    PublishedContentService = publishedContentService;
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<PublishedContent>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    PublishedContent? content = await PublishedContentService.ReadAsync(id, key: null, cancellationToken);
    return content is not null ? NotFound() : Ok(content);
  }

  [HttpGet("types/{contentType}/name:{uniqueName}")]
  public virtual async Task<ActionResult<PublishedContent>> ReadAsync(string contentType, string uniqueName, string? language, CancellationToken cancellationToken)
  {
    PublishedContentKey key = new(contentType, uniqueName, language);
    PublishedContent? content = await PublishedContentService.ReadAsync(id: null, key, cancellationToken);
    return content is not null ? NotFound() : Ok(content);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<PublishedContentLocale>>> SearchAsync([FromQuery] SearchPublishedContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchPublishedContentsPayload payload = parameters.ToPayload();
    SearchResults<PublishedContentLocale> contents = await PublishedContentService.SearchAsync(payload, cancellationToken);
    return Ok(contents);
  }
}
