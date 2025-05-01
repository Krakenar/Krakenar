using Krakenar.Contracts.Dictionaries;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Dictionary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/dictionaries")]
public class DictionaryController : ControllerBase
{
  protected virtual IDictionaryService DictionaryService { get; }

  public DictionaryController(IDictionaryService dictionaryService)
  {
    DictionaryService = dictionaryService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Dictionary>> CreateAsync([FromBody] CreateOrReplaceDictionaryPayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionaryResult result = await DictionaryService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<Dictionary>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await DictionaryService.DeleteAsync(id, cancellationToken);
    return dictionary is null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Dictionary>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await DictionaryService.ReadAsync(id, languageId: null, cancellationToken);
    return dictionary is null ? NotFound() : Ok(dictionary);
  }

  [HttpGet("language:{languageId}")]
  public virtual async Task<ActionResult<Dictionary>> ReadByLanguageAsync(Guid languageId, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await DictionaryService.ReadAsync(id: null, languageId, cancellationToken);
    return dictionary is null ? NotFound() : Ok(dictionary);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<Dictionary>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceDictionaryPayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceDictionaryResult result = await DictionaryService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<Dictionary>>> SearchAsync([FromQuery] SearchDictionariesParameters parameters, CancellationToken cancellationToken)
  {
    SearchDictionariesPayload payload = parameters.ToPayload();
    SearchResults<Dictionary> dictionaries = await DictionaryService.SearchAsync(payload, cancellationToken);
    return Ok(dictionaries);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<Dictionary>> UpdateAsync(Guid id, [FromBody] UpdateDictionaryPayload payload, CancellationToken cancellationToken)
  {
    Dictionary? dictionary = await DictionaryService.UpdateAsync(id, payload, cancellationToken);
    return dictionary is null ? NotFound() : Ok(dictionary);
  }

  protected virtual ActionResult<Dictionary> ToActionResult(CreateOrReplaceDictionaryResult result)
  {
    if (result.Dictionary is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/dictionaries/{result.Dictionary.Id}", UriKind.Absolute);
      return Created(location, result.Dictionary);
    }

    return Ok(result.Dictionary);
  }
}
