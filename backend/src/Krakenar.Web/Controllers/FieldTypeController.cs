using Krakenar.Contracts.Fields;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.FieldType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/fields/types")]
public class FieldTypeController : ControllerBase
{
  protected virtual IFieldTypeService FieldTypeService { get; }

  public FieldTypeController(IFieldTypeService fieldTypeService)
  {
    FieldTypeService = fieldTypeService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<FieldType>> CreateAsync([FromBody] CreateOrReplaceFieldTypePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await FieldTypeService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<FieldType>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await FieldTypeService.DeleteAsync(id, cancellationToken);
    return fieldType is null ? NotFound() : Ok(fieldType);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<FieldType>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await FieldTypeService.ReadAsync(id, uniqueName: null, cancellationToken);
    return fieldType is null ? NotFound() : Ok(fieldType);
  }

  [HttpGet("name:{uniqueName}")]
  public virtual async Task<ActionResult<FieldType>> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await FieldTypeService.ReadAsync(id: null, uniqueName, cancellationToken);
    return fieldType is null ? NotFound() : Ok(fieldType);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<FieldType>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceFieldTypePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceFieldTypeResult result = await FieldTypeService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<FieldType>>> SearchAsync([FromQuery] SearchFieldTypesParameters parameters, CancellationToken cancellationToken)
  {
    SearchFieldTypesPayload payload = parameters.ToPayload();
    SearchResults<FieldType> fieldTypes = await FieldTypeService.SearchAsync(payload, cancellationToken);
    return Ok(fieldTypes);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<FieldType>> UpdateAsync(Guid id, [FromBody] UpdateFieldTypePayload payload, CancellationToken cancellationToken)
  {
    FieldType? fieldType = await FieldTypeService.UpdateAsync(id, payload, cancellationToken);
    return fieldType is null ? NotFound() : Ok(fieldType);
  }

  protected virtual ActionResult<FieldType> ToActionResult(CreateOrReplaceFieldTypeResult result)
  {
    if (result.FieldType is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/fields/types/{result.FieldType.Id}", UriKind.Absolute);
      return Created(location, result.FieldType);
    }

    return Ok(result.FieldType);
  }
}
