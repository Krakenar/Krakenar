using Krakenar.Contracts.Contents;
using Krakenar.Contracts.Fields;
using Krakenar.Web.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/contents/types/{contentTypeId}/fields")]
public class FieldDefinitionController : ControllerBase
{
  protected virtual IFieldDefinitionService FieldDefinitionService { get; }

  public FieldDefinitionController(IFieldDefinitionService fieldTypeService)
  {
    FieldDefinitionService = fieldTypeService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<FieldDefinition>> CreateAsync(Guid contentTypeId, [FromBody] CreateOrReplaceFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentType? contentType = await FieldDefinitionService.CreateOrReplaceAsync(contentTypeId, payload, fieldId: null, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }

  [HttpDelete("{fieldId}")]
  public virtual async Task<ActionResult<FieldDefinition>> DeleteAsync(Guid contentTypeId, Guid fieldId, CancellationToken cancellationToken)
  {
    ContentType? contentType = await FieldDefinitionService.DeleteAsync(contentTypeId, fieldId, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }

  [HttpPut("{fieldId}")]
  public virtual async Task<ActionResult<FieldDefinition>> ReplaceAsync(Guid contentTypeId, Guid fieldId, [FromBody] CreateOrReplaceFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentType? contentType = await FieldDefinitionService.CreateOrReplaceAsync(contentTypeId, payload, fieldId, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }

  [HttpPatch("{fieldId}")]
  public virtual async Task<ActionResult<FieldDefinition>> UpdateAsync(Guid contentTypeId, Guid fieldId, [FromBody] UpdateFieldDefinitionPayload payload, CancellationToken cancellationToken)
  {
    ContentType? contentType = await FieldDefinitionService.UpdateAsync(contentTypeId, fieldId, payload, cancellationToken);
    return contentType is null ? NotFound() : Ok(contentType);
  }
}
