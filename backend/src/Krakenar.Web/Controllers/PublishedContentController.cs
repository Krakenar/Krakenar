﻿using Krakenar.Contracts.Contents;
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
  public virtual async Task<ActionResult<PublishedContent>> ReadAsync(string id, CancellationToken cancellationToken)
  {
    PublishedContent? content = null;
    if (int.TryParse(id, out int integerId))
    {
      content = await PublishedContentService.ReadAsync(integerId, uid: null, key: null, cancellationToken);
    }
    else if (Guid.TryParse(id, out Guid uid))
    {
      content = await PublishedContentService.ReadAsync(id: null, uid, key: null, cancellationToken);
    }
    return content is null ? NotFound() : Ok(content);
  }

  [HttpGet("types/{contentType}/name:{uniqueName}")]
  public virtual async Task<ActionResult<PublishedContent>> ReadAsync(string contentType, string uniqueName, string? language, CancellationToken cancellationToken)
  {
    PublishedContentKey key = new(contentType, uniqueName, language);
    PublishedContent? content = await PublishedContentService.ReadAsync(id: null, uid: null, key, cancellationToken);
    return content is null ? NotFound() : Ok(content);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<PublishedContentLocale>>> SearchAsync([FromQuery] SearchPublishedContentsParameters parameters, CancellationToken cancellationToken)
  {
    SearchPublishedContentsPayload payload = parameters.ToPayload();
    SearchResults<PublishedContentLocale> contents = await PublishedContentService.SearchAsync(payload, cancellationToken);
    return Ok(contents);
  }
}
