using Krakenar.Contracts.Search;
using Krakenar.Contracts.Templates;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Template;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/templates")]
public class TemplateController : ControllerBase
{
  protected virtual ITemplateService TemplateService { get; }

  public TemplateController(ITemplateService templateService)
  {
    TemplateService = templateService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Template>> CreateAsync([FromBody] CreateOrReplaceTemplatePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceTemplateResult result = await TemplateService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<Template>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Template? template = await TemplateService.DeleteAsync(id, cancellationToken);
    return template is null ? NotFound() : Ok(template);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Template>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Template? template = await TemplateService.ReadAsync(id, uniqueName: null, cancellationToken);
    return template is null ? NotFound() : Ok(template);
  }

  [HttpGet("name:{uniqueName}")]
  public virtual async Task<ActionResult<Template>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    Template? template = await TemplateService.ReadAsync(id: null, uniqueName, cancellationToken);
    return template is null ? NotFound() : Ok(template);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<Template>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceTemplatePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceTemplateResult result = await TemplateService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<Template>>> SearchAsync([FromQuery] SearchTemplatesParameters parameters, CancellationToken cancellationToken)
  {
    SearchTemplatesPayload payload = parameters.ToPayload();
    SearchResults<Template> templates = await TemplateService.SearchAsync(payload, cancellationToken);
    return Ok(templates);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<Template>> UpdateAsync(Guid id, [FromBody] UpdateTemplatePayload payload, CancellationToken cancellationToken)
  {
    Template? template = await TemplateService.UpdateAsync(id, payload, cancellationToken);
    return template is null ? NotFound() : Ok(template);
  }

  protected virtual ActionResult<Template> ToActionResult(CreateOrReplaceTemplateResult result)
  {
    if (result.Template is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/templates/{result.Template.Id}", UriKind.Absolute);
      return Created(location, result.Template);
    }

    return Ok(result.Template);
  }
}
