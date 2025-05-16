using Krakenar.Contracts.Contents;
using Krakenar.Web.Constants;
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
  public async Task<ActionResult<Content>> CreateAsync([FromBody] CreateContentPayload payload, CancellationToken cancellationToken)
  {
    Content content = await ContentService.CreateAsync(payload, cancellationToken);
    Uri location = new($"{Request.Scheme}://{Request.Host}/api/contents/{content.Id}", UriKind.Absolute);
    return Created(location, content);
  }
}
