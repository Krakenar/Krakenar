using Krakenar.Contracts.Localization;
using Krakenar.Contracts.Search;
using Krakenar.Web.Constants;
using Krakenar.Web.Models.Language;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Krakenar.Web.Controllers;

[ApiController]
[Authorize(Policy = Policies.KrakenarAdmin)]
[Route("api/languages")]
public class LanguageController : ControllerBase
{
  protected virtual ILanguageService LanguageService { get; }

  public LanguageController(ILanguageService languageService)
  {
    LanguageService = languageService;
  }

  [HttpPost]
  public virtual async Task<ActionResult<Language>> CreateAsync([FromBody] CreateOrReplaceLanguagePayload payload, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await LanguageService.CreateOrReplaceAsync(payload, id: null, version: null, cancellationToken);
    return ToActionResult(result);
  }

  [HttpDelete("{id}")]
  public virtual async Task<ActionResult<Language>> DeleteAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await LanguageService.DeleteAsync(id, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpGet("{id}")]
  public virtual async Task<ActionResult<Language>> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await LanguageService.ReadAsync(id, locale: null, isDefault: false, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpGet("locale:{locale}")]
  public virtual async Task<ActionResult<Language>> ReadAsync(string locale, CancellationToken cancellationToken)
  {
    Language? language = await LanguageService.ReadAsync(id: null, locale, isDefault: false, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpGet("default")]
  public virtual async Task<ActionResult<Language>> ReadDefaultAsync(CancellationToken cancellationToken)
  {
    Language? language = await LanguageService.ReadAsync(id: null, locale: null, isDefault: true, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpPut("{id}")]
  public virtual async Task<ActionResult<Language>> ReplaceAsync(Guid id, [FromBody] CreateOrReplaceLanguagePayload payload, long? version, CancellationToken cancellationToken)
  {
    CreateOrReplaceLanguageResult result = await LanguageService.CreateOrReplaceAsync(payload, id, version, cancellationToken);
    return ToActionResult(result);
  }

  [HttpGet]
  public virtual async Task<ActionResult<SearchResults<Language>>> SearchAsync([FromQuery] SearchLanguagesParameters parameters, CancellationToken cancellationToken)
  {
    SearchLanguagesPayload payload = parameters.ToPayload();
    SearchResults<Language> languages = await LanguageService.SearchAsync(payload, cancellationToken);
    return Ok(languages);
  }

  [HttpPatch("{id}/default")]
  public virtual async Task<ActionResult<Language>> SetDefaultAsync(Guid id, CancellationToken cancellationToken)
  {
    Language? language = await LanguageService.SetDefaultAsync(id, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  [HttpPatch("{id}")]
  public virtual async Task<ActionResult<Language>> UpdateAsync(Guid id, [FromBody] UpdateLanguagePayload payload, CancellationToken cancellationToken)
  {
    Language? language = await LanguageService.UpdateAsync(id, payload, cancellationToken);
    return language is null ? NotFound() : Ok(language);
  }

  protected virtual ActionResult<Language> ToActionResult(CreateOrReplaceLanguageResult result)
  {
    if (result.Language is null)
    {
      return NotFound();
    }
    else if (result.Created)
    {
      Uri location = new($"{Request.Scheme}://{Request.Host}/api/languages/{result.Language.Id}", UriKind.Absolute);
      return Created(location, result.Language);
    }

    return Ok(result.Language);
  }
}
